using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MusicBrainzDemo.Helpers
{
    public static class LoginHelper
    {
        private static string _accessToken { get; set; }

        // Check if user is logged in in MusicBrainz
        public static bool IsLoggedIn()
        {                    
            return HttpContext.Current.Session["accessToken"] != null;

        }
       
        // Get access token
        public static string GetAccessToken()
        {
            return (String.IsNullOrEmpty(_accessToken) ? string.Empty : _accessToken);
        }

        // Clear session upon logout
        public static void RemoveSession()
        {
            HttpContext.Current.Session.RemoveAll();
        }

        // Set user as logged in 
        public static void SetLoggedIn(string accessToken)
        {
            if (HttpContext.Current.Session["accessToken"] == null && !string.IsNullOrEmpty(accessToken))
            {
                HttpContext.Current.Session["accessToken"] = accessToken;
                _accessToken = accessToken;
            }
        }
    }
}