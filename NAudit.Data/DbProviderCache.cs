
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Reflection;

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

        [ImportMany(typeof(IAuditDbProvider))]
        private List<IAuditDbProvider> DatabaseProviderCache { get; set; }

        private void PopulateProviderCache()
        {
            if (this.Providers == null)
            {
                this.Providers = new Dictionary<string, IAuditDbProvider>();
            }

            var catalog = new AggregateCatalog(new DirectoryCatalog("."), new AssemblyCatalog(Assembly.GetExecutingAssembly()));
            var container = new CompositionContainer(catalog);
            container.ComposeParts(this);

            foreach (IAuditDbProvider dbProvider in this.DatabaseProviderCache)
            {
                this.Providers.Add(dbProvider.ProviderNamespace.ToLowerInvariant(), dbProvider);
            }
        }
    }
}
