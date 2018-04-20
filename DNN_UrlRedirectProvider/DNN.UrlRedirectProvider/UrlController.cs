using System;
using System.Collections;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Portals.Internal;
using DotNetNuke.Entities.Urls;
using DNN.Modules.UrlRedirectProvider.Entities;

namespace DNN.Modules.UrlRedirectProvider
{
    internal static class UrlController
    {
        //keys used for cache entries for Urls and Querystrings
        private const string ConfiguredRedirectsKey = "ConfiguredUrlRedirect_Portal_{0}";
        private const string RedirectIndexKey = "UrlRedirects_Portal_{0}";
        private const string RegexIndexKey = "UrlRedirectRegex_Portal_{0}";

        /// <summary>
        /// Returns a friendly url index from the cache or database.
        /// </summary>
        /// <param name="tabId"></param>
        /// <param name="portalId"></param>
        /// <param name="UrlRedirectModuleProvider"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        internal static void GetRedirectIndexes(int portalId, UrlRedirectProvider provider, FriendlyUrlOptions options, out Hashtable redirectIndex, out List<RedirectInfo> regexIndex, ref List<string> messages)
        {
            var redirectCacheKey = GetRedirectIndexKeyForPortal(portalId);
            var regexCacheKey = GetRegexIndexKeyForPortal(portalId);
            redirectIndex = (Hashtable)DataCache.GetCache(redirectCacheKey);
            regexIndex = (List<RedirectInfo>)DataCache.GetCache(regexCacheKey);
            if (redirectIndex == null || regexIndex == null)
            {
                //build index for tab
                BuildUrlIndexes(portalId, provider, options, out redirectIndex, out regexIndex, ref messages);
                StoreIndexes(redirectIndex, redirectCacheKey, regexIndex, regexCacheKey);
            }
        }

        private static object GetUrlRedirectsCallback(CacheItemArgs cacheItemArgs)
        {
            var portalId = (int) cacheItemArgs.ParamList[0];

            return CBO.FillCollection<RedirectInfo>(Data.DataProvider.Instance().GetUrlRedirectsForPortal(portalId));
        }

        public static List<RedirectInfo> GetUrlRedirects(int portalId)
        {
            return CBO.GetCachedObject<List<RedirectInfo>>(new CacheItemArgs(GetConfiguredRedirectsKeyForPortal(portalId), DataCache.PortalCacheTimeOut, DataCache.PortalCachePriority, portalId), GetUrlRedirectsCallback);
        }

        private static void BuildUrlIndexes(int portalId, UrlRedirectProvider provider, FriendlyUrlOptions options, out Hashtable friendlyUrlIndex, out List<RedirectInfo> regexIndex, ref List<string> messages)
        {
            friendlyUrlIndex = new Hashtable();
            regexIndex = new List<RedirectInfo>();
            //call database procedure to get list of redirects
            List<RedirectInfo> redirects = GetUrlRedirects(portalId);
            
            if (redirects != null)
            {
                foreach (RedirectInfo redirect in redirects)
                {
                    string validationMessage;
                    if (redirect.IsValid(out validationMessage))
                    {
                        if (redirect.IsRegex == false)
                        {
                            string url = redirect.MatchUrl.ToLower();
                            friendlyUrlIndex.Add(url, redirect);
                        }
                        else
                        {
                            regexIndex.Add(redirect);
                        }
                    }
                    else
                    {
                        //invalid redirect, don't add to list
                        messages.Add(validationMessage);
                    }
                }
            }
        }
        
        /// <summary>
        /// Store the two indexes into the cache
        /// </summary>
        /// <param name="redirectIndex"></param>
        /// <param name="redirectCacheKey"></param>
        /// <param name="queryStringIndex"></param>
        /// <param name="queryStringCacheKey"></param>
        private static void StoreIndexes(Hashtable redirectIndex, string redirectCacheKey, List<RedirectInfo> regexIndex, string regexCacheKey)
        {
            TimeSpan expire = new TimeSpan(24, 0, 0);
            DataCache.SetCache(redirectCacheKey, redirectIndex, expire);
            DataCache.SetCache(regexCacheKey, regexIndex, expire);
        }

        /// <summary>
        /// Return the cache key
        /// </summary>
        /// <param name="portalId"></param>
        /// <returns></returns>
        private static string GetRedirectIndexKeyForPortal(int portalId)
        {
            return string.Format(RedirectIndexKey, portalId);
        }
        private static string GetRegexIndexKeyForPortal(int portalId)
        {
            return string.Format(RegexIndexKey, portalId);
        }

