using System;
using System.Collections.Generic;
using System.Text;
using ZibaseDll;
using Scheduler;
using Scheduler.Classes;
using System.Text.RegularExpressions;
using System.Linq;
using System.Reflection;

namespace HSPI_ZIBASE_LPL
{
    [ObfuscationAttribute(Exclude = true, ApplyToMembers = false)]
    class WebConfig
    {
        [ObfuscationAttribute(Exclude = true)]
        public String link;
        [ObfuscationAttribute(Exclude = true)]
        public String linktext;
        [ObfuscationAttribute(Exclude = true)]
        public String page_title;





        public delegate void UserAddDeviceEventHandler(char hc, int dc, String DeviceName, SensorType st);
        //public delegate void UserUpdateControledDevicesHandler(List<DeviceAssociation> ControledDevices);
        public event UserAddDeviceEventHandler OnUserAddDevice;


        //public event UserUpdateControledDevicesHandler OnUserUpdateControledDevices;
        public event EventHandler OnConfigurationChanged;


        public List<DeviceAssociation> ControledDevices { get; set; }
        public PluginConfig Config { get; set; }
        public WebConfig()
        {
            this.link = "Zibase";
            this.linktext = "ZIBASE";
            this.page_title = "Zibase Plugin";

        }

        //         public static string Version()
        //         {
        //             return HSPI.PLUGIN_VERSION.ToString();
        //         }

        public List<Device> SensorList
        {
            set;
            get;
        }

        private int cnt = 0;
        public string DisplaySensorInfo(Device se)
        {

            StringBuilder p = new StringBuilder();

            //string classname = "tableroweven"; // ((m_iUnitNumber % 2) == 0 ? "tableroweven" : "tablerowodd");

            //HsObjet.getInstance().WriteLog("Debug", "Device : " + se.ID + " " + se.Name);
            bool firstTime = true;
            foreach (DeviceValue dv in se.Values)
            {
                string classname = ((cnt % 2) == 0 ? "tableroweven" : "tablerowodd");
                //HsObjet.getInstance().WriteLog("Debug", "Value : " + dv.DisplayValue + " " + dv.ValueType + " inhs:" + dv.AlreadyInHS + " HSDevice null? :" + (dv.HSDevice==null));

                p.Append("<tr>\n");
                if (firstTime)
                {
                    p.Append("<td  noWrap class=\"" + classname + "\" align=center valign=middle>" + se.Name + "</td>\n");
                    p.Append("<td  noWrap class=\"" + classname + "\" align=center valign=middle>" + se.ID + "</td>\n");
                    firstTime = false;
                }
                else
                {
                    p.Append("<td  noWrap class=\"" + classname + "\" align=center valign=middle></td>\n");
                    p.Append("<td  noWrap class=\"" + classname + "\" align=center valign=middle></td>\n");
                }

                p.Append("<td  class=\"" + classname + "\" align=center>\n");
                p.Append(dv.ValueType.GetStringValue());
                p.Append("</td>\n");


                p.Append("<td  class=\"" + classname + "\" align=center>\n");
                p.Append(dv.DisplayValue);
                p.Append("</td>\n");


                p.Append("<td  class=\"" + classname + "\" align=center>\n");
                p.Append(dv.DwValue);
                p.Append("</td>\n");

                /*
                p.Append("<td  class=\"" + classname + "\" align=center>\n");
                p.Append(se.RXLevel);
                p.Append("</td>\n");

                p.Append("<td class=\"" + classname + "\" align=center>\n");
                p.Append(se.BatLevel);
                p.Append("</td>\n");
                 */

                p.Append("<td class=\"" + classname + "\" align=center>\n");
                p.Append("<input type=\"checkbox\" onClick=\"return false\"" + (dv.AlreadyInHS ? " checked " : "") + "\" name=\"AlreadyInHS\">" + (dv.AlreadyInHS ? " (" + dv.HSDevice.hc + dv.HSDevice.dc + ")" : "") + "</input>\n");
                p.Append("</td>\n");



                p.Append("<td class=\"" + classname + "\" align=middle>\n");
                p.Append("<form name='frmMain" + se.ID + "' method='POST'><br>\n");
                p.Append("<input type=\"hidden\" name=\"ref_page\" value=\"" + link + "\">\n");
                //p.Append("<input type='submit' name='AddDeviceId' value='"+ se.ID + "'>\n");
                p.Append("<input type=\"hidden\" name=\"Action\" value=\"\">\n");
                p.Append("<input type=\"hidden\" name=\"dc\" value=\"\">\n");
                p.Append("<input type=\"hidden\" name=\"hc\" value=\"\">\n");
                p.Append("<input type=\"hidden\" name=\"SensorType\" value=\"" + (int)dv.ValueType + "\">\n");
                p.Append("<input type=\"hidden\" name=\"Device\" value=\"\">\n");
                p.Append("<input class=\"formbutton\" id=\"AddUnit\" onclick=\"DoAddDevice('" + se.ID + "', 'DoAddDevice',this.form)\" type=\"button\"value=\"Add Device \" name=\"AddDevice\">\n");


                p.Append("</form>\n");
                p.Append("</td>\n");

                p.Append("</tr>\n");
            }
            return p.ToString();
        }

