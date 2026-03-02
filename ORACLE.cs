#region Using declarations
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml.Serialization;
using NinjaTrader.Cbi;
using NinjaTrader.Gui;
using NinjaTrader.Gui.Chart;
using NinjaTrader.Gui.SuperDom;
using NinjaTrader.Data;
using NinjaTrader.NinjaScript;
using NinjaTrader.Core.FloatingPoint;
using NinjaTrader.NinjaScript.Indicators;
using NinjaTrader.NinjaScript.DrawingTools;
using NinjaTrader.NinjaScript.MarketAnalyzerColumns;
#endregion

//This namespace holds Strategies in this folder and is required. Do not change it. 
namespace NinjaTrader.NinjaScript.Strategies
{
    public class ORACLE : Strategy
    {
        private bool longButtonClicked;
        private bool shortButtonClicked;
        private System.Windows.Controls.Button longButton;
        private System.Windows.Controls.Button shortButton;
        private System.Windows.Controls.Grid myGrid;
        private int bet = 1;
        private int rebet = 1;
        private double lbp, gear, gears, tem;   //long base price
        private EMA bema;
        private int em = 27;
        private double point = 10;
        private int sl = 33;
        protected override void OnStateChange()
        {
            if (State == State.SetDefaults)
            {
                Description = @"ORACLE";
                Name = "ORACLE";
                Calculate = Calculate.OnEachTick;
                EntriesPerDirection = 1;
                EntryHandling = EntryHandling.AllEntries;
                IsExitOnSessionCloseStrategy = true;
                ExitOnSessionCloseSeconds = 30;
                IsFillLimitOnTouch = false;
                MaximumBarsLookBack = MaximumBarsLookBack.TwoHundredFiftySix;
                OrderFillResolution = OrderFillResolution.Standard;
                Slippage = 0;
                StartBehavior = StartBehavior.WaitUntilFlat;
                TimeInForce = TimeInForce.Gtc;
                TraceOrders = false;
                RealtimeErrorHandling = RealtimeErrorHandling.StopCancelClose;
                StopTargetHandling = StopTargetHandling.PerEntryExecution;
                BarsRequiredToTrade = 20;
            }
            else if (State == State.Configure)
            {
                bema = EMA(em);

            }
            else if (State == State.Historical)
            {
                if (UserControlCollection.Contains(myGrid))
                    return;

                Dispatcher.InvokeAsync((() =>
                {
                    myGrid = new System.Windows.Controls.Grid
                    {
                        Name = "MyCustomGrid",
                        HorizontalAlignment = HorizontalAlignment.Right,
                        VerticalAlignment = VerticalAlignment.Top
                    };

                    System.Windows.Controls.ColumnDefinition column1 = new System.Windows.Controls.ColumnDefinition();
                    System.Windows.Controls.ColumnDefinition column2 = new System.Windows.Controls.ColumnDefinition();

                    myGrid.ColumnDefinitions.Add(column1);
                    myGrid.ColumnDefinitions.Add(column2);

                    longButton = new System.Windows.Controls.Button
                    {
                        Name = "LongButton",
                        Content = "LONG",
                        Foreground = Brushes.White,
                        Background = Brushes.Green
                    };

                    shortButton = new System.Windows.Controls.Button
                    {
                        Name = "ShortButton",
                        Content = "SHORT",
                        Foreground = Brushes.Black,
                        Background = Brushes.Red
                    };

                    longButton.Click += OnButtonClick;
                    shortButton.Click += OnButtonClick;

                    System.Windows.Controls.Grid.SetColumn(longButton, 0);
                    System.Windows.Controls.Grid.SetColumn(shortButton, 1);

                    myGrid.Children.Add(longButton);
                    myGrid.Children.Add(shortButton);

                    UserControlCollection.Add(myGrid);
                }));
            }
            else if (State == State.Terminated)
            {
                Dispatcher.InvokeAsync((() =>
                {
                    if (myGrid != null)
                    {
                        if (longButton != null)
                        {
                            myGrid.Children.Remove(longButton);
                            longButton.Click -= OnButtonClick;
                            longButton = null;
                        }
                        if (shortButton != null)
                        {
                            myGrid.Children.Remove(shortButton);
                            shortButton.Click -= OnButtonClick;
                            shortButton = null;
                        }
                    }
                }));
            }
        }

