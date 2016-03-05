using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography.Pkcs;

namespace EmailBlaster.Models
{
    public class Campaign
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int?  Id { get; set; }

        [Required]
        [StringLength(150)]
        [Display(Name="Campaign Name")]
        public string Name  { get; set; }

        [Required]
        [StringLength(80)]
        [EmailAddress(ErrorMessage = "Invalid Sender Email")]
        [Display(Name = "Sender Email")]
        public string Sender { get; set; }

        [Required]
        [StringLength(80)]
        public string Subject { get; set; }

        [Display(Name = "Paste Html Here")]
        public string HtmlBody { get; set; }

        [Display(Name = "Text version of the Email")]
        public string TextBody { get; set; }

        [Display(Name="Send Date")]
        public DateTime? DateScheduled { get; set; }

        [Display(Name="Date Created")]
        public DateTime DateCreated { get; set; }

        [Display(Name = "Date Sent")]
        public DateTime? DateSent { get; set; }
    }
}