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
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;

namespace Volos_Fans
{
    public partial class UWrite : PhoneApplicationPage
    {
        
        public UWrite()
        {
            InitializeComponent();
        }

        private void uWrite_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
                GotFocus += UWrite_GotFocus;
                
        }
        
        

        private void UWrite_GotFocus(object sender, RoutedEventArgs e)
        {
           
        }

        private void RoutedEventHandler()
        {

        }

        public RoutedEventHandler UWrite_Loaded { get; set; }

        private void ApplicationBarIconButton_Click(object sender, EventArgs e)
        {
            if(string.IsNullOrEmpty( uWrite.Text.Trim() ))
            {
                MessageBox.Show("Το περιεχόμενο δεν μπορεί να είναι κενό");
            
            }
                else
                {
                    EmailComposeTask emailComposeTask = new EmailComposeTask();
                    emailComposeTask.Body = uWrite.Text;
                    emailComposeTask.Subject = "Εσείς γράφετε";
                    emailComposeTask.To = "volosfans@gmail.com";
                    emailComposeTask.Show();
                }
            }

        }
       
    }
