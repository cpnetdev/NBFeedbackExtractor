using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Cache;
using System.Text;
//using System.Threading.Tasks;

namespace NBFeedbackExtractor
{
    /// <summary>
    /// A Cookie-aware WebClient that will store authentication cookie information and persist it through subsequent requests.
    /// </summary>
    public class CookieAwareWebClient : WebClient
    {
        //Properties to handle implementing a timeout
        private int? _timeout = null;
        public int? Timeout
        {
            get
            {
                return _timeout;
            }
            set
            {
                _timeout = value;
            }
        }

        //A CookieContainer class to house the Cookie once it is contained within one of the Requests
        public CookieContainer CookieContainer { get; private set; }

        //Constructor
        public CookieAwareWebClient()
        {
            CookieContainer = new CookieContainer();
        }

        //Method to handle setting the optional timeout (in milliseconds)
        public void SetTimeout(int timeout)
        {
            _timeout = timeout;
        }

        //This handles using and storing the Cookie information as well as managing the Request timeout
        protected override WebRequest GetWebRequest(Uri address)
        {
            //Handles the CookieContainer
            var request = (HttpWebRequest)base.GetWebRequest(address);
            request.CookieContainer = CookieContainer;
            request.UserAgent = @"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/62.0.3202.94 Safari/537.36";
            request.Headers.Add(HttpRequestHeader.AcceptLanguage, "en-US,en;q=0.9");
            //request.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip, deflate, br");
            request.Accept = @"text / html,application / xhtml + xml,application / xml; q = 0.9,image / webp,image / apng,*/*;q=0.8";
            request.CachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.Default);
            //Sets the Timeout if it exists
            if (_timeout.HasValue)
            {
                request.Timeout = _timeout.Value;
            }
            return request;
        }

        public void BugFix_CookieDomain(CookieContainer cookieContainer)
        {
            System.Type _ContainerType = typeof(CookieContainer);
            Hashtable table = (Hashtable)_ContainerType.InvokeMember("m_domainTable",
                                       System.Reflection.BindingFlags.NonPublic |
                                       System.Reflection.BindingFlags.GetField |
                                       System.Reflection.BindingFlags.Instance,
                                       null,
                                       cookieContainer,
                                       new object[] { });
            ArrayList keys = new ArrayList(table.Keys);
            foreach (string keyObj in keys)
            {
                string key = (keyObj as string);
                if (key[0] == '.')
                {
                    string newKey = key.Remove(0, 1);
                    table[newKey] = table[keyObj];
                }
            }
        }
    }
}