        protected override void OnBarUpdate()
        {
            if (CurrentBar < BarsRequiredToTrade)
                return;
            if (Position.MarketPosition == MarketPosition.Flat)
            {
                if (longButtonClicked)
                {
                    EnterLong(bet);               
                    gear = 1;
                    longButtonClicked = false;
                }
                if(shortButtonClicked && gears == 0 )
                {
                    
                    EnterShort(bet);
                    gears = 1;
                   
                    shortButtonClicked = false;
                }
            }


       

            if (Position.MarketPosition == MarketPosition.Flat)
            {
                
                SetStopLoss("", CalculationMode.Ticks, sl, false);

            }
            else if (Position.MarketPosition == MarketPosition.Long)
            {
                if (Close[0] > Position.AveragePrice + point   && gear == 1)
                {
                    if (CrossBelow(Close, bema, 1))
                    {
                        EnterLong(rebet);
                        
                        gear = 2;
                        SetStopLoss(CalculationMode.Price, Position.AveragePrice);
                        tem = Weighted[0];
                    }
                    
                }
                else if (Close[0] > tem + (point/2) && gear ==2)
                {
                    if (CrossBelow(Close, bema, 1))
                    {
                        EnterLong(rebet);
                        gear = 3;
                        SetStopLoss(CalculationMode.Price, Position.AveragePrice + 1);
                        tem = Weighted[0];
                    }

                }
                else if (Close[0] > tem + (point / 2) && gear == 3)
                {
                    if (CrossBelow(Close, bema, 1))
                    {
                        EnterLong(rebet);
                        gear = 4;
                        SetStopLoss(CalculationMode.Price, Position.AveragePrice + 1);
                        tem = Weighted[0];
                    }
                }
                else if (Close[0] > tem + (point / 2) && gear == 4)
                {
                    if (CrossBelow(Close, bema, 1))
                    {
                        EnterLong(rebet);
                        gear = 5;
                        SetStopLoss(CalculationMode.Price, Position.AveragePrice + 1);
                        tem = Weighted[0];
                    }

                }
                else if (Close[0] > tem + (point / 2) && gear == 5)
                {
                    if (CrossBelow(Close, bema, 1))
                    {
                        EnterLong(rebet);
                        gear = 6;
                        SetStopLoss(CalculationMode.Price, Position.AveragePrice + 1);
                        tem = Weighted[0];
                    }

                }

            }
            else if (Position.MarketPosition == MarketPosition.Short)
            {
                if (Close[0] < Position.AveragePrice - point && gears == 1)
                {
                    if (CrossAbove(Close, bema, 1))
                    {
                        EnterShort(rebet);
                        gears = 2;
                       
                        SetStopLoss(CalculationMode.Price, Position.AveragePrice);
                        tem = Weighted[0];
                    }
                }
                else if (Close[0] < tem - (point / 2) && gears == 2)
                {
                    if (CrossAbove(Close, bema, 1))
                    {
                        EnterShort(rebet);
                        gears = 3;
                        SetStopLoss(CalculationMode.Price, Position.AveragePrice - 1);
                        tem = Weighted[0];
                    }
                }
                else if (Close[0] < tem - (point / 2) && gears == 3)
                {
                    if (CrossAbove(Close, bema, 1))
                    {
                        EnterShort(rebet);
                        gears = 4;
                        SetStopLoss(CalculationMode.Price, Position.AveragePrice - 1);
                        tem = Weighted[0];
                    }
                }
                else if (Close[0] < tem - (point / 2) && gears == 4)
                {
                    if (CrossAbove(Close, bema, 1))
                    {
                        EnterShort(rebet);
                        gears = 5;
                        SetStopLoss(CalculationMode.Price, Position.AveragePrice - 1);
                        tem = Weighted[0];
                    }
                }
                else if (Close[0] < tem - (point / 2) && gears == 5)
                {
                    if (CrossAbove(Close, bema, 1))
                    {
                        EnterShort(rebet);
                        gears = 6;
                        SetStopLoss(CalculationMode.Price, Position.AveragePrice - 1);
                        tem = Weighted[0];
                    }
                }
            }





        }

        private void OnButtonClick(object sender, RoutedEventArgs rea)
        {

            System.Windows.Controls.Button button = sender as System.Windows.Controls.Button;
            if (button == longButton && button.Name == "LongButton" && button.Content == "LONG")
            {             
                longButtonClicked = true;
                return;
            }
            if (button == shortButton && button.Name == "ShortButton" && button.Content == "SHORT")
            {
               
                shortButtonClicked = true;
                return;
            }


        }
        #region Properties
        [Range(1, int.MaxValue), NinjaScriptProperty]
        [Display(ResourceType = typeof(Custom.Resource), Name = "sl in ticks", GroupName = "NinjaScriptParameters", Order = 8)]
        public int Sl
        {
            get { return sl; }
            set { sl = value; }
        }
        [Range(1, int.MaxValue), NinjaScriptProperty]
        [Display(ResourceType = typeof(Custom.Resource), Name = "points to trigger", GroupName = "NinjaScriptParameters", Order = 7)]
        public double POINT
        {
            get { return point; }
            set { point = value; }
        }
        [Range(1, int.MaxValue), NinjaScriptProperty]
        [Display(ResourceType = typeof(Custom.Resource), Name = "Intial contracts", GroupName = "NinjaScriptParameters", Order = 1)]
        public int BET
        {
            get { return bet; }
            set { bet = value; }
        }
        [Range(1, int.MaxValue), NinjaScriptProperty]
        [Display(ResourceType = typeof(Custom.Resource), Name = "add contracts", GroupName = "NinjaScriptParameters", Order = 2)]
        public int REBET
        {
            get { return rebet; }
            set { rebet = value; }
        }
        [Range(1, int.MaxValue), NinjaScriptProperty]
        [Display(ResourceType = typeof(Custom.Resource), Name = "EMA juicer", GroupName = "NinjaScriptParameters", Order = 3)]
        public int EEM
        {
            get { return em; }
            set { em = value; }
        }



        #endregion
    }
}