using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace MusicBrainzDemo.Controllers
{
    public class HomeController : Controller
    {        
        public ActionResult Index()
        {
            // Get MusicBrainz access token from URL query 
            string accessToken = Request.QueryString["code"];


            // If access token exists store it
            if (accessToken != null)
            {
                MusicBrainzDemo.Helpers.LoginHelper.SetLoggedIn(accessToken);
            }            

            return View();
        }
    }
}