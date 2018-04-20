using System;
using System.Collections.Generic;
using System.Web;
using DotNetNuke.Modules.SocialUrlProvider.Entities;

namespace DotNetNuke.Modules.SocialUrlProvider.Data
{
    internal abstract class DataProvider
    {
        #region Shared/Static Methods

        // singleton reference to the instantiated object 
        static DataProvider objProvider = null;

        // constructor
        static DataProvider()
        {
            CreateProvider();
        }

        // dynamically create provider
        private static void CreateProvider()
        {
            //don't need run-time instancing when using built-in sqlDataProvider class
            objProvider = new Data.SqlDataProvider();
        }

        // return the provider
        public static DataProvider Instance()
        {
            return objProvider;
        }

        #endregion

        #region abstract methods
        /// <summary>
        /// Returns list of Urls related to your module
        /// </summary>
        /// <param name="tabId">The tabid the module is on.</param>
        /// <param name="entries">out parameter returns a zero-n list of DNNSocialUrlProvider Urls</param>
        /// <remarks>Example only - this call can be anything you like.</remarks>
        internal abstract void GetSocialUrls(int portalid, out FriendlyUrlInfoCol urls);
        internal abstract void GetSocialUrl(int portalId, int itemId, string itemType, out FriendlyUrlInfo url);
        #endregion
    }
}