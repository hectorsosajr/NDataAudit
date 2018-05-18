//*********************************************************************
// File:       		IAuditNoSqlProvider.cs
// Author:  	    Hector Sosa, Jr
// Date:			5/17/2018
//*********************************************************************
// Change Log
//*********************************************************************
// USER					DATE            COMMENTS
// Hector Sosa, Jr		5/17/2018	    Created
//*********************************************************************

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using SimpleInjector;

namespace NDataAudit.Data
{
    /// <summary>
    /// Class NoSqlProviderCache
    /// </summary>
    public class NoSqlProviderCache
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Data.NoSqlProviderCache"/> class.
        /// </summary>
        public NoSqlProviderCache()
        {
            this.PopulateProviderCache();
        }

        /// <summary>
        /// Gets the available NoSQL providers.
        /// </summary>
        /// <value>The providers.</value>
        public Dictionary<string, IAuditNoSqlProvider> Providers { get; private set; }

        private void PopulateProviderCache()
        {
            if (this.Providers == null)
            {
                this.Providers = new Dictionary<string, IAuditNoSqlProvider>();
            }

            string providerDirectory = AppDomain.CurrentDomain.BaseDirectory;

            var providerAssemblies =
                from file in new DirectoryInfo(providerDirectory).GetFiles()
                where file.Extension.ToLower() == ".dll"
                select Assembly.LoadFile(file.FullName);

            Container container = new Container();

            var providerTypes = container.GetTypesToRegister(typeof(IAuditNoSqlProvider), providerAssemblies);

            container.Collection.Register<IAuditNoSqlProvider>(providerTypes);

            foreach (var provider in container.GetAllInstances<IAuditNoSqlProvider>())
            {
                Providers.Add(provider.ProviderNamespace.ToLowerInvariant(), provider);
            }
        }
    }
}
