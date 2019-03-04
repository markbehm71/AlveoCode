
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
    public class keyLevels2 : IndicatorBase
    {
        #region Properties
        
        [Category("Settings")]
 		public double Poin;
 		public int lastMinute=0;
        public int NumLinesAboveBelow{ get; set;}
		public int updateSeconds=30;
        public Color heavy = Color.FromArgb(255,255,255,255);
		public Color lgt = Color.FromArgb(130,255,255,255);
        #endregion
        public datetime last_time=0;
        
        public keyLevels2()
        {
            // Basic indicator initialization. Don't use this constructor to calculate values
            
            indicator_buffers = 0;
           
            copyright = "behm";
            link = "";
            NumLinesAboveBelow = 3;
        }
        
        //+------------------------------------------------------------------+");
        //| Custom indicator initialization function                         |");
        //+------------------------------------------------------------------+");
        protected override int Init()
        {
        	// ENTER YOUR CODE HERE
        	 //foobar = 0;
              if(Point == 0.0001) {Poin = Point;} //4 digits
  				else {Poin = Point*10;}  //2,3,5 digits
  			
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
        	// ENTER YOUR CODE HERE
        	int obj_total= ObjectsTotal();
   
		   for (int i= obj_total-1; i>=0; i--) {
		      string name= ObjectName(i);
		    
		      if (StringSubstr(name,0,5)=="level") 
		         ObjectDelete(name);
		      }
        	return 0;

		
        for (int i=obj_total-1;i>=0;i--)
			{
			if( StringFind(ObjectName(i),"Session") != -1)
			      {
			         ObjectDelete(ObjectName(i));
			      }  
			}
		
        }
        
        //+------------------------------------------------------------------+");
        //| Custom indicator iteration function                              |");
        //+------------------------------------------------------------------+");

        protected override int Start()
        {
        	
        if (TimeCurrent() - last_time <= updateSeconds)
					return(0);
        	  
        last_time = TimeCurrent();
        	  
        	int counted_bars = IndicatorCounted();
			
        int NumLinesAboveBelow = 5;
	       double ssp1= Bid / Poin;
	       int place = 100;
	       ssp1= ssp1 - ssp1%place;
	       
	       // horizontal key levels 100,20,50,80
	       double t1=Time[1];
	       double t2=Time[3];
	        
	     for (int i= -NumLinesAboveBelow; i<NumLinesAboveBelow; i++) 
		      {
		       
		      
		    
		       double ssp = (ssp1+(i*place))*Poin;
		       string n="level100"+i;
		       DrawLineH(n,ssp,heavy);
		       
		       ssp = ((ssp1+(i*place))+20)*Poin;
		       n="level20"+i;
		       ssp=NormalizeDouble(ssp,Digits);
		       DrawLineH(n,ssp,lgt); 
		       //DrawTrendLine(n,0,t1,ssp,t2,ssp);
		       //DrawTrendLine(n,0,OrderOpenTime(),OrderOpenPrice(),OrderCloseTime(),OrderClosePrice());

		       SetObjectColor(n,lgt);
		       ssp = ((ssp1+(i*place))+50)*Poin;
		       ssp=NormalizeDouble(ssp,Digits);
		       n="level50"+i;   
		       
		       //SetObjectColor(n,lgt);
		       DrawLineH(n,ssp,lgt);
		       
		       ssp = ((ssp1+(i*place))+80)*Poin;
		       ssp=NormalizeDouble(ssp,Digits);
		       n="level80"+i;
		       DrawLineH(n,ssp,lgt);
		        //DrawTrendLine(n,0,t1,ssp,t2,ssp);
		      // SetObjectColor(n,lgt);		       
		        }	
	       // vert session lines $$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$
	  //     int shift;
 	//	  for (int i=Bars-1; i>=0; i--) 
	 //		{
	 //       if (TimeMinute(Time[i]) % 1440 == 0)
	 //       	{
	 //       	Color lgt = Color.FromArgb(60,255,255,255);
	 //       	DrawLineV("zero"+i,Time[i],lgt);
	 //       	}
	 //      	if (TimeMinute(Time[i]) % 120 == 0)
	 //       	{
	 //       	Color lgt = Color.FromArgb(60,255,0,0);
	 //       	DrawLineV("One"+i,Time[i],lgt);
	 //       	}
	//		}	      
			
		// delete all session lines each time
		int t = ObjectsTotal();
		for (int i=t-1;i>=0;i--)
			{
			if( StringFind(ObjectName(i),"Session") != -1)
			      {
			         ObjectDelete(ObjectName(i));
			      }  
			}
		// just do the last 100 bars
		 for (int i=0;i<100;i++)
			{
		 	Color ac = Color.FromArgb(60,254,20,20);
		 	Color lc = Color.FromArgb(70,90,90,255);
		 	Color uoc = Color.FromArgb(40,20,255,20);
		 	Color ucc = Color.FromArgb(20,0,220,0);
		 	//if (TimeHour(Time[i]) % 24 == 0 && TimeMinute(Time[i]) % 60  == 0)
		 	//    DrawLineV("Zero"+i,Time[i],ac);
		 /*	string n = "Session"+i;
		 	if (TimeHour(Time[i]) == 0 && TimeMinute(Time[i])  == 0)
		 	    DrawLineV(n,Time[i],ac,STYLE_SOLID);
			if (TimeHour(Time[i]) == 7 && TimeMinute(Time[i])  == 0)
		 	    DrawLineV(n,Time[i],lc,STYLE_SOLID);
			if (TimeHour(Time[i]) == 13 && TimeMinute(Time[i])  == 0)
		 	    DrawLineV(n,Time[i],uoc,STYLE_SOLID);
			if (TimeHour(Time[i]) == 17 && TimeMinute(Time[i])  == 0)
		 	    DrawLineV(n,Time[i],ucc,STYLE_DASH);
			*/
			}		 	
	       	 	
	     return 0;
	     }
        public void DrawLineH(string n,double level, color clr)
		   {
           //if (ObjectFind(n)>=0) return;	
		   ObjectDelete(n);
		   //Print(DoubleToStr(level*100,Digits));
		   //ObjectCreate(n,OBJ_HLINE,0,0,level);
		   //DrawHLine(n,0,level);
		   double t1 = Time[3];
		   double t2 = Time[4];
		   DrawTrendLine(n,0,t1,level,t2,level);

		   SetObjectColor (n,clr);
		   // not in alveo yet > ObjectSet(n,OBJPROP_SELECTABLE,false);
		   SetObjectBack(n,true);
		   }
        public void DrawLineV(string n,datetime t, color clr, int style)
		   {
           //if (ObjectFind(n)>=0) return;	
		   ObjectDelete(n);
		   //Print(DoubleToStr(level*100,Digits));
		   //ObjectCreate(n,OBJ_HLINE,0,0,level);
		   DrawVLine(n,0,t);
		   //SetObjectDashStyle(n,style);	
		   //ObjectSet(n,OBJPROP_STYLE,style);
		   SetObjectColor (n,clr);
		   //ObjectSet(n,OBJPROP_SELECTABLE,false);
		   		   
        }
        //+------------------------------------------------------------------+
        //| AUTO GENERATED CODE. THIS METHODS USED FOR INDICATOR CACHING     |
        //+------------------------------------------------------------------+
        #region Auto Generated Code
        
        [Description("Parameters order Symbol, TimeFrame, aboveBelow")]
        public override bool IsSameParameters(params object[] values)
        {
            if(values.Length != 3)
                return false;
            
            if(!CompareString(Symbol, (string)values[0]))
                return false;
            
            if(TimeFrame != (int)values[1])
                return false;
            
            if(NumLinesAboveBelow != (int)values[2])
                return false;

            return true;
        }
        
        [Description("Parameters order Symbol, TimeFrame, aboveBelow")]
        public override void SetIndicatorParameters(params object[] values)
        {
            if(values.Length != 3)
                throw new ArgumentException("Invalid parameters number");
            
            Symbol = (string)values[0];
            TimeFrame = (int)values[1];
            NumLinesAboveBelow = (int)values[2];
            
        }
        
        #endregion
    }
}

