//+------------------------------------------------------------------+
//|                                                                  |
//|       use: draw rectangle on chart. name "vprofile"              |
//|       custom color in description as rgb "100,120,220"           |
//|       -can have multiple vprofiles                               |
//|                                                                  |
//|                                                                  |
//|                                                                  |
//|                                                                  |
//|                                                                  |
//|                                                                  |
//|                                                                  |
//+------------------------------------------------------------------+

#property copyright "Mark Behm 2018"
#property link      ""
#property version   "001.000"
#property strict
#property indicator_chart_window

input color boxColor = clrGray;
input color vpColor = clrGray;
input color outlineColor = clrGray;
input int divisions = 120;
input string rectStringContains = "vprofile";
input bool outline = true;
input bool hlines = true;
input bool updateEachTick = false;
input bool writeDataFile = true;
//+------------------------------------------------------------------+
//| Custom indicator initialization function                         |
//+------------------------------------------------------------------+
int OnInit()
  {
//--- indicator buffers mapping
   DeleteAllContaining("vpLine_");
   DeleteAllContaining("vpOutline_");
    UpdateVProfiles();
//---
   return(INIT_SUCCEEDED);
  }
int OnDeinit()
   {
   
   DeleteAllContaining("vpLine_");
   DeleteAllContaining("vpOutline_");
   
   return(0);
   }
//+------------------------------------------------------------------+
//| Custom indicator iteration function                              |
//+------------------------------------------------------------------+
int OnCalculate(const int rates_total,
                const int prev_calculated,
                const datetime &time[],
                const double &open[],
                const double &high[],
                const double &low[],
                const double &close[],
                const long &tick_volume[],
                const long &volume[],
                const int &spread[])
   {
   if (updateEachTick)
      UpdateVProfiles();

      	return(rates_total);  
	  
	  }
	  
//--- return value of prev_calculated for next call
   
  
  
 
//+------------------------------------------------------------------+
//| Timer function                                                   |
//+------------------------------------------------------------------+
void OnTimer()
  {
//---
   
  }
//+------------------------------------------------------------------+
//| ChartEvent function                                              |
//+------------------------------------------------------------------+
void OnChartEvent(const int id,
                  const long &lparam,
                  const double &dparam,
                  const string &sparam)
  {
//---
   if (id==CHARTEVENT_CHART_CHANGE)
      {
      Print("chart change event");
      }
    if(id==CHARTEVENT_OBJECT_CREATE || id==CHARTEVENT_OBJECT_CHANGE || id==CHARTEVENT_OBJECT_DELETE || id==CHARTEVENT_OBJECT_DRAG) 
      {
      if (StringFind(sparam,rectStringContains,0)>=0)
       UpdateVProfiles();
      }
   
  }
