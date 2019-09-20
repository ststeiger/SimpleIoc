
namespace TestApplication.IIS
{


    class DsExample
    {

        // https://stackoverflow.com/questions/3938467/list-all-virtual-directories-in-iis-5-6-and-7
        static void Test()
        {
            string domainName = System.DirectoryServices.ActiveDirectory.Domain.GetComputerDomain().Name;

            GetSiteID(domainName);
        }

        public static string GetSiteID(string domain)
        {
            string siteId = string.Empty;

            // System.DirectoryServices.DirectoryEntry iis = new System.DirectoryServices.DirectoryEntry("IIS://localhost/W3SVC");
            System.DirectoryServices.DirectoryEntry iis = new System.DirectoryServices.DirectoryEntry("IIS://vmszhm7050/W3SVC");
            foreach (System.DirectoryServices.DirectoryEntry entry in iis.Children)
                if (entry.SchemaClassName.ToLower() == "iiswebserver")
                    if (entry.Properties["ServerComment"].Value.ToString().ToLower() == domain.ToLower())
                        siteId = entry.Name;

            if (string.IsNullOrEmpty(siteId))
                throw new System.Exception("Could not find site '" + domain + "'");

            return siteId;
        }


        public static System.DirectoryServices.DirectoryEntry[] GetAllChildren(System.DirectoryServices.DirectoryEntry entry)
        {
            System.Collections.Generic.List<System.DirectoryServices.DirectoryEntry> children = 
                new System.Collections.Generic.List<System.DirectoryServices.DirectoryEntry>();
            foreach (System.DirectoryServices.DirectoryEntry child in entry.Children)
            {
                children.Add(child);
                children.AddRange(GetAllChildren(child));
            }

            return children.ToArray();
        }


        public static System.Collections.Generic.Dictionary<string, string> MapSiteIDs()
        {
            System.DirectoryServices.DirectoryEntry IIS = 
                new System.DirectoryServices.DirectoryEntry("IIS://localhost/W3SVC");

            System.Collections.Generic.Dictionary<string, string> dictionary = 
                new System.Collections.Generic.Dictionary<string, string>(); // key=domain, value=siteId

            foreach (System.DirectoryServices.DirectoryEntry entry in IIS.Children)
            {
                if (entry.SchemaClassName.ToLower() == "iiswebserver")
                {
                    string domainName = entry.Properties["ServerComment"].Value.ToString().ToLower();
                    string siteID = entry.Name;
                    dictionary.Add(domainName, siteID);
                }
            }

            return dictionary;
        }


    }
}
