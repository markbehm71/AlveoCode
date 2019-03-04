
using System;
using System.ComponentModel;
using System.Windows.Media;
using Alveo.Interfaces.UserCode;
using Alveo.UserCode;
using Alveo.Common;
using Alveo.Common.Classes;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
	
namespace Alveo.UserCode
{
    [Serializable]
    [Description("")]
    public class vp_behm_05 : IndicatorBase
    {
        #region Properties
        [Category("Settings")]
        public string path_help{ get; set;}
        [Category("Settings")]
        public string path{ get; set;}

        [Category("Settings")]
        public color lineColor{ get; set;}

        [Category("Settings")]
        public color outlineColor{ get; set;}

        [Category("Settings")]
        public bool lines{ get; set;}

        [Category("Settings")]
        public bool outline{ get; set;}
        
		[Category("Settings")]
        public int updateEveryXsec{ get; set;}
        
        #endregion
        public datetime last_time=0;
        public DateTime lastModified;
        public vp_behm_05()
        
        
        	
        {
            // Basic indicator initialization. Don't use this constructor to calculate values
            
            indicator_buffers = 0;
            //fooBar = 0;
            copyright = "";
            link = "";
            // try using user path and prepping
          	path_help = "path writes to appdata/roaming/mt4/termnial directory. example...C://Users//User//AppData//Roaming//MetaQuotes//Terminal//F2262CFAFF47C27887389DAB2852351A//MQL4//Files";
            //path = "C://Users//mark.behm//AppData//Roaming//MetaQuotes//Terminal//F2262CFAFF47C27887389DAB2852351A//MQL4//Files";
            path = "C://Users//mark.behm//AppData//Roaming//MetaQuotes//Terminal//F2262CFAFF47C27887389DAB2852351A//MQL4//Files";
            
         	
            


            lineColor = "#33D9FFF9";
            outlineColor = "#7972827D";
            lines = true;
            outline = true;
            updateEveryXsec = 1;
            
        
        }
        
        //+-------- EXTERNAL PARAMETERS HERE --------+
        //[Category("My Category")]
        //[DisplayName("My Display Name")]
        //public int fooBar { get; set; }
        
        //+------------------------------------------------------------------+");
        //| Custom indicator initialization function                         |");
        //+------------------------------------------------------------------+");
        protected override int Init()
        {
        	DeleteAll("vpLine");
        	Print("vp 05 new init");
        	// override user colors for now
        	lineColor = "#16D9FFF9";
            outlineColor = "#7972827D";
            
            // reformat user path if pasted from windows...
            if (path.Contains(@"\"))
                             {
                             path = path.Replace(@"\","//");
                             Print ("using user path");
                             Print(path);
                            }
              
        	return 0;
        }
        
        //+------------------------------------------------------------------+");
        //| Custom indicator deinitialization function                       |");
        //+------------------------------------------------------------------+");
        protected override int Deinit()
        {
        	DeleteAll("vpLine");
        	return 0;
        }
        
        //+------------------------------------------------------------------+");
        //| Custom indicator iteration function                              |");
        //+------------------------------------------------------------------+");
        protected override int Start()
        {
        	//int counted_bars = IndicatorCounted();
        	if (!UpdateTimer())
        		return 0;
            
        	ReadDrawVPcsv();
        	//ReadDrawVPbin(); unwritten
        	
        	return 0;
        }	
        
        
        //+------------------------------------------------------------------+
        //| AUTO GENERATED CODE. THIS METHODS USED FOR INDICATOR CACHING     |
        //+------------------------------------------------------------------+
        #region Auto Generated Code
        
        [Description("Parameters order Symbol, TimeFrame, path, lineColor, outlineColor, lines, outline")]
        public override bool IsSameParameters(params object[] values)
        {
            if(values.Length != 7)
                return false;
            
            if(!CompareString(Symbol, (string)values[0]))
                return false;
            
            if(TimeFrame != (int)values[1])
                return false;
            
            if(!CompareString(path,(string)values[2]))
                return false;

            if(lineColor != (color)values[3])
                return false;

            if(outlineColor != (color)values[4])
                return false;

            if(lines != (bool)values[5])
                return false;

            if(outline != (bool)values[6])
                return false;
            if(updateEveryXsec != (int)values[7]);
             return false;

            return true;
        }
        
        [Description("Parameters order Symbol, TimeFrame, path, lineColor, outlineColor, lines, outline")]
        public override void SetIndicatorParameters(params object[] values)
        {
            if(values.Length != 7)
                throw new ArgumentException("Invalid parameters number");
            
            Symbol = (string)values[0];
            TimeFrame = (int)values[1];
            path = (string)values[2];
            lineColor = (color)values[3];
            outlineColor = (color)values[4];
            lines = (bool)values[5];
            outline = (bool)values[6];
            
        }
        
        #endregion
        

        //####################################################################################
         protected int DeleteAll(string str)
         //####################################################################################
			{
			int obj_total= ObjectsTotal();
			for (int i= obj_total-1; i>=0; i--) {
		      string name= ObjectName(i);
		    
		      if (StringFind(name,str,0)>=0)
		         ObjectDelete(name);
		      }
			return 0;
			}
         //#################################################################################### 		
         protected  bool UpdateTimer()
          //####################################################################################
         {
 			if (TimeCurrent() - last_time <= updateEveryXsec)
					return(false);
        	last_time = TimeCurrent();
        	return(true);
		}
         
         //#################################################################################### 		
         protected  void ReadDrawVPcsv()
         //####################################################################################         
         {

         	
         	string filepath = path+"//"+Symbol().Replace("/","")+".csv";
         	
         	DateTime lastWrite = File.GetLastWriteTime(filepath);
         	
         	if (lastModified==lastWrite)
        		return;
         	
         	lastModified = lastWrite;
        	String[] list = File.ReadAllText(filepath).Split(',');

         	int i=0;
         	int count=0;
         	DeleteAll("_vpLine_");
         
         	Random random = new Random();
         	DateTime t1,t2,t3;
         	double p1,p2;
         	string n;
         	while(count<(list.Count()/4)-1)
         			{
         			t1 = Convert.ToDateTime(list[i]);
         			p1 = Convert.ToDouble(list[i+1]); 
         			t2 = Convert.ToDateTime(list[i+2]);
					p2 = Convert.ToDouble(list[i+3]);					
					//p2 = Convert.ToDouble(list[i+4]);  					
					n = "_vpLine_"+p1+random.Next(0,10000);
					//Print(t1);					
         			DrawTrendLine(n,0,t1,p1,t2,p2);
         			SetObjectColor(n,lineColor);
         			count++;
         			i+=4;
         			}
         	return;
        
         }
    
         //................................................... end         
    }
}