//+------------------------------------------------------------------+
//@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
void HandleChartUpdate()
//@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
   {
   int profileB1[100];
   int profileB2[100];
   double profileP1[100];
   double profileP2[100];
   string boxnames[100];
   int boxTotal = GetBoxes(profileB1,profileB2,profileP1,profileP2,boxnames);
   for (int boxnum=1;boxnum<=boxTotal;boxnum++)
      {
     
      }
   }
 //@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
 //@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
 void UpdateVProfiles()
 //@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
 //@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
   {
   // start clean on every update
   DeleteAllContaining("vpLine_");
   DeleteAllContaining("vpOutline_");
   
    // get user created vprofile rectangles
   int profileB1[100];
   int profileB2[100];
   double profileP1[100];
   double profileP2[100];
   string boxnames[100];
   color overrideColor = 0;
   int volumeCount,rateCount;
   
   int boxTotal = GetBoxes(profileB1,profileB2,profileP1,profileP2,boxnames);
   DeleteAllContaining("vpLine_");
   //Print("boxes "+boxTotal);
   string fileData="";
   
   for (int boxnum=1;boxnum<=boxTotal;boxnum++)
      {
      
         color  outlineCol = outlineColor;
         color vpCol = vpColor;
          // box should always be sel
         ObjectSet(boxnames[boxnum],OBJPROP_SELECTED,True);
         //int boxnum = 1;
         //int divisions=40;
         int barFrom = profileB2[boxnum];
         int barTo = profileB1[boxnum];
         // any color overrides in description?
         string desc = ObjectDescription(boxnames[boxnum]);
         //Print(desc);
         
         if (StringFind(desc,",",0) > 0) // look for a comma (not very safe but I'm the only user!)
            {
            
            color c = StringToColor(desc);
            outlineCol = c;
            vpCol = c;
            }
        
         datetime timeFrom = iTime(Symbol(),PERIOD_CURRENT,barFrom-1);
         datetime timeTo = iTime(Symbol(),PERIOD_CURRENT,barTo);
        
         // get historical rates  ############################
         MqlRates rates[];
         
         rateCount = CopyRates(Symbol(), PERIOD_M1, timeFrom, timeTo, rates);
         // if we are out of data try a few times w highter TFs
         if (rateCount==-1)
            {
            rateCount = CopyRates(Symbol(), PERIOD_M5, timeFrom, timeTo, rates);
            Print("out of M1 data - Trying M5");
             if (rateCount==-1)
                  {
                  rateCount = CopyRates(Symbol(), PERIOD_M15, timeFrom, timeTo, rates);
                  Print("out of M5 data - Trying M15");
                  }
                  if (rateCount==-1)
                     {
                     Print ("OutOfData");
                     return;
                     }
            }
        
            
         double _high=Bid;
         double _low=Bid;
         double _volume[];
         
         ArrayResize(_volume,(rateCount+1));
         volumeCount = ArraySize(_volume);
         
         if (rateCount<0 || volumeCount<1)
            {
            volumeErr(boxnames[boxnum]);
            continue; // problem
            }
            
         // get highest and lowest ############################
         for (int i = 1; i < rateCount; i++)
      	   {
      	   if (rates[i].low < _low)
      	      _low = rates[i].low;
      	   if (rates[i].high > _high)
      	      _high = rates[i].high ;
      	   }
      	 
      	 
            
            
      	// adjust object on chart
      	ObjectSet(boxnames[boxnum],OBJPROP_PRICE1,_high);
         ObjectSet(boxnames[boxnum],OBJPROP_PRICE2,_low);
         
         
      	double step = NormalizeDouble((_high-_low)/divisions,Digits);
      	//Print("step "+step);
      	double d = 1;
        
         // get volumes for 1m bars depending on bar price locations (are they within that bar? add bars v) ############################
      	for (int i = 1; i < rateCount; i++)
      	   {
      	   int index=1;
         	for (double d=_low ; d<= _high;d+=step)
         	   {  
         	   if (d>rates[i].low && d<rates[i].high)
         	      {
         	      if (volumeCount>index && rateCount>i)
         	         _volume[index]+=double(rates[i].tick_volume);
         	      }
         	   index++; 
         	   }
      
      	   }
      	   
      	  // highest volume in rates   ############################
      	  double maxV = NormalizeDouble(_volume[ArrayMaximum(_volume)],0);
           
            
           // draw bars ############################
           int index=1;
           string n;
           datetime t1,t2;
           int b1,b2,b3;
           double price,v;
           double price2;
           if (ArraySize(_volume)<1)
            continue;
           
      	  for (double d=_low ; d<= _high;d+=step)
         	   {
                if (ArraySize(_volume)<1)
                  return;
         	   price = d;
         	   if (maxV==0)
         	      maxV=1;
         	   if (volumeCount<=index)
         	      {
         	      volumeErr(boxnames[boxnum]);
         	      continue;
         	      }
         	   v = _volume[index]/maxV; // volume normalized to highest = scalar
         	   b1 = (int) barTo;
         	   b2 = (int) barTo - (barTo-barFrom)*v;//(barTo - barFrom) / maxV ;
         	   if (ArraySize(_volume)>index+1)
         	      b3 = (int) barTo - (barTo-barFrom) * _volume[index+1]/maxV;
               else
                  b3 = b2;
                  
         	   // when time switched sometimes we are out of range
         	   if (b1 < 1)
         	      b1=1;
         	   if (b2 < 0)
         	      b2=0;
         	   if (b3 < 0)
         	      b3=0;
         	      
         	   t1 = Time[b2];
         	   t2 = Time[b1];
         	   
         	  // main horizontal lines
         	  
         	   n= "vpLine_"+boxnum+"_"+index;
         	   ObjectDelete(NULL,n);
            	   if (hlines)
            	   {
            	   ObjectCreate(NULL,n,OBJ_TREND,0,t1,price,t2,price);
            	   ObjectSet(n,OBJPROP_RAY,0);
            	   ObjectSet(n,OBJPROP_SELECTABLE,0);
            	   ObjectSet(n,OBJPROP_COLOR,vpCol);
            	   // write line to file
            	   fileData+=t1+","+price+","+t2+","+price+",";
            	   }
            	
            	// outline
            	
            	n="vpOutline_"+boxnum+"_"+index;
            	ObjectDelete(NULL,n);
            	   if (outline)
            	   {  
            	   t1 = Time[b2];
            	   t2 = Time[b3];
            	   price2 = d+step;
            	   ObjectCreate(NULL,n,OBJ_TREND,0,t1,price,t2,price2);
            	   ObjectSet(n,OBJPROP_RAY,0);
            	   ObjectSet(n,OBJPROP_SELECTABLE,0);
            	   ObjectSet(n,OBJPROP_COLOR,outlineCol);
            	   
            	   fileData+=t1+","+price+","+t2+","+price2+","; 
            	   
            	   }
            	
            	
            	// next line
         	   index++;
         	   }
         	   
     }
     if (writeDataFile)
         {
            //# write file for alveo
           string fileName = StringSubstr(Symbol(),0,6)+".csv";
           string fileNameBin = StringSubstr(Symbol(),0,6)+".bin";
           int file_handle=FileOpen(fileName,FILE_WRITE|FILE_CSV);
           if(file_handle!=INVALID_HANDLE)
             {
             FileWrite(file_handle,fileData);
             }
             else
             {
             PrintFormat("Failed to open %s file, Error code = %d",GetLastError());
             }
           FileClose(file_handle);   
           
           file_handle=FileOpen(fileNameBin,FILE_WRITE|FILE_BIN);
           if(file_handle!=INVALID_HANDLE)
             {
             FileWrite(file_handle,fileData);
             }
             else
             {
             PrintFormat("Failed to open %s file, Error code = %d",GetLastError());
             }
           FileClose(file_handle);   
          }
     
   }
