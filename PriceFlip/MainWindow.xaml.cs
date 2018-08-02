using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
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
using HtmlAgilityPack;
namespace PriceFlip
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        class MyWebClient : WebClient
        {
            protected override WebRequest GetWebRequest(Uri address)
            {
                HttpWebRequest request = base.GetWebRequest(address) as HttpWebRequest;
                request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
                return request;
            }
        }

        private void FavouritesButton_Click(object sender, RoutedEventArgs e)
        {
            var Url = @"http://currency.poe.trade/search?league=Incursion&online=x&want=4&have=5";
            
            var data = new MyWebClient().DownloadString(Url);

            var doc = new HtmlDocument();
            doc.LoadHtml(data);
            string seventh = doc.DocumentNode.SelectNodes("//*[@id=\"content\"]/div[9]/div[1]")[0].InnerText;
            Console.WriteLine(seventh);
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }
    }
}
