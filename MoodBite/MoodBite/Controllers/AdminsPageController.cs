using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MoodBite.Controllers
{
    public class AdminsPageController : Controller
    {
        [Authorize(Roles = "Admin")]
        public ActionResult AdminsHome()
        {
            return View();
        }
    }
}