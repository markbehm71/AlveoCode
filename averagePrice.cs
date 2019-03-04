
using System;
using System.ComponentModel;
using System.Windows.Media;
using Alveo.Interfaces.UserCode;
using Alveo.UserCode;
using Alveo.Common;
using Alveo.Common.Classes;
using System.Collections.Generic;
using System.Linq;

namespace Alveo.UserCode
{
    [Serializable]
    [Description("")]
    public class averagePrice : IndicatorBase
    {
        #region Properties
        
        
        #endregion
        public double Poin=0;
        public datetime last_time=0;
          public bool  ___Comissions_Adjustment = true;
        public bool    ___Swaps_Adjustment  = true;
        public int   Show_Micro_Mini_Full_123 = 1;
        public string   LOTstr,side;
        public string ignoreList {get;set;}
        int     pips2points;    // slippage  3 pips    3=points    30=points (OrderClosePrice()-OrderOpenPrice() )/pips2points) = #poins 
		double  pips2dbl;       // Stoploss 15 pips    0.0015      0.00150  (OrderClosePrice()-OrderOpenPrice() )/pips2dbl) = #pips
		int     DigitsPips;
		
        public averagePrice()
        {
            // Basic indicator initialization. Don't use this constructor to calculate values
            
            indicator_buffers = 0;
            //foobar = 0;
            ignoreList = "";
            copyright = "";
            link = "";

        }
        
        //+------------------------------------------------------------------+");
        //| Custom indicator initialization function                         |");
        //+------------------------------------------------------------------+");
        protected override int Init()
        {
        	// ENTER YOUR CODE HERE
        	  Poin = Point*10;
   if((StringSubstr(Symbol(),0,6)=="XAUUSD")||(StringSubstr(Symbol(),0,4)=="GOLD")) {Poin=0.01;}
   else {if (StringSubstr(Symbol(),0,6) == "EURTRY") {Poin = 0.001;}
   else {if (StringSubstr(Symbol(),0,6) == "USDTRY") {Poin = 0.001;}
   else {if (StringSubstr(Symbol(),0,6) == "USDMXN") {Poin = 0.001;}
   else {if (StringSubstr(Symbol(),0,6) == "USDCZK") {Poin = 0.001;} }}}}
	    
   if (Digits % 2 == 1){      // DE30=1/JPY=3/EURUSD=5 forum.mql4.com/43064#515262
                pips2dbl    = Point*10; pips2points = 10;   DigitsPips = 1;
    			} else {    pips2dbl    = Point;    pips2points =  1;   DigitsPips = 0; }



	   initArrow("avgPrice");
       initArrow("bidPrice");
        	return 0;
        }
        
        
        //+-------- EXTERNAL PARAMETERS HERE --------+
        //[Category("My Category")]
        //[DisplayName("My Display Name")]
        //public int fooBar { get; set; }
        
        
        //+------------------------------------------------------------------+");
        //| Custom indicator deinitialization function                       |");
        //+------------------------------------------------------------------+");
        protected override int Deinit()
        {
        	DeInitArrow("avgPrice");
      		DeInitArrow("bidPrice");
        	return 0;
        }
        
