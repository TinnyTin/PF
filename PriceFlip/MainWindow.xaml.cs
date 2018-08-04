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
        //Currency # association via poe.trade
        Dictionary<string, int> currency = new Dictionary<string, int>()
        {
            { "alt", 1 },
            { "fusing", 2 },
            { "alch", 3 },
            { "chaos", 4 },
            { "gcp", 5 },
            { "exalt", 6 },
            { "chrom", 7 },
            { "jeweller", 8 },
            { "chance", 9 },
            { "chisel", 10 },
            { "scour", 11 },
            { "blessed", 12 },
            { "regret", 13 },
            { "regal", 14 },
            { "divine", 15 },
            { "vaal", 16 },
            { "offering", 40 },
            { "apprentice", 45 },
            { "journeyman", 46 },
            { "masters", 47 },
            { "annul", 513 }

        };
        




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

        private void close(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void minimize(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void dragwindow(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        // Given want and have currency strings, retrieves the 7th listing info from currency.poe.trade
        // Returns an object array {want#,have#} representing ( want <- have )
        // Note that this is the same as ( receive <- pay )
        private Object[] refresh(string want, string have)
        {
            Object[] result = new Object[] { 0, 0 };
            
            var Url = @"http://currency.poe.trade/search?league=Incursion&online=x&stock=&want=" + currency[want] + "&have=" + currency[have];
            var data = new MyWebClient().DownloadString(Url);
            var doc = new HtmlDocument();
            doc.LoadHtml(data);

            var htmlNode = doc.DocumentNode.SelectNodes("//div[@class='displayoffer ']")[6];

            // IGN and stock are not necessary in the current implementation
            //var ign = htmlNode.GetAttributeValue("data-ign", "IGN not found");
            //var stock = htmlNode.GetAttributeValue("data-stock", "Stock not found");

            string receive = htmlNode.GetAttributeValue("data-sellvalue", "Sell value not found");
            string pay = htmlNode.GetAttributeValue("data-buyvalue", "Buy value not found");

            if(receive != "Sell value not found" && pay != "Buy value not found")
            {
                result[0] = Convert.ToDouble(receive);
                result[1] = Convert.ToDouble(pay);
            }

            //Test print
            //string msg = "IGN: " + ign + ", Currency: chaos to " + entry.Key + ", Receive: " + receive + ", Pay: " + pay + ", Stock: " + stock + "\n";

            return result;

        }

       

    }

}
