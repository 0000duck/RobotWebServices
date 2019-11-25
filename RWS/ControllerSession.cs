﻿using Newtonsoft.Json;
using RWS.Data;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using RWS.RobotWareServices;
using RWS.UserServices;
using RWS.SubscriptionServices;
using System.Globalization;
using System.Threading.Tasks;
using System.Net.Http;
using System.Linq;
using static RWS.Enums;

namespace RWS
{
    public partial class ControllerSession
    {

        const string templateUri = "{0}/{1}";
        public string IP { get; private set; }
        public UAS UAS { get; private set; }
        public CookieContainer CookieContainer { get; set; } = new CookieContainer();
        public ControllerService ControllerService { get; set; }
        public RobotWareService RobotWareService { get; set; }
        public FileService FileService { get; set; }
        public UserService UserService { get; set; }
        public SubscriptionService SubscriptionService { get; set; }
        public ControllerSession(string controllerIP, [Optional]UAS uas)
        {
            IP = controllerIP;

            UAS = uas ?? new UAS("Default User", "robotics");


            ControllerService = new ControllerService(this);
            RobotWareService = new RobotWareService(this);
            FileService = new FileService(this);
            SubscriptionService = new SubscriptionService(this);
            UserService = new UserService(this);
        }

        public void Connect(string ip, UAS uas)
        {
            IP = ip;
            UAS = uas;
        }

        public async Task<BaseResponse<T>> CallAsync<T>(RequestMethod requestMethod, string domain, Tuple<string, string>[] dataParameters, Tuple<string, string>[] urlParameters, params Tuple<string, string>[] headers)
        {
            Uri uri = BuildUri(domain, urlParameters);

            HttpResponseMessage response;
            var method1 = new HttpMethod(requestMethod.ToString());
            using (var handler = new HttpClientHandler()
            {
                Credentials = new NetworkCredential(UAS.User, UAS.Password),
                CookieContainer = CookieContainer,
            })
            using (var client = new HttpClient(handler))
            using (var requestMessage = new HttpRequestMessage(method1, uri))
            {
                requestMessage.Headers.Accept.ParseAdd("application/x-www-form-urlencoded");

                foreach (var header in headers)
                {
                    requestMessage.Headers.Add(header.Item1, header.Item2);
                }

                switch (requestMethod)
                {
                    case RequestMethod.GET:
                        break;
                    default:
                        //var base64authorization = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{UAS.User}:{UAS.Password}"));
                        //requestMessage.Headers.TryAddWithoutValidation("Authorization", $"Basic {base64authorization}");
                        requestMessage.Content = new StringContent(BuildDataParameters(dataParameters));

                        break;
                }


                response = await client.SendAsync(requestMessage).ConfigureAwait(false);
                response.EnsureSuccessStatusCode();
            }

            return await DeserializeJsonResponse<T>(response).ConfigureAwait(false);
        }

        private static async Task<BaseResponse<T>> DeserializeJsonResponse<T>(HttpResponseMessage resp1)
        {
            using (var sr = new StreamReader(await resp1.Content.ReadAsStreamAsync().ConfigureAwait(false)))
            {
                var content = sr.ReadToEnd();
                BaseResponse<T> jsonResponse = default;
                jsonResponse = JsonConvert.DeserializeObject<BaseResponse<T>>(content);
                return jsonResponse;
            }
        }

        private static string BuildDataParameters(Tuple<string, string>[] dataParameters)
        {
            StringBuilder combinedParams = new StringBuilder();

            foreach (var param in dataParameters)
            {
                combinedParams.Append((param.Item1 == dataParameters[0].Item1 ? "" : "&") + param.Item1 + "=" + param.Item2);
            }

            return combinedParams.ToString();
        }

        private Uri BuildUri(string domain, Tuple<string, string>[] urlParameters)
        {
            var uri = string.Format(CultureInfo.InvariantCulture, templateUri, "http://" + IP, domain);

            if (uri.EndsWith("/", StringComparison.InvariantCulture)) uri = uri.TrimEnd('/');

            StringBuilder extraParameters = new StringBuilder();

            foreach (var param in urlParameters)
            {
                extraParameters.Append((extraParameters.Length == 0 ? "?" : "&") + param.Item1 + "=" + param.Item2);
            }

            if (extraParameters.Length > 0)
            {
                uri += extraParameters.ToString();
            }

            Debug.WriteLine(uri);

            return new Uri(uri);
        }

    }

    public class UAS
    {
        public string User { get; set; }
        public string Password { get; set; }

        public UAS(string user, string password)
        {
            User = user;
            Password = password;
        }
    }



}
