using System;
using System.Collections.Generic;

namespace HSPI_ZIBASE_LPL
{
    class Device
    {
        public static SensorType EnumConverter(String ZibaseDllType)
        {
            if (ZibaseDllType == "hum")
                return SensorType.HUMIDITY;
            if (ZibaseDllType == "xse")
                return SensorType.SECURITY;
            if (ZibaseDllType == "tem")
                return SensorType.TEMPERATURE;
            if (ZibaseDllType == "sta")
                return SensorType.SWITCH;
            if (ZibaseDllType == "kwh")
                return SensorType.ENERGY_KWH;
            if (ZibaseDllType == "kw")
                return SensorType.ENERGY_KW;
            if (ZibaseDllType == "bat")
                return SensorType.BAT;
            if (ZibaseDllType == "lev")
                return SensorType.LEV;

            return SensorType.SWITCH;

        }
        public String ID { get; set; }
        public String Name { get; set; }
        public String BatLevel { get; set; }
        public String RXLevel { get; set; }
        public List<DeviceValue> Values;

        public void Check()
        {
            foreach (DeviceValue val in Values)
            {
                val.Check();
            }
        }

        public Device()
            : this("?")
        {

        }
        public Device(String Id)
        {
            ID = Id;
            Name = "?";
            BatLevel = "?";
            RXLevel = "?";
            Values = new List<DeviceValue>();
        }
    }

}
