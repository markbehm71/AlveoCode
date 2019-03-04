//+------------------------------------------------------------------+
//|                                                  newVprofile.mq4 |
//|                        Copyright 2018, MetaQuotes Software Corp. |
//|                                             https://www.mql5.com |
//+------------------------------------------------------------------+
#property copyright "Copyright 2018, MetaQuotes Software Corp."
#property link      "https://www.mql5.com"
#property version   "1.00"
#property strict
//+------------------------------------------------------------------+
//| Script program start function                                    |
//+------------------------------------------------------------------+
void OnStart()
  {
//---
   int barDropped = iBarShift(Symbol(),PERIOD_CURRENT,WindowTimeOnDropped(),false);
   
   int barLeft = MathMin(barDropped+10,Bars);
   int barRight = MathMax(1,barDropped-10);
   Print(barDropped," ",barLeft," ",barRight);
   int singleBar = 60*Period();

   string n="vprofile_"+(string)MathRand();
   color str_color=StringToColor((int)(MathRand()*255)+","+(int)(MathRand()*255)+","+(int)(MathRand()*255));
   long current_chart_id=ChartID(); 
   double p = WindowPriceOnDropped();
   ObjectCreate(current_chart_id,n,OBJ_RECTANGLE,0,Time[barLeft],p+(100*Point),Time[barRight],p-(100*Point));
   ObjectSet(n,OBJPROP_BACK,false);
   ObjectSet(n,OBJPROP_COLOR,str_color);
   ObjectSet(n,OBJPROP_SELECTED,true);
  
  }
//+------------------------------------------------------------------+
