using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EmailBlaster.Common;

namespace EmailBlaster.ViewModels
{
    public class UnsubscribeViewModel
    {
        public int SuppresionReason { get; set; }
        public SelectList SuppresionReasonList = new SelectList(new[]
                                                  {
                                                      new SelectListItem() { Text = "Unsubscribed", Value = "1" },
                                                      new SelectListItem() { Text = "Bounced", Value = "2" },
                                                      new SelectListItem() { Text = "MarkedSpam", Value = "3" },
                                                      new SelectListItem() { Text = "InvalidMailbox", Value = "4" },
                                                      new SelectListItem() { Text = "Blocked", Value = "5" }
                                                  },"Value","Text");
    }
}