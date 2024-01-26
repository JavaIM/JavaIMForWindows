using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using Windows.Storage.Pickers;
using Windows.Storage;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace JavaIM_for_Windows
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Server : Page
    {
        Button Button = new Button();
        // TextBox Text = new TextBox();
        public Server()
        {
            Button.Content = "Add Server";
            Button.Click += AddServerButton_Click;
            this.InitializeComponent();
        }

        private async void AddServerButton_Click(object sender, RoutedEventArgs e)
        {
            ContentDialogResult result = await AddServerContentDialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                // Terms of use were accepted.
                Button.Content = "Accept";
            }
            else
            {
                // User pressed Cancel, ESC, or the back arrow.
                // Terms of use were not accepted.
                Button.Content = "Cancel";
            }
        }

        private void AddServerContentDialog_Opened(ContentDialog sender, ContentDialogOpenedEventArgs args)
        {
            Name.Text = null;
            IP.Text = null;
            Port.Text = "0";
            //Button.Content = "Import Key";
            //Button.Click += ImportkeyButton_Click;
            this.InitializeComponent();
        }
        private void inputBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            AddServerContentDialog.IsPrimaryButtonEnabled = !(string.IsNullOrEmpty(Name.Text) || string.IsNullOrEmpty(IP.Text) || string.IsNullOrEmpty(Port.Text));
        }

        private void Port_ValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
        {
            AddServerContentDialog.IsPrimaryButtonEnabled = !(string.IsNullOrEmpty(Name.Text) || string.IsNullOrEmpty(IP.Text) || string.IsNullOrEmpty(Port.Text));
        }
        /**
        private async void ImportkeyButton_Click(object sender, RoutedEventArgs e)
        {
            FileOpenPicker openPicker = new FileOpenPicker();
            var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
            WinRT.Interop.InitializeWithWindow.Initialize(openPicker, hWnd);
            openPicker.ViewMode = PickerViewMode.Thumbnail;
            openPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            openPicker.FileTypeFilter.Add(".jpg");
            openPicker.FileTypeFilter.Add(".jpeg");
            openPicker.FileTypeFilter.Add(".png");
            StorageFile file = await openPicker.PickSingleFileAsync();
            if (file != null)
            {
                // Application now has read/write access to the picked file
                //OutputTextBlock.Text = "Picked photo: " + file.Name;
            }
            else
            {
                //OutputTextBlock.Text = "Operation cancelled.";
            }
        }
        **/
    }

}
