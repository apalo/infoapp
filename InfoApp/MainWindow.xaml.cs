using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace InfoApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            UpdateOverview();
        }

        private void UpdateOverview()
        {
            this.TextBlock_OsVersion.Text = SystemInfo.OSName;
            this.TextBlock_SystemPath.Text = SystemInfo.SystemPath;
            this.TextBlock_TotPhysMem.Text = SystemInfo.GetPhysicalMemory() + " MB";
            this.TextBlock_UsablePhysMem.Text = SystemInfo.GetUsablePhysicalMemory() + " MB";
            this.TextBlock_AvailPhysMem.Text = SystemInfo.GetAvailablePhysicalMemory() + " MB" +
                " (" + SystemInfo.GetMemoryPercentFree() + "%)";
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
