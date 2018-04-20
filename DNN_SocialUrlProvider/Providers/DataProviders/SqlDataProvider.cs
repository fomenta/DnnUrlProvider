using System;
using System.Collections.Generic;
using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;

using Microsoft.ApplicationBlocks.Data;

using DotNetNuke.Common.Utilities;
using DotNetNuke.Framework.Providers;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Entities.Modules;

using DotNetNuke.Modules.SocialUrlProvider.Entities;

namespace DotNetNuke.Modules.SocialUrlProvider.Data
{
    internal class SqlDataProvider : DataProvider 
    {
        private const string ModuleQualifier = "soc_";
        private const string OwnerQualifier = "";

        #region Private Members
        private const string ProviderType = "data";
        private ProviderConfiguration _providerConfiguration = ProviderConfiguration.GetProviderConfiguration(ProviderType);
        private string _connectionString;
        private string _providerPath;
        private string _objectQualifier;
        private string _databaseOwner;
        #endregion
        #region Constructor
        /// <summary>
        /// Constructs new SqlDataProvider instance
        /// </summary>
        internal SqlDataProvider()
        {
            //Read the configuration specific information for this provider
            Provider objProvider = (Provider)_providerConfiguration.Providers[_providerConfiguration.DefaultProvider];

            //Read the attributes for this provider
            //Get Connection string from web.config
            _connectionString = Config.GetConnectionString();

            if (_connectionString.Length == 0)
            {
                // Use connection string specified in provider
                _connectionString = objProvider.Attributes["connectionString"];
            }

            _providerPath = objProvider.Attributes["providerPath"];

            //override the standard dotNetNuke qualifier with a module qualifier one if it exists
            _objectQualifier = objProvider.Attributes["objectQualifier"];
            if ((_objectQualifier != "") && (_objectQualifier.EndsWith("_") == false))
            {
                _objectQualifier += "_";
            }
            else
                if (_objectQualifier == null) _objectQualifier = "";

            _objectQualifier += OwnerQualifier;

            _databaseOwner = objProvider.Attributes["databaseOwner"];
            if ((_databaseOwner != "") && (_databaseOwner.EndsWith(".") == false))
            {
                _databaseOwner += ".";
            }
        }
        #endregion
        #region Properties

        /// <summary>
        /// Gets and sets the connection string
        /// </summary>
        public string ConnectionString
        {
            get { return _connectionString; }
        }
        /// <summary>
        /// Gets and sets the Provider path
        /// </summary>
        public string ProviderPath
        {
            get { return _providerPath; }
        }
        /// <summary>
        /// Gets and sets the Object qualifier
        /// </summary>
        public string ObjectQualifier
        {
            get { return _objectQualifier; }
        }
        /// <summary>
        /// Gets and sets the database ownere
        /// </summary>
        public string DatabaseOwner
        {
            get { return _databaseOwner; }
        }

        #endregion
        #region private members
        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets the fully qualified name of the stored procedure
        /// </summary>
        /// <param name="name">The name of the stored procedure</param>
        /// <returns>The fully qualified name</returns>
        /// -----------------------------------------------------------------------------
        private string GetFullyQualifiedName(string name)
        {
            return DatabaseOwner + ObjectQualifier + ModuleQualifier + name;
        }
        #endregion

        #region abstract overridden properties
        internal override void GetSocialUrls(int portalId, out FriendlyUrlInfoCol urls)
        {
            urls = new FriendlyUrlInfoCol();
            string sp = GetFullyQualifiedName("GetSocialUrls");
            SqlParameter[] parms = new SqlParameter[1];
            parms[0] = new SqlParameter("@portalId", portalId);
            SqlDataReader rdr = SqlHelper.ExecuteReader(_connectionString, CommandType.StoredProcedure, sp, parms);
            while (rdr.Read())
            {
                FriendlyUrlInfo url = new FriendlyUrlInfo();
                if (!Convert.IsDBNull(rdr["UrlFragment1"]))
                    url.UrlFragment1 = (string)rdr["UrlFragment1"];
                if (!Convert.IsDBNull(rdr["UrlFragment2"]))
                    url.UrlFragment2 = (string)rdr["UrlFragment2"];
                if (!Convert.IsDBNull(rdr["ItemType"]))
                    url.ItemType = (string)rdr["ItemType"];
                if (!Convert.IsDBNull(rdr["ItemId"]))
                    url.ItemId = (int)rdr["ItemId"];
                //add to collection
                urls.Add(url);
            }

            rdr.Close();
            rdr.Dispose();
        }
        internal override void GetSocialUrl(int portalId, int itemId, string itemType, out FriendlyUrlInfo url)
        {
            url = null;
            string sp = GetFullyQualifiedName("GetSocialUrl");
            SqlParameter[] parms = new SqlParameter[3];
            parms[0] = new SqlParameter("@portalId", portalId);
            parms[1] = new SqlParameter("@itemId", itemId);
            parms[2] = new SqlParameter("@itemType", itemType);
            SqlDataReader rdr = SqlHelper.ExecuteReader(_connectionString, CommandType.StoredProcedure, sp, parms);
            if (rdr.Read())
            {
                url = new FriendlyUrlInfo();
                if (!Convert.IsDBNull(rdr["UrlFragment1"]))
                    url.UrlFragment1 = (string)rdr["UrlFragment1"];
                if (!Convert.IsDBNull(rdr["UrlFragment2"]))
                    url.UrlFragment2 = (string)rdr["UrlFragment2"];
                if (!Convert.IsDBNull(rdr["ItemType"]))
                    url.ItemType = (string)rdr["ItemType"];
                if (!Convert.IsDBNull(rdr["ItemId"]))
                    url.ItemId = (int)rdr["ItemId"];
            }

            rdr.Close();
            rdr.Dispose();
        }

        #endregion
    }
}
