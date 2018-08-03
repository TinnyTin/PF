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

        private void refresh(object sender, RoutedEventArgs e) { 
        //{
        //    foreach (KeyValuePair<string, int> entry in currency)
        //    {
        //        if (entry.Value != 4)
        //        {
        //            string data;
        //            using (var client = new WebClient())
        //            {
        //                string link = "http://currency.poe.trade/search?league=Incursion&online=x&stock=&want=" + entry.Value + "&have=" + currency["chaos"];
        //                data = client.DownloadString(link);
        //                HtmlDocument doc = new HtmlDocument();
        //                doc.LoadHtml(data);
        //                var htmlNode = doc.DocumentNode.SelectNodes("//div[@class='displayoffer ']")[6];

        //                var ign = htmlNode.GetAttributeValue("data-ign", "IGN not found");
        //                var receive = htmlNode.GetAttributeValue("data-sellvalue", "Sell value not found");
        //                var pay = htmlNode.GetAttributeValue("data-buyvalue", "Buy value not found");
        //                var stock = htmlNode.GetAttributeValue("data-stock", "Stock not found");

        //                //Test print
        //                string msg = "IGN: " + ign + ", Currency: " + entry.Key + ", Receive: " + receive + ", Pay: " + pay + ", Stock: " + stock + "\n";
        //                stringbox.Text = msg;

        //            }
        //        }
        //    }
        //}
                    foreach (KeyValuePair<string, int> entry in currency)
                        {
                            if (entry.Value != 4)
                            {
                                var Url = @"http://currency.poe.trade/search?league=Incursion&online=x&stock=&want=" + entry.Value + "&have=" + currency["chaos"];
                                var data = new MyWebClient().DownloadString(Url);
                                var doc = new HtmlDocument();
                                doc.LoadHtml(data);

                                var htmlNode = doc.DocumentNode.SelectNodes("//div[@class='displayoffer ']")[6];

                                var ign = htmlNode.GetAttributeValue("data-ign", "IGN not found");
                                var receive = htmlNode.GetAttributeValue("data-sellvalue", "Sell value not found");
                                var pay = htmlNode.GetAttributeValue("data-buyvalue", "Buy value not found");
                                var stock = htmlNode.GetAttributeValue("data-stock", "Stock not found");


                                //Test print
                                string msg = "IGN: " + ign + ", Currency: chaos to " + entry.Key + ", Receive: " + receive + ", Pay: " + pay + ", Stock: " + stock + "\n";
                                stringbox.Text += msg;


                                var Urlx = @"http://currency.poe.trade/search?league=Incursion&online=x&stock=&want=" + currency["chaos"] + "&have=" + entry.Value;
                                var datax = new MyWebClient().DownloadString(Urlx);
                                var docx = new HtmlDocument();
                                docx.LoadHtml(datax);

                                var htmlNodex = docx.DocumentNode.SelectNodes("//div[@class='displayoffer ']")[6];

                                var ignx = htmlNodex.GetAttributeValue("data-ign", "IGN not found");
                                var receivex = htmlNodex.GetAttributeValue("data-sellvalue", "Sell value not found");
                                var payx = htmlNodex.GetAttributeValue("data-buyvalue", "Buy value not found");
                                var stockx = htmlNodex.GetAttributeValue("data-stock", "Stock not found");


                                //Test print
                                string msgx = "IGN: " + ignx + ", Currency:" + entry.Key + " to chaos" + ", Receive: " + receivex + ", Pay: " + payx + ", Stock: " + stockx + "\n";
                                stringbox.Text += msgx;
                }
                        }
                    }

                }

}
