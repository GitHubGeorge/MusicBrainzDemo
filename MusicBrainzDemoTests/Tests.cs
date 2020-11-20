using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.IO;
using System.Web;
using System.Web.SessionState;
using System.Web.Mvc;
using System.Reflection;
using MusicBrainzDemo;
using MusicBrainzDemo.Controllers;
using Moq;

namespace MusicBrainzDemoTests
{
    [TestClass]
    public class Tests
    {       
        private HttpContext FakeHttpContext()
        {
            var httpRequest = new HttpRequest("", "http://example.com/", "");
            var stringWriter = new StringWriter();
            var httpResponse = new HttpResponse(stringWriter);
            var httpContext = new HttpContext(httpRequest, httpResponse);

            var sessionContainer = new HttpSessionStateContainer("id", new SessionStateItemCollection(),
                                                    new HttpStaticObjectsCollection(), 10, true,
                                                    HttpCookieMode.AutoDetect,
                                                    SessionStateMode.InProc, false);

            httpContext.Items["AspSession"] = typeof(HttpSessionState).GetConstructor(
                                        BindingFlags.NonPublic | BindingFlags.Instance,
                                        null, CallingConventions.Standard,
                                        new[] { typeof(HttpSessionStateContainer) },
                                        null)
                                .Invoke(new object[] { sessionContainer });

            return httpContext;
        }

        [TestMethod]
        public void HomeController_SetupSessionWorks()
        {
            // Arrange            
            Mock<HttpRequestBase> _mockRequest = new Mock<HttpRequestBase>();

            var mockHttpContext = new Mock<HttpContextBase>();
            mockHttpContext.Setup(c => c.Request).Returns(_mockRequest.Object);

            var mockControllerContext = new Mock<ControllerContext>();
            mockControllerContext.Setup(c => c.HttpContext).Returns(mockHttpContext.Object);

            var queryString = new NameValueCollection { { "code", "dummyAccessToken" } };
            _mockRequest.Setup(r => r.QueryString).Returns(queryString);

            HttpContext.Current = FakeHttpContext();

            HomeController HomeCon = new HomeController();
            HomeCon.ControllerContext = mockControllerContext.Object;

            // Act
            ViewResult result = HomeCon.Index() as ViewResult;
            
            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(HttpContext.Current.Session["accessToken"], "dummyAccessToken");            
        }

        [TestMethod]
        public void LoginController_SessionClearsOnLogout()
        {
            // Arrange            
            Mock<HttpRequestBase> _mockRequest = new Mock<HttpRequestBase>();

            var mockHttpContext = new Mock<HttpContextBase>();
            mockHttpContext.Setup(c => c.Request).Returns(_mockRequest.Object);

            var mockControllerContext = new Mock<ControllerContext>();
            mockControllerContext.Setup(c => c.HttpContext).Returns(mockHttpContext.Object);

            var queryString = new NameValueCollection { { "code", "dummyAccessToken" } };
            _mockRequest.Setup(r => r.QueryString).Returns(queryString);

            HttpContext.Current = FakeHttpContext();

            HomeController HomeCon = new HomeController();
            LoginController LoginCon = new LoginController();

            HomeCon.ControllerContext = mockControllerContext.Object;
            LoginCon.ControllerContext = mockControllerContext.Object;

            // Attempt to set access token
            ViewResult HomeConResult = HomeCon.Index() as ViewResult;
            Assert.AreEqual(HttpContext.Current.Session["accessToken"], "dummyAccessToken");

            // Act
            RedirectToRouteResult LoginConResult = LoginCon.Logout() as RedirectToRouteResult;

            // Assert
            Assert.IsNotNull(LoginConResult);
            Assert.AreEqual(LoginConResult.RouteValues.Values.ElementAt(0), "Index");
            Assert.AreEqual(LoginConResult.RouteValues.Values.ElementAt(1), "Home");
            Assert.IsNull(HttpContext.Current.Session["accessToken"]);
        }

