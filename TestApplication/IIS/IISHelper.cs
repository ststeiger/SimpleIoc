
using Microsoft.Web.Administration;
using System.Linq;

namespace TestApplication
{

    // https://github.com/luxpermanet/CommandLineDeploymentTool/blob/ce73cd7bf024b00eef4db8d155d71d90504b0b61/CommandLineDeploymentTool/Helpers/IISHelper.cs
    static class IISHelper
    {

        private const string IIS_CONFIG_LOCAL_PATH = "";


        public static ServerManager GetRemoteServerManager(string serverName)
        {
            //ServerManager manager = ServerManager.OpenRemote("IIS02"); //Or, by IP "1.1.1.1"
            ServerManager manager = ServerManager.OpenRemote(serverName); //Or, by IP "1.1.1.1"
            return manager;
        }



        // https://github.com/GTRekter/ServerManager/blob/master/IISWCF/Service.svc.cs
        public static string CreateSite(string applicationName, string applicationPath, int portNumber
            , string applicationPoolName = null)
        {

            try
            {
                using (ServerManager serverManager = new ServerManager(IIS_CONFIG_LOCAL_PATH))
                {
                    Site newSite = serverManager.Sites.Add(applicationName, applicationPath, portNumber);
                    newSite.ServerAutoStart = true;

                    if (!string.IsNullOrEmpty(applicationPoolName))
                    {
                        newSite.Applications[0].ApplicationPoolName = applicationPoolName;
                        ApplicationPool applicationPool = serverManager.ApplicationPools[applicationPoolName];
                        serverManager.CommitChanges();
                        applicationPool.Recycle();
                    }
                    else
                    {
                        serverManager.CommitChanges();
                    }
                }

                return "Fuck yeah! It works baby";
            }
            catch (System.Exception exc)
            {
                return exc.Message;
            }
        }

        public static string CreateNewApplicationPool(string applicationPoolName)
        {
            try
            {
                using (ServerManager serverManager = new ServerManager(IIS_CONFIG_LOCAL_PATH))
                {
                    ApplicationPool applicationPool = serverManager.ApplicationPools.Add(applicationPoolName);
                    serverManager.CommitChanges();
                    applicationPool.Recycle();

                }

                return "Fuck yeah! It works baby";
            }
            catch (System.Exception exc)
            {
                return exc.Message;
            }
        }


        // https://github.com/GTRekter/ServerManager/tree/master/IISWCF
        public static string AddAssemblyToWebsite(string applicationName, string assemblyName, byte[] assemblyByteArray)
        {
            try
            {
                string applicationAssembliesLocalPath = getApplicationLocalPath(applicationName);

                //using (var fileStream = new FileStream(Path.Combine(applicationAssembliesLocalPath, assemblyName), FileMode.Create, FileAccess.Write))
                //{
                //    fileStream.Write(assemblyByteArray, 0, assemblyByteArray.Length);
                //}

                return "Fuck yeah! It works baby";
            }
            catch (System.Exception exc)
            {
                return exc.Message;
            }

        }

        public static string EditSiteConfig(string applicationName, string xPath, string attribute, string value)
        {
            try
            {
                using (ServerManager serverManager = new ServerManager(IIS_CONFIG_LOCAL_PATH))
                {
                    Configuration webConfiguration = serverManager.GetWebConfiguration(applicationName);
                    ConfigurationSection configurationSection = webConfiguration.GetSection(xPath);
                    ConfigurationAttribute configurationAttribute = configurationSection.GetAttribute(attribute);

                    configurationAttribute.Value = value;
                    serverManager.CommitChanges();
                }

                return "Fuck yeah! It works baby";
            }
            catch (System.Exception exc)
            {
                return exc.Message;
            }
        }


        private static string getApplicationLocalPath(string applicationName)
        {
            string windowsDrive = System.IO.Path.GetPathRoot(System.Environment.SystemDirectory);

            using (ServerManager serverManager = new ServerManager(IIS_CONFIG_LOCAL_PATH))
            {
                Site site = serverManager.Sites.Where(s => s.Name.Equals(applicationName)).Single();
                if (site == null)
                {
                    throw new System.Exception("Application not found");
                }

                Application applicationRoot = site.Applications.Where(a => a.Path == "/").Single();
                VirtualDirectory virtualRoot = applicationRoot.VirtualDirectories.Where(v => v.Path == "/").Single();

                return System.IO.Path.Combine(virtualRoot.PhysicalPath.Replace("%SystemDrive%", windowsDrive), "bin");
            }
        }


        public static void ApplicationPoolRecycle(string applicationPoolName)
        {
            ServerManager iis = new ServerManager();
            iis.ApplicationPools[applicationPoolName].Recycle();
        }

        public static void ApplicationPoolStop(string applicationPoolName)
        {
            ServerManager iis = new ServerManager();
            iis.ApplicationPools[applicationPoolName].Stop();
        }

        public static void ApplicationPoolStart(string applicationPoolName)
        {
            ServerManager iis = new ServerManager();
            iis.ApplicationPools[applicationPoolName].Start();
        }

        public static void WebsiteStart(string siteName)
        {
            ServerManager iis = new ServerManager();
            iis.Sites[siteName].Start();
        }

        public static void WebsiteStop(string siteName)
        {
            ServerManager iis = new ServerManager();
            iis.Sites[siteName].Stop();
        }
    }


}
