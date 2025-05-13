

namespace EventHub.Utils
{
    public class SyncHistory
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public DateTime SyncDate { get; set; }
        public DateTime LastSyncTime { get; set; }
        public string SyncType { get; set; } 
    }
}
