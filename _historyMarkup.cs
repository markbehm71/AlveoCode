
using System;
using System.ComponentModel;
using System.Windows.Media;
using Alveo.Interfaces.UserCode;
using Alveo.UserCode;
using Alveo.Common;
using Alveo.Common.Classes;

namespace Alveo.UserCode
{
    [Serializable]
    [Description("")]
    public class _historyMarkup : IndicatorBase
    {
        #region Properties
        
        [Category("Settings")]
        public int updateSeconds{ get; set;}
		public color buyColor = Color.FromArgb(50,10,255,10);
        public color sellColor = Color.FromArgb(50,255,90,50);
        
        #endregion
        public datetime last_time=0;
        
        public _historyMarkup()
        {
            // Basic indicator initialization. Don't use this constructor to calculate values
            
            indicator_buffers = 0;
            //foobar = 0;
            
            copyright = "behm";
            link = "";
            updateSeconds = 58;
        }
        
        //+------------------------------------------------------------------+");
        //| Custom indicator initialization function                         |");
        //+------------------------------------------------------------------+");
        protected override int Init()
        {
        	// ENTER YOUR CODE HERE
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
        	int obj_total= ObjectsTotal();
        	for (int i= obj_total-1; i>=0; i--) {
		      string name= ObjectName(i);
		    
		      if (StringSubstr(name,0,5)=="_hist") 
		         ObjectDelete(name);
		      }

        	return 0;
        }
        
        //+------------------------------------------------------------------+");
        //| Custom indicator iteration function                              |");
        //+------------------------------------------------------------------+");
        protected override int Start()
        {
        	// colorize every tick as they seem to rewrite themselves a lot
        	ColorizeOrders();
        	// access anywhere�


	
	
	
        	if (TimeCurrent() - last_time <= updateSeconds)
					return(0);
        	  
        	  last_time = TimeCurrent();
			string inditag = "_entryMarkup_";
        	// history
        	 int shift=0;
           int max=20;
           int count=0;
           color col=Tomato;
           double offset=10;
           double pips=0;
           int tlWidth = 1;
            int total=OrdersTotal();
  			// colorize open orders
			
  
  
  
             for (int i = OrdersHistoryTotal();i>0; i--) { 
            	 if (OrderSelect(i, SELECT_BY_POS, MODE_HISTORY)) {
            		if (OrderSymbol() == Symbol()) {
						shift = iBarShift(null, 0, OrderOpenTime(), false);
						 
						if (OrderType() == OP_BUY) {
							offset=100*Point;
							pips = ( OrderClosePrice()-OrderOpenPrice() )/(Point*10);
							}
						 if (OrderType() == OP_SELL) {								
							offset=-100*Point;	
							pips = ( OrderOpenPrice()- OrderClosePrice() )/(Point*10);							
							}
						 if (OrderType() == OP_BUY || OrderType() == OP_SELL) {
								count++;
								if (count>max) return(0);
								if(OrderProfit()>0) col= buyColor;
								if(OrderProfit()<=0) col = sellColor;

								// markups
								//line
								string n="_history_"+OrderTicket();
								ObjectDelete(n);
								DrawTrendLine(n,0,OrderOpenTime(),OrderOpenPrice(),OrderCloseTime(),OrderClosePrice());
								SetObjectColor(n,col);
								SetObjectWidth(n,tlWidth);
								// pips
								n="_historyPips_"+OrderTicket();
								ObjectDelete(n);
								
									
								DrawText(n,0,OrderOpenTime(),OrderClosePrice()+offset,DoubleToStr(pips,1));
								SetObjectWidth(n,10);								
								SetObjectColor(n,col);
																
							}
	            		}
            		}
            	}
            return 0;
        	return 0;
        }	
        
        
        //+------------------------------------------------------------------+
        //| AUTO GENERATED CODE. THIS METHODS USED FOR INDICATOR CACHING     |
        //+------------------------------------------------------------------+
        #region Auto Generated Code
        
        [Description("Parameters order Symbol, TimeFrame, updateSeconds")]
        public override bool IsSameParameters(params object[] values)
        {
            if(values.Length != 3)
                return false;
            
            if(!CompareString(Symbol, (string)values[0]))
                return false;
            
            if(TimeFrame != (int)values[1])
                return false;
            
            if(updateSeconds != (int)values[2])
                return false;

            return true;
        }
        
        [Description("Parameters order Symbol, TimeFrame, updateSeconds")]
        public override void SetIndicatorParameters(params object[] values)
        {
            if(values.Length != 3)
                throw new ArgumentException("Invalid parameters number");
            
            Symbol = (string)values[0];
            TimeFrame = (int)values[1];
            updateSeconds = (int)values[2];
            
        }
        
        #endregion
    protected void ColorizeOrders()
	{    	
    	int obj_total=ObjectsTotal();
		string name;
		
		Color clr =  Color.FromArgb(50,10,255,10);
		int num,ticket;
		double last2;
		int toRange;
		string n;
    	for (int i = OrdersTotal();i>=0; i--) { 
            	 if (OrderSelect(i, SELECT_BY_POS)) 
            	 {
            		if (OrderSymbol() == Symbol()) 
            		{
            		ticket = OrderTicket();
            		var r = new Random(ticket%100);
            		var rr = r.Next(0,255);
            		var rg = r.Next(0,255);
            		var rb = r.Next(0,255);
					// colorize entry lines
            		clr = Color.FromArgb(180,(byte)rr,(byte)rg,(byte)rb);	
            		
            		n = "Order "+Convert.ToString(ticket)+" - Sell";
            		if (ObjectFind(n)>=0){
            			SetObjectColor(n,clr);
            			SetObjectWidth(n,2); 
						          			
	            		}
            		n = "Order "+Convert.ToString(ticket)+" - Buy";
            		if (ObjectFind(n)>=0){
            			SetObjectColor(n,clr);
            			SetObjectWidth(n,2);  }
            		// colorize SL and TP
            		clr = Color.FromArgb(60,(byte)rr,(byte)rg,(byte)rb);	
            		n = "Order "+Convert.ToString(ticket)+" - SL";
            		SetObjectColor(n,clr);
            		SetObjectDashStyle(n,1); // should be dash but doesn't show
            		SetObjectWidth(n,4);
            		clr = Color.FromArgb(80,(byte)rr,(byte)rg,(byte)rb);	
            		n = "Order "+Convert.ToString(ticket)+" - TP";
            		SetObjectColor(n,clr);

            		
    	 			}
				
				}
			}
    	 			
    	/*for( int i=0;i<obj_total;i++)
			    {
			    name = ObjectName(i);
			    
    	 if (ObjectType(name)==OBJ_HLINE)
			      	{
    	 			
	           		if (StringFind(name," - SL",0)>=0 && GetObjectColor(name) != clr)
	           			{
	           			num = Convert.ToInt16(StringSubstr(name,9,2));
	           			Print(name);
	           			//clr = Color.FromArgb(50,(byte)num,128,128);
	           			//SetObjectColor(name,clr);
	           			}
	           		if (StringFind(name," - TP",0)>=0 && GetObjectColor(name) != clr)
	           			{
	           			//SetObjectColor(name,clr);
	           			}
	           		if (StringFind(name," - Sell",0)>=0 && GetObjectColor(name) != clr)
	           			{
	           			//SetObjectColor(name,clr);
	           			}
	           		if (StringFind(name," - Buy",0)>=0 && GetObjectColor(name) != clr)
	           			{
	           			//SetObjectColor(name,clr);
	           			}
	           		}
    	 		}
    	 	*/
    	}
    }
}

