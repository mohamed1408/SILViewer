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
    public MainWindow()
    {
        InitializeComponent();
        DataContext = this;
        // Initialize the collection of TabItems
        TabItems = new ObservableCollection<TabItemViewModel>();
        OpenSilFile(@"C:\Work\pnx\CAD client logs\phoenixlog-2024-05-30-01-09-15.sil");
        //TabItems.Add(new TabItemViewModel
        //{
        //    Header = "Tab 1",
        //    Items = new ObservableCollection<ListViewItemViewModel>
        //        {
        //            new ListViewItemViewModel { Column1 = "Item 1.1", Column2 = "Item 1.2" },
        //            new ListViewItemViewModel { Column1 = "Item 1.3", Column2 = "Item 1.4" }
        //        }
        //});
        //TabItems.Add(new TabItemViewModel
        //{
        //    Header = "Tab 2",
        //    Items = new ObservableCollection<ListViewItemViewModel>
        //        {
        //            new ListViewItemViewModel { Column1 = "Item 2.1", Column2 = "Item 2.2" },
        //            new ListViewItemViewModel { Column1 = "Item 2.3", Column2 = "Item 2.4" }
        //        }
        //});
        //TabItems.Add(new TabItemViewModel
        //{
        //    Header = "Tab 3",
        //    Items = new ObservableCollection<ListViewItemViewModel>
        //        {
        //            new ListViewItemViewModel { Column1 = "Item 3.1", Column2 = "Item 3.2" },
        //            new ListViewItemViewModel { Column1 = "Item 3.3", Column2 = "Item 3.4" }
        //        }
        //});
    }
    private void NewMenuItem_Click(object sender, RoutedEventArgs e)
    {
        //TabItems.Add(new TabItemViewModel
        //{
        //    Header = "Tab 4",
        //    Items = new ObservableCollection<ListViewItemViewModel>
        //        {
        //            new ListViewItemViewModel { Column1 = "Item 4.1", Column2 = "Item 4.2" },
        //            new ListViewItemViewModel { Column1 = "Item 4.3", Column2 = "Item 4.4" }
        //        }
        //});
    }
    private async void OpenSilFile(string fileName)
    {
        // Open the log file using the LogFile class
        // The 'using' statement ensures the log is properly disposed after use
        TabItemViewModel viewModel = new TabItemViewModel
        {
            Header = Path.GetFileName(fileName),
            Items = new ObservableCollection<ListViewItemViewModel>()
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
                    viewModel.Items.Add(new ListViewItemViewModel { Icon = icon, ThreadId = logEntry.ThreadId.ToString(), TimeStamp = logEntry.Timestamp.ToString(), Title = logEntry.Title });
                    // If it is a LogEntry, add a new row to the DataGridView
                    // with the LogEntry's level, size, and title properties
                    //dataGridView1.Rows.Add(
                    //    logEntry.Level.ToString(),
                    //    logEntry.Size,
                    //    logEntry.Title
                    //);
                }
            }
            TabItems.Add(viewModel);
        }
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
}
public class TabItemViewModel
{
    public string Header { get; set; }
    public ObservableCollection<ListViewItemViewModel> Items { get; set; }
}

public class ListViewItemViewModel
{
    public string Icon { get; set; }
    public string Title { get; set; }
    public string ThreadId { get; set; }
    public string TimeStamp { get; set; }
    // Add more properties as needed for additional columns
}