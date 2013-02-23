using System;
using Scheduler.Classes;

namespace HSPI_ZIBASE_LPL
{
    class DeviceValue
    {
        public SensorType ValueType { get; set; }
        public long DwValue { get; set; }
        public String Value { get; set; }
        public bool AlreadyInHS { get; set; }
        public DeviceClass HSDevice { get; set; }
        public String DisplayValue { get; set; }
        public DeviceValue()
        {
            AlreadyInHS = false;
        }

        public void Check()
        {
            if (HSDevice != null)
            {
                if (!HSDevice.ExistInHs())
                {
                    AlreadyInHS = false;
                    HSDevice = null;
                }

            }
        }
        public void HSUpdate()
        {
            // foreach (DeviceClass dc in q2)
            //{
            if (HSDevice != null)
            {
                //HSDevice.status = (int)DwValue;
                HsObjet.getInstance().SetDeviceStatus(HSDevice.hc + HSDevice.dc, (int)DwValue);

                HsObjet.getInstance().SetDeviceValue(HSDevice.hc + HSDevice.dc, (int)DwValue);
                HsObjet.getInstance().SetDeviceString(HSDevice.hc + HSDevice.dc, DisplayValue, false);
            }
            //}
        }
    }
}
