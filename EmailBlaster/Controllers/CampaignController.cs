﻿using System;
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
        private string _sendgridapikey = ConfigurationManager.AppSettings["SendGridAPIKey"];
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
                // get existing campaign
                var o = new Campaign();

                // jason todo: use automapper here or tryupdateodel
                o.Name = model.Campaign.Name;
                o.Sender = model.Campaign.Sender;
                o.Subject = model.Campaign.Subject;
                o.HtmlBody = model.Campaign.HtmlBody;
                o.TextBody = model.Campaign.TextBody;
                o.DateScheduled = model.Campaign.DateScheduled;

                db.Campaigns.Add(o);
                await db.SaveChangesAsync();
            }

            return View();
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
        public ActionResult Edit(CampaignViewModel model, string command)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // get existing campaign
                    var o = db.Campaigns.SingleOrDefault(x => x.Id == model.Campaign.Id);

                    // jason todo: use automapper here or tryupdateodel
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
                            EmailHelper.SendEmail(model.Campaign, model.SendTestEmail.Split(',',';') , _sendgridapikey);
                            break;
                        case "Schedule Email":
                            break;
                        case "Save":
                            break;
                        case "Send Now":
                            o.DateCreated = DateTime.Now;
                            SendCampaign(o);
                            break;
                    }

                    db.SaveChanges();
                }
            }
            catch (Exception)
            {
                throw;
            }

            return View(model);
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

                foreach (var c in _contacts)
                {
                    EmailHelper.SendEmail(campaign, c.Email.Split(',', ';'), _sendgridapikey);

                    Debug.WriteLine(c.Email);
                    counttotal++;
                }

                // sleep for one second, SendGrid disallow more than 3K request per second
                Thread.Sleep(1000);

                skip += _emailbatchcount;
            }

            Debug.WriteLine("XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX");
            Debug.WriteLine(counttotal);
            Debug.WriteLine("XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX");
        }


    }
}