using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Xml.Schema;

namespace WufooExporter
{
    public class EntryExporter
    {
        private static string m_saveFolder = @"E:\CNS";
        private static string l_entryFolder;

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
            if (!Directory.Exists(Path.Combine(m_saveFolder, directoryName)))
            {
                Directory.CreateDirectory(Path.Combine(m_saveFolder, directoryName));
            }
            foreach (Entry l_entry in entries)
            {
                DownloadAndCreateFolder(l_entry, directoryName);
            }
        }

        private static void DownloadAndCreateFolder(Entry entry, string directoryName)
        {
            l_entryFolder = Path.Combine(m_saveFolder, directoryName, entry.EntryId.ToString());
            if (!Directory.Exists(l_entryFolder))
            {
                Directory.CreateDirectory(l_entryFolder);
            }
            string l_letterOfAttestationLink = GetDocumentLink(entry.LetterOfAttestationLink);
            string l_additionalDocument1 = GetDocumentLink(entry.AdditionalDocument1);
            string l_additionalDocument2 = GetDocumentLink(entry.AdditionalDocument2);
            string l_additionalDocument3 = GetDocumentLink(entry.AdditionalDocument3);
            string l_additionalDocument4 = GetDocumentLink(entry.AdditionalDocument4);

            WebClient l_Client = new WebClient();
            if (!String.IsNullOrWhiteSpace(l_letterOfAttestationLink))
            {
                var l_lastIndexOfPeriod = l_letterOfAttestationLink.LastIndexOf(".", StringComparison.Ordinal);
                string l_extension = l_letterOfAttestationLink.Substring(l_lastIndexOfPeriod + 1);
                l_Client.DownloadFile(l_letterOfAttestationLink,
                    Path.Combine(l_entryFolder, "LetterOfAttestation." + l_extension));
            }

            if (!String.IsNullOrWhiteSpace(l_additionalDocument1))
            {
                var l_lastIndexOfPeriod = l_additionalDocument1.LastIndexOf(".", StringComparison.Ordinal);
                string l_extension = l_additionalDocument1.Substring(l_lastIndexOfPeriod + 1);
                l_Client.DownloadFile(l_additionalDocument1,
                    Path.Combine(l_entryFolder, "AdditionalDocument1." + l_extension));
            }

            if (!String.IsNullOrWhiteSpace(l_additionalDocument2))
            {
                var l_lastIndexOfPeriod = l_additionalDocument2.LastIndexOf(".", StringComparison.Ordinal);
                string l_extension = l_additionalDocument2.Substring(l_lastIndexOfPeriod + 1);
                l_Client.DownloadFile(l_additionalDocument2,
                    Path.Combine(l_entryFolder, "additionalDocument2." + l_extension));
            }

            if (!String.IsNullOrWhiteSpace(l_additionalDocument3))
            {
                var l_lastIndexOfPeriod = l_additionalDocument3.LastIndexOf(".", StringComparison.Ordinal);
                string l_extension = l_additionalDocument3.Substring(l_lastIndexOfPeriod + 1);
                l_Client.DownloadFile(l_additionalDocument3,
                    Path.Combine(l_entryFolder, "additionalDocument3." + l_extension));
            }

            if (!String.IsNullOrWhiteSpace(l_additionalDocument4))
            {
                var l_lastIndexOfPeriod = l_additionalDocument4.LastIndexOf(".", StringComparison.Ordinal);
                string l_extension = l_additionalDocument4.Substring(l_lastIndexOfPeriod + 1);
                l_Client.DownloadFile(l_additionalDocument4,
                    Path.Combine(l_entryFolder, "additionalDocument4." + l_extension));
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