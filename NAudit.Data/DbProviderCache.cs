
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using SimpleInjector;

namespace NAudit.Data
{
    /// <summary>
    /// Class DbProviderCache
    /// </summary>
    public class DbProviderCache
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DbProviderCache"/> class.
        /// </summary>
        public DbProviderCache()
        {
            this.PopulateProviderCache();
        }

        /// <summary>
        /// Gets the available database providers.
        /// </summary>
        /// <value>The providers.</value>
        public Dictionary<string, IAuditDbProvider> Providers { get; private set; }

        private void PopulateProviderCache()
        {
            if (this.Providers == null)
            {
                this.Providers = new Dictionary<string, IAuditDbProvider>();
            }

            string providerDirectory = AppDomain.CurrentDomain.BaseDirectory;

            var providerAssemblies = 
                from file in new DirectoryInfo(providerDirectory).GetFiles()
                where file.Extension.ToLower() == ".dll"
                select Assembly.LoadFile(file.FullName);

            Container container = new Container();

            var providerTypes = container.GetTypesToRegister(typeof(IAuditDbProvider), providerAssemblies);

            container.RegisterCollection<IAuditDbProvider>(providerTypes);

            foreach (var provider in container.GetAllInstances<IAuditDbProvider>())
            {
                Providers.Add(provider.ProviderNamespace.ToLowerInvariant(),provider);
            }
        }
    }
}
