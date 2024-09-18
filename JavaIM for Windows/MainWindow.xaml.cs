using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace JavaIM_for_Windows
{
    public class ServerInfo
    {
        public string ServerId { get; set; }
        public string ServerToken { get; set; }
        public string ServerName { get; set; }
        public string ServerIP { get; set; }
        public int ServerPort { get; set; }
    }
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public IntPtr Hwnd { get; }

        private static MainWindow instance;
        public static MainWindow GetInstance()
        {
            return instance;
        }

        public ServerInfo SelectServer { get; set; }

        public MainWindow()
        {
            SelectServer = null;
            instance = this;
            AppWindow.SetIcon("Assets/logo.ico");
            // Get the current window's HWND by passing in the Window object
            Hwnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
            InitializeComponent();
        }

        private void NavigationView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            var selectedItem = (NavigationViewItem)args.SelectedItem;
            if (args.IsSettingsSelected)
            {
                contentFrame.Navigate(typeof(Settings));
            }
            if ((string)selectedItem.Tag == "Chat") contentFrame.Navigate(typeof(Chat));
            else if ((string)selectedItem.Tag == "Server") contentFrame.Navigate(typeof(Server));
        }
    }
}
