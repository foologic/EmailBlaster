using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Runtime.CompilerServices;
using System.Web;
using System.Web.Mvc;
using EmailBlaster.Helpers;
using EmailBlaster.Models;
using EmailBlaster.Common;
using EmailBlaster.ViewModels;
using PagedList.EntityFramework;

namespace EmailBlaster.Controllers
{
    public class ContactController : Controller
    {
        private EmailBlasterContext db = new EmailBlasterContext();

        // GET: Contact
        public async Task<ActionResult> Index(string searchstring, int? page)
        {
            if (page == null) page = 1;

            var contacts = from c in db.Contacts select c;

            // filter
            if (!string.IsNullOrEmpty(searchstring))
            {
               contacts = contacts.Where(x => x.Email.Contains(searchstring));
            }

            // sort
            contacts = contacts.OrderBy(x => x.Email);

            return View(await contacts.OrderBy(x=>x.Email).ToPagedListAsync((int)page, 100));
        }

        // GET: Contact/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Contact contact = await db.Contacts.FindAsync(id);
            if (contact == null)
            {
                return HttpNotFound();
            }
            return View(contact);
        }

        // GET: Contact/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Contact/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Email,FirstName,LastName,Active,DateAdded,DateDeactivated,Unsubscribed,Bounced,MarkedSpam,InvalidMailbox")] Contact contact)
        {
            if (ModelState.IsValid)
            {
                db.Contacts.Add(contact);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(contact);
        }

        // GET: Contact/CreateBulk
        public ActionResult CreateBulk()
        {
            return View();
        }

        // POST: Contact/CreateBulk
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateBulk(HttpPostedFileBase upload)
        {
            try
            { 
                if (ModelState.IsValid)
                {
                    if (upload != null && upload.ContentLength > 0)
                    {
                        // save uploaded file
                        string filename = Server.MapPath("~/Files/uploads/" + upload.FileName);
                        upload.SaveAs(filename);

                        // parse uploaded file into strongly typed contacts 
                        IEnumerable<Contact> contacts = FileHelper.ParseNewContacts(filename);

                        // exclude already existing emails
                        var newcontacts = contacts.Where(x => !db.Contacts.Any(y => y.Email == x.Email)).ToList();

                        // insert new contacts in db
                        db.Contacts.AddRange(newcontacts);
                        db.SaveChanges();

                        ViewBag.Message = string.Format("{0} contacts imported.", newcontacts.Count);
                    }
                }

                return View();
            }
            catch (Exception)
            {
                
                throw;
            }

            return View();
        }

        // GET: Contact/Unsubscribe
        public ActionResult Unsubscribe()
        {
            var model = new UnsubscribeViewModel();
                         
            return View(model);
        }

        // POST: Contact/Unsubscribe
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Unsubscribe(HttpPostedFileBase upload, int suppresionReason)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (upload != null && upload.ContentLength > 0)
                    {
                        // save uploaded file
                        string filename = Server.MapPath("~/Files/uploads/" + upload.FileName);
                        upload.SaveAs(filename);

                        // parse uploaded file into strongly typed contacts 
                        IEnumerable<Contact> contacts = FileHelper.ParseUnsubscribedContacts(filename);

                        // loop through all emails
                        int count = 0;
                        foreach (var contact in contacts)
                        {
                            var c = db.Contacts.SingleOrDefault(x => x.Email == contact.Email);

                            if (c != null)
                            {
                                // set deactivated date to today
                                c.DateDeactivated = DateTime.Now;
                                c.Active = false;   

                                // assign reason for suppression
                                switch (suppresionReason)
                                {
                                    case (int)Enums.SuppressionReason.Unsubscribed:
                                        c.Unsubscribed = true;
                                        break;
                                    case (int)Enums.SuppressionReason.Bounced:
                                        c.Bounced = true;
                                        break;
                                    case (int)Enums.SuppressionReason.MarkedSpam:
                                        c.MarkedSpam = true;
                                        break;
                                    case (int)Enums.SuppressionReason.InvalidMailbox:
                                        c.InvalidMailbox = true;
                                        break;
                                    case (int)Enums.SuppressionReason.Blocked:
                                        c.Blocked = true;
                                        break;
                                }

                                // assign suppresion details
                                c.SuppressionReason = contact.SuppressionReason;

                                // commit changes to db
                                db.SaveChanges();

                                count++;
                            }
                        }
                        ViewBag.Message = string.Format("{0} contacts unsubscribed.", count);
                    }

                    var model = new UnsubscribeViewModel();

                    return View(model);
                }
            }
            catch (Exception)
            {

                throw;
            }

            return View();
        }

        // GET: Contact/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Contact contact = await db.Contacts.FindAsync(id);
            if (contact == null)
            {
                return HttpNotFound();
            }
            return View(contact);
        }

        // POST: Contact/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Email,FirstName,LastName,Active,DateAdded,DateDeactivated,Unsubscribed,Bounced,MarkedSpam,InvalidMailbox")] Contact contact)
        {
            if (ModelState.IsValid)
            {
                db.Entry(contact).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(contact);
        }

        // GET: Contact/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Contact contact = await db.Contacts.FindAsync(id);
            if (contact == null)
            {
                return HttpNotFound();
            }
            return View(contact);
        }

        // POST: Contact/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Contact contact = await db.Contacts.FindAsync(id);
            db.Contacts.Remove(contact);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
