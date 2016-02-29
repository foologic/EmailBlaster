using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using EmailBlaster.Models;

namespace EmailBlaster.ViewModels
{
    public class CampaignViewModel
    {
        public Campaign Campaign { get; set; }

        [EmailAddress(ErrorMessage = "Invalid Sender Email")]
        [DefaultValue("jason@onlinecarstereo.com")]
        public string SendTestEmail { get; set; }
    }
}