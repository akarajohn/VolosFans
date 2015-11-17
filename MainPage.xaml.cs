using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using System.IO;
using System.ServiceModel.Syndication;
using System.Xml;
using Microsoft.Phone.Tasks;
using System.Windows.Media.Imaging;
using System.Collections.ObjectModel;
using Microsoft.Phone.Shell;
using System.ComponentModel;



namespace Volos_Fans
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            

            InitializeComponent();
            
            // Set the data context of the listbox control to the sample data
            DataContext = App.ViewModel;
            this.Loaded += new RoutedEventHandler(MainPage_Loaded);
            this.loadFeedButton_Click(null, null);

            ApplicationBar = new ApplicationBar();

            ApplicationBarIconButton button1 = new ApplicationBarIconButton();
            button1.IconUri = new Uri("/Assets/Images/Tiles/refresh.png", UriKind.Relative);
            button1.Text = "ανανέωση";
            ApplicationBar.Buttons.Add(button1);
            button1.Click += new EventHandler(ApplicationBarIconButton_Click);
     
        }

        WebClient webClient = new WebClient();
        /*----------------------------------------------------------------------------------- */
        // Click handler that runs when the 'Load Feed' or 'Refresh Feed' button is clicked. 
        private void loadFeedButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            this.progressBar.Opacity = 1;
            // WebClient is used instead of HttpWebRequest in this code sample because 
            // the implementation is simpler and easier to use, and we do not need to use 
            // advanced functionality that HttpWebRequest provides, such as the ability to send headers.
            // WebClient webClient = new WebClient();


            // Subscribe to the DownloadStringCompleted event prior to downloading the RSS feed.
            webClient.DownloadStringCompleted += new DownloadStringCompletedEventHandler(webClient_DownloadStringCompleted);

            // Download the RSS feed. DownloadStringAsync was used instead of OpenStreamAsync because we do not need 
            // to leave a stream open, and we will not need to worry about closing the channel. 
            webClient.DownloadStringAsync(new System.Uri("http://www.volosfans.com/feeds/posts/default?alt=rss"));
        }
        // Event handler which runs after the feed is fully downloaded.
        protected void webClient_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            ApplicationBarIconButton btn = (ApplicationBarIconButton)ApplicationBar.Buttons[0];
            if (btn.Text == "ακύρωση")
            {
                btn.Text = "ανανέωση";
                btn.IconUri = new Uri("/Assets/Images/Tiles/refresh.png", UriKind.Relative);
            }
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

        // This method determines whether the user has navigated to the application after the application was tombstoned.
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            // First, check whether the feed is already saved in the page state.
            if (this.State.ContainsKey("feed"))
            {
                // Get the feed again only if the application was tombstoned, which means the ListBox will be empty.
                // This is because the OnNavigatedTo method is also called when navigating between pages in your application.
                // You would want to rebind only if your application was tombstoned and page state has been lost. 
                if (this.lst.Items.Count == 0)
                {
                    UpdateFeedList(State["feed"] as string);
                }
            }
        }

        // This method sets up the feed and binds it to our ListBox. 
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
                lst.ItemsSource = ParseFeed(feed);
                ApplicationBarIconButton btn = (ApplicationBarIconButton)ApplicationBar.Buttons[0];
                btn.Text = "ανανέωση";
                btn.IconUri = new Uri("/Assets/Images/Tiles/refresh.png", UriKind.Relative);

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
        private void lst_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
            ListBox listBox = sender as  ListBox;
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
        
        /* ---------------------------------------------------------------------------------*/


        // Load data for the ViewModel Items
        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (!App.ViewModel.IsDataLoaded)
            {
                App.ViewModel.LoadData();
            }
        }

        /*-----------------------------------------------------------*/


        /*----------------------------------------------------------- */

        public class TileItem
        {
            public string ImageUri
            {
                get;
                set;
            }

            public string Title
            {
                get;
                set;
            }

            public string Notification
            {
                get;
                set;
            }

            public bool DisplayNotification
            {
                get
                {
                    return !string.IsNullOrEmpty(this.Notification);
                }
            }

            public string Message
            {
                get;
                set;
            }

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/fl_feeds.xaml", UriKind.Relative));

        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/followers.xaml", UriKind.Relative));
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/myBrowser.xaml", UriKind.Relative));
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/diafora.xaml", UriKind.Relative));
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/UWrite.xaml", UriKind.Relative));
        }

        private void Button_Click_6(object sender, RoutedEventArgs e)
        {

            NavigationService.Navigate(new Uri("/History.xaml", UriKind.Relative));

        }

        private void ApplicationBarIconButton_Click(object sender, EventArgs e)
        {
           ApplicationBarIconButton btn = (ApplicationBarIconButton)ApplicationBar.Buttons[0];
            if (btn.Text == "ανανέωση")
            {
                btn.Text = "ακύρωση";
                btn.IconUri = new Uri("/Assets/Images/Tiles/appbar.cancel.rest.png", UriKind.Relative);
                try
                {
                    WebClient webClient = new WebClient();
                    webClient.DownloadStringCompleted += new DownloadStringCompletedEventHandler(webClient_DownloadStringCompleted);
                    webClient.DownloadStringAsync(new System.Uri("http://www.blogger.com/feeds/1168979936941427528/posts/default?alt=rss"));
                }
                catch {
                    MessageBox.Show("Αδύνατη η ανανέωση των ειδήσεων");
                    btn.Text = "ανανέωση";
                    btn.IconUri = new Uri("/Assets/Images/Tiles/refresh.png", UriKind.Relative);
                }
            }
            else if (btn.Text == "ακύρωση")
            {
                webClient.CancelAsync();
                btn.Text = "ανανέωση";
                btn.IconUri = new Uri("/Assets/Images/Tiles/refresh.png", UriKind.Relative);

            }
            
        }

        private void Panorama_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int currentIndex = ((Panorama)sender).SelectedIndex;
            switch (currentIndex)
            {
                case 0:
                    ApplicationBar.IsVisible = true;
                    break;
                case 1:
                case 2:
                    ApplicationBar.IsVisible = false;
                    break;

            }
        }

        private void Button_Click_Gmail(object sender, RoutedEventArgs e)
        {

            EmailComposeTask emailComposeTask = new EmailComposeTask();

            emailComposeTask.Subject = "";
            emailComposeTask.Body = "";
            emailComposeTask.To = "volosfans@gmail.com";


            emailComposeTask.Show();
        }

        private void Button_Click_Web(object sender, RoutedEventArgs e)
        {
            
            WebBrowserTask webBrowserTask = new WebBrowserTask();
            webBrowserTask.Uri = new Uri("http://www.volosfans.com/");
            webBrowserTask.Show();
            
        }

        private void Button_Click_MyGmail(object sender, RoutedEventArgs e)
        {
            EmailComposeTask emailComposeTask = new EmailComposeTask();

            emailComposeTask.Subject = "";
            emailComposeTask.Body = "";
            emailComposeTask.To = "akarajohn1@gmail.com";


            emailComposeTask.Show();
        }

        private void Button_Click_Rate(object sender, RoutedEventArgs e)
        {
            MarketplaceReviewTask review = new MarketplaceReviewTask();
            review.Show();
        }

        private void Button_Click_Fb(object sender, RoutedEventArgs e)
        {
            Uri uri = new Uri("http://www.facebook.com/pages/VolosFanscom/170697769658287");

            WebBrowserTask webBrowserTask = new WebBrowserTask();
            webBrowserTask.Uri = uri;
            webBrowserTask.Show();
        }
       
    }
}