using IniParser;
using IniParser.Model;
using System.IO;

namespace Sineva.VHL.GUI.TouchPad

{
    internal class IniFile
    {
        private static IniFile _instance;
        private readonly string m_iniFilePath = "./list.ini";
        public static IniData IniData;
        public FileIniDataParser Parser { get => m_parser; set => m_parser = value; }

        public string IniFilePath => m_iniFilePath;

        FileIniDataParser m_parser = new FileIniDataParser();

        public static IniFile Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (typeof(IniFile))
                    {
                        if (_instance == null)
                        {
                            _instance = new IniFile();
                        }
                    }
                }
                return _instance;
            }
        }

        public void Initilize()
        {
            if (File.Exists(IniFilePath))
            {
                IniData = Parser.ReadFile(IniFilePath);
            }
            else
            {
                File.Create(IniFilePath).Dispose();
                IniData = new IniData();
            }
        }

        public void Save()
        {
            Parser.WriteFile(IniFilePath, IniData);
        }
    }
}
