using Newtonsoft.Json;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MediaRadar.API.SDK
{
    public static class RequestMethod
    {
        public const string Get = "GET";
        public const string Put = "PUT";
        public const string Post = "POST";
        public const string Delete = "DELETE";

        public const string per_page = "per_page";
        public const string sort_by = "sort_by";
        public const string sort_order = "sort_order";
        public const string page = "page";
    }

    public interface ICore
    {
        T GetByPageUrl<T>(string pageUrl, int perPage = 100);
        T RunRequest<T>(string resource, string requestMethod, object body = null, int? timeout = null, string formFile = null);
        RequestResult RunRequest(string resource, string requestMethod, object body = null, int? timeout = null, string formFile = null);
        Task<T> GetByPageUrlAsync<T>(string pageUrl, int perPage = 100);
        Task<T> RunRequestAsync<T>(string resource, string requestMethod, object body = null, int? timeout = null, string formFile = null);
        Task<RequestResult> RunRequestAsync(string resource, string requestMethod, object body = null, int? timeout = null, string formFile = null);
    }

    public class Core : ICore
    {
        private string ocp_SubscriptionKey;
        private string serviceUrl;
        private IWebProxy Proxy;
        private JsonSerializerSettings jsonSettings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            DateParseHandling = DateParseHandling.DateTimeOffset,
            DateFormatHandling = DateFormatHandling.IsoDateFormat,
            DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate,
            ContractResolver = Serialization.MediaRadarContractResolver.Instance

        };

        public Core(string serviceUrl, string ocp_SubscriptionKey)
            : this(serviceUrl, ocp_SubscriptionKey, null)
        {

        }
        public Core(string serviceUrl, string ocp_SubscriptionKey, IWebProxy proxy)
        {
            this.serviceUrl = serviceUrl;
            this.ocp_SubscriptionKey = ocp_SubscriptionKey;
            Proxy = proxy;
        }

        public T GetByPageUrl<T>(string pageUrl, int perPage = 100)
        {
            if (string.IsNullOrEmpty(pageUrl))
            {
                return JsonConvert.DeserializeObject<T>("");
            }

            var resource = pageUrl + "&" + RequestMethod.per_page + "=" + perPage;
            return RunRequest<T>(resource, RequestMethod.Get);
        }

        public RequestResult RunRequest(string resource, string requestMethod,
            object body = null, int? timeout = null, string formKey = null)
        {
            try
            {
                string requestUrl = serviceUrl + resource;

                HttpWebRequest req = WebRequest.Create(requestUrl) as HttpWebRequest;

                if (Proxy != null)
                {
                    req.Proxy = Proxy;
                }

                req.Headers["Ocp-Apim-Subscription-Key"] = GetApimSubscriptionKey();
                req.PreAuthenticate = true;
                req.Method = requestMethod;
                req.Accept = "application/json, application/xml, text/json, text/x-json, text/javascript, text/xml";
                req.Timeout = timeout ?? req.Timeout;

                if (body != null)
                {
                    byte[] data = null;
                    req.ContentType = "application/json";
                    data = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(body, jsonSettings));
                    req.ContentLength = data.Length;
                    using (var dataStream = req.GetRequestStream())
                    {
                        dataStream.Write(data, 0, data.Length);
                    }
                }
                var res = req.GetResponse();
                var response = res as HttpWebResponse;
                string responseFromServer = string.Empty;
                using (var responseStream = response.GetResponseStream())
                {
                    using (var reader = new StreamReader(responseStream))
                    {
                        responseFromServer = reader.ReadToEnd();
                    }
                }
                return new RequestResult
                {
                    Content = responseFromServer,
                    HttpStatusCode = response.StatusCode
                };
            }
            catch (WebException ex)
            {
                WebException wException = GetWebException(resource, body, ex);
                throw wException;
            }
        }

        public T RunRequest<T>(string resource, string requestMethod, object body = null, int? timeout = null, string formKey = null)
        {
            var response = RunRequest(resource, requestMethod, body, timeout, formKey);
            var obj = JsonConvert.DeserializeObject<T>(response.Content, jsonSettings);
            return obj;
        }

        protected T GenericGet<T>(string resource)
        {
            return RunRequest<T>(resource, RequestMethod.Get);
        }

        protected T GenericPagedGet<T>(string resource, int? perPage = null, int? page = null)
        {
            var parameters = new Dictionary<string, string>();
            var paramString = "";
            if (perPage.HasValue)
            {
                parameters.Add(RequestMethod.per_page, perPage.Value.ToString(CultureInfo.InvariantCulture));
            }

            if (page.HasValue)
            {
                parameters.Add(RequestMethod.page, page.Value.ToString(CultureInfo.InvariantCulture));
            }

            if (parameters.Any())
            {
                paramString = (resource.Contains("?") ? "&" : "?") + string.Join("&", parameters.Select(x => x.Key + "=" + x.Value).ToArray());
            }

            return GenericGet<T>(resource + paramString);
        }

        protected T GenericPagedSortedGet<T>(string resource, int? perPage = null, int? page = null, string sortCol = null, bool? sortAscending = null)
        {
            var parameters = new Dictionary<string, string>();

            var paramString = "";
            if (perPage.HasValue)
            {
                parameters.Add(RequestMethod.per_page, perPage.Value.ToString(CultureInfo.InvariantCulture));
            }

            if (page.HasValue)
            {
                parameters.Add(RequestMethod.page, page.Value.ToString(CultureInfo.InvariantCulture));
            }

            if (!string.IsNullOrEmpty(sortCol))
            {
                parameters.Add(RequestMethod.sort_by, sortCol);
            }

            if (sortAscending.HasValue)
            {
                parameters.Add(RequestMethod.sort_order, sortAscending.Value ? "" : "desc");
            }

            if (parameters.Any())
            {
                paramString = (resource.Contains("?") ? "&" : "?") + string.Join("&", parameters.Select(x => x.Key + "=" + x.Value).ToArray());
            }
            return GenericGet<T>(resource + paramString);
        }

        protected bool GenericDelete(string resource)
        {
            var res = RunRequest(resource, RequestMethod.Delete);
            return res.HttpStatusCode == HttpStatusCode.OK || res.HttpStatusCode == HttpStatusCode.NoContent;
        }

        protected T GenericDelete<T>(string resource)
        {
            var res = RunRequest<T>(resource, RequestMethod.Delete);
            return res;
        }

        protected T GenericPost<T>(string resource, object body = null)
        {
            var res = RunRequest<T>(resource, RequestMethod.Post, body);
            return res;
        }

        protected bool GenericBoolPost(string resource, object body = null)
        {
            var res = RunRequest(resource, RequestMethod.Post, body);
            return res.HttpStatusCode == HttpStatusCode.OK;
        }

        protected T GenericPut<T>(string resource, object body = null, string formKey = null)
        {
            var res = RunRequest<T>(resource, RequestMethod.Put, body, formKey: formKey);
            return res;
        }

        protected bool GenericBoolPut(string resource, object body = null)
        {
            var res = RunRequest(resource, RequestMethod.Put, body);
            return res.HttpStatusCode == HttpStatusCode.OK;
        }

        protected string GetApimSubscriptionKey()
        {
            return ocp_SubscriptionKey;
        }

        public async Task<T> GetByPageUrlAsync<T>(string pageUrl, int perPage = 100)
        {
            if (string.IsNullOrEmpty(pageUrl))
            {
                return JsonConvert.DeserializeObject<T>("");
            }

            var resource = pageUrl + "&" + RequestMethod.per_page + "=" + perPage;
            return await RunRequestAsync<T>(resource, RequestMethod.Get);
        }

        public async Task<T> RunRequestAsync<T>(string resource, string requestMethod, object body = null, int? timeout = null, string formKey = null)
        {
            var response = await RunRequestAsync(resource, requestMethod, body, timeout, formKey);
            var obj = Task.Factory.StartNew(() => JsonConvert.DeserializeObject<T>(response.Content));
            return await obj;
        }

        public async Task<RequestResult> RunRequestAsync(string resource, string requestMethod,
            object body = null, int? timeout = null, string formKey = null)
        {
            var requestUrl = serviceUrl + resource;
            try
            {
                HttpWebRequest req = WebRequest.Create(requestUrl) as HttpWebRequest;
                req.ContentType = "application/json";
                req.Headers["Ocp-Apim-Subscription-Key"] = GetApimSubscriptionKey();
                req.Method = requestMethod;
                req.Accept = "application/json, application/xml, text/json, text/x-json, text/javascript, text/xml";

                if (body != null)
                {
                    byte[] data = null;
                    string json = JsonConvert.SerializeObject(body, jsonSettings);
                    data = Encoding.UTF8.GetBytes(json);
                    using (Stream requestStream = await req.GetRequestStreamAsync())
                    {
                        await requestStream.WriteAsync(data, 0, data.Length);
                    }
                }

                using (HttpWebResponse response = (HttpWebResponse)await req.GetResponseAsync())
                {
                    string content = string.Empty;
                    using (Stream responseStream = response.GetResponseStream())
                    {
                        using (StreamReader sr = new StreamReader(responseStream))
                        {
                            content = await sr.ReadToEndAsync();
                        }
                    }
                    return new RequestResult { HttpStatusCode = response.StatusCode, Content = content };
                }
            }
            catch (WebException ex)
            {
                WebException wException = GetWebException(resource, body, ex);
                throw wException;
            }
        }

        protected async Task<T> GenericGetAsync<T>(string resource)
        {
            return await RunRequestAsync<T>(resource, RequestMethod.Get);
        }

        protected async Task<T> GenericPagedGetAsync<T>(string resource, int? perPage = null, int? page = null)
        {
            var parameters = new Dictionary<string, string>();

            var paramString = "";
            if (perPage.HasValue)
            {
                parameters.Add(RequestMethod.per_page, perPage.Value.ToString(CultureInfo.InvariantCulture));
            }

            if (page.HasValue)
            {
                parameters.Add(RequestMethod.page, page.Value.ToString(CultureInfo.InvariantCulture));
            }

            if (parameters.Any())
            {
                paramString = (resource.Contains("?") ? "&" : "?") +
                    string.Join("&", parameters.Select(x => x.Key + "=" + x.Value));
            }


            return await GenericGetAsync<T>(resource + paramString);
        }

        protected async Task<T> GenericPagedSortedGetAsync<T>(string resource, int? perPage = null, int? page = null, string sortCol = null, bool? sortAscending = null)
        {
            var parameters = new Dictionary<string, string>();

            var paramString = "";
            if (perPage.HasValue)
            {
                parameters.Add(RequestMethod.per_page, perPage.Value.ToString(CultureInfo.InvariantCulture));
            }

            if (page.HasValue)
            {
                parameters.Add("page", page.Value.ToString(CultureInfo.InvariantCulture));
            }

            if (!string.IsNullOrEmpty(sortCol))
            {
                parameters.Add(RequestMethod.sort_by, sortCol);
            }

            if (sortAscending.HasValue)
            {
                parameters.Add(RequestMethod.sort_order, sortAscending.Value ? "" : "desc");
            }

            if (parameters.Any())
            {
                paramString = (resource.Contains("?") ? "&" : "?") + string.Join("&", parameters.Select(x => x.Key + "=" + x.Value));
            }


            return await GenericGetAsync<T>(resource + paramString);
        }

        protected async Task<bool> GenericDeleteAsync(string resource)
        {
            var res = RunRequestAsync(resource, RequestMethod.Delete);
            return await res.ContinueWith(x => x.Result.HttpStatusCode == HttpStatusCode.OK || x.Result.HttpStatusCode == HttpStatusCode.NoContent);
        }

        protected async Task<T> GenericDeleteAsync<T>(string resource)
        {
            var res = RunRequestAsync<T>(resource, RequestMethod.Delete);
            return await res;
        }

        protected async Task<T> GenericPostAsync<T>(string resource, object body = null)
        {
            var res = RunRequestAsync<T>(resource, RequestMethod.Post, body);
            return await res;
        }

        protected async Task<bool> GenericBoolPostAsync(string resource, object body = null)
        {
            var res = RunRequestAsync(resource, RequestMethod.Post, body);
            return await res.ContinueWith(x => x.Result.HttpStatusCode == HttpStatusCode.OK);
        }

        protected async Task<T> GenericPutAsync<T>(string resource, object body = null, string formKey = null)
        {
            var res = RunRequestAsync<T>(resource, RequestMethod.Put, body, formKey: formKey);
            return await res;
        }

        protected async Task<bool> GenericBoolPutAsync(string resource, object body = null)
        {
            var res = RunRequestAsync(resource, RequestMethod.Put, body);
            return await res.ContinueWith(x => x.Result.HttpStatusCode == HttpStatusCode.OK);
        }

        private WebException GetWebException(string resource, object body, WebException originalWebException)
        {
            string error = string.Empty;
            WebException innerException = originalWebException.InnerException as WebException;

            if (originalWebException.Response != null || (innerException != null && innerException.Response != null))
            {
                using (Stream stream = (originalWebException.Response ?? innerException.Response).GetResponseStream())
                {
                    if (stream != null && stream.CanRead)
                    {
                        using (var sr = new StreamReader(stream))
                        {
                            error = sr.ReadToEnd();
                        }
                    }
                    else
                    {
                        error = "Cannot read error stream.";
                    }
                }
            }
            Debug.WriteLine(originalWebException.Message);
            Debug.WriteLine(error);

            string headersMessage = string.Format("Error content: {0} \r\n Resource String: {1}  + \r\n", error, resource);
            string bodyMessage = string.Empty;

            if (body != null)
            {
                bodyMessage = string.Format(" Body: {0}", JsonConvert.SerializeObject(body, Formatting.Indented, jsonSettings));
            }

            headersMessage += bodyMessage;

            if (originalWebException.Response != null && originalWebException.Response.Headers != null)
            {
                headersMessage += originalWebException.Response.Headers;
            }

            var wException = new WebException(originalWebException.Message + headersMessage, originalWebException, originalWebException.Status, originalWebException.Response);
            wException.Data.Add("jsonException", error);

            return wException;
        }
    }
}