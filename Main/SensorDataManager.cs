using System;
using System.Collections.Generic;
using System.Linq;
using ZibaseDll;
using Scheduler.Classes;
using Scheduler;

namespace HSPI_ZIBASE_LPL
{
    class SensorDataManager
    {
        public List<Device> SensorList {get;set;}

        // List of HomeSeer devices we are managing in our plugin
        public  List<DeviceClass> HSDevice { get; set; }

        public SensorDataManager()
        {
            SensorList = new List<Device>();

            HSDevice = new List<DeviceClass>();
            //  Enumerate through all the devices looking for mine and add it the the HSDevice array
            clsDeviceEnumeration de=null;
            de = (clsDeviceEnumeration)HsObjet.getInstance().GetDeviceEnumerator();
            while (!de.Finished)
            {
                DeviceClass dev = (DeviceClass)de.GetNext();
                System.Diagnostics.Debug.WriteLine(dev.Name + " " + dev.@interface + " " + dev.GetCompleteDeviceCode() + " IOMisc:" + dev.iomisc);
                if (dev.@interface == HSPI.PlugInName)
                    HSDevice.Add(dev);
            }
        }

        private void Update(Device sd, ZiBase.SensorInfo se)
        {
          /*  if (se.sType == "lev") // common value for all sensor
                sd.RXLevel = se.sValue;
            else if (se.sType == "bat")// common value for all sensor
                sd.BatLevel = se.sValue;
            else
            {*/
                UpdateValue(sd, se, Device.EnumConverter(se.sType));
            //}
        }

        private void UpdateValue(Device sd, ZibaseDll.ZiBase.SensorInfo si, SensorType st)
        {
            DeviceValue dv = null;
            
            var q = sd.Values.Where(x => x.ValueType == st);
            if (q.Any())
            {
                dv = q.First();
                dv.Value = si.sHTMLValue;

                if (st == SensorType.ENERGY_KW || st == SensorType.ENERGY_KWH)
                {
                    String[] splitted = si.sValue.Split(new char[] { ' ' });
                    if (splitted.Length == 2)
                    {
                        float val = Convert.ToSingle(dv.Value,System.Globalization.CultureInfo.InvariantCulture); // Convert.ToInt32(splitted[0]);
                        //val /= 1000.0F;
                        dv.DisplayValue = val + " " + splitted[1];
                    }
                }
                else
                    dv.DisplayValue = si.sValue;
            }
            else
            {
                dv = new DeviceValue {Value = si.sHTMLValue, ValueType = st};
                if (st == SensorType.ENERGY_KW || st == SensorType.ENERGY_KWH)
                {
                    String[] splitted = si.sValue.Split(new char[] { ' ' });
                    if (splitted.Length == 2)
                    {
                        //float val = Convert.ToInt32(splitted[0]);
                        //val /= 1000.0F;
                        float val = Convert.ToSingle(dv.Value, System.Globalization.CultureInfo.InvariantCulture); // Convert.ToInt32(splitted[0]);
                        dv.DisplayValue = val + " " + splitted[1];
                    }
                }
                else
                    dv.DisplayValue = si.sValue;
                sd.Values.Add(dv);
            }

            // if no HSDevice associated,look for the current device in HS
            // IOMisc contains the ID followed by the SensorType
            if (dv.HSDevice == null)
            {
                var q2 = HSDevice.Where(x => x.iomisc.Contains(si.sID) && x.iomisc.Contains(st.ToString()));

                if (q2.Any())
                {
                    dv.AlreadyInHS = true;
                    dv.HSDevice = q2.First();
                }
            }
            dv.DwValue = si.dwValue;
            dv.HSUpdate();
        }

        public void UpdateSensorData(ZiBase.SensorInfo se)
        {
            var q = from c in SensorList where c.ID.Contains(se.sID) select c;
            if (q.Any())
            {
                foreach (Device sd in q)
                {
                    //SensorData sd = q.First();
                    Update(sd, se);
                }
            }
            else
            {
                Device sd = new Device(se.sID) {ID = se.sID, Name = se.sName};


                Update(sd, se);
                SensorList.Add(sd);
            }
        }
    }
}