        public string GenLinksMenu()
        {
            StringBuilder p = new StringBuilder();

            p.Append(@"<p>");
            p.Append(@"<A href=""/" + link + @"?page=1"">Zibase Sensors</A>");
            p.Append(@"&nbsp;| <A href=""/" + link + @"?page=2"">Zibase controled devices</A>");
            p.Append(@"&nbsp;| <A href=""/" + link + @"?page=3"">Setup</A>");
            //             p.Append(@"&nbsp;| <A href=""/" + link + @"?page=4"">Configuration</A>");
            //             p.Append(@"&nbsp;| <A href=""/" + link + @"?page=5"">Devices</A>");
            //             p.Append(@"&nbsp;| <A href=""/" + link + @"?page=6"">Advanced Setup</A>");
            p.Append(@"</p>");


            p.Append("If you like this software and would like to make a donation, <br> please click the link below, to help pay for the development and maintenance of this program.");

            p.Append("<form action=\"https://www.paypal.com/cgi-bin/webscr\" method=\"post\">" +
                    "<input type=\"hidden\" name=\"cmd\" value=\"_s-xclick\">" +
                    "<input type=\"hidden\" name=\"hosted_button_id\" value=\"ASSANDB4NM4SJ\">" +
                    "<input type=\"image\" src=\"https://www.paypalobjects.com/WEBSCR-640-20110401-1/en_US/BE/i/btn/btn_donateCC_LG.gif\" border=\"0\" name=\"submit\" alt=\"PayPal - The safer, easier way to pay online!\">" +
                    "<img alt=\"\" border=\"0\" src=\"https://www.paypalobjects.com/WEBSCR-640-20110401-1/en_US/i/scr/pixel.gif\" width=\"1\" height=\"1\">" +
                    "</form>");
            return p.ToString();
        }

        private string GenerateZibaseProtocolListBox(String ListBoxName, ZiBase.Protocol SelectedProtocol)
        {
            StringBuilder p = new StringBuilder();
            p.Append("<select name=" + ListBoxName + ">\n");

            foreach (ZiBase.Protocol Protocol in Enum.GetValues(typeof(ZiBase.Protocol)))
                p.Append("<option value=" + (int)Protocol + " " + (Protocol == SelectedProtocol ? "Selected" : "") + ">" + Protocol + "</option>\n");

            p.Append("</select>\n");
            return p.ToString();
        }

