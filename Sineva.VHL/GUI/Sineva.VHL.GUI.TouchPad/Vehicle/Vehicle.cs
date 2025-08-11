using IniParser.Model;
using System.Collections.Generic;
using System.Linq;

namespace Sineva.VHL.GUI.TouchPad
{
    internal class Vehicle
    {
        public static Vehicle Instance = new Vehicle();



        private SortedDictionary<string, string> m_VehicleMap;

        public SortedDictionary<string, string> VehicleMap
        {

            get { return m_VehicleMap; }
            set { m_VehicleMap = value; }
        }

        public void Initilize()
        {
            m_VehicleMap = new SortedDictionary<string, string>();
            foreach (SectionData sectionData in IniFile.IniData.Sections)
            {
                m_VehicleMap.Add(sectionData.SectionName, IniFile.IniData[sectionData.SectionName]["IP"]);
            }
        }

        public string GetVehicleIP(string vehicleNum)
        {
            //Debug.Print(m_UserMap["软件开发部——徐昊翔"]);
            return m_VehicleMap[vehicleNum];
        }

        public void AddVehicle(string vehicleNum, string ip)
        {
            if (m_VehicleMap.ContainsKey(vehicleNum)) return;
            m_VehicleMap[vehicleNum] = ip;
        }

        public void RemoveVehicle(string vehicleNum)
        {
            m_VehicleMap.Remove(vehicleNum);
            IniFile.IniData.Sections.RemoveSection(vehicleNum);
        }

    }
}
