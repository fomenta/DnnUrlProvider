/*
' Copyright (c) 2013  DotNetNuke
'  All rights reserved.
' 
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED
' TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
' THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF
' CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
' DEALINGS IN THE SOFTWARE.
' 
*/

using System;
using System.Collections.Generic;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Entities.Urls;

namespace DotNetNuke.Modules.SocialUrlProvider
{
    /// -----------------------------------------------------------------------------
    /// <summary>
    /// The Settings class manages Module Settings
    /// 
    /// Typically your settings control would be used to manage settings for your module.
    /// There are two types of settings, ModuleSettings, and TabModuleSettings.
    /// 
    /// ModuleSettings apply to all "copies" of a module on a site, no matter which page the module is on. 
    /// 
    /// TabModuleSettings apply only to the current module on the current page, if you copy that module to
    /// another page the settings are not transferred.
    /// 
    /// If you happen to save both TabModuleSettings and ModuleSettings, TabModuleSettings overrides ModuleSettings.
    /// 
    /// Below we have some examples of how to access these settings but you will need to uncomment to use.
    /// 
    /// Because the control inherits from DNN.SocialUrlProviderSettingsBase you have access to any custom properties
    /// defined there, as well as properties from DNN such as PortalId, ModuleId, TabId, UserId and many more.
    /// </summary>
    /// -----------------------------------------------------------------------------
    public partial class Settings : PortalModuleBase , IExtensionUrlProviderSettingsControl
    {

    
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            LocalResourceFile = "~/DesktopModules/DNN_SocialUrlProvider/App_LocalResources/Settings.ascx.resx";
        }

        public void LoadSettings()
        {
            if (!IsPostBack)
            {
                if (Provider != null)
                {
                    txtUrlPath.Text = GetSafeString(Constants.UrlPathSettingName, "");
                    lblUrlPath.Text = Localization.GetString("UrlPath.Text", LocalResourceFile);
                    chkHideGroupPagePath.Checked = GetSafeBool(Constants.HideGroupPathPageSettingName, false);
                    lblHideGroupPagePath.Text = Localization.GetString("HideGroupPagePath.Text", LocalResourceFile);
                    cboSocialGroupPage.SelectedPage = GetSafeTab(Constants.GroupPageTabIdSettingName, null);
                    lblSocialGroupPage.Text = Localization.GetString("GroupPageTabId", LocalResourceFile);
                }
                else
                {
                    throw new ArgumentNullException("ExtensionUrlProviderInfo is null on LoadSettings()");
                }
            }

        }

        public ExtensionUrlProviderInfo Provider { get; set; }

        public Dictionary<string, string> SaveSettings()
        {
            var settings = new Dictionary<string, string>();
            if (string.IsNullOrEmpty(txtUrlPath.Text) == false)
            {
                settings.Add(Constants.UrlPathSettingName, txtUrlPath.Text);
                
            }
            settings.Add(Constants.HideGroupPathPageSettingName, chkHideGroupPagePath.Checked.ToString());

            if (cboSocialGroupPage.SelectedPage != null)
            {
                settings.Add(Constants.GroupPageTabIdSettingName , cboSocialGroupPage.SelectedPage.TabID.ToString());
            }

            return settings;
        }

        #region private helper methods

        private string GetSafeString(string settingName, string defaultValue)
        {
            string raw = defaultValue;
            if (Provider != null && Provider.Settings != null && Provider.Settings.ContainsKey(settingName))
            {
                raw = Provider.Settings[settingName];
            }
            return raw;
        }

        private TabInfo GetSafeTab(string settingName, TabInfo defaultValue)
        {
            TabInfo result = defaultValue;
            int tabId = GetSafeInt(settingName, -1);
            if (tabId > 0)
            {
                TabController tc = new TabController();
                result = tc.GetTab(tabId, this.PortalId, false);

            }
            return result;
        }

        private int GetSafeInt(string settingName, int defaultValue)
        {
            int result = defaultValue;
            string raw = null;
            if (Provider != null && Provider.Settings != null && Provider.Settings.ContainsKey(settingName))
            {
                raw = Provider.Settings[settingName];
            }
            if (string.IsNullOrEmpty(raw) == false) int.TryParse(raw, out result);
            return result;
        }

        private bool GetSafeBool(string settingName, bool defaultValue)
        {
            bool result = defaultValue;
            string raw = null;
            if (Provider != null && Provider.Settings != null && Provider.Settings.ContainsKey(settingName))
            {
                raw = Provider.Settings[settingName];
            }
            if (string.IsNullOrEmpty(raw) == false) bool.TryParse(raw, out result);
            return result;
        }

        #endregion
    }

}