        public String BuildHSDevicesPage()
        {
            StringBuilder p = new StringBuilder();
            p.Append("<form name='frmMain' method='POST'><br>\n");

            p.Append("<table border=\"1\" cellspacing = \"0\" cellpadding = \"5\">\n<tr>\n");
            p.Append("<tr><td class=\"tableheader\" colspan=\"8\">Zibase Controled devices</td></tr>");
            p.Append("<td class=\"tablecolumn\">Name</td>\n");
            p.Append("<td class=\"tablecolumn\">Address</td>\n");
            p.Append("<td class=\"tablecolumn\">Use with Zibase?</td>\n");
            p.Append("<td class=\"tablecolumn\">Protocol</td>\n");
            p.Append("</tr>");


            clsDeviceEnumeration de = (clsDeviceEnumeration)HsObjet.getInstance().GetDeviceEnumerator();
            string classname = "";
            int i = 0;
            while (!de.Finished)
            {
                classname = ((i++ % 2) == 0 ? "tableroweven" : "tablerowodd");
                DeviceClass dev = de.GetNext();

                //HsObjet.getInstance().WriteLog("debug", "Device : " + dev.Name);

                // Get DeviceAssociation Class
                DeviceAssociation DeviceAssoc = ControledDevices.FirstOrDefault(x => x.DeviceCode == dev.GetCompleteDeviceCode());

                //HsObjet.getInstance().WriteLog("debug", "DeviceAssoc is null : " + (DeviceAssoc == null));

                p.Append("<tr>\n");

                // Device Name
                p.Append("<td class=\"" + classname + "\" align=center valign=middle>" + dev.Name + "</td>\n");

                // Device Code
                p.Append("<td class=\"" + classname + "\" align=center valign=middle>" + dev.hc + dev.dc + "</td>\n");


                // Controled by zibase checkbox
                p.Append("<td class=\"" + classname + "\" align=center valign=middle >\n");

                bool IsZibaseSensor = false;

                if (DeviceAssoc != null)
                {
                    // this device is controled by Zibase
                    p.Append("<input type=\"checkbox\" checked  name=\"DeviceCode_" + dev.GetCompleteDeviceCode() + "\">\n");
                }
                else if (dev.iomisc.Contains("zibase_sensor"))
                {
                    // This is a Zibase managed sensor
                    IsZibaseSensor = true;
                    //                    p.Append("<input type=\"checkbox\" " + (dev.iomisc.Contains("zibase_sensor") ? "disabled=\"disabled\" checked" : "") + "\" name=\"DeviceCode_" + dev.GetCompleteDeviceCode() /*dev.@ref*/ + "\">\n");
                    p.Append("<input type=\"checkbox\" disabled=\"disabled\" checked \" name=\"DeviceCode_" + dev.GetCompleteDeviceCode() + "\">\n");
                }
                else
                {
                    // Not controlled by Zibase
                    p.Append("<input type=\"checkbox\" name=\"DeviceCode_" + dev.GetCompleteDeviceCode() + "\">\n");
                }

                p.Append("</td>\n");

                p.Append("<td class=\"" + classname + "\" align=center valign=middle >\n");
                if (!IsZibaseSensor)
                    p.Append(GenerateZibaseProtocolListBox("Protocol_" + dev.GetCompleteDeviceCode(), DeviceAssoc != null ? DeviceAssoc.Protocol : ZiBase.Protocol.PROTOCOL_BROADCAST));
                else
                    p.Append("Zibase sensor");
                p.Append("</td>\n");

                p.Append("</tr>\n");

            }

            p.Append("</table>\n");
            p.Append("<br>");

            p.Append("<input type=\"hidden\" name=\"ref_page\" value=\"" + link + "\">\n");
            p.Append("<input type=\"submit\" value=\"Submit\" />");

            p.Append("</form>\n");
            return p.ToString();
        }

