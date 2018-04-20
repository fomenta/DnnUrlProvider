using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DotNetNuke.Entities.Urls;

namespace DNN.Modules.UrlRedirectProvider
{
    internal class UrlRedirectProviderInfo : ExtensionUrlProviderInfo
    {
        internal string _ignoreRedirectRegex;
        internal UrlRedirectProviderInfo()
        {
            if (this.Settings != null && this.Settings.ContainsKey("ignoreRedirectRegex"))
                _ignoreRedirectRegex = this.Settings["ignoreRedirectRegex"];
        }
    }
}