//@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
 int volumeErr(string boxname)
//@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
   {
   ObjectSet(boxname,OBJPROP_COLOR,clrRed);
   Print("volume or rate problem w "+boxname+" "+Symbol());
   return(0);
   }
 //@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
 int GetBoxes(int &profileB1[],int &profileB2[],double &profileP1[],double &profileP2[],string &boxnames[])
//@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
   {
   int profileCount = 0;
   string objName;
   
   int i,objType;
   int obj_total = ObjectsTotal(ChartID());
   for (i=obj_total-1; i>=0; i--) // NOTE! When deleting objects as below, must count DOWN in this loop!
         {
        objName = ObjectName(i);
        objType = ObjectType(objName);
         if  ( StringFind(objName,rectStringContains,0) == 0 )
            {
           profileCount++;
           string box = objName;
           boxnames[profileCount] = objName;
           datetime timeFrom,timeTo;
           double onetick;
           double lowest,highest;
           int startBar,endBar;
           int sb,eb;
           double y1;
           
           int point = Digits;
           double volumes[];

           
           timeFrom = ObjectGet(box,OBJPROP_TIME1);
           sb = iBarShift(NULL,0,timeFrom,true);
           timeTo = ObjectGet(box,OBJPROP_TIME2);
           eb = iBarShift(NULL,0,timeTo,true);
           
           y1 = ObjectGet(box,OBJPROP_PRICE1);
           onetick = 1/(MathPow(10,Digits)); 
           
           // start and end in order
           startBar = MathMax(sb,eb);
           endBar = MathMin(sb,eb);
           if (startBar<2)
            startBar=2;
           if (endBar<1)
            endBar=1;
           // get highest and lowest price in range
           highest = High[iHighest(NULL,0,MODE_HIGH,MathAbs(endBar-startBar),endBar)];
           lowest = Low[iLowest(NULL,0,MODE_LOW,MathAbs(endBar-startBar),endBar)];
           // rescale box
           ObjectSet(box,OBJPROP_PRICE1,highest);
           ObjectSet(box,OBJPROP_PRICE2,lowest);
           ObjectSet(box,OBJPROP_TIME1,Time[startBar]); // force-reorder times in order
           ObjectSet(box,OBJPROP_TIME2,Time[endBar]);
           ObjectSet(box,OBJPROP_BACK,false);
           //ObjectSet(box,OBJPROP_COLOR,boxColor);
           
           profileB1[profileCount]=startBar;
           profileB2[profileCount]=endBar;
           profileP1[profileCount]=highest;
           profileP2[profileCount]=lowest;
            }
           } 
  
  return (profileCount);
  
  }
//@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
void DeleteAllContaining(string prefix)
//@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
   {
     string objName;
     int i,objType;
     int obj_total = ObjectsTotal(ChartID());
     for (i=obj_total-1; i>=0; i--) // NOTE! When deleting objects as below, must count DOWN in this loop!
         {
        objName = ObjectName(i);
        objType = ObjectType(objName);
         if  ( StringFind(objName,prefix,0) >= 0 )
                  ObjectDelete(objName);
         }

   }         