        public String BuildPage()
        {
            StringBuilder p = new StringBuilder();
            p.Append("<table border=\"1\" cellspacing = \"0\" cellpadding = \"5\">\n<tr>\n");
            p.Append("<tr><td class=\"tableheader\" colspan=\"8\">Zibase Sensors</td></tr>");
            p.Append("<td class=\"tablecolumn\">Unit#</td>\n");
            p.Append("<td class=\"tablecolumn\" align=center>Name</td>\n");
            p.Append("<td class=\"tablecolumn\" align=center>Type</td>\n");
            p.Append("<td class=\"tablecolumn\" align=center>String</td>\n");
            p.Append("<td class=\"tablecolumn\" align=center>Value</td>\n");
            //p.Append("<td class=\"tablecolumn\" align=center>Rx Level</td>\n");
            //p.Append("<td class=\"tablecolumn\" align=center>Batt</td>\n");
            p.Append("<td class=\"tablecolumn\" align=center>Already in HS ?</td>\n");
            p.Append("<td class=\"tablecolumn\" align=center>Action</td>\n");
            p.Append("</tr>");
            
            cnt = 0;
            foreach (Device sd in SensorList)
            {
                p.Append(DisplaySensorInfo(sd));
                cnt++;
            }
            p.Append("</table>");
            return p.ToString();
        }
        [ObfuscationAttribute(Exclude = true)]
        public string PagePut(string lnk)
        {
            //HsObjet.getInstance().WriteLog("Debug", "Link : " + lnk);

            string[] sList = lnk.Split('&');
            //string pg;
            Dictionary<string, string> dctParams = new Dictionary<string, string>();

            foreach (string sl in sList)
            {
                string[] sListParams = sl.Split('=');
                if (!dctParams.ContainsKey(sListParams[0]))
                {
                    dctParams.Add(sListParams[0], sListParams[1]);
                }
            }



            if (lnk.Contains("page=2"))
            {
                //ControledDevices = new List<DeviceAssociation>();
                //List<DeviceAssociation>  = new List<DeviceAssociation>();
                ControledDevices.Clear();

                Regex rg = new Regex("[a-zA-Z][0-9]?[0-9]");
                foreach (String key in dctParams.Keys)
                {
                    Match m = rg.Match(key);
                    if (m.Success)
                    {
                        DeviceAssociation DeviceAssoc = null;
                        if (key.ToLower().Contains("devicecode"))
                        {
                            if (dctParams[key] == "on")
                            {
                                DeviceAssoc = new DeviceAssociation {DeviceCode = m.Value};
                                ControledDevices.Add(new DeviceAssociation { DeviceCode = m.Value });
                            }
                        }
                        else
                        {
                            if (key.ToLower().Contains("protocol"))
                            {
                                var q = ControledDevices.Where(x => x.DeviceCode == m.Value);
                                if (q.Any())
                                {
                                    DeviceAssoc = q.First();
                                    DeviceAssoc.Protocol = (ZiBase.Protocol)Convert.ToInt16(dctParams[key]);
                                }

                            }
                            //= ControledDevices.Where(x => x.DeviceCode == m.Value);
                        }

                        System.Diagnostics.Debug.WriteLine("Key : " + key);
                    }
                }
                //                 if (OnUserUpdateControledDevices != null)
                //                     OnUserUpdateControledDevices(ControledDevices);

                if (OnConfigurationChanged != null)
                    OnConfigurationChanged(this, null);
            }

            if (lnk.Contains("page=3"))
            {

                Config.RestartZibaseSearch = false;
                Config.DisplayZibaseMsgInHSLog = false;

                if (dctParams.ContainsKey("start_zibase_search") && dctParams["start_zibase_search"] == "on")
                {
                    Config.RestartZibaseSearch = true;
                }
                if (dctParams.ContainsKey("display_zibase_msg_in_hs") && dctParams["display_zibase_msg_in_hs"] == "on")
                {
                    Config.DisplayZibaseMsgInHSLog = true;
                }

                if (dctParams.ContainsKey("watchdog_timer"))
                {
                    Config.WatchdogTimerMinutes = Convert.ToInt16(dctParams["watchdog_timer"]);
                }



                Config.ZibaseToken = dctParams["token"];

                if (OnConfigurationChanged != null)
                    OnConfigurationChanged(this, null);
            }

            string Value;
            if (dctParams.TryGetValue("Device", out Value))
            {
                String hc = dctParams["hc"];
                String dc = dctParams["dc"];
                int DeviceCode = Convert.ToInt16(dc);
                SensorType type = (SensorType)Convert.ToInt16(dctParams["SensorType"]);

                if (OnUserAddDevice != null)
                    OnUserAddDevice(hc[0], DeviceCode, Value, type);

            }

            return GenPage(ref lnk);
        }

