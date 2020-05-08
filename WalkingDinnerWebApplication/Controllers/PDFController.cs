using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WalkingDinnerWebApplication.DAL;
using WalkingDinnerWebApplication.Models;
using WalkingDinnerWebApplication.PDF_Printer;

namespace WalkingDinnerWebApplication.Controllers
{
    public class PDFController : Controller
    {
        WalkingDinnerContext db = new WalkingDinnerContext();

        public ActionResult GeneratePDF(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            EventSchema schema = db.EventSchemas.Find(id);

            if (schema == null)
            {
                return HttpNotFound();
            }

            using (MemoryStream stream = new MemoryStream())
            {
                var document = PDF.Print(schema);
                document.Save(stream, false);
                return File(stream.ToArray(), "application/pdf");
            }
        }
    }
}