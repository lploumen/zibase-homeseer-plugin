using System;
using ZibaseDll;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Scheduler.Classes;
using System.IO;
using System.Text;
using System.Reflection;
using System.Diagnostics;


namespace HSPI_ZIBASE_LPL
{

    enum SensorType
    {
        [StringValue("Switch")]
        SWITCH,
        [StringValue("Humidity")]
        HUMIDITY,
        [StringValue("Temperature")]
        TEMPERATURE,
        [StringValue("Security")]
        SECURITY,
        [StringValue("KwH")]
        ENERGY_KWH,
        [StringValue("Kw")]
        ENERGY_KW,
        [StringValue("Battery")]
        BAT,
        [StringValue("RX level")]
        LEV
    }

    class HsObjet
    {
        private static Scheduler.hsapplication m_Instance = null;
        private static Scheduler.clsHSPI m_callback = null;
        public static Scheduler.hsapplication getInstance()
        {
            if (m_Instance == null)
            {
                throw new Exception();
            }
            return m_Instance;
        }
        public static void Register(Scheduler.clsHSPI obj)
        {
            m_callback = obj;
            m_Instance = (Scheduler.hsapplication)m_callback.GetHSIface();
        }

    }

    struct ZibaseScenario
    {
        public String Name;
        public int ID;
    }


    /// <summary>
    /// Summary description for Class1.
    /// </summary>
    /// 
    [ObfuscationAttribute(Exclude = true, ApplyToMembers = false)]
    public class HSPI : ISynchronizeInvoke
    {
        public static String PlugInName = "Zibase Plug-In";
        public const String EVENTACTION_RUN_SCENARIO = "RUN_SCENARIO";
        public const String EVENTACTION_SET_VARIABLE = "SET_VARIABLE";
        public static Version PLUGIN_VERSION;// = new Version(0, 1);
        public static Version ZIBASE_SDK_VERSION;

        private ZiBase m_Zibase;

        //private List<DeviceAssociation> m_ControledDevices;

        private WebConfig m_WebConfig;

        private SensorDataManager m_SensorDataManager;

        public String ConfigFilePath { get; set; }
        private PluginConfig m_Config;
        private ZiBase.ZibaseInfo m_ZibaseInfo;
        private List<ZibaseScenario> m_Scenarios = null;
        private System.Timers.Timer m_WatchdogTimer;


        public void RegisterCallback(Object frm)
        {
            HsObjet.Register(frm as Scheduler.clsHSPI);
        }

        private void RefreshScenarioList()
        {
            String[] ScenarioArray = m_Zibase.GetScenarioList(m_ZibaseInfo.sLabelBase).Split('|');
            if (m_Scenarios == null)
                m_Scenarios = new List<ZibaseScenario>();
            m_Scenarios.Clear();
            var q = from c in ScenarioArray select new ZibaseScenario { Name = c.Substring(0, c.LastIndexOf(';')), ID = Convert.ToInt16(c.Substring(c.LastIndexOf(';') + 1)) };

            m_Scenarios.AddRange(q.AsEnumerable());
        }