        [ObfuscationAttribute(Exclude = true)]
        public string GenPage(ref string lnk)
        {
            //HsObjet.getInstance().WriteLog("Debug", "Link : " + lnk);
            StringBuilder p = new StringBuilder();

            p.Append(GenLinksMenu());

            if (lnk.Contains("page=1"))
            {
                p.Append(BuildPage());
                //p.Append(BuildThermoPage());
            }
            else if (lnk.Contains("page=2"))
            {
                p.Append(BuildHSDevicesPage());
            }
            else if (lnk.Contains("page=3"))
            {
                p.Append(BuildSetupPage());
            }
            else
            {
                p.Append(BuildPage());
            }
            p.Append("Plugin version : " + HSPI.PLUGIN_VERSION);
            p.Append("<br>");
            p.Append("<a href=\"http://www.planete-domotique.com/\" target=\"_blank\">Zibase SDK</a> version : " + HSPI.ZIBASE_SDK_VERSION);

            p.Append("<script language=\"JavaScript\">\n");
            p.Append("function DoAddDevice(unit, doaction,form) {\n");
            //p.Append("  if (confirm(\"Are you sure you want to delete the unit\" + unit)) {\n");
            p.Append("    var hc = prompt(\"House Code\", \"\");\n");
            p.Append("    var dc = prompt(\"Device Code\", \"\");\n");
            p.Append("    form.hc.value = hc;\n");
            p.Append("    form.dc.value = dc;\n");
            p.Append("    form.Action.value = doaction;\n");
            p.Append("    form.Device.value = unit;\n");
            p.Append("    form.submit();\n");
            //p.Append("  }\n");
            p.Append("}\n");
            p.Append("</script>\n");
            return p.ToString();
        }

