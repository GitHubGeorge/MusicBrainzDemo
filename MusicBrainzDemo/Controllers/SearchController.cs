using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.IO;
using System.Text;
using System.Net.Http;
using System.Web.Mvc;
using MusicBrainzDemo.ViewModels.Search;
using MusicBrainzDemo.Helpers;
using System.Web.Script.Serialization;


namespace MusicBrainzDemo.Controllers
{
    public class SearchController : Controller
    {       

        [HttpPost]
        // Search for an artist
        public string SearchArtist(SearchViewModel model)
        {
            if (model == null) return string.Empty;

            List<ArtistList> result = new MusicBrainzHelper().SearchArtist(model.ArtistName);

            string myJsonString = (new JavaScriptSerializer()).Serialize(result);            

            return myJsonString;
        }

        [HttpPost]
        // Search for songs given an artist
        public string SearchSongs(string artistID)
        {
            if (string.IsNullOrEmpty(artistID)) return string.Empty;

            List<SongList> result = new MusicBrainzHelper().SearchSongs(artistID);
                        
            string myJsonString = (new JavaScriptSerializer()).Serialize(result);

            return myJsonString;
        }
    }
}