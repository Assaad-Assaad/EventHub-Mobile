using EventHub.Models;
using EventHub.Utils;
using SQLite;
using System.Diagnostics;
using System.Linq.Expressions;

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

                await Database.CreateTableAsync<Event>();
                await Database.CreateTableAsync<User>();
                await Database.CreateTableAsync<LoggedInUser>();
                await Database.CreateTableAsync<UserEvent>();
                await Database.CreateTableAsync<SyncHistory>();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"DB Error: {ex.Message}");
            }
        }

       

        public async Task SaveItemAsync<T>(T item) where T : new()
        {
            await Database.InsertOrReplaceAsync(item);
        }

        public async Task SaveItemsAsync<T>(List<T> items) where T : new()
        {
            Debug.WriteLine($"💾 Saving {items.Count} items...");
            foreach (var item in items)
            {
                await Database.InsertOrReplaceAsync(item);
            }
            Debug.WriteLine("✅ Items saved successfully.");
        }

        public async Task<List<T>> GetAllItemsAsync<T>() where T : new()
        {
            return await Database.Table<T>().ToListAsync();
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
                // Create table if not exists
                await Database.CreateTableAsync<SyncHistory>();

                var lastSync = await Database.Table<SyncHistory>()
                    .OrderByDescending(sh => sh.SyncDate)
                    .FirstOrDefaultAsync();

                return lastSync?.SyncDate;
            }
            catch
            {
                return null;
            }
        }

        public async Task UpdateLastSyncTime(DateTime syncTime)
        {
            try
            {
                await Database.InsertOrReplaceAsync(new SyncHistory { SyncDate = syncTime });
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"⚠️ Failed to update sync time: {ex.Message}");
            }
        }
    }
}
