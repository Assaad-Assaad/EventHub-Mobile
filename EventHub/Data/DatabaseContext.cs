﻿

namespace EventHub.Data
{
    public class DatabaseContext : IAsyncDisposable
    {
        private const string DbName = "EventHub.db3";
        private static string DbPath => Path.Combine(FileSystem.AppDataDirectory, DbName);

        private SQLiteAsyncConnection _connection;
       

        private SQLiteAsyncConnection Database =>
            _connection ??= new SQLiteAsyncConnection(
                DbPath,
                SQLiteOpenFlags.Create | SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.SharedCache);

        public async Task InitializeDatabaseAsync()
        {
            try
            {
                if (_connection != null) return;

                // Drop existing database if it exists
                //if (File.Exists(DbPath))
                //{
                //    File.Delete(DbPath);
                //    Debug.WriteLine("Existing database deleted");
                //}

                _connection = new SQLiteAsyncConnection(DbPath);
                Debug.WriteLine("Database connection created");

                await _connection.CreateTableAsync<SyncHistory>();
                await _connection.CreateTableAsync<Event>();
                await _connection.CreateTableAsync<User>();
                await _connection.CreateTableAsync<LoggedInUser>();
                await _connection.CreateTableAsync<UserEvent>();

                // Create unique index for UserEvent composite key
                await _connection.ExecuteAsync(
                    "CREATE UNIQUE INDEX IF NOT EXISTS IX_UserEvent_UserId_EventId ON UserEvent(UserId, EventId)");

                Debug.WriteLine("Tables created successfully");

                await _connection.InsertAsync(new SyncHistory
                {
                    LastSyncTime = DateTime.UtcNow,
                    SyncType = "Initial"
                });
                Debug.WriteLine("Initial sync history created");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error initializing database: {ex.Message}");
                Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                throw;
            }
        }

        public async Task SaveItemAsync<T>(T item) where T : new()
        {
            await Database.InsertOrReplaceAsync(item);
        }

        public async Task SaveItemsAsync<T>(List<T> items) where T : new()
        {
            Debug.WriteLine($"Saving {items.Count} items...");
            foreach (var item in items)
            {
                await Database.InsertOrReplaceAsync(item);
            }
            Debug.WriteLine("Items saved successfully.");
        }

        public async Task<List<T>> GetAllItemsAsync<T>() where T : new()
        {
            return await Database.Table<T>().ToListAsync();
        }
        public async Task GetItemAsync<T>(T item) where T : new()
        {
            await Database.InsertOrReplaceAsync(item);
        }

        public async Task<T> GetItemByIdAsync<T>(int id) where T : new()
        {
            return await Database.FindAsync<T>(id);
        }




        public async Task DeleteItemAsync<T>(T item) where T : new()
        {
            await Database.DeleteAsync(item);
        }

        public async Task DeleteItemByIdAsync<T>(int id) where T : new()
        {
            await Database.DeleteAsync<T>(id);
        }

        public async Task DeleteAllItemsAsync<T>() where T : new()
        {
            await Database.DeleteAllAsync<T>();
        }

        public async Task<T> GetItemAsync<T>(Expression<Func<T, bool>> predicate) where T : new()
        {
            return await Database.Table<T>().Where(predicate).FirstOrDefaultAsync();
        }

        public async Task<List<T>> GetItemsAsync<T>(Expression<Func<T, bool>> predicate) where T : new()
        {
            return await Database.Table<T>().Where(predicate).ToListAsync();
        }

        public async ValueTask DisposeAsync()
        {
            if (_connection != null)
            {
                await _connection.CloseAsync();
                _connection = null;
            }
        }

        public async Task<DateTime?> GetLastSyncTime()
        {
            try
            {
                var lastSync = await Database.Table<SyncHistory>()
                    .OrderByDescending(sh => sh.SyncDate)
                    .FirstOrDefaultAsync();

                return lastSync?.SyncDate;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to get last sync time: {ex.Message}");
                return null;
            }
        }

        public async Task UpdateLastSyncTime(DateTime syncTime)
        {
            try
            {
                await Database.InsertOrReplaceAsync(new SyncHistory { SyncDate = syncTime });
                Debug.WriteLine($"Sync time updated to: {syncTime}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to update sync time: {ex.Message}");
                
            }
        }
    }
}
