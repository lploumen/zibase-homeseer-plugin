using System;
using System.Text.RegularExpressions;
using ZibaseDll;

namespace HSPI_ZIBASE_LPL
{
    class OwnDeviceManager
    {
        public delegate void UpdateSensorInfoHandler(ZiBase.SensorInfo se);
        public event UpdateSensorInfoHandler OnUpdateSensorInfo;
        
        public const String ID = "id";
        public const String BAT = "bat";
        public const String LEV = "lev";



        public String GetValue(String InputString,String Tag)
        {
            Regex re = new Regex("<" + Tag + "[^>]*>(.*?)</"+Tag+">", RegexOptions.Singleline);
            Match m = re.Match(InputString);
            if (m.Success)
            {
                return m.Groups[1].Value;
            }
            return String.Empty;
        }

        public void GetInfo(String InputString)
        {
            String Id = GetValue(InputString,ID);
            ZiBase.SensorInfo se = new ZiBase.SensorInfo();
            if (Id == String.Empty)
                return;

            if (Id.ToLower().StartsWith("vs"))
            {
                Id = Id.Substring(2);
                long LongId = Convert.ToInt64(Id);
                se.sID = "VS" + ((LongId & 0xffffff00) >> 8).ToString();
                se.sType = "vse";
                se.sName = "Visonic";
                se.dwValue = 0;
                se.sHTMLValue = "NORMAL";

                for (int i = 0; i < 4; i++)
                {
                    Regex RegexFlags = new Regex("<flag" + i + "[^>]*>(.*?)</flag" + i + ">", RegexOptions.Singleline);
                    Match RegexFlagsMatch = RegexFlags.Match(InputString);
                    if (RegexFlagsMatch.Success)
                    {
                        se.dwValue = i;
                        se.sHTMLValue = RegexFlagsMatch.Groups[1].Value;
                    }
                }
                se.sValue = se.sHTMLValue;

                if (OnUpdateSensorInfo != null)
                    OnUpdateSensorInfo(se);
            }
            else
                return;


            


            String val = GetValue(InputString, BAT);
            if (val != String.Empty)
            {
                se.sType = BAT;
                se.sHTMLValue = val;
                se.sValue = val;
                se.dwValue = val.ToLower() == "low" ? 0 : 1;

                if (OnUpdateSensorInfo != null)
                    OnUpdateSensorInfo(se);
            }


            val = GetValue(InputString, LEV);
            if (val != String.Empty)
            {
                se.sType = LEV;
                se.sHTMLValue = val + "/5";
                se.sValue = val + "/5";
                se.dwValue = Convert.ToInt16(val);

                if (OnUpdateSensorInfo != null)
                    OnUpdateSensorInfo(se);
            }





        }


    }
}