        [TestMethod]
        public void MusicBrainzHelper_SearchArtistCanExtractFromXML()
        {
            // Arrange
            string webRespose = "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?><metadata created=\"2020-11-20T00:53:34.239Z\" xmlns=\"http://musicbrainz.org/ns/mmd-2.0#\" xmlns:ns2=\"http://musicbrainz.org/ns/ext#-2.0\"><artist-list count=\"3\" offset=\"0\"><artist id=\"ca891d65-d9b0-4258-89f7-e6ba29d83767\" type=\"Group\" type-id=\"e431f5f6-b5d2-343d-8b36-72607fffb74b\" ns2:score=\"100\"><name>Iron Maiden</name><sort-name>Iron Maiden</sort-name><country>GB</country><area id=\"8a754a16-0027-3a29-b6d7-2b40ea0481ed\" type=\"Country\" type-id=\"06dd0ae4-8c74-30bb-b43d-95dcedf961de\"><name>United Kingdom</name><sort-name>United Kingdom</sort-name><life-span><ended>false</ended></life-span></area><begin-area id=\"f999fe3f-b2e5-4a2a-8f5b-eed90aae23d9\" type=\"District\" type-id=\"84039871-5e47-38ca-a66a-45e512c8290f\"><name>Leyton</name><sort-name>Leyton</sort-name><life-span><ended>false</ended></life-span></begin-area><disambiguation>English heavy metal band</disambiguation><isni-list><isni>0000000123245420</isni></isni-list><life-span><begin>1975-12-25</begin><ended>false</ended></life-span><alias-list><alias sort-name=\"鉄の処女\">鉄の処女</alias><alias sort-name=\"Ironmaiden\">Ironmaiden</alias><alias sort-name=\"Maiden\">Maiden</alias></alias-list><tag-list><tag count=\"0\"><name>rock</name></tag><tag count=\"21\"><name>heavy metal</name></tag><tag count=\"0\"><name>progressive metal</name></tag><tag count=\"4\"><name>metal</name></tag><tag count=\"3\"><name>british</name></tag><tag count=\"0\"><name>uk</name></tag><tag count=\"0\"><name>hard rock</name></tag><tag count=\"0\"><name>power metal</name></tag><tag count=\"4\"><name>nwobhm</name></tag><tag count=\"0\"><name>english</name></tag><tag count=\"0\"><name>awesome</name></tag><tag count=\"0\"><name>rock and indie</name></tag><tag count=\"5\"><name>new wave of british heavy metal</name></tag><tag count=\"0\"><name>825646222902</name></tag></tag-list></artist><artist id=\"7c3762a3-51f8-4cf3-8565-1ee26a90efe2\" type=\"Group\" type-id=\"e431f5f6-b5d2-343d-8b36-72607fffb74b\" ns2:score=\"93\"><name>Iron Maiden</name><sort-name>Iron Maiden</sort-name><area id=\"9b4cb463-9777-46c3-8190-e1cb3da2749f\" type=\"City\" type-id=\"6fd8f29a-3d0a-32fc-980d-ea697b69da78\"><name>Basildon</name><sort-name>Basildon</sort-name><life-span><ended>false</ended></life-span></area><disambiguation>late 60s prog rock/doom metal band</disambiguation><life-span><begin>1966</begin><ended>false</ended></life-span><alias-list><alias locale=\"ja\" sort-name=\"アイアン・メイデン\" type=\"Artist name\" type-id=\"894afba6-2816-3c24-8072-eadb66bd04bc\" primary=\"primary\">アイアン・メイデン</alias></alias-list></artist><artist id=\"8a12e54c-229f-4dd1-92f1-bef0b13495ee\" ns2:score=\"78\"><name>Iron Maiden Strength</name><sort-name>Iron Maiden Strength</sort-name><disambiguation>lolicore</disambiguation><life-span><ended>false</ended></life-span><tag-list><tag count=\"1\"><name>lolicore</name></tag></tag-list></artist></artist-list></metadata>";
            var musicBrainzHelper = new Moq.Mock<MusicBrainzDemo.Helpers.MusicBrainzHelper> { CallBase = true };
            musicBrainzHelper.Setup(s => s.SearchArtistInMusicBrainz(It.IsAny<string>())).Returns(webRespose);

            // Act
            var result = musicBrainzHelper.Object.SearchArtist("iron maiden") as List<MusicBrainzDemo.Helpers.ArtistList>;
            
            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(List<MusicBrainzDemo.Helpers.ArtistList>));
            Assert.AreEqual(result.Count, 3);
            musicBrainzHelper.Verify(x => x.SearchArtistInMusicBrainz(It.IsAny<string>()), Times.Once());            
            Assert.AreEqual(result.ElementAt(0).ArtistName.ToLower(), "iron maiden");
        }

        [TestMethod]
        public void MusicBrainzHelper_SearchArtistDoesNotWorkWithNoSearchCriteria()
        {
            // Arrange        
            var musicBrainzHelper = new Moq.Mock<MusicBrainzDemo.Helpers.MusicBrainzHelper> { CallBase = true };            

            // Act
            var result = musicBrainzHelper.Object.SearchArtist("") as List<MusicBrainzDemo.Helpers.ArtistList>;

            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(List<MusicBrainzDemo.Helpers.ArtistList>));
            Assert.AreEqual(result.Count, 0);
            musicBrainzHelper.Verify(x => x.SearchArtistInMusicBrainz(It.IsAny<string>()), Times.Never);            
        }
    }
}
