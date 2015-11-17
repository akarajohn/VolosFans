using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using System.ServiceModel.Syndication;

namespace Volos_Fans
{
    public partial class myBrowser : PhoneApplicationPage
    {

        public myBrowser()
        {
            InitializeComponent();
            this.Loaded += myBrowser_Loaded;
            
        }

        void myBrowser_Loaded(object sender, RoutedEventArgs e)
        {
            browser.Navigate(new Uri("https://dl.dropboxusercontent.com/u/106199435/shedule.html", UriKind.Absolute));
        }

        private void ApplicationBarIconButton_Click_1(object sender, EventArgs e)
        {
                WebBrowserTask webBrowserTask = new WebBrowserTask();
                webBrowserTask.Uri = new Uri("http://www.volosfans.com/p/2012-13.html",UriKind.Absolute);
                webBrowserTask.Show(); 
        }
    }
    }