        private static string GetConfiguredRedirectsKeyForPortal(int portalId)
        {
            return string.Format(ConfiguredRedirectsKey, portalId);
        }
        /// <summary>
        /// Removes the scheme from the Url and returns as a stirng
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        internal static string StripSchemeAndQueryStringFromUrl(Uri uri)
        {
            string url = uri.AbsoluteUri;
            if (uri != null)
            {
                //remove hte scheme
                string scheme = uri.Scheme + Uri.SchemeDelimiter;
                if (url != null && url.StartsWith(scheme) && url.Length > scheme.Length)
                    url = url.Substring(scheme.Length);
                //remove the querystring
                if (string.IsNullOrEmpty(uri.Query) == false)
                    url = url.Substring(0, url.Length - uri.Query.Length);
            }
            return url;
        }
        internal static string StripSchemeFromUrl(Uri uri)
        {
            string url = uri.AbsoluteUri;
            if (uri != null)
            {
                string scheme = uri.Scheme + Uri.SchemeDelimiter;
                if (url != null && url.StartsWith(scheme) && url.Length > scheme.Length)
                    url = url.Substring(scheme.Length);
            }
            return url;
        }

        internal static string StripAliasFromUrl(string absoluteUrl, string httpAlias)
        {
            string url = "";
            if (string.IsNullOrEmpty(absoluteUrl) == false 
             && string.IsNullOrEmpty(httpAlias) == false 
             && httpAlias.Length < absoluteUrl.Length)
            {
                if (absoluteUrl.StartsWith(httpAlias))
                    url = absoluteUrl.Substring(httpAlias.Length);
            }
            return url;
        }
        /// <summary>
        /// Given a rule, returns a redirect location
        /// </summary>
        /// <param name="redirectInfo"></param>
        /// <param name="redirectUrl"></param>
        /// <returns></returns>
        internal static string RedirectLocation(UrlRedirectProvider provider, RedirectInfo redirectInfo, string redirectUrl, string scheme, NameValueCollection queryString, ref List<string> messages)
        {
            string destinationUrl = "";
            switch (redirectInfo.DestType.ToLower())
            {
                case "tab":
                    string path = redirectInfo.DestUrl;
                    
                    if (redirectInfo.DestTabId > -1)
                    {
                        //construct the portal settings required for the destination
                        PortalAliasController pac = new PortalAliasController();
                        PortalAliasInfo pa = null;
                        if (string.IsNullOrEmpty(redirectInfo.HttpAlias))
                        {
                            var aliases = pac.GetPortalAliasArrayByPortalID(redirectInfo.PortalId);
                            foreach (PortalAliasInfo alias in aliases)
                            {
                                if (alias.IsPrimary)
                                {
                                    pa = alias;
                                    break;
                                }
                            }

                            //safety check for failed alias
                            if (pa == null)
                                throw new NullReferenceException(
                                        "Either Redirect HttpAlias must be specified for DestType of 'Tab', or this portal must have a primary alias.  Redirect Id : " +
                                        redirectInfo.RedirectId.ToString());
                        }
                        else
                            //get this specific alias
                            pa =  pac.GetPortalAlias(redirectInfo.HttpAlias, redirectInfo.PortalId);
                        //create a new portal settings object with this alias
                        PortalSettings ps = new PortalSettings(redirectInfo.DestTabId, pa);
                        //build the redirect path
                        if (string.IsNullOrEmpty(path) == false)
                        {
                            //add a url path onto the url created for the tab
                            string qs;
                            SplitPathAndQuerystring(path, out path, out qs);
                            path = provider.EnsureNotLeadingChar("/", path);
                            //now call down to the navigate url
                            destinationUrl = DotNetNuke.Common.Globals.NavigateURL(redirectInfo.DestTabId, ps, "", path);
                            if (queryString != null && queryString.Count > 0)
                                destinationUrl = AddQueryString(destinationUrl, queryString);
                            //check for querystring preservation
                            if (redirectInfo.KeepQueryString && qs != null)
                            {
                                //put the querystring back on, if it existed
                                destinationUrl = AddQueryString(destinationUrl, qs);
                                messages.Add("URP: Redirecting " + redirectUrl + " to " + destinationUrl + " by tab + path + qs. RuleId: "+  redirectInfo.RedirectId.ToString());
                            }
                            else
                                messages.Add("URP: Redirecting " + redirectUrl + " to " + destinationUrl + " by tab + path. RuleId: " + redirectInfo.RedirectId.ToString());
                        }
                        else
                        {
                            //no path -just the tab
                            destinationUrl = DotNetNuke.Common.Globals.NavigateURL(redirectInfo.DestTabId, ps, "");
                            if (redirectInfo.KeepQueryString && queryString != null && queryString.Count > 0)
                            {
                                destinationUrl = AddQueryString(destinationUrl, queryString);
                                messages.Add("URP: Redirecting " + redirectUrl + " to " + destinationUrl + " by tab + qs. RuleId: " + redirectInfo.RedirectId.ToString());
                            }
                            else
                                messages.Add("URP: Redirecting " + redirectUrl + " to " + destinationUrl + " by tab. RuleId: " + redirectInfo.RedirectId.ToString());
                        }
                    }
                    break;
                case "url":
                    destinationUrl = redirectInfo.DestUrl;
                    if (Regex.IsMatch(destinationUrl, @"^https?://", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant) == false)
                    {
                        //no scheme, add on supplied scheme
                        destinationUrl = scheme + Uri.SchemeDelimiter + destinationUrl;
                    }
                    //keep query string from redirect destination
                    if (redirectInfo.KeepQueryString)
                    {
                        destinationUrl = AddQueryString(destinationUrl, queryString);
                        messages.Add("URP: Redirecting " + redirectUrl + " to " + destinationUrl +
                                     " by url + qs. RuleId: " + redirectInfo.RedirectId.ToString());
                    }
                    else
                    {
                        messages.Add("URP: Redirecting " + redirectUrl + " to " + destinationUrl + " by url. RuleId: " +
                                     redirectInfo.RedirectId.ToString());
                    }
                    try
                    {
                        Uri redirectUri = new Uri(destinationUrl);
                    }
                    catch (Exception invalidEx)
                    {
                        messages.Add("Invalid redirect URL '" + redirectUrl + "'.  Message: " + invalidEx.Message);
                        throw;
                    }
                    break;
            }
            return destinationUrl;
        }

