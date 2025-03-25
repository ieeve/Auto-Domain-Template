using System.Windows;

namespace Blazor.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            //new QrcodeCommon().FindQrPoint("");
        }
    }
}
