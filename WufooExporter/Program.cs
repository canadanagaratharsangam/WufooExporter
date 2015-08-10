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
        static List<Entry> m_formEntries = new List<Entry>(); 
        static void Main(string[] args)
        {
            GetEntries();
            
        }

        private static async void GetEntries()
        {
            int l_totalPages = 5;
            for (int i = 0; i < l_totalPages; i++)
            {
                var l_json = RestApiCaller.GetJsonFromApi(i).Result;
                if (String.IsNullOrWhiteSpace(l_json))
                    return;
                List<Entry> l_entries = JsonConvert.DeserializeObject<RootObject>(l_json).Entries;
                m_formEntries.AddRange(l_entries);
            }
            int l_enumerable = m_formEntries.Select(i => i.EntryId).Distinct().Count();
        }
    }
}
