using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventHub.Utils
{
    public class SyncHistory
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public DateTime SyncDate { get; set; }
    }
}
