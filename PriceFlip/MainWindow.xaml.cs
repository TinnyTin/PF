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

        // Initializes all currency information according to currency.poe.trade.
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
                new Currency(){name="annulment", id=513, image="ImageAssets/Annulment.png", tag="orb-of-annulment"}

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

            return Math.Round(profit,1);
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

            return Math.Round(profit, 2) * 100;
        }

        // Given an array, add it to the favourites list.
        private void addFavourite(CurrencyRow data)
        {
            favouritesList.Add(data);
        }

        // Initializes all available currency items for flipping.
        private void populateList()
        {
            foreach (Currency entry in currency)
            {
                if(entry.name != "chaos")
                {
                    dataList.Add(new CurrencyRow { CTYPE1 = entry.name, CTYPE2 = "chaos", CIMAGE1=entry.image, CIMAGE2 = "ImageAssets/Chaos.png" });
                }
                if (entry.name != "exalted")
                {
                    dataList.Add(new CurrencyRow { CTYPE1 = entry.name, CTYPE2 = "exalted", CIMAGE1= entry.image, CIMAGE2 = "ImageAssets/Exalted.png" });
                }

            }
        }

        // Given receive and pay currency strings, retrieves the 7th listing info from currency.poe.trade. If there are <=12 listings, index = listing.size/2.
        // Returns an array[receive#,have#] representing ( receive <- pay )
        private double[] refresh(string receive, string pay)
        {
            double[] result = new double[] { 0, 0 };
            int receiveID = currency.Find(currency => currency.name == receive).id;
            int payID = currency.Find(currency => currency.name == pay).id;
            var Url = @"http://currency.poe.trade/search?league=Incursion&online=x&stock=&want=" + receiveID + "&have=" + payID;
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

            return result;

        }

        
        private void profit_Changed(object sender, TextChangedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            string text = (tb.Text.TrimEnd('%'));
            if (text != "") //can't convert whitespace to double. change later for efficiency? 
            {
                double percentage = Convert.ToDouble(text);

                if (percentage >= 7.5)
                {
                    tb.Foreground = Brushes.GreenYellow;

                }
                else if (percentage > 0)
                {
                    tb.Foreground = Brushes.LightGoldenrodYellow;
                }
                else
                {
                    tb.Foreground = Brushes.Red;
                }
            }
        }

        private void flatprofit_Changed(object sender, TextChangedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            string text = tb.Text.TrimEnd('c');
            if (text != "")
            {
                double flatprofit = Convert.ToDouble(text);
                if(flatprofit > 5)
                {
                    tb.Foreground = Brushes.GreenYellow;
                }
                else if(flatprofit > 0)
                {
                    tb.Foreground = Brushes.LightGoldenrodYellow;
                }
                else
                {
                    tb.Foreground = Brushes.OrangeRed;
                }
             
            }
        }

        private void copyToClipboard(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            Grid g = (Grid)btn.Content;
            
            TextBox textbox = g.Children.OfType<TextBox>().FirstOrDefault();
            Image currencyimage = g.Children.OfType<Image>().LastOrDefault();
            
            string text = textbox.Text;
            string[] textarray = text.Split('⇐');
            string c1value = textarray[0].Trim();
            string c2value = textarray[1].Trim();

            string currencytype = (string)currencyimage.ToolTip;


            Clipboard.SetText("~b/o " + c2value + "/" + c1value + " " + currency.Find(c => c.name == currencytype).tag);
        }

        // Update numbers for the row by sending a request to currency.poe.trade **function is costly, optimize wherever possible**
        private void updateRow(object sender, RoutedEventArgs e)
        {
            Button b = (Button)sender;
            Grid g = (Grid) b.Parent;
            IEnumerable<Button> buttonList = g.Children.OfType<Button>();
            Button sellbutton = buttonList.ElementAtOrDefault(0);
            Button buybutton = buttonList.ElementAtOrDefault(1);

            Grid sbgrid = (Grid) sellbutton.Content;
            Grid bbgrid = (Grid) buybutton.Content;
            string ctype1 = (string) sbgrid.Children.OfType<Image>().FirstOrDefault().ToolTip;
            string ctype2 = (string) sbgrid.Children.OfType<Image>().LastOrDefault().ToolTip;
            double[] sellvalues = refresh(ctype1, ctype2);
            double[] buyvalues = refresh(ctype2, ctype1);

            TextBox sbtextbox = sbgrid.Children.OfType<TextBox>().FirstOrDefault();
            TextBox bbtextbox = bbgrid.Children.OfType<TextBox>().LastOrDefault();
            sbtextbox.Text = sellvalues[0] + " ⇐ " + sellvalues[1];
            bbtextbox.Text = buyvalues[0] + " ⇐ " + buyvalues[1];

            g.Children.OfType<TextBox>().FirstOrDefault().Text = profitmargin(sellvalues[0],sellvalues[1],buyvalues[0],buyvalues[1]) + "%";
            g.Children.OfType<TextBox>().LastOrDefault().Text = flatprofit(sellvalues[0], sellvalues[1], buyvalues[0], buyvalues[1]) + "c";
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
