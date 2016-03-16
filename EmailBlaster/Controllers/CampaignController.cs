using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using EmailBlaster.Models;
using EmailBlaster.ViewModels;
using EmailBlaster.Helpers;
using Microsoft.Ajax.Utilities;
using System.Threading;
using System.Threading.Tasks;

namespace EmailBlaster.Controllers
{
    public class CampaignController : Controller
    {
        private EmailBlasterContext db = new EmailBlasterContext();
        private string _sendgridapikey = Environment.GetEnvironmentVariable("SENDGRID_API_KEY");
        private int _emailbatchcount = Convert.ToInt32( ConfigurationManager.AppSettings["EmailBatchCount"]);

        // GET: Campaign
        public async Task< ActionResult> Index()
        {
            var model = await db.Campaigns.OrderByDescending(x => x.DateCreated).ToListAsync();

            return View(model);
        }

        // GET: Campaign/Create
        public ActionResult Create()
        {
            var model = new CampaignViewModel();

            return View(model);
        }

        // POST: Campaign/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public async Task<ActionResult> Create([Bind(Exclude = "Id")] CampaignViewModel model)
        {
            if (ModelState.IsValid)
            {
                var o = new Campaign();

                // todo: use automapper here 
                o.Name = model.Campaign.Name;
                o.Sender = model.Campaign.Sender;
                o.Subject = model.Campaign.Subject;
                o.HtmlBody = model.Campaign.HtmlBody;
                o.TextBody = model.Campaign.TextBody;
                o.DateCreated = DateTime.Now;
                o.DateScheduled = model.Campaign.DateScheduled;

                db.Campaigns.Add(o);
                await db.SaveChangesAsync();

                TempData["SuccessMessage"] = "Campaign successfully created.";

                // use PRG(post redirect get) pattern
                return RedirectToAction("Edit", new {Id = o.Id});
            }
            else
            {
                return View(model);
            }
        }


        // GET: Campaign/Edit
        [HttpGet]
        public async Task<ActionResult> Edit(int id)
        {
            var model = new CampaignViewModel() {Campaign = db.Campaigns.SingleOrDefault(x => x.Id == id)};

            return View(model);
        }

        // POST: Campaign/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public async Task<ActionResult> Edit(CampaignViewModel model, string command)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // get existing campaign
                    var o = await db.Campaigns.SingleOrDefaultAsync(x => x.Id == model.Campaign.Id);

                    // todo: use automapper here 
                    o.Name = model.Campaign.Name;
                    o.Sender = model.Campaign.Sender;
                    o.Subject = model.Campaign.Subject;
                    o.HtmlBody = model.Campaign.HtmlBody;
                    o.TextBody = model.Campaign.TextBody;
                    o.DateScheduled = model.Campaign.DateScheduled;

                    // check which submit button was clicked
                    switch (command)
                    {
                        case "Send Test Email":
                            SendTestEmail(model, _sendgridapikey);
                            TempData["SuccessMessage"] = "Test email sent successfully.";
                            break;
                        case "Schedule Email":
                            TempData["SuccessMessage"] = "Email successfully  scheduled.";
                            break;
                        case "Save":
                            TempData["SuccessMessage"] = "Campaign saved successfully.";
                            break;
                        case "Send Now":
                            o.DateCreated = DateTime.Now;
                            SendCampaign(o);
                            TempData["SuccessMessage"] = "Campaign sent successfully.";
                            break;
                    }

                    await db.SaveChangesAsync();

                    return RedirectToAction("Edit", new {Id = o.Id});
                }
                else
                {
                    return View(model);
                }
            }
            catch (Exception)
            {
                throw;
            }

            return View(model);
        }

        private void SendTestEmail(CampaignViewModel  model, string apiKey)
        {
            //foreach(var email in model.SendTestEmail.Split(',', ';'))
            //{
            //    EmailHelper.SendEmail(model.Campaign, email, _sendgridapikey);
            //}
            EmailHelper.SendEmail(model.Campaign, model.SendTestEmail.Split(',',';'), _sendgridapikey);
        }

        private void SendCampaign(Campaign campaign)
        {
            // get how many contacts
            double contactscount = db.Contacts.Count(x => x.Active == true);

            // get all 
            IQueryable<Contact> contacts = db.Contacts.Where(x => x.Active == true).OrderBy(x=>x.Email);

            double counttotal = 0; // used for debugging only
            int skip = 0;

            // loop through all subscribers, by batches of N
            while (contactscount > skip )
            {
                Debug.WriteLine("==========================");
                Debug.WriteLine(skip);
                Debug.WriteLine("==========================");

                IQueryable<Contact> _contacts = contacts.Skip(skip).Take(_emailbatchcount);

                string[] recipients = _contacts.Select(x => x.Email).ToArray();

                EmailHelper.SendEmail(campaign, recipients, _sendgridapikey);

                //foreach (var c in _contacts)
                //{
                //    EmailHelper.SendEmail(campaign, c.Email, _sendgridapikey);

                //    Debug.WriteLine(c.Email);
                //    counttotal++;
                //}

                // sleep for one second, SendGrid disallow more than 3K request per second
                Thread.Sleep(1000);

                skip += _emailbatchcount;
            }

            Debug.WriteLine("================================");
            Debug.WriteLine("Total Emails Sent: {counttotal}");
            Debug.WriteLine("================================");
        }


    }
}