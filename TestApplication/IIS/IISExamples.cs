
using System.Linq;
using Microsoft.Web.Administration;


namespace TestApplication.IIS
{

    // https://johnlnelson.com/2014/06/15/the-microsoft-web-administration-namespace/
    // https://docs.microsoft.com/en-us/iis/manage/scripting/how-to-use-microsoftwebadministration
    class IISExamples
    {
        public static void Test()
        {
            ServerManager server = ServerManager.OpenRemote("vmszhm7050");
            System.Console.WriteLine(server);
            server.Dispose();

            TestServerManager();


            System.Console.WriteLine("Hello World!");
        }

        public static void TestServerManager()
        {
            ServerManager server = new ServerManager();

            SiteCollection sites = server.Sites;
            foreach (Site site in sites)
            {
                ApplicationDefaults defaults = site.ApplicationDefaults;

                //get the name of the ApplicationPool under which the Site runs
                string appPoolName = defaults.ApplicationPoolName;

                ConfigurationAttributeCollection attributes = defaults.Attributes;
                foreach (ConfigurationAttribute configAttribute in attributes)
                {
                    //put code here to work with each ConfigurationAttribute
                }

                ConfigurationAttributeCollection attributesCollection = site.Attributes;
                foreach (ConfigurationAttribute attribute in attributesCollection)
                {
                    //put code here to work with each ConfigurationAttribute
                }

                //Get the Binding objects for this Site
                BindingCollection bindings = site.Bindings;
                foreach (Microsoft.Web.Administration.Binding binding in bindings)
                {
                    //put code here to work with each Binding
                }

                //retrieve the State of the Site
                ObjectState siteState = site.State;

                //Get the list of all Applications for this Site
                ApplicationCollection applications = site.Applications;
                foreach (Microsoft.Web.Administration.Application application in applications)
                {
                    //put code here to work with each Application
                }
            }
        }

        public static void GetApplicationsAssociatedWithSite(Site site)
        {
            ApplicationCollection applications = site.Applications;
            foreach (Microsoft.Web.Administration.Application application in applications)
            {
                //get the name of the ApplicationPool
                string applicationPoolName = application.ApplicationPoolName;

                VirtualDirectoryCollection directories = application.VirtualDirectories;
                foreach (VirtualDirectory directory in directories)
                {
                    //put code here to work with each VirtualDirectory
                }
            }
        }

        public static void GetVirtualDirectoriesAssociatedWithApplication(Microsoft.Web.Administration.Application application)
        {
            VirtualDirectoryCollection directories = application.VirtualDirectories;
            foreach (VirtualDirectory directory in directories)
            {
                ConfigurationAttributeCollection attributes = directory.Attributes;
                foreach (ConfigurationAttribute attribute in attributes)
                {
                    //put code here to work with each attribute
                }

                ConfigurationChildElementCollection childElements = directory.ChildElements;
                foreach (ConfigurationElement element in childElements)
                {
                    //put code here to work with each ConfigurationElement
                }

                //get the directory.Path
                string path = directory.Path;

                //get the physical path
                string physicalPath = directory.PhysicalPath;
            }
        }

        public static void GetApplicationPoolsAssociatedWithServer()
        {
            ServerManager server = new ServerManager();

            ApplicationPoolCollection applicationPools = server.ApplicationPools;
            foreach (ApplicationPool pool in applicationPools)
            {
                //get the AutoStart boolean value
                bool autoStart = pool.AutoStart;

                //get the name of the ManagedRuntimeVersion
                string runtime = pool.ManagedRuntimeVersion;

                //get the name of the ApplicationPool
                string appPoolName = pool.Name;

                //get the identity type
                ProcessModelIdentityType identityType = pool.ProcessModel.IdentityType;

                //get the username for the identity under which the pool runs
                string userName = pool.ProcessModel.UserName;

                //get the password for the identity under which the pool runs
                string password = pool.ProcessModel.Password;
            }
        }



        public static void CreateNewApplicationPool()
        {
            ServerManager server = new ServerManager();

            ApplicationPool myApplicationPool = null;

            //we will create a new ApplicationPool named 'MyApplicationPool'
            //we will first check to make sure that this pool does not already exist
            //since the ApplicationPools property is a collection, we can use the Linq FirstOrDefault method
            //to check for its existence by name
            if (server.ApplicationPools != null && server.ApplicationPools.Count > 0)
            {
                if (server.ApplicationPools.FirstOrDefault(p => p.Name == "MyApplicationPool") == null)
                {
                    //if we find the pool already there, we will get a referecne to it for update
                    myApplicationPool = server.ApplicationPools.FirstOrDefault(p => p.Name == "MyApplicationPool");
                }
                else
                {
                    //if the pool is not already there we will create it
                    myApplicationPool = server.ApplicationPools.Add("MyApplicationPool");
                }
            }
            else
            {
                //if the pool is not already there we will create it
                myApplicationPool = server.ApplicationPools.Add("MyApplicationPool");
            }

            if (myApplicationPool != null)
            {
                //for this sample, we will set the pool to run under the NetworkService identity
                myApplicationPool.ProcessModel.IdentityType = ProcessModelIdentityType.NetworkService;

                //we set the runtime version
                myApplicationPool.ManagedRuntimeVersion = "v4.0";

                //we save our new ApplicationPool!
                server.CommitChanges();
            }
        }


        public static void CreateNewSite(ApplicationPool myApplicationPool)
        {
            ServerManager server = new ServerManager();

            if (server.Sites != null && server.Sites.Count > 0)
            {
                //we will first check to make sure that the site isn't already there
                if (server.Sites.FirstOrDefault(s => s.Name == "MySite") == null)
                {
                    //we will just pick an arbitrary location for the site
                    string path = @"c:\MySiteFolder\";

                    //we must specify the Binding information
                    string ip = "*";
                    string port = "80";
                    string hostName = "*";

                    string bindingInfo = string.Format(@"{0}:{1}:{2}", ip, port, hostName);

                    //add the new Site to the Sites collection
                    Site site = server.Sites.Add("MySite", "http", bindingInfo, path);

                    // set the ApplicationPool for the new Site
                    site.ApplicationDefaults.ApplicationPoolName = myApplicationPool.Name;

                    //save the new Site!
                    server.CommitChanges();
                }
            }
        }





        internal static void Deploy(Application application, string sourceDir
            , System.IO.Abstractions.IFileSystem fileSystem
            , string relativePathUnderVDir = "")
        {

            if (!fileSystem.Directory.Exists(sourceDir))
            {
                throw new System.Exception(string.Format("Failed to deploy files to application, source directory does not exist: '{0}'.", sourceDir));
            }

            if (application.VirtualDirectories.Count <= 0)
            {
                throw new System.Exception(string.Format("Application '{0}' does not have a virtual directory.", application.Path));
            }

            var physicalPath = application.VirtualDirectories[0].PhysicalPath;
            if (!fileSystem.Directory.Exists(physicalPath))
            {
                fileSystem.Directory.CreateDirectory(physicalPath);
            }

            var relativeDirectoryPath = System.IO.Path.Combine(physicalPath, relativePathUnderVDir);
            if (!fileSystem.Directory.Exists(relativeDirectoryPath))
            {
                fileSystem.Directory.CreateDirectory(relativeDirectoryPath);
            }

            fileSystem.DirectoryCopy(sourceDir, relativeDirectoryPath);

            if (string.IsNullOrEmpty(relativeDirectoryPath))
            {
                fileSystem.DirectorySetAttribute(relativeDirectoryPath, System.IO.FileAttributes.Normal);
            }
        }


    }
}
