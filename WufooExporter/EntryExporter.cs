using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Schema;

namespace WufooExporter
{
    public class EntryExporter
    {
        public static void SaveEntries(List<Entry> entries)
        {
            // First Find and merge duplicate applications
            List<Entry> l_orderedEntries = entries.OrderBy(i => i.Email).ToList();
            List<Entry> l_distinctEntries = new List<Entry>();
            foreach (Entry l_entry in l_orderedEntries)
            {
                string l_entryEmail = l_entry.Email;
                if (l_distinctEntries.Any(e => e.Email.ToUpperInvariant() == l_entryEmail.ToUpperInvariant()))
                {
                    Entry l_existingEntry =
                        l_distinctEntries.Find(i => i.Email.ToUpperInvariant() == l_entryEmail.ToUpperInvariant());
                    //Duplicate entry - Merge
                    Entry l_mergedEntry = MergeEntries(l_entry, l_existingEntry);
                    //Use this entry
                    int l_index = l_distinctEntries.IndexOf(l_existingEntry);
                    if (l_index != -1)
                        l_distinctEntries[l_index] = l_mergedEntry;

                }
                else
                {
                    l_distinctEntries.Add(l_entry);
                }
            }
           
            List<Entry> l_completeDistinctApplications =
               l_distinctEntries.Where(i => !String.IsNullOrWhiteSpace(i.LetterOfAttestationLink)
               ).ToList();

            List<Entry> l_incompleteDistinctApplications =
               l_distinctEntries.Where(i => String.IsNullOrWhiteSpace(i.LetterOfAttestationLink)
               ).ToList();

            DownloadAndSaveEntries(l_completeDistinctApplications, "With 1 or more Document");
            DownloadAndSaveEntries(l_incompleteDistinctApplications, "With No Documents");

        }

        private static Entry MergeEntries(Entry entry, Entry existingEntry)
        {
            if (entry.CompleteSubmission==1)
            {
                if (existingEntry.CompleteSubmission==0)
                {
                    return entry;
                }
            }
            if (!String.IsNullOrWhiteSpace(entry.LetterOfAttestationLink))
            {
                if (String.IsNullOrWhiteSpace(existingEntry.LetterOfAttestationLink))
                {
                    return entry;
                }
            }
            Entry l_mergedEntry = AutoMapper.Mapper.Map(entry, existingEntry);
            return l_mergedEntry;
        }

        private static void DownloadAndSaveEntries(List<Entry> entries, string directoryName)
        {
            foreach (Entry l_entry in entries)
            {
                string l_letterOfAttestationLink = GetDocumentLink(l_entry.LetterOfAttestationLink);
                string l_additionalDocument1 = GetDocumentLink(l_entry.AdditionalDocument1);
                string l_additionalDocument2 = GetDocumentLink(l_entry.AdditionalDocument2);
                string l_additionalDocument3 = GetDocumentLink(l_entry.AdditionalDocument3);
                string l_additionalDocument4 = GetDocumentLink(l_entry.AdditionalDocument4);
            }
        }

        private static string GetDocumentLink(string link)
        {
            if (String.IsNullOrWhiteSpace(link))
                return null;
            var l_indexOfLinkStart = link.IndexOf("(", StringComparison.Ordinal);
            if (l_indexOfLinkStart <= -1)
                return null;
            return link.Substring(l_indexOfLinkStart+1).TrimEnd(')');
        }
    }
}