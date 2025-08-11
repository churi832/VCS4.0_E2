using Sineva.VHL.Data.DbAdapter;
using Sineva.VHL.Library;
using Sineva.VHL.Library.Remoting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace Sineva.VHL.IF.WebApi
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            if (!Initialize())
            {
                throw new Exception("Initialize Error!");
            }
        }
        private bool Initialize()
        {
            try
            {
                bool result = true;
                result &= ReadIniConfig();
                result &= RemoteManager.TouchInstance.Initialize(ConnectionMode.Client, CHANNEL_TYPE.IPC, "127.0.0.1");
                result &= DatabaseAdapter.Instance.Initialize();
                result &= TeachingOffsetAdapter.Instance.Initialize();
                //if (RemoteManager.TouchInstance.Remoting == null || RemoteManager.TouchInstance.Remoting.TouchGUI == null) return false;
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        private bool ReadIniConfig()
        {
            try
            {
                string iniFileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Config.ini");
                AppConfig.Instance.ConfigPath.SelectedFolder = IniProfile.GetPrivateProfileString("BasicData", "ConfigFilePath", string.Empty, iniFileName);
                //var value = System.Configuration.ConfigurationManager.AppSettings["OCSConfigDir"];
                if (AppConfig.Instance.ConfigPath.SelectedFolder == string.Empty)
                {
                    throw new Exception("ConfigFilePath");
                }
                else
                {
                    DirectoryInfo directoryInfo = new DirectoryInfo(AppConfig.Instance.ConfigPath.SelectedFolder);
                    if (directoryInfo.Exists == false)
                    {
                        throw new Exception("ConfigFilePath");
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