        private String BuildSetupPage()
        {
            StringBuilder p = new StringBuilder();
            p.Append("<form name='frmMain' method='POST'><br>\n");
            p.Append("<table border=\"1\" cellspacing = \"0\" cellpadding = \"5\">\n<tr>\n");
            p.Append("<tr><td class=\"tableheader\" colspan=\"8\">Zibase Plugin Setup</td></tr>");
            //p.Append("<td class=\"tablecolumn\">Unit#</td>\n");
            p.Append("<td class=\"tablecolumn\" align=center>Parameter</td>\n");
            p.Append("<td class=\"tablecolumn\" align=center>Value</td>\n");
            //             p.Append("<td class=\"tablecolumn\" align=center>Rx Level</td>\n");
            //             p.Append("<td class=\"tablecolumn\" align=center>Batt</td>\n");
            //             p.Append("<td class=\"tablecolumn\" align=center>Already in HS ?</td>\n");
            //             p.Append("<td class=\"tablecolumn\" align=center>Action</td>\n");
            p.Append("</tr>");

            // ===

            string classname = "tableroweven"; // 

            p.Append("<tr>\n");

            p.Append("<td  noWrap class=\"" + classname + "\" align=center valign=middle>" + "Token" + "</td>\n");
            p.Append("<td  noWrap class=\"" + classname + "\" align=center valign=middle>" +
                "<input type=\"text\" name=\"token\" value=\"" + (Config != null ? Config.ZibaseToken : "") + "\"></input>" +
                     "</td>\n");

            //             p.Append("<td  class=\"" + classname + "\" align=center>\n");
            //             p.Append(dv.DisplayValue);
            //             p.Append("</td>\n");
            // 
            //             p.Append("<td  class=\"" + classname + "\" align=center>\n");
            //             p.Append(se.RXLevel);
            //             p.Append("</td>\n");
            // 
            //             p.Append("<td class=\"" + classname + "\" align=center>\n");
            //             p.Append(se.BatLevel);
            //             p.Append("</td>\n");
            // 
            //             p.Append("<td class=\"" + classname + "\" align=center>\n");
            //             p.Append("<input type=\"checkbox\" onClick=\"return false\"" + ((dv.AlreadyInHS == true) ? " checked " : "") + "\" name=\"AlreadyInHS\">" + ((dv.AlreadyInHS == true) ? " (" + dv.HSDevice.hc + dv.HSDevice.dc + ")" : "") + "</input>\n");
            //             p.Append("</td>\n");
            // 
            // 
            // 
            //             p.Append("<td class=\"" + classname + "\" align=middle>\n");
            //             p.Append("<form name='frmMain" + se.ID + "' method='POST'><br>\n");
            //             p.Append("<input type=\"hidden\" name=\"ref_page\" value=\"" + link + "\">\n");
            //             //p.Append("<input type='submit' name='AddDeviceId' value='"+ se.ID + "'>\n");
            //             p.Append("<input type=\"hidden\" name=\"Action\" value=\"\">\n");
            //             p.Append("<input type=\"hidden\" name=\"dc\" value=\"\">\n");
            //             p.Append("<input type=\"hidden\" name=\"hc\" value=\"\">\n");
            //             p.Append("<input type=\"hidden\" name=\"SensorType\" value=\"" + (int)dv.ValueType + "\">\n");
            //             p.Append("<input type=\"hidden\" name=\"Device\" value=\"\">\n");
            //             p.Append("<input class=\"formbutton\" id=\"AddUnit\" onclick=\"DoAddDevice('" + se.ID + "', 'DoAddDevice',this.form)\" type=\"button\"value=\"Add Device \" name=\"AddDevice\">\n");
            // 
            // 
            //             p.Append("</form>\n");
            // p.Append("</td>\n");

            p.Append("</tr>\n");

            p.Append("<tr>\n");

            p.Append("<td  noWrap class=\"" + classname + "\" align=left valign=middle>" + "Restart Zibase Search?<i><br>If checked, when submit button is pressed,<br> the plugin will search for Zibases</i>" + "</td>\n");
            p.Append("<td  noWrap class=\"" + classname + "\" align=center valign=middle>" +
                "<input type=\"checkbox\" name=\"start_zibase_search\"></input>" +
                     "</td>\n");
            p.Append("</tr>\n");


            p.Append("<tr>\n");

            p.Append("<td  noWrap class=\"" + classname + "\" align=left valign=middle>Display Zibase messages in HomeSeer log?<i><br>There can be a lot of messages comming from Zibase.<br>Sometimes it may be necessary to not display them.</i></td>\n");
            p.Append("<td  noWrap class=\"" + classname + "\" align=center valign=middle>" +
                "<input type=\"checkbox\"" + (Config.DisplayZibaseMsgInHSLog ? " checked " : "") + "name=\"display_zibase_msg_in_hs\"></input>" +
                     "</td>\n");
            p.Append("</tr>\n");

            // ===

            p.Append("<td  noWrap class=\"" + classname + "\" align=left valign=left>" + "Watchdog<br><i>If no messages received from Zibase after x minutes,<br>the plugin will search for Zibase.<br>0 to disable.</i>" + "</td>\n");
            p.Append("<td  noWrap class=\"" + classname + "\" align=center valign=left>" +
                "<input type=\"text\" name=\"watchdog_timer\" value=\"" + (Config != null ? Config.WatchdogTimerMinutes.ToString() : "") + "\"></input>" +
                     "minutes</td>\n");



            p.Append("</table>");

            p.Append("<input type=\"hidden\" name=\"ref_page\" value=\"" + link + "\">\n");
            p.Append("<input type=\"submit\" value=\"Submit\" />");

            p.Append("</form>\n");
            return p.ToString();
        }

    }
}
