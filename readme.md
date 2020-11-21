
# MusicBrainz demo

This is a very simple test for the MusicBrainz API. It was made using Visual Studio 2013 and demonstrates the most basic actions you can perfom using .NET. This implementation can only search for an artist and retrieve their songs. It will then display a little graphic made with [ChartJS](https://www.chartjs.org/), whish shows in scatter plot the number of words in every title retrieved. 

The demo is a bit temperamental due to some issues with the API, among which is the authentication process. Although it authenticates to MusicBrainz through login and retrieves the access token, it seems that it is not responding well whenever someone performs multiple searches. 

The demo uses a custom method with a WebRequest for authentication and artist searches, as well as the MusicBrainz client for song searches. Both methods have shown some erratic behaviour. The demo works somewhat but freezes whenever someone makes a second search (a second click in the results list). 

I wanted to add more functionality but it was simply too much trouble...

## How to use

Once you run the demo, you'll see the index page, click the login link in the menu bar or the link inside the grey area. 
 
![Index page](https://raw.githubusercontent.com/GitHubGeorge/MusicBrainzDemo/master/Screenshots/01_IndexPage.png)

Now click the blue login button, you should be redirected to the MusicBrainz login screen.
![Login button](https://raw.githubusercontent.com/GitHubGeorge/MusicBrainzDemo/master/Screenshots/02_LoginButton.png)

When you see this page you should type your MusicBrainz credentials (you will need a MusicBrainz login account). **You might see an error message** telling you the form has expired, just re-enter your credentials.
![MusicBrainz login page](https://raw.githubusercontent.com/GitHubGeorge/MusicBrainzDemo/master/Screenshots/03_MusicBrainzLogin.png)

On this screen you should grant permission so that MusicBrainz demo can access your MusicBraiz account. This is needed to retrieve the access token.
![Grand permission to access your account](https://raw.githubusercontent.com/GitHubGeorge/MusicBrainzDemo/master/Screenshots/04_GrandPermission.png)

If everything has gone right you should be redirected back to the demo, where you will see a text field and a search button. You can search for an artist now, try 'Iron Maiden' and click search. You should be presented with a list similar to the one below.
![Search artist](https://raw.githubusercontent.com/GitHubGeorge/MusicBrainzDemo/master/Screenshots/05_SearchArtist.png)

Now if you click on an artist, and scroll down a bit you will see a list of songs.
![Search song](https://raw.githubusercontent.com/GitHubGeorge/MusicBrainzDemo/master/Screenshots/06_SearchSong.png)

At this point it would have been preferred if clicking on a song performed something else with the API but I stopped due to various problems I was having with it. If you try various extra searches the demo will hang or appear to be slow. **You will need to logout from MusicBrainz and login again to search a new artist.** 

To provide an alternative result I put a small example showing a scatter plot diagram.  If you scroll down a bit more you will see a scatter plot with some basic data taken only from the list of songs, You can choose between: the number of words in each tile or the min, max and average length of words among all the titles of the selected artist.
![Scatter plot](https://raw.githubusercontent.com/GitHubGeorge/MusicBrainzDemo/master/Screenshots/07_ScatterPlot.png)

## Known issues 

- I think authentication on MusicBrainz is a bit problematic, although it works it has problems. It uses OAuth but both the MusicBrainz client described in their wiki and almost every method I tried by Googling on the Internet didn't satisfy me a lot.
- I haven't tried using a proper OAuth plugin in Visual Studio to test the authentication process better, in order to keep the project clean from extra packages and dependencies.
- Multiple searches freeze the demo and I think this might be due to how I use the web requests but also how the API handles them. You will have to logout and login to make a new search.
