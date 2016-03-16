using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Http;
using System.Net;
using System.Net.Mail;
using SendGrid;
using EmailBlaster.Models;
using SendGrid.SmtpApi;
using System.Threading;
using System.Threading.Tasks;

namespace EmailBlaster.Helpers
{
    public static class EmailHelper
    {
        public static void SendEmail(Campaign model, string[] recipients, string sendgridapiKey)
        {
            var message = CreateSendgridMessage(model, recipients);

            var transportWeb = new SendGrid.Web(sendgridapiKey);
            transportWeb.DeliverAsync(message);
        }

        //public static SendGridMessage CreateSendgridMessage(Campaign campaign, string emailTo)
        //{
        //    var myMessage = new SendGridMessage();

        //    // add sendgrid directive headers
        //    var header  = new Header();
        //    var uniqueArgs = new Dictionary<string, string> { { "MARKETING_CAMPAIGN_NAME", campaign.Name} };
        //    header.AddUniqueArgs(uniqueArgs);

        //    var xmstpapiJson = header.JsonString();

        //    myMessage.Headers.Add("X-SMTPAPI", xmstpapiJson);

        //    // add from, recipients and subject
        //    myMessage.From = new MailAddress(campaign.Sender);
        //    myMessage.AddTo(emailTo);
        //    myMessage.Subject = campaign.Subject;

        //    //Add the HTML and Text bodies
        //    myMessage.Html = campaign.HtmlBody;
        //    myMessage.Text = campaign.TextBody;

        //    return myMessage;
        //}

        public static SendGridMessage CreateSendgridMessage(Campaign campaign, string[] recipients)
        {
            var myMessage = new SendGridMessage();

            // add sendgrid directive headers
            var header = new Header();
            var uniqueArgs = new Dictionary<string, string> { { "MARKETING_CAMPAIGN_NAME", campaign.Name } };
            header.AddUniqueArgs(uniqueArgs);

            var xmstpapiJson = header.JsonString();

            myMessage.Headers.Add("X-SMTPAPI", xmstpapiJson);

            // add from, recipients and subject
            myMessage.From = new MailAddress(campaign.Sender);
            myMessage.AddTo(campaign.Sender);

            foreach(string email in recipients)
                myMessage.AddBcc(email);

            myMessage.Subject = campaign.Subject;

            //Add the HTML and Text bodies
            myMessage.Html = campaign.HtmlBody;
            myMessage.Text = campaign.TextBody;

            return myMessage;
        }
    }
}