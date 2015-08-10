using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WufooExporter
{
    class Program
    {
        private const int MAX_PAGESIZE = 100;
        private static List<Entry> m_formEntries = new List<Entry>(); 
        static void Main(string[] args)
        {
            GetEntries();
            
        }

        private static async void GetEntries()
        {
            int entryCount = RestApiCaller.GetEntryCount().Result;
            if (entryCount == 0)
                return;

            int l_numberOfPages = (int)Math.Ceiling((double)entryCount / (double)MAX_PAGESIZE);
            
            for (int i = 0; i < l_numberOfPages; i++)
            {
                var l_json = RestApiCaller.GetJsonEntriesFromApi(i*MAX_PAGESIZE, MAX_PAGESIZE).Result;
                if (String.IsNullOrWhiteSpace(l_json))
                    return;
                List<Entry> l_entries = JsonConvert.DeserializeObject<RootObject>(l_json).Entries;
                m_formEntries.AddRange(l_entries);
            }
            int l_enumerable = m_formEntries.Select(i => i.EntryId).Distinct().Count();
        }
    }
}