        private static string AddQueryString(string url, NameValueCollection queryString)
        {
            bool first = true;
            if (url.Contains("?"))
                first = false;
            if (queryString != null)
            {
                foreach (string key in queryString.AllKeys)
                {
                    string value = queryString[key];
                    if (value == null) value = "";
                    if (first)
                        url += "?" + key + "=" + value;
                    else
                        url += "&" + key + "=" + value;
                    first = false;
                }
            }
            return url;
        }

        private static string AddQueryString(string url, string qs)
        {
            bool first = true;
            if (url.Contains("?"))
                first = false;
            if (qs.StartsWith("?") || qs.StartsWith("&"))
                if (qs.Length > 0)
                    qs = qs.Substring(1);

            if (first)
                url += "?" + qs;
            else
                url += "&" + qs;

            return url;
        }
        internal static void SplitPathAndQuerystring(string url, out string path, out string queryString)
        {
            path = url;
            queryString = null;
            if (string.IsNullOrEmpty(path))
            {
                if (path.Contains("?"))
                {
                    string[] bits = path.Split('?');
                    if (bits.GetUpperBound(0) >=0)
                    {
                        path = bits[0];
                        queryString = bits[1];
                    }
                }
            }
        }
        
        internal static void CheckRegexRule(UrlRedirectProvider provider, RedirectInfo regexRedirect, string requestedUrl, string requestedScheme, NameValueCollection queryStringCol, out bool doRedirect, out string redirectLocation, ref List<string> messages)
        {
            doRedirect = false;
            redirectLocation = null;
            if (string.IsNullOrEmpty(regexRedirect.RedirectUrl) == false)
            {
                string pattern = regexRedirect.RedirectUrl;
                string replace = regexRedirect.DestUrl;
                Regex urlRegex = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
                if (urlRegex.IsMatch(requestedUrl))
                {
                    //matches
                    messages.Add("URP : Regex Match on " + requestedUrl + " with pattern " + pattern);
                    string replacedUrl = urlRegex.Replace(requestedUrl, replace);
                    redirectLocation = RedirectLocation(provider, regexRedirect, requestedUrl, requestedScheme, queryStringCol, ref messages);
                    doRedirect = true;
               }
            }
            
        }
    }
}
