
using Market.Classes;
using Market.Models;
using DocumentFormat.OpenXml.Drawing.Charts;
using Market;
using NiKu.Classes;
using NiKu.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Printing.IndexedProperties;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace NiKu
{
    /// <summary>
    /// Interaction logic for winOptionChain.xaml
    /// </summary>
    public partial class winOptionChain : Window
    {
        public ObservableCollection<clsOptionChain> ocCalls = new ObservableCollection<clsOptionChain>();
        List<Instruments> callputInstr;
        //clsBrokerDataSetting clsLivFeed=new clsBrokerDataSetting();
        public winOptionChain()
        {
            InitializeComponent();
            clsBrokerDataSetting.OnConnect += Onconnect;
            //live chart
            Onconnect(true); //pointing to method
        }

        void Onconnect(bool blnConnect)
        {
            if (blnConnect) { clsBrokerDataSetting.LiveData += livedata; }
            else { clsBrokerDataSetting.LiveData -= livedata; }
        }

            private void Window_Loaded_1(object sender, RoutedEventArgs e)
        {
            //load Exchanges............
            clsApp.loadExchanges(ref cmbGroup);


            //check for instruments list.............. if instruments are not available return with a message
            if (clsGenfx.liveFeedInstr.Count <= 0)
            {
                MessageBox.Show("Instruments not found");
                callputInstr = new List<Instruments>();
            }
            else
            {
                if(callputInstr==null) callputInstr= new List<Instruments>();
                //load call put instruments in the list..............
                callputInstr = clsGenfx.liveFeedInstr.Where(x => x.Symbol.Contains("CE") || x.Symbol.Contains("PE")).ToList();

                //set default selected exchange to "NFO"
                cmbGroup.SelectedItem = "NFO";

            }

         
            //initialize ticker for live data............
            Live();
        }
        
        private void cmbExchanges_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            cmbCompName.ItemsSource = null;

            string exchange = cmbGroup.SelectedValue.ToString();

            //select Company names from the call put instruments where exchanges match the selected exchange and assign to combobox.....................
            cmbCompName.ItemsSource = callputInstr.Where(x => x.Exchange == exchange).Select(x => x.Company).Distinct(); 
            //cmbCompName.ItemsSource = callputInstr.Where(x => x.Exchange == exchange).Distinct();
        }

        string compName;
        string compScriptCode;
        private void cmbCompName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            cmbExpiry.ItemsSource = null;

            if (cmbCompName.SelectedItem == null) return;
            //else
            //{
            //    Instruments instr=new Instruments();
            //    instr= cmbCompName.SelectedItem as Instruments;
            //    if(instr==null) return;
            //    else
            //    {
            //        compName = instr.Company.ToString();
            //        compScriptCode = instr.ScriptCode;
            //    }
            //}
            compName = cmbCompName.SelectedItem.ToString();
            

            //select exprity dates where companies call put where the company name matches in the list and assign to combobox........
            cmbExpiry.ItemsSource = callputInstr.Where(x => x.Company == compName).Select(x => x.Expiry).Distinct();
        }

        List<Instruments> instr;
        DateTime expiry;

        private void btnshow_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                ////subscribe the scripts of call's and put's..............
                ocCalls.ToList().ForEach(x => { App.brk.unSubscribe(x.exchangeCode, x.scriptCode.ToString(), 1); App.brk.unSubscribe(x.P_ExchangeCode, x.P_ScriptCode.ToString(), 1); });

                //clear the collection before assigning new collection...............
                ocCalls.Clear();

                expiry = Convert.ToDateTime(cmbExpiry.SelectedItem);

                //getting data where expiry and company name matches to a list........................
                instr = callputInstr.Where(x => x.Expiry == expiry && x.Company == compName).Distinct().ToList();
                //if (clsGenfx.livefeed == FeedAPI.TrueData)
                //{
                //    ForTrueData(compName, expiry);
                //}

                addItemsToListView();
            }
            catch (Exception ex) 
            {
                MessageBox.Show(ex.Message);
            }

            

        }

        void ForTrueData(string sc,DateTime dt)
        {
            instr = clsBrokerDataSetting.getInstrumentsOptionchaing(sc, dt);
        }
        /// <summary>
        /// add call put items to listview
        /// </summary>
        /// 
        
        private void addItemsToListView()
        {
            List<Instruments> ocput = new List<Instruments>();
            List<Instruments> occall = new List<Instruments>();
            //getting Calls and Puts in two different lists............            
            ocput = instr.Where(x => x.Symbol.Contains("PE")).OrderBy(x=> x.Strike).ToList();
            occall = instr.Where(x => x.Symbol.Contains("CE")).ToList().OrderBy(x => x.Strike).ToList();

            for (int i = 0; i < ocput.Count; i++)
            {
                clsOptionChain oc = new clsOptionChain();
                //assign put's script details to a object of clas...............
                oc.P_ScriptCode =  ocput[i].ScriptCode;
                oc.P_ExchangeCode = ocput[i].ExchangeCode;
                oc.P_StrikePrice = Convert.ToDouble(ocput[i].Strike);
                oc.P_background = Brushes.Transparent;


                //assign call's script details to a object of clas...............
                oc.background = Brushes.Transparent;
                if (i <= occall.Count - 1)
                {
                    oc.scriptCode = occall[i].ScriptCode;
                    oc.exchangeCode = occall[i].ExchangeCode;
                    ocCalls.Add(oc);
                }                
                
            }

            List<clsOptionChain> ocallFiletered = new List<clsOptionChain>();
            ocallFiletered = ocCalls.GroupBy(x=>x.scriptCode).Select(g=>g.First()).ToList();
            //ocallFiletered = ocCalls.ToList();
            //For true data load data from its option chain method first
            if (clsGenfx.livefeed == FeedAPI.TrueData)
            {
                List<Market.Classes.clsOptionChain> getTrueOptions = new List<Market.Classes.clsOptionChain>();

                getTrueOptions = clsBrokerDataSetting.getInstrumentsOptionchaingLive(compName, expiry);
                if(getTrueOptions.Count > 0)
                {
                    for(int i = 0; i < getTrueOptions.Count; i++)
                    {
                        if(i<=ocallFiletered.Count - 1)
                        {
                            //Calls data
                            ocallFiletered[i].OI = getTrueOptions[i].OI;
                            ocallFiletered[i].ChngInOI = getTrueOptions[i].ChngInOI;
                            ocallFiletered[i].Volume = getTrueOptions[i].Volume;
                            ocallFiletered[i].IV = getTrueOptions[i].IV;
                            ocallFiletered[i].LTP = getTrueOptions[i].LTP;
                            ocallFiletered[i].NetChng = getTrueOptions[i].NetChng;
                            ocallFiletered[i].BidQty = getTrueOptions[i].BidQty;
                            ocallFiletered[i].BidPrice = getTrueOptions[i].BidPrice;
                            ocallFiletered[i].AskPrice = getTrueOptions[i].AskPrice;
                            ocallFiletered[i].AskQty = getTrueOptions[i].AskQty;

                            ocallFiletered[i].StrikePrice = getTrueOptions[i].StrikePrice;
                            ocallFiletered[i].delta = getTrueOptions[i].delta;
                            ocallFiletered[i].gamma = getTrueOptions[i].gamma;
                            ocallFiletered[i].theta = getTrueOptions[i].theta;
                            ocallFiletered[i].vega = getTrueOptions[i].vega;
                            ocallFiletered[i].rho = getTrueOptions[i].rho;

                            //Puts data
                            ocallFiletered[i].P_OI = getTrueOptions[i].P_OI;
                            ocallFiletered[i].P_ChngInOI = getTrueOptions[i].P_ChngInOI;
                            ocallFiletered[i].P_Volume = getTrueOptions[i].P_Volume;
                            ocallFiletered[i].P_IV = getTrueOptions[i].P_IV;
                            ocallFiletered[i].P_LTP = getTrueOptions[i].P_LTP;
                            ocallFiletered[i].P_NetChng = getTrueOptions[i].P_NetChng;
                            ocallFiletered[i].P_BidQty = getTrueOptions[i].P_BidQty;
                            ocallFiletered[i].P_BidPrice = getTrueOptions[i].P_BidPrice;
                            ocallFiletered[i].P_AskPrice = getTrueOptions[i].P_AskPrice;
                            ocallFiletered[i].P_AskQty = getTrueOptions[i].P_AskQty;

                            ocallFiletered[i].P_StrikePrice = getTrueOptions[i].P_StrikePrice;
                            ocallFiletered[i].P_delta = getTrueOptions[i].P_delta;
                            ocallFiletered[i].P_gamma = getTrueOptions[i].P_gamma;
                            ocallFiletered[i].P_theta = getTrueOptions[i].P_theta;
                            ocallFiletered[i].P_vega = getTrueOptions[i].P_vega;
                            ocallFiletered[i].P_rho = getTrueOptions[i].P_rho;
                        }
                    }
                }
            }

            //assign itemsource..................
             //lvCalls.ItemsSource = ocCalls;
            lvCalls.ItemsSource = ocallFiletered;
            ////subscribe the scripts of call's and put's..............
            ocCalls.ToList().ForEach(x => { App.brk.subscribe(x.exchangeCode, x.scriptCode.ToString(),1); App.brk.subscribe(x.P_ExchangeCode, x.P_ScriptCode.ToString(),1); });

        }

        public void Live()
        {
            ////live chart
            //clsk.initTicker();
            //clsk.LiveData(livedata);//pointing to method          
            //clsk._OnConnect = OnConnect;
            //clsk._OnError = OnError;
            //clsk._OnReconnect = OnReconnect;
        }

        private void OnReconnect()
        {
        }

        private void OnError(string obj)
        {
        }

        private void OnConnect()
        {
        }

        //Display LIVE DATA ----------------
        void livedata(LiveTick TickData)
        {
            if(TickData == null) { return; }    

            if(TickData.ScriptCode == null) { return; } 
            if(!ocCalls.Any(x=>x.scriptCode== TickData.ScriptCode || x.P_ScriptCode==TickData.ScriptCode )) { return; }
            //-------------------------------------------------
            // Testing: display Live data using Dispatcher
            //-------------------------------------------------
            try
            {
                //----------------------------------------------
                // Invoke Dispatcher
                //----------------------------------------------
                Dispatcher.Invoke(new Action(() => 
                {

                    //get object from collection where the instrumentsToken of TickData matches the script either of call or put...........
                    clsOptionChain op = ocCalls.ToList().Find(x => x.scriptCode.ToString() == TickData.ScriptCode || x.P_ScriptCode.ToString() == TickData.ScriptCode);
                    double ltp = op.LTP;
                    double putltp = op.P_LTP;

                    //when instruments token matches with the call's script code
                    //assign TickData's values to call properties...................
                    if (op.scriptCode.ToString() == TickData.ScriptCode)
                    {
                        //op.OI = TickData.OI;
                        if(Convert.ToDouble(TickData.Ask)!=op.AskPrice && TickData.Ask>0) op.AskPrice = Convert.ToDouble(TickData.Ask);
                        if (Convert.ToDouble(TickData.AskQuantity) != op.AskQty && TickData.AskQuantity > 0) op.AskQty = Convert.ToUInt32(TickData.AskQuantity);
                        op.BidPrice = Convert.ToDouble(TickData.Bid);
                        op.BidQty = Convert.ToUInt32(TickData.BidQuantity);

                        //op.ChngInOI = TickData.OIDayHigh - TickData.OIDayLow;
                        op.LTP = Convert.ToDouble(TickData.LastPrice);

                        if (ltp > op.LTP)
                        {
                            op.background = Brushes.YellowGreen;
                        }
                        else if (ltp < op.LTP)
                            op.background = Brushes.BlanchedAlmond;

                        op.NetChng = Convert.ToDouble(TickData.LastPrice - TickData.Close);
                        op.Volume = Convert.ToDouble(TickData.Volume);
                        op.rho=TickData.rho;
                        op.delta = TickData.delta;
                        op.gamma = TickData.gamma;
                        op.theta = TickData.theta;
                        op.vega = TickData.vega;
                        op.IV = TickData.IV;

                    }
                    else
                    {
                        //when instruments token matches with the put's script code
                        //assign TickData's values to Put properties...................
                        //op.P_OI = TickData.OI;
                        op.P_AskPrice = Convert.ToDouble(TickData.Ask);
                        op.P_AskQty = Convert.ToUInt32(TickData.AskQuantity);
                        op.P_BidPrice = Convert.ToDouble(TickData.Bid);
                        op.P_BidQty = Convert.ToUInt32(TickData.BidQuantity);

                        //op.P_ChngInOI = TickData.OIDayHigh - TickData.OIDayLow;
                        op.P_LTP = Convert.ToDouble(TickData.LastPrice);
                        op.P_NetChng = Convert.ToDouble(TickData.LastPrice - TickData.Close);
                        op.P_Volume = Convert.ToDouble(TickData.Volume);

                        if (putltp > op.P_LTP)
                        {
                            op.P_background = Brushes.YellowGreen;
                        }
                        else if (putltp < op.P_LTP)
                            op.P_background = Brushes.BlanchedAlmond;

                        op.P_rho = TickData.rho;
                        op.P_delta = TickData.delta;
                        op.P_gamma = TickData.gamma;
                        op.P_theta = TickData.theta;
                        op.P_vega = TickData.vega;
                        op.P_IV = TickData.IV;
                    }

                    //---------------------------------------------------------
                    // Update the OptionChain Model Values in ocCalls list
                    //---------------------------------------------------------

                    //Get index of optionchain in list containing the Scriptcode and P ScriptCode
                    //int index = ocCalls.IndexOf(ocCalls.FirstOrDefault(x => x.scriptCode.ToString() == TickData.ScriptCode || x.P_ScriptCode.ToString() == TickData.ScriptCode));

                    //if (ocCalls[index].scriptCode == TickData.ScriptCode)
                    //{
                    //    ocCalls[index].AskPrice = op.AskPrice;
                    //    ocCalls[index].AskQty = op.AskQty;
                    //    ocCalls[index].BidPrice = op.BidPrice;
                    //    ocCalls[index].BidQty = op.BidQty;
                    //    ocCalls[index].LTP = op.LTP;
                    //    ocCalls[index].NetChng = op.NetChng;
                    //    ocCalls[index].Volume = op.Volume;
                    //    ocCalls[index].background = op.background;
                    //}
                    //else if (ocCalls[index].P_ScriptCode == TickData.ScriptCode)
                    //{
                    //    ocCalls[index].P_AskPrice = op.P_AskPrice;
                    //    ocCalls[index].P_AskQty = op.P_AskQty;
                    //    ocCalls[index].P_BidPrice = op.P_BidPrice;
                    //    ocCalls[index].P_BidQty = op.P_BidQty;
                    //    ocCalls[index].P_LTP = op.P_LTP;
                    //    ocCalls[index].P_NetChng = op.P_NetChng;
                    //    ocCalls[index].P_Volume = op.P_Volume;
                    //    ocCalls[index].P_background = op.P_background;
                    //}
                    lvCalls.Items.Refresh();

                }),System.Windows.Threading.DispatcherPriority.ContextIdle);
            }

            catch(Exception ex) 
            {
                string msg = ex.Message;    
            }

           

        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //MessageBoxResult res = MessageBox.Show("Are you sure you want to exit ??", "Option Chain", MessageBoxButton.YesNo, MessageBoxImage.Question);
            //if (res == MessageBoxResult.Yes)
            //{
            //    if(ocCalls.Count > 0)
            //    {
            try
            {
                ocCalls.ToList().ForEach(x => {
                    App.brk.unSubscribe(x.exchangeCode, x.scriptCode.ToString(), 1);
                    App.brk.unSubscribe(x.P_ExchangeCode, x.P_ScriptCode.ToString(), 1);
                });
            }
            catch (Exception ex)
            {

            }
                
            //    }
            //}

            //else
            //    e.Cancel = true;
        }
    }

    public class clsOptionChain
    {
        public string scriptCode { get; set; } 

        public int exchangeCode { get; set; }
        
        public uint OI { get; set; }
        public uint ChngInOI { get; set; }
        public double Volume { get; set; }
        public double IV { get; set; }
        public double LTP { get; set; }
        public double NetChng { get; set; }
        public uint BidQty { get; set; }
        public double BidPrice { get; set; }
        public double AskPrice { get; set; }
        public uint AskQty { get; set; }
        public Brush background { get; set; }
        public double StrikePrice { get; set; }
        //Added on 02-02-2025
        public float delta { get; set; }
        public float gamma { get; set; }
        public float theta { get; set; }
        public float vega { get; set; }
        public float rho { get; set; }
        

        public int P_ExchangeCode { get; set; }

        public string P_ScriptCode { get; set; }
        public uint P_OI { get; set; }
        public uint P_ChngInOI { get; set; }
        public double P_Volume { get; set; }
        public double P_IV { get; set; }
        public double P_LTP { get; set; }
        public double P_NetChng { get; set; }
        public uint P_BidQty { get; set; }
        public double P_BidPrice { get; set; }
        public double P_AskPrice { get; set; }
        public uint P_AskQty { get; set; }
        public Brush P_background { get; set; }
        public double P_StrikePrice { get; set; }
        //Added on 02-02-2025
        public float P_delta { get; set; }
        public float P_gamma { get; set; }
        public float P_theta { get; set; }
        public float P_vega { get; set; }
        public float P_rho { get; set; }
        

    }

    
}