        public HSPI()
        {
            int Major = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).ProductMajorPart;
            int Minor = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).ProductMinorPart;
            int Build = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).ProductBuildPart;
            PLUGIN_VERSION = new Version(Major, Minor, Build);
        }
        public string Name()
        {
            return PlugInName;
        }
        public int Capabilities()
        {
            return HomeSeer.CA_IO;
        }
        public int AccessLevel()
        {
            return HomeSeer.AL_FREE; // Free Plug in
            //return HomeSeer.AL_LICENSED; // Free Plug in

        }

        public String Test(int a)
        {
            return (a + 1).ToString();

        }


        public bool SupportsActionUI()
        {
            return true;
        }
        public String ActionUI(String action_string)
        {
            StringBuilder p = new StringBuilder();

            const string attributes = "0";
            //String CATTR_ALLOW_TEXT_ENTRY = "1";
            //p.Append("\t" + "\t" + HomeSeer.TRIG_UI_TEXT + "\t" + attributes + "\t" + "Display Message:" + "\t" + "" + "\r\n");
            // if (action_string == "")
            {

                String[] data = action_string.Split((char)0x02);
                // p.Append("\t" + HomeSeer.TRIG_UI_TEXT + "\t" + attributes  "\t" + "Scenario number:" + "\t");
                p.Append("Zibase Action" + "\t" + HomeSeer.TRIG_UI_DROP_LIST + "\t" + /*attributes*/  @"\O=True" + "\t" + "The Action:" + "\t" + "No Action" + "\t" + "Run Scenario" + (char)0x05 + EVENTACTION_RUN_SCENARIO + "\t" + "Set variable" + (char)0x05 + EVENTACTION_SET_VARIABLE + "\r\n");
                // "\t" + "\t" + HomeSeer.TRIG_UI_DROP_LIST + "\t" + CATTR_ALLOW_TEXT_ENTRY + "\t" + "Available Actions:" + "\t" + "No Action" + "\t" + "Enable Action" + "\t" + "Disable Action" + "\r\n" +


                if (data.Length == 1 || data[2] == EVENTACTION_SET_VARIABLE)
                {
                    p.Append("\t" + "\t" + HomeSeer.TRIG_UI_DROP_LIST + "\t" + attributes + "\t" + "Variable :");  //+ "\t" + "" + "\r\n");
                    for (int i = 0; i < 32; i++)
                        p.Append("\tV" + i.ToString());
                    p.Append("\r\n");

                    p.Append("\t" + "\t" + HomeSeer.TRIG_UI_TEXT + "\t" + /*attributes*/ @"\W=0" + "\t" + "Value:" + "\t" + "\r\n");
                }
                else if (data[2] == EVENTACTION_RUN_SCENARIO)
                {
                    RefreshScenarioList();
                    //String[] ScenarioArray = m_Zibase.GetScenarioList(m_ZibaseInfo.sLabelBase).Split('|');


                    //var q = from c in ScenarioArray select new { ScenarioName = c.Substring(0, c.LastIndexOf(';')) , ScenarioNumber = c.Substring(c.LastIndexOf(';')+ 1) };

                    //p.Append("\t" + "\t" + HomeSeer.TRIG_UI_TEXT + "\t" + attributes + "\t" + "Scenario number :" + "\t" + "" + "\r\n");
                    p.Append("\t" + "\t" + HomeSeer.TRIG_UI_DROP_LIST + "\t" + attributes + "\t" + "Scenario :");  //+ "\t" + "" + "\r\n");

                    foreach (var item in m_Scenarios)
                    {
                        p.Append("\t" + item.Name + (char)0x05 + item.ID);
                    }

                    //for (int i = 0; i < 16; i++)
                    //  p.Append("\tV" + i.ToString());
                    p.Append("\r\n");

                }

                //             "\t" + "\t" + HomeSeer.TRIG_UI_CHECK_BOX + "\t" + attributes + "\t" + "Close Relay 1:" + "\t" + "1" + "\r\n" +
                //                              "\t" + "\t" + HomeSeer.TRIG_UI_LABEL + "\t" + attributes + "\t" + "Notes" + "\t" + "\r\n" +
                //                              "\t" + "\t" + HomeSeer.TRIG_UI_BUTTON + "\t" + attributes + "\t" + "More ..." + "\t");

                //             p.Append((char)0x01 + PlugInName + (char)0x04 + "Our Plugin Action 2" + "\t" + HomeSeer.TRIG_UI_DROP_LIST + "\t" + attributes + "\t" + "The Action:" + "\t" + "No Action" + "\t" + "eee" + "\r\n" +
                //                          "\t" + "\t" + HomeSeer.TRIG_UI_DROP_LIST + "\t" + CATTR_ALLOW_TEXT_ENTRY + "\t" + "Available Actions:" + "\t" + "No Action" + "\t" + "Enable Action" + "\t" + "Disable Action" + "\r\n" +
                //                          "\t" + "\t" + HomeSeer.TRIG_UI_TEXT + "\t" + attributes + "\t" + "Display Message:" + "\t" + "" + "\r\n" +
                //                          "\t" + "\t" + HomeSeer.TRIG_UI_CHECK_BOX + "\t" + attributes + "\t" + "Close Relay 1:" + "\t" + "1" + "\r\n" +
                //                          "\t" + "\t" + HomeSeer.TRIG_UI_LABEL + "\t" + attributes + "\t" + "Notes" + "\t" + "\r\n" +
                //                          "\t" + "\t" + HomeSeer.TRIG_UI_BUTTON + "\t" + attributes + "\t" + "More ..." + "\t");

                //p.Append("test\t" + HomeSeer.TRIG_UI_TEXT + "\t0\tSelect");
                //p.Append("\tRun Scenario\tAssign Variable\r\n");

                //p.Append("\t\t" + HomeSeer.TRIG_UI_TEXT + "\t0\tScenario Number\t");
                //p.Append("Run Scenario\r\n");
            }
            return p.ToString();
        }

        public String ValidateActionUI(String action_str)
        {
            String[] data = action_str.Split((char)0x02);
            if (data[2] == EVENTACTION_RUN_SCENARIO)
            {
                int Result;
                if (!int.TryParse(data[3], out Result))
                {
                    //return data[2] + " is not a number";
                }
            }
            if (data[2] == EVENTACTION_SET_VARIABLE)
            {

            }
            return String.Empty;
        }
        public String ActionUIFormat(String action)
        {
            String[] data = action.Split((char)0x02);
            if (data[2] == EVENTACTION_RUN_SCENARIO)
            {
                String ScenarioName = m_Scenarios.FirstOrDefault(x => x.ID == Convert.ToInt16(data[3])).Name;
                return "Run scenario '" + ScenarioName + "'";
            }
            if (data[2] == EVENTACTION_SET_VARIABLE)
            {
                return "Set Variable " + data[3] + "=" + data[4];
            }
            return "rr";

        }
        // 
        //         public function setVariable($numVar, $value) {
        //  		
        //  		$request = new ZbRequest();
        // 		$request->command = 11;
        // 		$request->param1 = 5;
        // 		$request->param3 = 1;		
        // 		$request->param4 = $numVar;	
        // 	    $request->param2 = $value & 0xFFFF;
        // 	    
        //     	$this->sendRequest($request); 		
        //  	}

        public int GetVariable(uint variableNumber)
        {
            return (int)m_Zibase.GetVar(variableNumber);
        }

        public void SetVariable(uint variableNumber, uint variableValue)
        {
            m_Zibase.SetVar(variableNumber, variableValue);
            //              Old method
            //             ZBClass class2 = new ZBClass();
            //             class2.header = class2.GetBytesFromString("ZSIG");
            //             class2.command = 11;
            //             class2.alphacommand = class2.GetBytesFromString("SendCmd");
            //             class2.label_base = class2.GetBytesFromString("");
            //             class2.command_text = class2.GetBytesFromString("");
            //             class2.serial = 0;
            //             class2.param1 = 5;
            //             class2.param2 = variableValue & 0xffff;
            //             class2.param3 = 1;
            //             class2.param4 = variableNumber;
            //             byte[] rBuff = null;
            //             byte[] bytes = class2.GetBytes();
            //             ZibaseDll.ZBClass zb = new ZibaseDll.ZBClass();
            //             string ipAddress = LongToIP(m_ZibaseIP);
            //             zb.UDPDataTransmit(bytes, ref rBuff, ipAddress, 0xc34f);
        }

        public void RunScenario(uint ScenarioNumber)
        {
            m_Zibase.RunScenario((int)ScenarioNumber);
            return;
            //              Old Method
            ZBClass class2 = new ZBClass();
            class2.header = class2.GetBytesFromString("ZSIG");
            class2.command = 11;
            class2.alphacommand = class2.GetBytesFromString("SendCmd");
            class2.label_base = class2.GetBytesFromString("");
            class2.command_text = class2.GetBytesFromString("");
            class2.serial = 0;
            class2.param1 = 1;
            class2.param2 = ScenarioNumber;
            class2.param3 = 0;
            class2.param4 = 0;
            byte[] rBuff = null;
            byte[] bytes = class2.GetBytes();
            ZibaseDll.ZBClass zb = new ZibaseDll.ZBClass();
            string ipAddress = LongToIP(m_ZibaseInfo.lIpAddress);
            zb.UDPDataTransmit(bytes, ref rBuff, ipAddress, 0xc34f);


        }
        public void TriggerAction(String actionStr)
        {
            String[] data = actionStr.Split((char)0x02);
            if (data[0] == Name())
            {
                if (data[2] == EVENTACTION_RUN_SCENARIO)
                {
                    try
                    {
                        RunScenario(Convert.ToUInt16(data[3]));
                    }
                    catch (System.Exception ex)
                    {
                        HsObjet.getInstance().WriteLog("Error", "TriggerAction :" + ex.Message);
                    }
                }
                else if (data[2] == EVENTACTION_SET_VARIABLE)
                {
                    String VariableNumber = data[3].Substring(1);
                    uint Variable = Convert.ToUInt32(VariableNumber);
                    uint Value = Convert.ToUInt16(data[4]);
                    SetVariable(Variable, Value);
                }


            }
        }


        //         public void SetIO(object dv, string housecode, string devicecode, int command, int brightness, int data1, int data2)
        //         {
        //             int a;
        //         }


        public String InitIO(long port)
        {

            if (Util.HighestNETFrameworkVersion() < 3.5)
                return "Oops, you need at least the .NET framework 3.5.";

            // Init Zibase and subscribe to its events
            m_Zibase = new ZiBase();

            //m_Zibase.RestartZibaseSearch();
            m_Zibase.WriteMessage += m_Zibase_WriteMessage;
            m_Zibase.NewSensorDetected += m_Zibase_NewSensorDetected;
            m_Zibase.UpdateSensorInfo += m_Zibase_UpdateSensorInfo;
            m_Zibase.NewZibaseDetected += m_Zibase_NewZibaseDetected;


            AppDomain MyDomain = AppDomain.CurrentDomain;
            Assembly[] AssembliesLoaded = MyDomain.GetAssemblies();
            var q = from a in AssembliesLoaded where a.FullName.ToLower().Contains("zibasedll") select a;
            Assembly asm = q.First();
            ZIBASE_SDK_VERSION = asm.GetName().Version;


            m_SensorDataManager = new SensorDataManager();

            // Init our web configuration class ...
            m_WebConfig = new WebConfig {SensorList = m_SensorDataManager.SensorList};

            m_WebConfig.OnUserAddDevice += cfg_OnUserAddDevice;
            //m_WebConfig.OnUserUpdateControledDevices += new WebConfig.UserUpdateControledDevicesHandler(cfg_OnUserUpdateControledDevices);
            m_WebConfig.OnConfigurationChanged += m_WebConfig_OnConfigurationChanged;
            Object obj = m_WebConfig;
            HsObjet.getInstance().RegisterLinkEx(ref obj, Name());

            // ...and the event we will manage
            object objMaster = (object)this;
            int nEvent = HomeSeer.EV_TYPE_X10 |
                HomeSeer.EV_TYPE_STATUS_CHANGE |
                HomeSeer.EV_TYPE_X10_TRANSMIT |
                HomeSeer.EV_TYPE_CONFIG_CHANGE |
                HomeSeer.EV_TYPE_STRING_CHANGE;

            HsObjet.getInstance().RegisterEventCB(ref nEvent, ref objMaster);



            String s = HsObjet.getInstance().GetINISetting("Settings", "app_path", "def", "Settings.ini");
            ConfigFilePath = Path.Combine(s, "config\\Zibase.xml"); // config file for V 0.x

            try
            {
                if (File.Exists(ConfigFilePath)) // file exists, this looks like an upgrade from 0.x version.
                {
                    /*m_ControledDevices*/
                    //List<DeviceAssociation> ControledDevices = Util.DeserializeFromXml<List<DeviceAssociation>>(ConfigFilePath);
                    try
                    {
                        HsObjet.getInstance().WriteLog("Debug", "Get configuration from file");
                        m_Config = Util.DeserializeFromXml<PluginConfig>(ConfigFilePath);
                        HsObjet.getInstance().WriteLog("Debug", "Config file successfully read");
                    }
                    catch (Exception)
                    {
                        HsObjet.getInstance().WriteLog("Error", "Cannot get configuration from file, maybe it is from an older version");
                        HsObjet.getInstance().WriteLog("Error", "It can happens for the first time you are running the plugin with a version above 0.x");
                        HsObjet.getInstance().WriteLog("Debug", "Try to read configuration from 0.x version");
                        List<DeviceAssociation> ControledDevices = Util.DeserializeFromXml<List<DeviceAssociation>>(ConfigFilePath);
                        HsObjet.getInstance().WriteLog("Debug", "Config from version 0.x successfully read");
                        m_Config = new PluginConfig {ControledDevices = ControledDevices};
                        Util.SerializeToXml(m_Config, ConfigFilePath);
                    }

                }
                else
                {
                    m_Config = new PluginConfig(); // default configuration
                }

            }
            catch
            {
                HsObjet.getInstance().WriteLog("Debug", "No device association file found");
                m_Config = new PluginConfig();
            }
            Debug.Assert(m_Config != null);
            m_WebConfig.ControledDevices = m_Config.ControledDevices;
            m_WebConfig.Config = m_Config;


            m_WatchdogTimer = new System.Timers.Timer();

            if (m_Config.WatchdogTimerMinutes > 0)
            {
                m_WatchdogTimer.Interval = m_Config.WatchdogTimerMinutes * 60 * 1000;
                m_WatchdogTimer.Start();
            }
            m_WatchdogTimer.Elapsed += m_WatchdogTimer_Elapsed;
            m_WatchdogTimer.AutoReset = true;

            //HsObjet.getInstance().WriteLog("Debug", "Waiting for Zibase");
            //m_ZibaseDetectedEvent.WaitOne();
            //HsObjet.getInstance().WriteLog("Debug", "ZibaseDetected, set Token");
            m_Zibase.StartZB(true, 17202);
            HsObjet.getInstance().WriteLog("Debug", "IOInit End");
            return "";
        }

        void m_WebConfig_OnConfigurationChanged(object sender, EventArgs e)
        {
            try
            {
                Util.SerializeToXml(m_Config, ConfigFilePath);

                if (m_ZibaseInfo.sLabelBase != null) // at leat one Zibase detected
                    m_Zibase.SetZibaseToken(m_ZibaseInfo.sLabelBase, m_Config.ZibaseToken);

                if (m_Config.RestartZibaseSearch)
                {
                    HsObjet.getInstance().WriteLog("Debug", "Restarting Zibase search");
                    m_Zibase.RestartZibaseSearch();
                }



                if (m_Config.WatchdogTimerMinutes > 0)
                {
                    HsObjet.getInstance().WriteLog("Debug", "Setting watchdog timer to " + m_Config.WatchdogTimerMinutes + "minute(s)...");
                    m_WatchdogTimer.Interval = m_Config.WatchdogTimerMinutes * 60 * 1000;
                    m_WatchdogTimer.Start();
                    m_WatchdogTimer.AutoReset = true;

                }
                else
                {
                    HsObjet.getInstance().WriteLog("Debug", "Stopping watchdog timer...");
                    m_WatchdogTimer.Stop();
                }
                HsObjet.getInstance().WriteLog("Debug", "...OK");
            }
            catch (Exception ex)
            {
                HsObjet.getInstance().WriteLog("Error", ex.Message);
            }

        }

        void m_WatchdogTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            HsObjet.getInstance().WriteLog("Debug", "Watchdog timer elapsed, searching Zibase again");
            m_Zibase.RestartZibaseSearch();
        }

        void m_Zibase_NewZibaseDetected(ZiBase.ZibaseInfo zbInfo)
        {
            HsObjet.getInstance().WriteLog("Debug", "Zibase detected");
            m_ZibaseInfo = zbInfo;
            //m_ZibaseDetectedEvent.Set();
            m_Zibase.SetZibaseToken(m_ZibaseInfo.sLabelBase, m_Config.ZibaseToken);
            RefreshScenarioList();
        }

        static public string LongToIP(long longIP)
        {
            StringBuilder b = new StringBuilder();

            long tempLong = longIP;

            tempLong = tempLong & 0xffffffff;

            long temp = tempLong / (256 * 256 * 256);
            tempLong = tempLong - (temp * 256 * 256 * 256);
            b.Append(Convert.ToString(temp)).Append(".");
            temp = tempLong / (256 * 256);
            tempLong = tempLong - (temp * 256 * 256);
            b.Append(Convert.ToString(temp)).Append(".");
            temp = tempLong / 256;
            tempLong = tempLong - (temp * 256);
            b.Append(Convert.ToString(temp)).Append(".");
            temp = tempLong;
            tempLong = tempLong - temp;
            b.Append(Convert.ToString(temp));

            return b.ToString().ToLower();
        }

        //         void cfg_OnUserUpdateControledDevices(List<DeviceAssociation> ControledDevices)
        //         {
        //             //             ZBClass class2 = new ZBClass();
        //             //             class2.header = class2.GetBytesFromString("ZSIG");
        //             //             class2.command = 11;
        //             //             class2.alphacommand = class2.GetBytesFromString("SendCmd");
        //             //             class2.label_base = class2.GetBytesFromString("");
        //             //             class2.command_text = class2.GetBytesFromString("");
        //             //             class2.serial = 0;
        //             //             class2.param1 = 1;
        //             //             class2.param2 = 22;
        //             //             class2.param3 = 0;
        //             //             class2.param4 = 0;
        //             //             byte[] rBuff = null;
        //             //             byte[] bytes = class2.GetBytes();
        //             //             ZibaseDll.ZBClass zb = new ZibaseDll.ZBClass();
        //             //             zb.UDPDataTransmit(bytes, ref rBuff, "192.168.0.210", 0xc34f);
        // 
        //             m_Config.ControledDevices = ControledDevices;
        //             //m_ControledDevices = ControledDevices;
        //             try
        //             {
        //                 Util.SerializeToXml(m_Config, ConfigFilePath);
        //             }
        //             catch (Exception ex)
        //             {
        //                 HsObjet.getInstance().WriteLog("Error", ex.Message);
        //             }
        // 
        //         }

        void cfg_OnUserAddDevice(char hc, int dc, string DeviceName, SensorType st)
        {
            Scheduler.Classes.DeviceClass dev = HsObjet.getInstance().NewDeviceEx(DeviceName);
            dev.dc = dc.ToString();
            dev.hc = hc.ToString();
            dev.@interface = HSPI.PlugInName;
            dev.iomisc = DeviceName + " " + st.ToString() + " zibase_sensor";
            dev.misc = 0x10; //status only

            var q = m_SensorDataManager.SensorList.Where(x => x.ID == DeviceName);
            if (q.Any())
            {
                Device AddedDevice = q.First();
                var AddedDeviceValue = AddedDevice.Values.First(x => x.ValueType == st);
                if (AddedDeviceValue != null)
                {
                    AddedDeviceValue.AlreadyInHS = true;
                    AddedDeviceValue.HSDevice = dev;
                    AddedDeviceValue.HSUpdate();
                }
            }
            m_SensorDataManager.HSDevice.Add(dev);
        }

        void m_Zibase_UpdateSensorInfo(ZiBase.SensorInfo seInfo)
        {
            m_SensorDataManager.UpdateSensorData(seInfo);
        }

        void m_Zibase_NewSensorDetected(ZiBase.SensorInfo seInfo)
        {
            m_SensorDataManager.UpdateSensorData(seInfo);
        }

        private OwnDeviceManager m_OwnDeviceManager = null;
        void m_Zibase_WriteMessage(string sMsg, int level)
        {
            if (m_Config.WatchdogTimerMinutes > 0)
            {
                m_WatchdogTimer.Stop();
                m_WatchdogTimer.Start();
            }


            if (m_Config.DisplayZibaseMsgInHSLog)
                HsObjet.getInstance().WriteLog("Debug", sMsg);
            //m_Zibase.WriteMessage -= m_Zibase_WriteMessage;
            //sMsg = "ZiBASE001e16:Received radio ID (<rf>433Mhz</rf> <lev>5</lev>/5  <flag1>Alarm</flag1> <dev>Oregon TH V1.0</dev> T=<tem>+22.2</tem>°C (+71.9°F)  Batt=<bat>Ok</bat>): <id>VS2823456050</id>";
            //sMsg = "ZiBASE001e16:Received radio ID (<rf>433Mhz</rf> <lev>5</lev>/5  <dev>Oregon TH V1.0</dev> T=<tem>+22.2</tem>°C (+71.9°F)  Batt=<bat>Ok</bat>): <id>VS2823456050</id>";
            if (m_OwnDeviceManager == null)
            {
                m_OwnDeviceManager = new OwnDeviceManager();
                m_OwnDeviceManager.OnUpdateSensorInfo += m_Zibase_UpdateSensorInfo;
            }

            m_OwnDeviceManager.GetInfo(sMsg);
            //m_Zibase.WriteMessage += m_Zibase_WriteMessage;



        }

        public bool SupportsHS2()
        {
            return true;
        }

        public void HSEvent(object parms)
        {
            Object[] obj = (Object[])parms;
            int Cmd = (int)obj[0];
            if (Cmd == HomeSeer.EV_TYPE_X10_TRANSMIT)
            {
                int Action = (int)obj[3];
                String DeviceAddr = String.Format("{0}{1}", obj[2], obj[1]);

                var q = m_Config.ControledDevices.Where(x => x.DeviceCode == DeviceAddr);
                if (q.Any())
                {
                    int DimValue = (int)obj[4];

                    DeviceAssociation DeviceAssoc = q.First();

                    // Dim/Bright command
                    if (Action == HomeSeer.UDim || Action == HomeSeer.UBright)
                    {
                        Debug.WriteLine("Dim : " + DimValue);
                        m_Zibase.SendCommand(DeviceAddr, ZiBase.State.STATE_DIM, 100 - DimValue, /*ZiBase.Protocol.PROTOCOL_CHACON*/ DeviceAssoc.Protocol, 1);
                    }
                    else
                    {
                        // On off Command
                        m_Zibase.SendCommand(DeviceAddr, (Action == HomeSeer.UOff ? ZiBase.State.STATE_OFF : ZiBase.State.STATE_ON), 100, DeviceAssoc.Protocol /*ZiBase.Protocol.PROTOCOL_CHACON*/, 1);
                    }

                    //HsObjet.getInstance().WriteLog("Debug", "Request send to " + obj[2] + obj[1] + " value : " + obj[3]);
                }
            }
            if (Cmd == HomeSeer.EV_TYPE_CONFIG_CHANGE)
            {
                foreach (Device dev in m_SensorDataManager.SensorList)
                    dev.Check();
            }
            if (Cmd == HomeSeer.EV_TYPE_STRING_CHANGE)
            {
            }
        }

        #region ISynchronizeInvoke Membres

        public IAsyncResult BeginInvoke(Delegate method, object[] args)
        {
            throw new NotImplementedException();
        }

        public object EndInvoke(IAsyncResult result)
        {
            throw new NotImplementedException();
        }

        public object Invoke(Delegate method, object[] args)
        {
            throw new NotImplementedException();
        }

        public bool InvokeRequired
        {
            get { return false; }
        }

        #endregion

    }


    static class MyExstension
    {
        public static string GetCompleteDeviceCode(this DeviceClass dev)
        {
            return dev.hc + dev.dc;
        }
        public static bool ExistInHs(this DeviceClass dev)
        {
            return (HsObjet.getInstance().GetDeviceByRef(dev.@ref) != null);
        }
    }
}
