using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace HSPI_ZIBASE_LPL
{
    public class PluginConfig
    {
        public List<DeviceAssociation> ControledDevices { get; set; }
        public String ZibaseToken { get; set; }
        public bool DisplayZibaseMsgInHSLog { get; set; }
        [XmlIgnore]
        public bool RestartZibaseSearch { get; set; }
        public int WatchdogTimerMinutes { get; set; }
        public PluginConfig()
        {
            ControledDevices = new List<DeviceAssociation>();
            ZibaseToken = "0000";
            RestartZibaseSearch = false;
            WatchdogTimerMinutes = 0;
        }
    }
}
