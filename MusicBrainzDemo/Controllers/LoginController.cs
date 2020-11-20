using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace MusicBrainzDemo.Controllers
{
    public class LoginController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        // Handle the logout link
        public ActionResult Logout()
        {
            MusicBrainzDemo.Helpers.LoginHelper.RemoveSession();
            return RedirectToAction("Index", "Home");
        }

        // Fetch content from MusicBrainz login page
        private HttpResponseMessage MusicBrainz_Authenticate()
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls
                                                   | SecurityProtocolType.Tls11
                                                   | SecurityProtocolType.Tls12
                                                   | SecurityProtocolType.Ssl3;
            
            // Setup url to reach MusicBrainz authetincation page
            string html = String.Empty;
            string url = "http://" + HttpContext.Request.Url.Host + ":" + HttpContext.Request.Url.Port;
            StringBuilder sbUrl = new StringBuilder();

            sbUrl.Append("https://musicbrainz.org/oauth2/authorize?");
            sbUrl.Append("client_id=mJAE1ZhAnqLnC3Pg6Hnt9Q85V0PA-Qgo&");
            sbUrl.Append("response_type=code&");
            sbUrl.Append("redirect_uri=" + url + "&");
            sbUrl.Append("grant_type=authorization_code&");
            sbUrl.Append("scope=profile");

            // Prepare web request
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(sbUrl.ToString());            
            using (HttpWebResponse webResponse = (HttpWebResponse)request.GetResponse())
            using (Stream stream = webResponse.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                html = reader.ReadToEnd();
            }

            // Get response from authentication page
            var response = new HttpResponseMessage();
            response.Content = new StringContent(html);
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");

            return response;
        }

        // Go to MusicBrainz webpage and authenticate
        public ContentResult AuthenticateToMusicBrainz()
        {
            string response = MusicBrainz_Authenticate().Content.ReadAsStringAsync().Result;
            return Content(response);
        }   
    }
}