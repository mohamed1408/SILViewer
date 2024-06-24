using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using Gurock.SmartInspect;
using SmartInspect.SDK;

namespace SILViewer;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public ObservableCollection<TabItemViewModel> TabItems { get; set; }
    public ObservableCollection<LogItem> LogItems { get; set; }
    public MainWindow()
    {
        InitializeComponent();
        DataContext = this;
        // Initialize the collection of TabItems
        TabItems = new ObservableCollection<TabItemViewModel>();
        LogItems = new ObservableCollection<LogItem>();
        OpenSilFile(@"C:\Work\pnx\CAD client logs\phoenixlog-2024-05-30-01-09-15.sil");
    }
    private async void OpenSilFile(string fileName)
    {
        // Open the log file using the LogFile class
        // The 'using' statement ensures the log is properly disposed after use
        TabItemViewModel viewModel = new TabItemViewModel
        {
            Header = Path.GetFileName(fileName),
            Items = new ObservableCollection<LogItem>()
        };
        using (ILog log = new LogFile(fileName))
        {
            foreach (Packet packet in log)
            {
                // Check if the packet is a LogEntry
                if (packet is LogEntry logEntry)
                {
                    string app_path = System.Reflection.Assembly.GetExecutingAssembly().Location;
                    app_path = Path.GetDirectoryName(app_path);
                    string icon = $"{app_path}\\Images\\";
                    if (logEntry.Level == Level.Message)
                        icon += "Message.png";
                    else if (logEntry.Level == Level.Error)
                        icon += "Error.png";
                    else if (logEntry.Level == Level.Fatal)
                        icon += "FatalError.png";
                    viewModel.Items.Add(new LogItem { Icon = icon, ThreadId = logEntry.ThreadId.ToString(), TimeStamp = logEntry.Timestamp.ToString(), Title = logEntry.Title, Level = logEntry.Level.ToString() });
                }
            }
            TabItems.Add(viewModel);
        }
    }
    private void NewMenuItem_Click(object sender, RoutedEventArgs e)
    {
        TabItems.Add(new TabItemViewModel { Header = "Tab2", Items = new ObservableCollection<LogItem>() });
    }
    private void OpenMenuItem_Click(object sender, RoutedEventArgs e)
    {
        MessageBox.Show("Open menu item clicked!");
    }
    private void SaveMenuItem_Click(object sender, RoutedEventArgs e)
    {
        MessageBox.Show("Save menu item clicked!");
    }
    private void ExitMenuItem_Click(object sender, RoutedEventArgs e)
    {
        this.Close();
    }
    private void CutMenuItem_Click(object sender, RoutedEventArgs e)
    {
        MessageBox.Show("Cut menu item clicked!");
    }
    private void CopyMenuItem_Click(object sender, RoutedEventArgs e)
    {
        MessageBox.Show("Copy menu item clicked!");
    }
    private void PasteMenuItem_Click(object sender, RoutedEventArgs e)
    {
        MessageBox.Show("Paste menu item clicked!");
    }
    private void AboutMenuItem_Click(object sender, RoutedEventArgs e)
    {
        MessageBox.Show("About menu item clicked!");
    }

    private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        TabItem tabItem = sender as TabItem;
        if(tabItem != null)
        {
            string header = tabItem.Header.ToString();
        }
    }
}
public class TabItemViewModel
{
    public string Header { get; set; }
    public ObservableCollection<LogItem> Items { get; set; }
}

public class LogItem
{
    public string Icon { get; set; }
    public string Title { get; set; }
    public string ThreadId { get; set; }
    public string TimeStamp { get; set; }
    public string Level { get; set; }
    // Add more properties as needed for additional columns
}