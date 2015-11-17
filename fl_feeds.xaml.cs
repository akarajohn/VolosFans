using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.IO;
using System.Xml;
using System.ServiceModel.Syndication;
using Microsoft.Phone.Tasks;

namespace Volos_Fans
{
    public partial class fl_feeds : PhoneApplicationPage
    {
        public fl_feeds()
        {
            InitializeComponent();


            // Set the data context of the listbox control to the sample data
            DataContext = App.ViewModel;
            this.loadFeedButton_Click(null, null);

        }

        WebClient webClient = new WebClient();
        
 
        // Click handler that runs when the 'Load Feed' or 'Refresh Feed' button is clicked. 
        private void loadFeedButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            this.progressBar.Opacity = 1;
            // WebClient is used instead of HttpWebRequest in this code sample because 
            // the implementation is simpler and easier to use, and we do not need to use 
            // advanced functionality that HttpWebRequest provides, such as the ability to send headers.
            

            // Subscribe to the DownloadStringCompleted event prior to downloading the RSS feed.
            webClient.DownloadStringCompleted += new DownloadStringCompletedEventHandler(webClient_DownloadStringCompleted);

            // Download the RSS feed. DownloadStringAsync was used instead of OpenStreamAsync because we do not need 
            // to leave a stream open, and we will not need to worry about closing the channel. 
            webClient.DownloadStringAsync(new System.Uri("http://www.volosfans.com/feeds/posts/default/-/%CE%92%27%20%CE%95%CE%B8%CE%BD%CE%B9%CE%BA%CE%AE?alt=rss"));
        }

        // Event handler which runs after the feed is fully downloaded.
        private void webClient_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    // Showing the exact error message is useful for debugging. In a finalized application, 
                    // output a friendly and applicable string to the user instead. 
                    MessageBox.Show("Ελέγξτε τη σύνδεση με τα δεδομένα");
                });
            }
            else
            {
                // Save the feed into the State property in case the application is tombstoned. 
                this.State["feed"] = e.Result;

                UpdateFeedList(e.Result);
            }

            this.progressBar.Opacity = 0;
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            // First, check whether the feed is already saved in the page state.
            if (this.State.ContainsKey("feed"))
            {
                // Get the feed again only if the application was tombstoned, which means the ListBox will be empty.
                // This is because the OnNavigatedTo method is also called when navigating between pages in your application.
                // You would want to rebind only if your application was tombstoned and page state has been lost. 
                if (this.fl.Items.Count == 0)
                {
                    UpdateFeedList(State["feed"] as string);
                }
            }
        }

        private void UpdateFeedList(string feedXML)
        {
            // Load the feed into a SyndicationFeed instance.
            StringReader stringReader = new StringReader(feedXML);
            XmlReader xmlReader = XmlReader.Create(stringReader);
            SyndicationFeed feed = SyndicationFeed.Load(xmlReader);

            // In Windows Phone OS 7.1 or later versions, WebClient events are raised on the same type of thread they were called upon. 
            // For example, if WebClient was run on a background thread, the event would be raised on the background thread. 
            // While WebClient can raise an event on the UI thread if called from the UI thread, a best practice is to always 
            // use the Dispatcher to update the UI. This keeps the UI thread free from heavy processing.
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                // Bind the list of SyndicationItems to our ListBox.
                fl.ItemsSource = ParseFeed(feed);
                ApplicationBarIconButton btn1 = (ApplicationBarIconButton)ApplicationBar.Buttons[0];
                btn1.Text = "ανανέωση";
                btn1.IconUri = new Uri("/Assets/Images/Tiles/refresh.png", UriKind.Relative);
            });
        }
        private static IEnumerable<MyFeedItem> ParseFeed(SyndicationFeed feed)
        {
            return feed.Items.Select(item => new MyFeedItem
            {
                Title = item.Title.Text,
                Link = item.Links.First().Uri,
                Description = item.Summary.Text,
                PubDate = item.PublishDate.AddHours(2).ToString("HH:mm dd/MM/yyyy")
            });
        }

        // The SelectionChanged handler for the feed items 
        private void fl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox listBox = sender as ListBox;

            if (listBox != null && listBox.SelectedItem != null)
            {
                    // Get the SyndicationItem that was tapped.
                    MyFeedItem sItem = (MyFeedItem)listBox.SelectedItem;
                
                // Set up the page navigation only if a link actually exists in the feed item.
                if (sItem.Link.IsAbsoluteUri)
                {
                    // Get the associated URI of the feed item.
                    Uri uri = sItem.Link;

                    // Create a new WebBrowserTask Launcher to navigate to the feed item. 
                    // An alternative solution would be to use a WebBrowser control, but WebBrowserTask is simpler to use. 
                    WebBrowserTask webBrowserTask = new WebBrowserTask();
                    webBrowserTask.Uri = uri;
                    webBrowserTask.Show();
                    
                   
                }
            }
        }

        private void ApplicationBarIconButton_Click(object sender, EventArgs e)
        {
            ApplicationBarIconButton btn1 = (ApplicationBarIconButton)ApplicationBar.Buttons[0];
            if (btn1.Text == "ανανέωση")
            {
                btn1.Text = "ακύρωση";
                btn1.IconUri = new Uri("/Assets/Images/Tiles/appbar.cancel.rest.png", UriKind.Relative);
                try
                {
                    WebClient webClient = new WebClient();
                    webClient.DownloadStringCompleted += new DownloadStringCompletedEventHandler(webClient_DownloadStringCompleted);
                    webClient.DownloadStringAsync(new System.Uri("http://www.volosfans.com/feeds/posts/default/-/%CE%92%27%20%CE%95%CE%B8%CE%BD%CE%B9%CE%BA%CE%AE?alt=rss"));
                }
                catch
                {
                    MessageBox.Show("Αδύνατη η ανανέωση των ειδήσεων");
                    btn1.Text = "ανανέωση";
                    btn1.IconUri = new Uri("/Assets/Images/Tiles/refresh.png", UriKind.Relative);
                }
            }
            else if (btn1.Text == "ακύρωση")
            {
                webClient.CancelAsync();
                btn1.Text = "ανανέωση";
                btn1.IconUri = new Uri("/Assets/Images/Tiles/refresh.png", UriKind.Relative);

            }
            
        }


    }
}