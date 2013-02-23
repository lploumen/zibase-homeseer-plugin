using System;
using ZibaseDll;

namespace HSPI_ZIBASE_LPL
{
    [Serializable]
    public class DeviceAssociation
    {
        private String m_DeviceCode;
        private ZiBase.Protocol m_Protocol;

        public String DeviceCode
        {
            get { return m_DeviceCode; }
            set { m_DeviceCode = value; }
        }


        public ZiBase.Protocol Protocol
        {
            get { return m_Protocol; }
            set { m_Protocol = value; }
        }
    }
}
