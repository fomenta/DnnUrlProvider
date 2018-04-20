using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Entities.Urls;
namespace DNN.Modules.UrlRedirectProvider.UI
{
    /// <summary>
    /// This is the code-behind for the Settings.ascx control.  This inherits from the standard .NET UserControl, but also implements the provider specific IExtensionUrlProviderSettingsControl.
    /// This control will be loaded by the Admin->Site Settings page.  It is optional for Extension URL providers, but allows users to control module settings via the interface, rather than 
    /// having to set options through database tables.  The writing / reading of the items from the configuration file is handled by the DNN, and doesn't need to explicitly implemented.
    /// </summary>
    public partial class Settings : PortalModuleBase, IExtensionUrlProviderSettingsControl
    {
        private int _portalId;
        private UrlRedirectProviderInfo _provider;
        #region controls
        protected Label lblHeader;
        protected TextBox txtIgnoreRedirectRegex;
        
        #endregion
        #region Web Form Designer Generated Code
        //[System.Diagnostics.DebuggerStepThrough]
        override protected void OnInit(EventArgs e)
        {
            InitializeComponent();
            base.OnInit(e);
        }

        /// <summary>
        ///		Required method for Designer support - do not modify
        ///		the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.Load += new System.EventHandler(this.Page_Load);
        }

        #endregion
        #region events code
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                //note page load runs after LoadSettings(); because of dynamic control loading
            }
            catch (Exception ex)
            {
                DotNetNuke.Services.Exceptions.Exceptions.ProcessModuleLoadException(this, ex);
            }
        }
        #endregion
        #region content methods
        private void LocalizeControls()
        {
            lblHeader.Text = DotNetNuke.Services.Localization.Localization.GetString("Header.Text");

        }
        #endregion
        #region IProviderSettings Members
        /// <summary>
        /// LoadSettings is called when the module control is first loaded onto the page
        /// </summary>
        /// <remarks>
        /// This method shoudl read all the custom properties of the provider and set the controls
        /// of the page to reflect the current settings of the provider.
        /// </remarks>
        /// <param name="provider"></param>
        public void LoadSettings()
        {
            if (_provider != null && !IsPostBack)
            {
                //build list of controls
                if (!IsPostBack)
                    LocalizeControls();

                txtIgnoreRedirectRegex.Text = _provider.Settings["ignoreRedirectRegex"];
            }
        }
        /// <summary>
        /// UpdateSettings is called when the 'update' button is clicked on the interface.
        /// This should take any values from the page, and set the individual properties on the 
        /// instance of the module provider.
        /// </summary>
        /// <param name="provider"></param>
        public Dictionary<string,string> UpdateSettings()
        {
            Dictionary<string, string> settings = new Dictionary<string, string>();
            if (_provider != null)
            {
                settings.Add("ignoreRedirectRegex", txtIgnoreRedirectRegex.Text);
            }
            return settings;

        }
        
        public System.Web.UI.Control Control
        {
            get { return this;  }
        }

        public string ControlPath
        {
            get { return base.TemplateSourceDirectory; }
        }

        public string ControlName
        {
            get { return "ProviderSettings";  }
        }

        public string LocalResourceFile
        {
            get { return "DesktopModules/DNN_UrlRedirectProvider/App_LocalResources/Settings.ascx.resx"; }
        }
        public int PortalId
        {
            get { return _portalId; }
            set { _portalId = value; }
        }

        #endregion


        void IExtensionUrlProviderSettingsControl.LoadSettings()
        {
            LoadSettings();
        }

        ExtensionUrlProviderInfo IExtensionUrlProviderSettingsControl.Provider
        {
            get
            {
                return (ExtensionUrlProviderInfo)_provider;
            }
            set
            {
                if (value.GetType() == typeof(UrlRedirectProviderInfo))
                {
                    _provider = (UrlRedirectProviderInfo)value;
                }
            }
        }

        Dictionary<string, string> IExtensionUrlProviderSettingsControl.SaveSettings()
        {
            return UpdateSettings();
        }
    }
}