        //+------------------------------------------------------------------+");
        //| Custom indicator iteration function                              |");
        //+------------------------------------------------------------------+");
        protected override int Start()
        {
        	 if (TimeCurrent() - last_time <= .5)
					return(0);
        	  
        	  last_time = TimeCurrent();
        	  

     double avg = getAvgPriceOfOpenOrders();
  	 updateArrow("avgPrice",avg,2,230,0,255,105);
	 updateArrow("bidPrice",Bid,0,200,40,40,255);
    	
      return 0;
        }	
public bool initArrow(string n)
     {
	//ObjectCreate(n+"_0",OBJ_TREND,0,Time[0],Ask,Time[1],Bid);
	//ObjectCreate(n+"_1",OBJ_TREND,1,Time[1],Bid,Time[2],Bid);
	
	return true;
	}
public bool DeInitArrow(string n)
     {
	ObjectDelete(n+"_0");
	ObjectDelete(n+"_1");
	return true;
	}
		
public bool updateArrow(string nm,double price,datetime t0,byte a,byte r,byte g,byte b) 
	{

	// top
	string n; 
	
	double pp = NormalizeDouble(0.25*pips2dbl,Digits); // size = 1/4 pip
	 n = nm+"_0";
	 if (ObjectFind(n)<0)
		ObjectCreate(n,OBJ_TREND,0,Time[t0],price,Time[t0+1],price);
	 SetObjectPrice1(n,NormalizeDouble(price+pp,Digits)); 
	 SetObjectPrice2(n,price);
     SetObjectTime1(n,Time[t0]);
     SetObjectTime2(n,Time[t0+1]);
     SetObjectColor(n,Color.FromArgb(a,r,g,b));
	 SetObjectWidth(n,1);
	 // bottom
	 n = nm+"_1";
	  if (ObjectFind(n)<0)
		ObjectCreate(n,OBJ_TREND,0,Time[t0],price,Time[t0+1],price);
	 SetObjectPrice1(n,NormalizeDouble(price-pp,Digits)); 
	 SetObjectPrice2(n,price);
     SetObjectTime1(n,Time[t0]); 
     SetObjectTime2(n,Time[t0+1]);
     SetObjectColor(n,Color.FromArgb(a,r,g,b));
	 SetObjectWidth(n,1);
	 
	 return true;
	}
public double getAvgPriceOfOpenOrders()
{  
   double openMicroLots = 0;
   double openTotalPips = 0;
   double openProfit = 0;
   double openComm = 0;
   double openSwap = 0;
  double  openSize = 0;
   double openAvgPips = 0;
  double  openAvgPrice = 0;
   double openpriceLotProduct = 0;
 	// reparse ignore list
   	List<int> ignoreIds = new List<int>() ;
   	if (ignoreList.Length>0) // to elim c# errors
		ignoreIds = ignoreList.Split(',').Select(int.Parse).ToList(); // split string to list or ints
   	 // loop through orders
   for(int i=0; i<OrdersTotal(); i++)
   {
      if(OrderSelect(i, SELECT_BY_POS, MODE_TRADES))
      {
         if (Symbol() == OrderSymbol() && (OrderType() == OP_BUY || OrderType() == OP_SELL))
         {
         	// skip if in ignore list
         	if (ignoreIds.Contains(OrderTicket())) continue;
			//
            openMicroLots += OrderLots()*100;
            openpriceLotProduct += OrderOpenPrice()*(OrderLots()*100);
            if(OrderType()==OP_BUY) {openTotalPips += OrderLots()*100*(Bid-OrderOpenPrice())/Poin;}
            if(OrderType()==OP_SELL) {openTotalPips += OrderLots()*100*(OrderOpenPrice()-Ask)/Poin;}
            openProfit += OrderProfit();
            openComm += OrderCommission();
            openSwap += OrderSwap();
         }
      }
   }

   // apply selected adjustments to open trade PL
   if(___Comissions_Adjustment)  {openProfit += openComm;}
   if(___Swaps_Adjustment)  {openProfit += openSwap;}

   // use input lot type to get open trade size factors ("openSize" and "os") determining
   // for the PL label, the factored trade size and avg/total pips displayed
   // for the TP/SL labels, the factored pips at TP/SL displayed
   int os = 0;  if(Show_Micro_Mini_Full_123 == 1) {os=1;}
   else {if(Show_Micro_Mini_Full_123 == 2) {os=10;}
   else {if(Show_Micro_Mini_Full_123 == 3) {os=100;} }}
   openSize= openMicroLots/os;

   // use "openSize" to determine qty of units (LOTstr) to display in PL label
   // first, get the digits to the right of the decimal
   int lot0 = (int)(100 * openSize);
   int lot2 = lot0%10;
   int lot1 = lot0%100 - lot2;
   // next, use digits info to determine how many digits to display
   if((lot1==0) && (lot2==0)) {LOTstr=DoubleToStr(openSize,0);}
   else if (lot2==0) {LOTstr=DoubleToStr(openSize,1);}
   else {LOTstr=DoubleToStr(openSize,2);}

   // use "openSize" and "os" factors to get avg/total pips for PL label
   openTotalPips = openTotalPips/os;
   openAvgPips = openTotalPips/openSize;

   // get average price of open trade for PL label
   openAvgPrice = openpriceLotProduct/openMicroLots;

   // return the avg. price of the open trade
   if(openSize >0) {return(openAvgPrice);}

   else {return(0);}
} 
public double getAvgPriceOfOpenOrdersPending()
{  
   double openMicroLots = 0;
   double openTotalPips = 0;
   double openProfit = 0;
   double openComm = 0;
   double openSwap = 0;
  double  openSize = 0;
   double openAvgPips = 0;
  double  openAvgPrice = 0;
   double openpriceLotProduct = 0;

   for(int i=0; i<OrdersTotal(); i++)
   {
      if(OrderSelect(i, SELECT_BY_POS, MODE_TRADES))
      {
         if (Symbol() == OrderSymbol())
         {
            openMicroLots += OrderLots()*100;
            openpriceLotProduct += OrderOpenPrice()*(OrderLots()*100);
            if(OrderType()==OP_BUY) {openTotalPips += OrderLots()*100*(Bid-OrderOpenPrice())/Poin;}
            if(OrderType()==OP_SELL) {openTotalPips += OrderLots()*100*(OrderOpenPrice()-Ask)/Poin;}
            openProfit += OrderProfit();
            openComm += OrderCommission();
            openSwap += OrderSwap();
         }
      }
   }

   // apply selected adjustments to open trade PL
   if(___Comissions_Adjustment)  {openProfit += openComm;}
   if(___Swaps_Adjustment)  {openProfit += openSwap;}

   // use input lot type to get open trade size factors ("openSize" and "os") determining
   // for the PL label, the factored trade size and avg/total pips displayed
   // for the TP/SL labels, the factored pips at TP/SL displayed
   int os = 0;  if(Show_Micro_Mini_Full_123 == 1) {os=1;}
   else {if(Show_Micro_Mini_Full_123 == 2) {os=10;}
   else {if(Show_Micro_Mini_Full_123 == 3) {os=100;} }}
   openSize= openMicroLots/os;

   // use "openSize" to determine qty of units (LOTstr) to display in PL label
   // first, get the digits to the right of the decimal
   int lot0 = (int)(100 * openSize);
   int lot2 = lot0%10;
   int lot1 = lot0%100 - lot2;
   // next, use digits info to determine how many digits to display
   if((lot1==0) && (lot2==0)) {LOTstr=DoubleToStr(openSize,0);}
   else if (lot2==0) {LOTstr=DoubleToStr(openSize,1);}
   else {LOTstr=DoubleToStr(openSize,2);}

   // use "openSize" and "os" factors to get avg/total pips for PL label
   openTotalPips = openTotalPips/os;
   openAvgPips = openTotalPips/openSize;

   // get average price of open trade for PL label
   openAvgPrice = openpriceLotProduct/openMicroLots;

   // return the avg. price of the open trade
   if(openSize >0) {return(openAvgPrice);}

   else {return(0);}
}  
	
        //+------------------------------------------------------------------+
        //| AUTO GENERATED CODE. THIS METHODS USED FOR INDICATOR CACHING     |
        //+------------------------------------------------------------------+
        #region Auto Generated Code
        
        [Description("Parameters order Symbol, TimeFrame, ignoreList")]
        public override bool IsSameParameters(params object[] values)
        {
            if(values.Length != 2)
                return false;
            
            if(!CompareString(Symbol, (string)values[0]))
                return false;
            
            if(TimeFrame != (int)values[1])
                return false;
            
            if(ignoreList != (string)values[2])
            	return false;
            		
            return true;
        }
        
        [Description("Parameters order Symbol, TimeFrame, ignoreList")]
        public override void SetIndicatorParameters(params object[] values)
        {
            if(values.Length != 2)
                throw new ArgumentException("Invalid parameters number");
            
            Symbol = (string)values[0];
            TimeFrame = (int)values[1];
            ignoreList = (string)values[2];
            
        }
        
        #endregion
    }
    
}

