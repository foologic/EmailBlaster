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
                using (var csv = new CsvReader(sr))
                {
                    csv.Configuration.RegisterClassMap(new ContactMap());

                    return csv.GetRecords<Contact>().ToList();

                }
            }
        }

        public static IEnumerable<Contact> ParseUnsubscribedContacts(string filename)
        {
            using (StreamReader sr = new StreamReader(filename))
            {
                using (var csv = new CsvReader(sr))
                {
                    csv.Configuration.RegisterClassMap(new UnsubsContactMap());

                    return csv.GetRecords<Contact>().ToList();

                }
            }
        }
    }

    // class for mapping the contact class with columns in the csv file
    // http://joshclose.github.io/CsvHelper/#mapping
    public sealed class ContactMap : CsvClassMap<Contact>
    {
        public ContactMap()
        {
            Map(x => x.Email);
            Map(x => x.FirstName);
            Map(x => x.LastName);
        }
    }

    public sealed class UnsubsContactMap : CsvClassMap<Contact>
    {
        public UnsubsContactMap()
        {
            Map(x => x.Email);
            Map(x => x.SuppressionReason);
        }
    }
}