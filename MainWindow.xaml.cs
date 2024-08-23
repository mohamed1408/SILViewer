using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
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
    public List<LogItem> LogItems { get; set; }
    public Filter LogFilter { get; set; }
    public MainWindow()
    {
        InitializeComponent();
        DataContext = this;
        // Initialize the collection of TabItems
        TabItems = new ObservableCollection<TabItemViewModel>();
        LogItems = new List<LogItem>();
        LogFilter = new Filter();
        OpenSilFile(@"G:\CRM_Attachments\168936\logs\phoenixlog-2024-04-25-14-52-07.sil");
        Initialize();
    }
    public void Initialize()
    {

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
                    if (logEntry.LogEntryType == LogEntryType.Message)
                        icon += "Message.png";
                    else if (logEntry.LogEntryType == LogEntryType.Error)
                        icon += "Error.png";
                    else if (logEntry.LogEntryType == LogEntryType.Fatal)
                        icon += "FatalError.png";
                    LogItems.Add(new LogItem { Icon = icon, ThreadId = logEntry.ThreadId.ToString(), TimeStamp = logEntry.Timestamp.ToString(), Title = logEntry.Title, Level = logEntry.LogEntryType.ToString(), RowHeight = 22, Data = logEntry.Data });
                    //viewModel.Items.Add(new LogItem { Icon = icon, ThreadId = logEntry.ThreadId.ToString(), TimeStamp = logEntry.Timestamp.ToString(), Title = logEntry.Title, Level = logEntry.LogEntryType.ToString(), RowHeight = 22 });
                    if(logEntry.LogEntryType != LogEntryType.Message)
                    {

                    }
                    if (logEntry.Title.Contains("EXCEPTION"))
                    {
                        //byte[] buffer = new byte[logEntry.Data.Length];
                        //var res = logEntry.Data.Read(buffer, 0, buffer.Length);
                    }
                }
                else
                {

                }
            }
            TabItems.Add(viewModel);
            filterLogs();
        }
    }
    private bool LogFilterPredicate(LogItem logItem)
    {
        switch (logItem.Level)
        {
            case "Message":
                return LogFilter.Message;
            case "Error":
                return LogFilter.Error;
            case "Fatal":
                return LogFilter.Fatal;
            default: return false;
        }
    }
    private void filterLogs()
    {
        TabItems[0].Items = new ObservableCollection<LogItem>(LogItems.Where(x => LogFilterPredicate(x)));
            
        CollectionViewSource.GetDefaultView(TabItems[0].Items).Refresh();
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

    private void CheckBox_Checked(object sender, RoutedEventArgs e)
    {
        //foreach (LogItem item in TabItems[0].Items.Where(x => x.Level == ((CheckBox)e.Source).Content.ToString()))
        //{
        //    item.RowHeight = (((CheckBox)e.Source).IsChecked == true) ? 22 : 0;
        //}
        //CollectionViewSource.GetDefaultView(TabItems[0].Items).Refresh();
        switch (((CheckBox)e.Source).Content.ToString())
        {
            case "Message":
                LogFilter.Message = ((CheckBox)e.Source).IsChecked == true;
                break;
            case "Error":
                LogFilter.Error = ((CheckBox)e.Source).IsChecked == true;
                break;
            case "Fatal":
                LogFilter.Fatal = ((CheckBox)e.Source).IsChecked == true;
                break;
        }
        filterLogs();
    }

    private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if(e.AddedItems.Count > 0)
            Console.WriteLine(((LogItem)e.AddedItems[0]).GetData());
    }
}
public class TabItemViewModel : INotifyPropertyChanged
{
    public string Header { get; set; }
    ObservableCollection<LogItem> _Items { get; set; }
    public ObservableCollection<LogItem> Items {
        get
        {
            return _Items;
        }
        set
        {
            if (_Items != value)
            {
                _Items = value;
                OnPropertyChanged();
            }
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string memberName = "")
    {
        if (PropertyChanged != null)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(memberName));
        }
    }
}

public class LogItem
{
    public string Icon { get; set; }
    public string Title { get; set; }
    public string ThreadId { get; set; }
    public string TimeStamp { get; set; }
    public string Level { get; set; }
    public int RowHeight { get; set; }
    public Stream? Data { get; set; }
    private string _data { get; set; }
    public string GetData()
    {
        if(Data != null)
        {
            if (_data != null)
                return _data;
            StreamReader reader = new StreamReader(Data);
            _data = reader.ReadToEnd();
            return _data;
        }
        return "";
    }
}
public class Filter
{
    public bool Message { get; set; }
    public bool Error { get; set; }
    public bool Fatal { get; set; }
    public Filter(Filter? fltr = null)
    {
        if(fltr != null)
        {
            this.Message = fltr.Message;
            this.Error = fltr.Error;
            this.Fatal = fltr.Fatal;
        }
        else
        {
            this.Message = true;
            this.Error = true;
            this.Fatal = true;
        }
    }
}