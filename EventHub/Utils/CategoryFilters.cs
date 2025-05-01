using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventHub.Utils
{
    public static class CategoryFilters
    {
        public static List<string> All => new() {"All", "Music", "Technology","Science","Culture", "Business" };
    }
}
