using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Windows.Foundation.Metadata;
using Windows.Storage;
using Windows.Storage.Pickers;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace JavaIM_for_Windows
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Server : Page
    {
        public ObservableCollection<ServerInfo> Servers { get; } = new ObservableCollection<ServerInfo>();
        // TextBox Text = new TextBox();
        public Server()
        {
            InitializeComponent();
            _ = LoadServersAsync();
        }

        public async Task LoadServersAsync()
        {
            Servers.Clear();
            var task = GetServerListAsync();
            if (task == null)
                return;
            foreach (var serverInfo in await task)
            {
                if (serverInfo == null)
                    return;
                Servers.Add(serverInfo);
            }
        }

        
        private async Task<StorageFolder> RequestDirectory(StorageFolder parentDirectory, String name)
        {
            StorageFolder dir;
            try
            {
                dir = await parentDirectory.GetFolderAsync(name);
            }
            catch (FileNotFoundException)
            {
                try
                {
                    dir = await parentDirectory.CreateFolderAsync(name);
                }
                catch (UnauthorizedAccessException)
                {
                    ContentDialog dialog = new ContentDialog
                    {
                        Title = "错误",
                        Content = "无权创建" + name + "文件夹",
                        CloseButtonText = "关闭",
                        XamlRoot = XamlRoot
                    };
                    await dialog.ShowAsync();
                    return null;
                }
            }
            catch (UnauthorizedAccessException)
            {
                ContentDialog dialog = new ContentDialog
                {
                    Title = "错误",
                    Content = "无权访问"+name+"文件夹",
                    CloseButtonText = "关闭",
                    XamlRoot = XamlRoot
                };
                await dialog.ShowAsync();
                return null;
            }// 处理文件夹
            return await Task.FromResult(dir);
        }

        private async void AddServerButton_Click(object sender, RoutedEventArgs e)
        {
            Port.Value = double.NaN;
            IP.Text = null;
            Name.Text = null;
            FileNameDisplay.Text = null;
            serverCA = null;
            ContentDialogResult result = await AddServerContentDialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                var serverListTask = GetServerListAsync();
                if (serverListTask == null)
                    return;
                foreach (var server in await serverListTask)
                {
                    if (server.ServerName.Equals(Name.Text))
                    {
                        
                        ContentDialog dialog = new ContentDialog
                        {
                            Title = "错误",
                            Content = "服务器名重复",
                            CloseButtonText = "关闭",
                            XamlRoot = XamlRoot
                        };
                        await dialog.ShowAsync();
                        return;
                    }
                }
                StorageFolder currentFolder = await StorageFolder.GetFolderFromPathAsync(Directory.GetCurrentDirectory());
                
                string serverId = Guid.NewGuid().ToString();// 写入crt+创建json
                var task = RequestDirectory(currentFolder, "data");
                if (task == null)
                    return;
                var folder = await task;
                task = RequestDirectory(folder, "savedServers");
                if (task == null)
                    return;
                var savedServerFolder = await task;
                var asyncResult = savedServerFolder.CreateFileAsync(serverId + ".json");
                StorageFile serverCert = await savedServerFolder.CreateFileAsync(serverId + ".crt");
                StorageFile serverJson = await asyncResult;
                await serverCA.CopyAndReplaceAsync(serverCert);
                var serverInfo = new ServerInfo
                {
                    ServerName = Name.Text,
                    ServerIP = IP.Text,
                    ServerId = serverId,
                    ServerToken = "",
                    ServerPort = (int)Port.Value
                };
                string jsonString = JsonSerializer.Serialize(serverInfo);
                await FileIO.WriteTextAsync(serverJson, jsonString);
                await LoadServersAsync();
            }
        }
        private void RequestDialogPrimaryButtonUpdate()
        {
            if (string.IsNullOrEmpty(Name.Text) || string.IsNullOrEmpty(IP.Text) || double.IsNaN(Port.Value) || serverCA == null)
                AddServerContentDialog.IsPrimaryButtonEnabled = false;
            else
                AddServerContentDialog.IsPrimaryButtonEnabled = true;
        }


        private void Name_TextChanged(object sender, TextChangedEventArgs e) => RequestDialogPrimaryButtonUpdate();

        private void Port_ValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
        {
            if (args.NewValue.ToString().Contains('.'))
                Port.Value = args.OldValue;
            RequestDialogPrimaryButtonUpdate();
        }

        private StorageFile serverCA;

        private async void ChooseCACert_Click(object sender, RoutedEventArgs e)
        {
            var filePicker = new FileOpenPicker();
            filePicker.FileTypeFilter.Add(".crt");
            // Associate the HWND with the file picker
            WinRT.Interop.InitializeWithWindow.Initialize(filePicker, MainWindow.GetInstance().Hwnd);
            serverCA = await filePicker.PickSingleFileAsync();
            if (serverCA == null)
                return;
            FileNameDisplay.Text = "CA证书文件名：" + serverCA.Name;
            RequestDialogPrimaryButtonUpdate();
        }

        private void SelectServerButton_Click(object sender, RoutedEventArgs e)
        {
            // 获取点击的 Button
            Button clickedButton = sender as Button;

            // 从 Button 的 DataContext 获取对应的 Server 信息
            var selectedServer = clickedButton?.DataContext as ServerInfo;
            MainWindow.GetInstance().SelectServer = selectedServer;
        }

        private async Task<ObservableCollection<ServerInfo>> GetServerListAsync()
        {
            ObservableCollection<ServerInfo> servers = new ObservableCollection<ServerInfo>();
            StorageFolder currentFolder = await StorageFolder.GetFolderFromPathAsync(Directory.GetCurrentDirectory());
            var task = RequestDirectory(currentFolder, "data");
            if (task == null)
                return null;
            var folder = await task;
            task = RequestDirectory(folder, "savedServers");
            if (task == null)
                return null;
            var savedServerFolder = await task;
            var files = await savedServerFolder.GetFilesAsync();
            foreach (var file in files)
            {
                if (!file.Name.EndsWith(".json"))
                    continue;
                try
                {
                    string jsonContent = await FileIO.ReadTextAsync(file);
                    var server = JsonSerializer.Deserialize<ServerInfo>(jsonContent);
                    servers.Add(server);
                }
                catch (Exception)
                {
                    ContentDialog dialog = new ContentDialog
                    {
                        Title = "错误",
                        Content = "读取服务器列表时出错",
                        CloseButtonText = "关闭",
                        XamlRoot = XamlRoot
                    };
                    await dialog.ShowAsync();
                    return null;
                }
            }
            return await Task.FromResult(servers);
        }
    }

}
