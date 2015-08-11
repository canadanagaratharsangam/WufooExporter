using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
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
            l_entryFolder = Path.Combine(m_saveFolder, directoryName, entry.EntryId.ToString() + " - " + entry.Email);
            if (!Directory.Exists(l_entryFolder))
            {
                Directory.CreateDirectory(l_entryFolder);
            }
            StreamWriter l_writer =
                File.CreateText(Path.Combine(l_entryFolder, entry.EntryId + " - " + entry.Email + ".txt"));
            WriteEntryToFile(l_writer, entry);
            l_writer.Close();
            string l_letterOfAttestationLink = GetDocumentLink(entry.LetterOfAttestationLink);
            string l_additionalDocument1 = GetDocumentLink(entry.AdditionalDocument1);
            string l_additionalDocument2 = GetDocumentLink(entry.AdditionalDocument2);
            string l_additionalDocument3 = GetDocumentLink(entry.AdditionalDocument3);
            string l_additionalDocument4 = GetDocumentLink(entry.AdditionalDocument4);

            WebClient l_Client = new WebClient();
            DownloadDocument(l_letterOfAttestationLink, l_Client, "LetterOfAttestation");
            DownloadDocument(l_additionalDocument1, l_Client, "AdditionalDocument1");
            DownloadDocument(l_additionalDocument2, l_Client, "AdditionalDocument2");
            DownloadDocument(l_additionalDocument3, l_Client, "AdditionalDocument3");
            DownloadDocument(l_additionalDocument4, l_Client, "AdditionalDocument4");

        }

        private static void DownloadDocument(string link, WebClient client, string fileName)
        {
            //LetterOfAttestation
            if (!String.IsNullOrWhiteSpace(link))
            {
                var l_lastIndexOfPeriod = link.LastIndexOf(".", StringComparison.Ordinal);
                string l_extension = link.Substring(l_lastIndexOfPeriod + 1);
                var l_destinationFileName = l_extension.Length > 4 ? Path.Combine(l_entryFolder, fileName) : Path.Combine(l_entryFolder, fileName + "." + l_extension);
                if (!File.Exists(l_destinationFileName))
                    client.DownloadFile(link, l_destinationFileName);
            }
        }

        private static void WriteEntryToFile(StreamWriter writer, Entry entry)
        {
            writer.WriteLine("Name:" + entry.Name);
            writer.WriteLine("Father's Name: " + entry.FathersName);
            writer.WriteLine("Mother's Name: " + entry.MothersName);
            writer.WriteLine("Date of Birth: " + entry.DateOfBirth);
            writer.WriteLine("Email: " + entry.Email);
            writer.WriteLine("Phone: " + entry.Phone);
            writer.WriteLine("Address:");
            writer.Write(GetMailingAddress(entry));
            writer.WriteLine("Nagarathar Village: " + entry.NagaratharVillage);
            writer.WriteLine("Degree: " + entry.Degree);
            writer.WriteLine("Specialization: " + entry.Specialization);
            writer.WriteLine("Year of Study: " + entry.YearOfStudy);
            writer.WriteLine("College Name: " + entry.College);
            writer.WriteLine("College Address:");
            writer.Write(GetCollegeAddress(entry));
            writer.WriteLine("Annual Family Income: " + entry.AnnualFamilyIncome);
            writer.WriteLine("Extra Curricular Activities: " + entry.ExtraCurricularActivities);
            writer.WriteLine("Additional Comments: " + entry.AdditionalComments);
            writer.WriteLine("Submitted on: " + entry.DateCreated.ToShortDateString());
        }

        private static string GetMailingAddress(Entry entry)
        {
            StringBuilder sb = new StringBuilder();
            if(!String.IsNullOrWhiteSpace(entry.AddressLine1))
            sb.AppendLine(entry.AddressLine1);
            if (!String.IsNullOrWhiteSpace(entry.AddressLine2))
                sb.AppendLine(entry.AddressLine2);
            sb.AppendLine(entry.City + ", " + entry.State + "-" + entry.ZipCode);
            if (!String.IsNullOrWhiteSpace(entry.Country))
                sb.AppendLine(entry.Country);
            return sb.ToString();
        }

        private static string GetCollegeAddress(Entry entry)
        {
            StringBuilder sb = new StringBuilder();
            if (!String.IsNullOrWhiteSpace(entry.CollegeAddressLine1))
                sb.AppendLine(entry.CollegeAddressLine1);
            if (!String.IsNullOrWhiteSpace(entry.CollegeAddressLine2))
                sb.AppendLine(entry.CollegeAddressLine2);
            sb.AppendLine(entry.CollegeCity + ", " + entry.CollegeState + "-" + entry.CollegeZipCode);
            if (!String.IsNullOrWhiteSpace(entry.CollegeCountry))
                sb.AppendLine(entry.CollegeCountry);
            return sb.ToString();
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