using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace EmailBlaster.Models
{
    public class Contact
    {
        public Contact()
        {
            Active = true;
            DateAdded = DateTime.Now;
        }

        [Required]
        public int Id { get; set; }

        [Required]
        [StringLength(80)]
        public string Email { get; set; }

        [Display(Name="First Name")]
        [StringLength(50)]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        [StringLength(80)]
        public string LastName { get; set; }

        [Required] 
        [DefaultValue(true)]
        public bool Active { get; set; }

        [Required]
        [Display(Name = "Date Added")]
        public DateTime DateAdded { get; set; }

        [Display(Name="Date Dectivated")]
        public DateTime? DateDeactivated { get; set; }

        [DefaultValue(false)]
        public Boolean Unsubscribed { get; set; }

        [DefaultValue(false)]
        public Boolean Bounced { get; set; }

        [DefaultValue(false)]
        public Boolean MarkedSpam  { get; set; }

        [DefaultValue(false)]
        public Boolean InvalidMailbox { get; set; }

        [DefaultValue(false)]
        public Boolean Blocked { get; set; }

        [StringLength(1000)]
        public string SuppressionReason { get; set; }
    }

}