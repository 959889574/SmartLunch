using SL.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SL.Utility;


namespace SL.CAS
{
    [SmartEyeLoginFilter]
    public class AdminController : Controller
    {
        LoginInfoModel model = SmartEyeLoginSession.GetSession();
       
        [SmartEyeLoginFilter]
        public ActionResult Index()
        {
            ViewData["OwnerId"] =model.UserName;
            return View();
        }


        public ActionResult Home() {
            return View();
        }

 

    }

}