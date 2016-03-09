using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Web.Profile;
using System.ComponentModel.DataAnnotations;

namespace EmailBlaster.Models
{
    public class EmailBlasterContext : DbContext
    {
        public EmailBlasterContext()
            : base("name=DefaultConnection")
        {

#if DEBUG
            // This will simply log every query Entity Framework executes to the Output window of Visual Studio when running in debug mode. 
            this.Database.Log = s => System.Diagnostics.Debug.WriteLine(s);
#endif
        }

        public virtual DbSet<Contact> Contacts { get; set; }
        public virtual DbSet<Campaign> Campaigns { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // were not using code first so prevent creation/update of tables
            // Database.SetInitializer<DbContext>(null);

            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}