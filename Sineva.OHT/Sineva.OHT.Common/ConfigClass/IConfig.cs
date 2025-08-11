namespace Sineva.OHT.Common
{
    public interface IConfig
    {
        bool Load(string configPath);
        bool Save(string configPath);
    }
}
