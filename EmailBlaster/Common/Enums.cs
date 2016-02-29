using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EmailBlaster.Common
{
    public class Enums
    {
        public enum SuppressionReason
        {
            Unsubscribed = 1,
            Bounced = 2,
            MarkedSpam = 3,
            InvalidMailbox = 4,
            Blocked = 5
        }
    }
}