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
using System.Xml;
using System.Threading.Tasks;
using MetaBrainz.MusicBrainz;

namespace MusicBrainzDemo.Helpers
{
    public class ArtistList
    {
        public string ArtistID { get; set; }
        public string ArtistName { get; set; }
    }

    public class SongList
    {
        public string SongID { get; set; }
        public string SongName { get; set; }
    }

    public class MusicBrainzHelper
    {
        const string rootURL = "http://www.musicbrainz.org/ws/2/";

        private string PerformWebRequest(string url)
        {
            if (string.IsNullOrEmpty(url)) return null;

            byte[] data = Encoding.GetEncoding("UTF-8").GetBytes(url);

            // Create a web-encoded URL 		
            Uri uri = new Uri(string.Format(url));
            var request = (HttpWebRequest)WebRequest.Create(uri);

            // Set the credentials and other information about the request
            request.Credentials = CredentialCache.DefaultCredentials;
            request.Headers.Add(HttpRequestHeader.Authorization, "Bearer " + LoginHelper.GetAccessToken());
            request.Headers.Add("client_id", "mJAE1ZhAnqLnC3Pg6Hnt9Q85V0PA");
            request.Headers.Add("client_secret", "ahb54cxChyLFXebAX4BFMezbm6IgXXac");
            request.ContentType = "application/json";
            request.Method = "POST";
            request.ContentLength = data.Length;
            request.UserAgent = @"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/51.0.2704.106 Safari/537.36";

            // Get the stream to write request data
            Stream stream = request.GetRequestStream();
            stream.Write(data, 0, data.Length);
            stream.Close();

            // Get the response.
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            // Get the status (might be needed)
            string status = response.StatusDescription;

            // Get the stream containing content returned by the server
            Stream dataStream = response.GetResponseStream();

            // Open the stream using a StreamReader for easy access
            StreamReader reader = new StreamReader(dataStream);

            // Read the content
            string responseFromServer = reader.ReadToEnd();

            // Cleanup the streams and the response
            reader.Close();
            dataStream.Close();
            response.Close();

            return responseFromServer;
        }

        // Extract artist names from XML
        private List<ArtistList> GetArtists(string XMLStream)
        {
            List<ArtistList> artists = new List<ArtistList>();

            if (string.IsNullOrEmpty(XMLStream)) return artists;

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(XMLStream);
            XmlNodeList titles = xmlDoc.GetElementsByTagName("artist");
            foreach (XmlNode node in titles)
            {
                // Add tuple of id-artist name
                artists.Add(new ArtistList { ArtistID = node.Attributes[0].InnerText, ArtistName = node.FirstChild.InnerText });
            }

            return artists;
        }

        // Extract song names from MusicBrainz list
        private List<SongList> GetArtistSongsFromResult(IReadOnlyList<MetaBrainz.MusicBrainz.Interfaces.Entities.IWork> result)
        {
            List<SongList> songs = new List<SongList>();

            if (result == null) return songs;

            foreach (var item in result)
            {
                songs.Add(new SongList { SongID = item.MbId.ToString(), SongName = item.Title });
            }

            return songs;
        }

        // Call MusicBrainz API to get artist songs
        private List<SongList> GetArtistSongs(string artistID)
        {
            if (string.IsNullOrEmpty(artistID)) return new List<SongList>();

            // This is taken from Github documentation
            // https://github.com/Zastai/MetaBrainz.MusicBrainz/blob/master/UserGuide.md

            var oa = new OAuth2();
            oa.ClientId = "mJAE1ZhAnqLnC3Pg6Hnt9Q85V0PA-Qgo";
            // If using a local MusicBrainz server instance, make sure to set up the correct address and port.  
            var url = oa.CreateAuthorizationRequest(OAuth2.OutOfBandUri, AuthorizationScope.Rating | AuthorizationScope.Tag);
            var at = oa.GetBearerToken(LoginHelper.GetAccessToken(), "ahb54cxChyLFXebAX4BFMezbm6IgXXac", OAuth2.OutOfBandUri);
            var q = new MetaBrainz.MusicBrainz.Query("Red Stapler", "19.99", "mailto:milton.waddams@initech.com");
            q.BearerToken = at.AccessToken;
            var artist = q.BrowseArtistWorks(new Guid(artistID));
            q.Close();

            return GetArtistSongsFromResult(artist.Results);
        }

        public virtual string SearchArtistInMusicBrainz(string url)
        {
            return PerformWebRequest(url);
        }

        // Search MusicBrainz for an artist's name
        public List<ArtistList> SearchArtist(string artistName)
        {
            if (string.IsNullOrEmpty(artistName)) return new List<ArtistList>();

            // Form the URL
            string url = rootURL + "artist/?query=" + artistName;

            // Perform web request
            string webRequest = SearchArtistInMusicBrainz(url);

            // Extact artists
            List<ArtistList> artists = GetArtists(webRequest);

            return artists;
        }

        // Get songs from a certain artist
        public List<SongList> SearchSongs(string artistID)
        {
            if (string.IsNullOrEmpty(artistID)) return new List<SongList>();

            // Extact songs
            List<SongList> songs = GetArtistSongs(artistID);

            return songs;
        }

    }
}