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
        Dictionary<string, int> currency1 = new Dictionary<string, int>()
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

        Dictionary<string, string> abbrevcurrency = new Dictionary<string, string>()
        {
            { "alt", "alt" },
            { "fusing", "fuse" },
            { "alch", "alch" },
            { "chaos", "chaos" },
            { "gcp", "gcp" },
            { "exalt", "exa" },
            { "chrom", "chrom" },
            { "jeweller", "jew" },
            { "chance", "chance" },
            { "chisel", "chisel" },
            { "scour", "scour" },
            { "blessed", "blesse" },
            { "regret", "regret" },
            { "regal", "regal" },
            { "divine", "divine" },
            { "vaal", "vaal" },
            { "offering", "offer" },
            { "apprentice", "apprentice-sextant" },
            { "journeyman", "journeyman-sextant" },
            { "masters", "master-sextant" },
            { "annul", "orb-of-annulment" }

        };

        // Currency main list initialization
        List<Currency> currency;
        List<CurrencyRow> dataList = new List<CurrencyRow>(0);
        List<CurrencyRow> favouritesList = new List<CurrencyRow>(0);




        public MainWindow()
        {
            InitializeComponent();

            initCurrency();
            populateList();
            items.ItemsSource = dataList;

        }

        private void initCurrency()
        {
            currency = new List<Currency>()
            {
                new Currency(){name="alteration",id=1,image="ImageAssets/Alteration.png",tag="alt"},
                new Currency(){name="fusing",id=2,image="ImageAssets/Fusing.png",tag="fuse"},
                new Currency(){name="alchemy",id=3,image="ImageAssets/Alchemy.png",tag="alch"},
                new Currency(){name="chaos",id=4,image="ImageAssets/Chaos.png",tag="chaos"},
                new Currency(){name="gemcutters",id=5,image="ImageAssets/Gemcutter.png",tag="gcp"},
                new Currency(){name="exalted", id=6, image="ImageAssets/Exalted.png", tag="exa"},
                new Currency(){name="chromatic", id=7, image="ImageAssets/Chromatic.png", tag="chrom"},
                new Currency(){name="jeweller", id=8, image="ImageAssets/Jeweller.png", tag="jew"},
                new Currency(){name="chance", id=9, image="ImageAssets/Chance.png", tag="chance"},
                new Currency(){name="chisel", id=10, image="ImageAssets/Chisel.png", tag="chisel"},
                new Currency(){name="scouring", id=11, image="ImageAssets/Scouring.png", tag="scour"},
                new Currency(){name="blessed", id=12, image="ImageAssets/Blessed.png", tag="blesse"},
                new Currency(){name="regret", id=13, image="ImageAssets/Regret.png", tag="regret"},
                new Currency(){name="regal", id=14, image="ImageAssets/Regal.png", tag="regal"},
                new Currency(){name="divine", id=15, image="ImageAssets/Divine.png", tag="divine"},
                new Currency(){name="vaal", id=16, image="ImageAssets/Vaal.png", tag="vaal"},
                new Currency(){name="offering", id=40, image="ImageAssets/Offering.png", tag="offer"},
                new Currency(){name="apprentice", id=45, image="ImageAssets/ApprenticeSextant.png", tag="apprentice-sextant"},
                new Currency(){name="journeyman", id=46, image="ImageAssets/JourneymanSextant.png", tag="journeyman-sextant"},
                new Currency(){name="masters", id=47, image="ImageAssets/MasterSextant.png", tag="master-sextant"},
                new Currency(){name="annulment", id=513, image="ImageAssets/Annulment.png", tag="orb-of-annulment"},

            };
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
        private void addFavourite(CurrencyRow data)
        {
            favouritesList.Add(data);
        }

        // Initializes all available currency items for flipping.
        private void populateList()
        {
            foreach (KeyValuePair<string, int> entry in currency)
            {
                if(entry.Key != "chaos")
                {
                    dataList.Add(new CurrencyRow { CTYPE1 = entry.Key, CTYPE2 = "chaos", CIMAGE1=images[entry.Key], CIMAGE2 = images["chaos"]});
                }
                if (entry.Key != "exalt")
                {
                    dataList.Add(new CurrencyRow { CTYPE1 = entry.Key, CTYPE2 = "exalt", CIMAGE1= images[entry.Key], CIMAGE2 = images["exalt"] });
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
            var currencyimage = g.Children.OfType<Image>().LastOrDefault();
            string currencytype = (string)currencyimage.ToolTip;
            Debug.Print(currencytype);
            var text = textbox.Text;
            var textarray = text.Split('⇐');
            string c1value = textarray[0];
            string c2value = textarray[1];
            //for (int i=0; i<2; i++)
            //{
            //    Debug.Print(textarray[i].Trim());
            //}

            Clipboard.SetText("~b/o " + c1value.Trim() + "/" + c2value.Trim() + " " + currencytype);

            //string c1 = "chaos";
            //double c1value = 0;
            //double c2value = 0;
            //string result =  "~b/o" + c1value + "/" + c2value + " " + c1;
        }

        private void updateRow(object sender, RoutedEventArgs e)
        {

        }
    }

    public class Currency
    {
        public string name = "";
        public int id = 0;
        public string tag = "";
        public string image = "";
    }

    public class CurrencyRow
    {
        private string ctype1 = "";
        private string ctype2 = "";

        public double receive1 = 0;
        public double pay1 = 0;
        public double receive2 = 0;
        public double pay2 = 0;

        private string cimage1 = "";
        private string cimage2 = "";

        public string CTYPE1
        {
            get
            {
                return ctype1;
            }
            set
            {
                ctype1 = value;
            }
        }

        public string CTYPE2
        {
            get
            {
                return ctype2;
            }
            set
            {
                ctype2 = value;
            }
        }

        public string CIMAGE1
        {
            get
            {
                return cimage1;
            }
            set
            {
                cimage1 = value;
            }
        }
        public string CIMAGE2
        {
            get
            {
                return cimage2;
            }
            set
            {
                cimage2 = value;
            }
        }
    }

}
