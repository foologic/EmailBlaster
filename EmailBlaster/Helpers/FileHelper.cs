using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using CsvHelper;
using CsvHelper.Configuration;
using EmailBlaster.Models;

namespace EmailBlaster.Helpers
{
    public static class FileHelper
    {
        public static IEnumerable<Contact> ParseNewContacts(string filename)
        {
            using (StreamReader sr = new StreamReader(filename))
            {
                using (var csv =  GetCsvReader(sr, new ContactMap()))
                {
                    return csv.GetRecords<Contact>().ToList();
                }
            }
        }

        public static IEnumerable<Contact> ParseUnsubscribedContacts(string filename)
        {
            using (StreamReader sr = new StreamReader(filename))
            {
                using (var csv = GetCsvReader(sr, new UnsubsContactMap()))
                {
                    return csv.GetRecords<Contact>().ToList();

                }
            }
        }

        private static CsvReader GetCsvReader(StreamReader sr, CsvClassMap mapper)
        {
            var csv = new CsvReader(sr);
            csv.Configuration.IsHeaderCaseSensitive = false;
            csv.Configuration.WillThrowOnMissingField = false; // set true to enforce required columns

            csv.Configuration.RegisterClassMap(mapper);
            
            return csv;
        }
    }

    // class for mapping the contact class with columns in the csv file
    // http://joshclose.github.io/CsvHelper/#mapping
    public sealed class ContactMap : CsvClassMap<Contact>
    {
        public ContactMap()
        {
            Map(x => x.Email).Name("Email");
            Map(x => x.FirstName).Name("FirstName", "First Name");
            Map(x => x.LastName).Name("LastName", "First LastName");
        }
    }

    public sealed class UnsubsContactMap : CsvClassMap<Contact>
    {
        public UnsubsContactMap()
        {
            Map(x => x.Email).Name("Email");
            Map(x => x.SuppressionReason).Name("SuppressionReason", "Suppression Reason");
        }
    }
}