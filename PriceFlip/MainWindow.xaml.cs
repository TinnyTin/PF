using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
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

        //Currency image association
        Dictionary<string, string> images = new Dictionary<string, string>()
        {
            { "alt", "ImageAssets/Alteration.png" },
            { "fusing", "ImageAssets/Fusing.png" },
            { "alch", "ImageAssets/Alchemy.png" },
            { "chaos", "ImageAssets/Chaos.png" },
            { "gcp", "ImageAssets/Gemcutter.png" },
            { "exalt", "ImageAssets/Exalted.png" },
            { "chrom", "ImageAssets/Chromatic.png" },
            { "jeweller", "ImageAssets/Jeweller.png" },
            { "chance", "ImageAssets/Chance.png" },
            { "chisel", "ImageAssets/Chisel.png" },
            { "scour", "ImageAssets/Scouring.png" },
            { "blessed", "ImageAssets/Blessed.png" },
            { "regret", "ImageAssets/Regret.png" },
            { "regal", "ImageAssets/Regal.png" },
            { "divine", "ImageAssets/Divine.png" },
            { "vaal", "ImageAssets/Vaal.png" },
            { "offering", "ImageAssets/Offering.png" },
            { "apprentice", "ImageAssets/ApprenticeSextant.png" },
            { "journeyman", "ImageAssets/JourneymanSextant.png" },
            { "masters", "ImageAssets/MasterSextant.png" },
            { "annul", "ImageAssets/Annulment.png" }

        };

        // Currency main list initialization
        List<Object[]> DataList = new List<Object[]>();

        List<Object[]> favouritesList = new List<Object[]>(0);




        public MainWindow()
        {
            InitializeComponent();
            populateList();
            Debug.Print(DataList.Count +"");

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




        // Calculates the flat profit value of 1 trade cycle.
        // receive and pay represents the cash-out trade. receive2 and pay2 should represent the buy-in trade.
        // Returns a double representing the flat profit in *chaos* units.
        private double flatprofit(double receive, double pay, double receive2, double pay2)
        {
            double profit = 0.0;
            double totalbuy = pay2 - receive;
            double buyvalue = pay2 / receive2;
            profit = totalbuy / buyvalue;

            return profit;
        }

        // Calculates the profit margin % of 1 trade cycle.
        // receive and pay represents the cash-out trade. receive2 and pay2 should represent the buy-in trade.
        // Returns a double representing the profit margin %.
        private double profitmargin(double receive, double pay, double receive2, double pay2)
        {
            double profit = 0.0;
            double buyvalue = pay2 / receive2;
            double sellvalue = receive / pay;
            profit = (buyvalue / sellvalue) - 1;

            return profit;
        }

        // Given an array, add it to the favourites list.
        private void addFavourite(Object[] data)
        {
            favouritesList.Add(data);
        }

        private void populateList()
        {
            //new Object[] { "alt", "chaos", 0, 0, 0, 0 }
            foreach (KeyValuePair<string, int> entry in currency)
            {
                if(entry.Key != "chaos")
                {
                    DataList.Add(new Object[] { entry.Key, "chaos", 0, 0, 0, 0 });
                }
                if (entry.Key != "exalt")
                {
                    DataList.Add(new Object[] { entry.Key, "exalt", 0, 0, 0, 0 });
                }

            }
        }

        // Given receive and pay currency strings, retrieves the 7th listing info from currency.poe.trade. If there are <=12 listings, index = listing.size/2.
        // Returns an array[receive#,have#] representing ( receive <- pay )
        private Object[] refresh(string receive, string pay)
        {
            Object[] result = new Object[] { 0, 0 };
            
            var Url = @"http://currency.poe.trade/search?league=Incursion&online=x&stock=&want=" + currency[receive] + "&have=" + currency[pay];
            var data = new MyWebClient().DownloadString(Url);
            var doc = new HtmlDocument();
            doc.LoadHtml(data);

            var htmlNodeList = doc.DocumentNode.SelectNodes("//div[@class='displayoffer ']");
            int index = 6;
            int htmlListSize = (int) htmlNodeList.LongCount();
            if (htmlListSize <= 12)
            {
                index = htmlListSize / 2;
            }
            var htmlNode = htmlNodeList[index];

            // IGN and stock are not necessary in the current implementation
            //var ign = htmlNode.GetAttributeValue("data-ign", "IGN not found");
            //var stock = htmlNode.GetAttributeValue("data-stock", "Stock not found");

            string receivevalue = htmlNode.GetAttributeValue("data-sellvalue", "Sell value not found");
            string payvalue = htmlNode.GetAttributeValue("data-buyvalue", "Buy value not found");

            if(receivevalue != "Sell value not found" && payvalue != "Buy value not found")
            {
                result[0] = Convert.ToDouble(receivevalue);
                result[1] = Convert.ToDouble(payvalue);
            }

            //Test print
            //string msg = "IGN: " + ign + ", Currency: chaos to " + entry.Key + ", Receive: " + receive + ", Pay: " + pay + ", Stock: " + stock + "\n";

            return result;

        }

        
        private void Textbox_Changed(object sender, TextChangedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            string text = (tb.Text.TrimEnd('%'));
            Debug.Print("reached");
            if (text != "") //can't convert whitespace to double. change later for efficiency? 
            {
                double percentage = Convert.ToDouble(text);

                if (percentage >= 10)
                {
                    tb.Foreground = Brushes.Green;

                }
                else if (percentage >= 5 && percentage < 10)
                {
                    tb.Foreground = Brushes.Yellow;
                }
                else
                {
                    tb.Foreground = Brushes.Red;
                }
            }
        }

        private void copyToClipboard(object sender, RoutedEventArgs e)
            //sender = button object that sent it,
            //e = event
        {
            Button btn = (Button)sender;
            var g = (Grid)btn.Content;
            var textbox = g.Children.OfType<TextBox>().FirstOrDefault();
            var text = textbox.Text;
            var textarray = text.Split('⇐');
            string c1value = textarray[0];
            string c2value = textarray[1];
            //for (int i=0; i<2; i++)
            //{
            //    Debug.Print(textarray[i].Trim());
            //}

            Clipboard.SetText("~b/o" + c1value + "/" + c2value + "currency 1");

            //string c1 = "chaos";
            //double c1value = 0;
            //double c2value = 0;
            //string result =  "~b/o" + c1value + "/" + c2value + " " + c1;
        }

    }

}
