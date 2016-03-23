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

        // WARNING!! SENDGRID DOES NOT SEND INDIVIDUAL EMAILS TO RECIPIENTS WHEN USING SENDGRIDMESSAGE
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

            foreach (string email in recipients)
                myMessage.AddTo(email);

            myMessage.Subject = campaign.Subject;

            //Add the HTML and Text bodies
            myMessage.Html = campaign.HtmlBody;
            myMessage.Text = campaign.TextBody;

            //enable clicktracking
            myMessage.EnableClickTracking();

            return myMessage;
        }

        public static void SendViaSmtpWithRetry (Campaign campaign, string[] recipients, string smtpuser, string smtppass)
        {
            // add sendgrid directive headers
            var header = new Header();

            var uniqueArgs = new Dictionary<string, string> { { "MARKETING_CAMPAIGN_NAME", campaign.Name } };
            header.AddUniqueArgs(uniqueArgs);

            // send recipient individual emails
            header.SetTo(recipients);

            var xmstpapiJson = header.JsonString();

            SmtpClient client = new SmtpClient();
            client.Port = 587;
            client.Host = "smtp.sendgrid.net";
            client.Timeout = 10000;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.Credentials = new System.Net.NetworkCredential(smtpuser, smtppass);

            MailMessage mail = new MailMessage();
            foreach(var email in recipients)
               mail.To.Add(new MailAddress(email));
            mail.From = new MailAddress(campaign.Sender);
            mail.Subject = campaign.Subject;
            mail.Body = campaign.HtmlBody;
            mail.IsBodyHtml = true;

            // add the custom header that we built above
            mail.Headers.Add("X-SMTPAPI", xmstpapiJson);

            try
            {
                //await client.SendAsync(mail, null);
                client.Send(mail);
            }
            catch (SmtpFailedRecipientsException ex)
            {
                for (int i = 0; i < ex.InnerExceptions.Length; i++)
                {
                    SmtpStatusCode status = ex.InnerExceptions[i].StatusCode;
                    if (status == SmtpStatusCode.MailboxBusy ||
                        status == SmtpStatusCode.MailboxUnavailable)
                    {
                        Console.WriteLine("SmtpFailedRecipientsException - retrying in 5 seconds.");
                        System.Threading.Thread.Sleep(5000);
                        client.Send(mail);
                    }
                    else
                    {
                        Console.WriteLine("Failed to deliver message to {0}",
                            ex.InnerExceptions[i].FailedRecipient);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception caught in SendViaSmtpWithRetry(): {0}", ex.ToString());

                Console.WriteLine("Exception - retrying in 5 seconds.");
                System.Threading.Thread.Sleep(5000);
                client.Send(mail);
            }

        }
    }
}