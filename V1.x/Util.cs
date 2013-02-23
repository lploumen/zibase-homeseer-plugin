using System;
using System.Xml.Serialization;
using System.IO;
using Microsoft.Win32;
using System.Globalization;

namespace HSPI_ZIBASE_LPL
{
    class Util
    {
        public static void SerializeToXml<T>(T obj, string fileName)
        {
            FileStream fileStream = new FileStream(fileName, FileMode.Create);
            XmlSerializer ser = new XmlSerializer(typeof(T));
            ser.Serialize(fileStream, obj);
            fileStream.Close();
           
        }
        public static T DeserializeFromXml<T>(string xml)
        {
            T result;

            using (FileStream fs = new FileStream(xml, FileMode.Open))
            {
                XmlSerializer ser = new XmlSerializer(typeof(T));
                result = (T)ser.Deserialize(fs);
            }
            return result;
        }

        public static double HighestNETFrameworkVersion()
        {
            RegistryKey installed_versions = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\NET Framework Setup\NDP");
            string[] version_names = installed_versions.GetSubKeyNames();
            //version names start with 'v', eg, 'v3.5' which needs to be trimmed off before conversion
            double Framework = Convert.ToDouble(version_names[version_names.Length - 1].Remove(0, 1), CultureInfo.InvariantCulture);
            //int SP = Convert.ToInt32(installed_versions.OpenSubKey(version_names[version_names.Length - 1]).GetValue("SP", 0));
            return Framework;

        }

    }
}
