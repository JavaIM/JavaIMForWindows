using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace JavaIM_for_Windows
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Server : Page
    {
        Button AddServerButton = new Button();
        public Server()
        {
            AddServerButton.Content = "Add Server";
            AddServerButton.Click += AddServerButton_Click;
            this.InitializeComponent();
        }


        private async void AddServerButton_Click(object sender, RoutedEventArgs e)
        {
            ContentDialogResult result = await AddServerContentDialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                // Terms of use were accepted.
                AddServerButton.Content = "Accept";
            }
            else
            {
                // User pressed Cancel, ESC, or the back arrow.
                // Terms of use were not accepted.
                AddServerButton.Content = "Accept";
            }
        }

        private void AddServerContentDialog_Opened(ContentDialog sender, ContentDialogOpenedEventArgs args)
        {
            // Ensure that the check box is unchecked each time the dialog opens.
            //ConfirmAgeCheckBox.IsChecked = false;
        }

        private void ConfirmAgeCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            AddServerContentDialog.IsPrimaryButtonEnabled = true;
        }

        private void ConfirmAgeCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            AddServerContentDialog.IsPrimaryButtonEnabled = false;
        }

    }

}
