using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
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
        ObservableCollection<CurrencyRow> dataList = new ObservableCollection<CurrencyRow>();
        ObservableCollection<CurrencyRow> favouritesList = new ObservableCollection<CurrencyRow>();
        HashSet<CurrencyRow> removefav_queue = new HashSet<CurrencyRow>();
        public string link = "http://currency.poe.trade/search?league=Delve&online=x&stock=&want=";



        public MainWindow()
        {
            InitializeComponent();

            initCurrency();
            PopulateList();
            items.ItemsSource = dataList;
            items_Fav.ItemsSource = favouritesList;

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

        private void CloseWindow(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void MinimizeWindow(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void DragWindow(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }




        // Calculates the flat profit value of 1 trade cycle.
        // receive and pay represents the cash-out trade. receive2 and pay2 should represent the buy-in trade.
        // Returns a double representing the flat profit in *chaos* units.
        private double Flatprofit(double receive, double pay, double receive2, double pay2)
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
        private double Profitmargin(double receive, double pay, double receive2, double pay2)
        {
            double profit = 0.0;
            double buyvalue = pay2 / receive2;
            double sellvalue = receive / pay;
            profit = (buyvalue / sellvalue) - 1;

            return Math.Round(profit, 2) * 100;
        }

        // Given an array, add it to the favourites list.
        private void AddFavourite(CurrencyRow data)
        {
            data.CHECKED = false;
            favouritesList.Add(data);
        }

        // Initializes all available currency items for flipping.
        private void PopulateList()
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
        private double[] Refresh(string receive, string pay)
        {
            double[] result = new double[] { 0, 0 };
            int receiveID = currency.Find(currency => currency.name == receive).id;
            int payID = currency.Find(currency => currency.name == pay).id;
            var Url = link + receiveID + "&have=" + payID;
            var data = new MyWebClient().DownloadString(Url);
            var doc = new HtmlDocument();
            doc.LoadHtml(data);

            var htmlNodeList = doc.DocumentNode.SelectNodes("//div[@class='displayoffer ']");
            if (htmlNodeList == null) {
                result[0] = 0;
                result[1] = 0;
                return result;
            }

            int index = 6;
            int htmlListSize = htmlNodeList.Count;
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

        
        private void Profit_Changed(object sender, TextChangedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            string text = (tb.Text.TrimEnd('%'));
            if (text != "") 
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
        private void Profit_Loaded(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            string text = (tb.Text.TrimEnd('%'));
            if (text != "") 
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
        private void Flatprofit_Changed(object sender, TextChangedEventArgs e)
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
        private void Flatprofit_Loaded(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            string text = tb.Text.TrimEnd('c');
            if (text != "")
            {
                double flatprofit = Convert.ToDouble(text);
                if (flatprofit > 5)
                {
                    tb.Foreground = Brushes.GreenYellow;
                }
                else if (flatprofit > 0)
                {
                    tb.Foreground = Brushes.LightGoldenrodYellow;
                }
                else
                {
                    tb.Foreground = Brushes.OrangeRed;
                }

            }
        }
        private void CopyToClipboard(object sender, RoutedEventArgs e)
        {

            Button btn = (Button)sender;
            btn.IsEnabled = false;
            Grid g = (Grid)btn.Content;
            
            TextBox textbox = g.Children.OfType<TextBox>().FirstOrDefault();
            Image currencyimage = g.Children.OfType<Image>().LastOrDefault();
            
            string text = textbox.Text;

            string[] textarray = text.Split('⇐');
            string c1value = textarray[0].Trim();
            string c2value = textarray[1].Trim();

            string currencytype = (string)currencyimage.ToolTip;


            Clipboard.SetText("~b/o " + c2value + "/" + c1value + " " + currency.Find(c => c.name == currencytype).tag);
            var backgroundWorker = new BackgroundWorker();
            backgroundWorker.DoWork += (s, ea) => Thread.Sleep(TimeSpan.FromSeconds(0.5));
            backgroundWorker.RunWorkerCompleted += (s, ea) =>
            {
                textbox.FontSize = 15;
                textbox.Text = text;
                btn.IsEnabled = true;
            };

            textbox.FontSize = 10;
            textbox.Text = "Copied to Clipboard";
            backgroundWorker.RunWorkerAsync();



        }

        // Update numbers for the row by sending a request to currency.poe.trade 
        private void UpdateRow(object sender, RoutedEventArgs e)
        {
            Button b = (Button)sender;
            Grid g = (Grid) b.Parent;

            CurrencyRow cr = (CurrencyRow)g.DataContext;

            double[] sellvalues = Refresh(cr.CTYPE1, cr.CTYPE2);
            double[] buyvalues = Refresh(cr.CTYPE2, cr.CTYPE1);

            


            //real data
            foreach (CurrencyRow entry in dataList)
            {
                if (entry.CTYPE1 == cr.CTYPE1 && entry.CTYPE2 == cr.CTYPE2)
                {
                    entry.Receive1 = sellvalues[0];
                    entry.Pay1 = sellvalues[1];
                    entry.Receive2 = buyvalues[0];
                    entry.Pay2 = buyvalues[1];
                    entry.PROFIT = Profitmargin(sellvalues[0], sellvalues[1], buyvalues[0], buyvalues[1]) + "%";
                    entry.FLATPROFIT = Flatprofit(sellvalues[0], sellvalues[1], buyvalues[0], buyvalues[1]) + "c";
                    entry.Sellstring = "";
                    entry.Buystring = "";
                    
                }
            }

        }

        private void AddFavourites_Click(object sender, RoutedEventArgs e)
        {
            
            foreach (CurrencyRow cr in dataList)
            {
                if (cr.CHECKED && favouritesList.Contains(cr) == false)
                {
                    cr.CHECKED = false;
                    favouritesList.Add(cr);
                }
                
            }
            
            
        }

        private void Checkmarked(object sender, RoutedEventArgs e)
        {
            CheckBox cb = (CheckBox)sender;
            Grid g = (Grid)cb.Parent;
            CurrencyRow row = (CurrencyRow)g.DataContext;
            row.CHECKED = true;

        }

        private void RemoveFromFavourites_Click(object sender, RoutedEventArgs e)
        {
            foreach (CurrencyRow cr in removefav_queue)
            {
                if (cr.CHECKED == true)
                {
                    favouritesList.Remove(cr);
                   
                }
                Console.WriteLine(cr.CHECKED.ToString());
            }
            removefav_queue.Clear();

        }

        private void Favourites_Checkmarked(object sender, RoutedEventArgs e)
        {
            CheckBox cb = (CheckBox)sender;
            Grid g = (Grid)cb.Parent;
            CurrencyRow row = (CurrencyRow)g.DataContext;
            removefav_queue.Add(row);
            row.CHECKED = true;
        }

        private bool entireListChecked(ObservableCollection<CurrencyRow> list)
        {
            foreach (CurrencyRow cr in list)
            {
                if (cr.CHECKED == false)
                {
                    return false;
                }
            }
            return true;
        }

        private void SelectAllCurrency_Click(object sender, RoutedEventArgs e)
        {
            if (entireListChecked(dataList) == true)
            {
                foreach (CurrencyRow cr in dataList)
                {
                    cr.CHECKED = false;
                }
            }
            else
            {
                foreach (CurrencyRow cr in dataList)
                {
                    cr.CHECKED = true;
                }
            }

        }

        private void SelectAllFav_Click(object sender, RoutedEventArgs e)
        {
            if (entireListChecked(favouritesList) == true)
            {
                foreach (CurrencyRow cr in favouritesList)
                {
                    cr.CHECKED = false;
                }
            }
            else
            {
                foreach (CurrencyRow cr in favouritesList)
                {
                    cr.CHECKED = true;
                }
            }
        }

        private void Fav_Unchecked(object sender, RoutedEventArgs e)
        {
            CheckBox cb = (CheckBox)sender;
            Grid g = (Grid)cb.Parent;
            CurrencyRow row = (CurrencyRow)g.DataContext;
            removefav_queue.Remove(row);
            row.CHECKED = false;
        }


        private void DropDownClosed_Event(object sender, EventArgs e)
        {
            ComboBox cb = (ComboBox)sender;
            //Console.WriteLine(cb.SelectionBoxItem);
            if (cb.SelectionBoxItem.ToString() == "Standard")
            {
                link = "http://currency.poe.trade/search?league=Standard&online=x&stock=&want=";
            }
            if (cb.SelectionBoxItem.ToString() == "Delve")
            {
                link = "http://currency.poe.trade/search?league=Delve&online=x&stock=&want=";
            }
            //Console.WriteLine(link);
        }
    }

    public class Currency
    {
        public string name = "";
        public int id = 0;
        public string tag = "";
        public string image = "";
    }

    public class CurrencyRow:INotifyPropertyChanged
    {
        public double Receive1 { get; set; }
        public double Pay1 { get; set; }
        public double Receive2 { get; set; }
        public double Pay2 { get; set; }
        private string Profit = "0%";
        private string FlatProfit = "0.0";
        private bool Checked = false;


        public string CTYPE1 { get; set; }
        public string CTYPE2 { get; set; }

        public string CIMAGE1 { get; set; }
        public string CIMAGE2 { get; set; }

        private string sellstring = "0 ⇐ 0";
        private string buystring = "0 ⇐ 0";

        public event PropertyChangedEventHandler PropertyChanged;

        public bool CHECKED
        {
            get
            {
                return Checked;
            }
            set
            {
                if (value != Checked)
                {
                    Checked = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public string PROFIT
        {
            get
            {
                return Profit;
            }
            set
            {
                
                if (value != Profit)
                {
                    Profit = value;
                    NotifyPropertyChanged();
                }

            }
        }

        public string FLATPROFIT
        {
            get
            {
                return FlatProfit;
            }
            set
            {
                if (value != FlatProfit)
                {
                    FlatProfit = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public string Buystring
        {
            get
            {
                return buystring;
            }
            set
            {
                if (value != buystring)
                {
                    buystring = Receive2 + " ⇐ " + Pay2;
                    NotifyPropertyChanged();
                }
                
            }
        }
        public string Sellstring
        {
            get
            {
                return sellstring;
            }
            set
            {
                if (value != sellstring)
                {
                    sellstring = Receive1 + " ⇐ " + Pay1;
                    NotifyPropertyChanged();
                }
                

            }
        }
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }

}
