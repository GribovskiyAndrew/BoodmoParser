using Newtonsoft.Json.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace BoodmoParser
{
    public class RequestManager
    {
        private readonly HttpClient _httpClient;

        private Dictionary<string, string> headers = new Dictionary<string, string>();

        public RequestManager()
        {
            var handler = new HttpClientHandler();
            handler.UseCookies = false;

            //handler.Proxy = new WebProxy()
            //{
            //    Address = new Uri($"http://104.144.108.112:8800"),
            //    BypassProxyOnLocal = false,
            //    UseDefaultCredentials = false,

            //    Credentials = new NetworkCredential(userName: "161891", password: "9s2bvJEmMaN")
            //};

            handler.AutomaticDecompression = ~DecompressionMethods.None;

            _httpClient = new HttpClient(handler);

            headers.Add("sec-ch-ua", "\"Google Chrome\";v=\"105\", \"Not)A;Brand\";v=\"8\", \"Chromium\";v=\"105\"");
            headers.Add("X-Client-Version", "4.4.1");
            headers.Add("sec-ch-ua-mobile", "?0");
            headers.Add("X-Api", "CustomerAPI");headers.Add("X-Client-App", "web");
            headers.Add("Accept", "application/json, text/plain, */*");
            headers.Add("Referer", "https://boodmo.com/");
            headers.Add("X-Client-Id", "e3d9bcb2915ce83b40a3a90724e12b0b");
            headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/105.0.0.0 Safari/537.36");
            headers.Add("X-Date", "2022-10-04T07:44:50.006Z");
            headers.Add("Accept-Version", "v1");
            headers.Add("X-Client-Build", "220928.1448");
            headers.Add("X-Boo-Sign", "6c83e42effccd58531ebffd1ee9bdb15");
            headers.Add("sec-ch-ua-platform", "\"Windows\"");
        }

        public async Task SaveImage(string name)
        {
            if (string.IsNullOrEmpty(name))
                return;

            string file = "C:/Users/lifebookE/source/repos/BoodmoParser/BoodmoParser/Images/" + "maruti/" + name.Substring(name.LastIndexOf('/') + 1);

            string url = "https://boodmo.com/media/cache/part_zoom_horizontal" + name;

            await SaveImage(url, file);
        }

        private async Task SaveImage(string url, string path)
        {
            if (string.IsNullOrEmpty(path))
                return;

            if (string.IsNullOrEmpty(url))
                return;

            if (File.Exists(path))
                return;

            using (var request = new HttpRequestMessage(HttpMethod.Get, url))
            {
                AddHeaders(request);

                var response = await _httpClient.SendAsync(request);

                var bytes = await response.Content.ReadAsByteArrayAsync();

                await File.WriteAllBytesAsync(path, bytes);
            }
        }

        private void AddHeaders(HttpRequestMessage request)
        {
            request.Headers.TryAddWithoutValidation("sec-ch-ua", "\"Google Chrome\";v=\"105\", \"Not)A;Brand\";v=\"8\", \"Chromium\";v=\"105\"");
            request.Headers.TryAddWithoutValidation("X-Client-Version", "4.4.1");
            request.Headers.TryAddWithoutValidation("sec-ch-ua-mobile", "?0");
            request.Headers.TryAddWithoutValidation("X-Api", "CustomerAPI");
            request.Headers.TryAddWithoutValidation("Authorization", "Bearer eyJhbGciOiJSUzI1NiIsImtpZCI6IjU4NWI5MGI1OWM2YjM2ZDNjOTBkZjBlOTEwNDQ1M2U2MmY4ODdmNzciLCJ0eXAiOiJKV1QifQ.eyJuYW1lIjoic2hhc2hpayBLaGV0YW5pIiwicGljdHVyZSI6Imh0dHBzOi8vbGgzLmdvb2dsZXVzZXJjb250ZW50LmNvbS9hL0FJdGJ2bWtGWC0yNWhTR1M0U1pvNE12UlpIWWpkelBtc1QxM1lFQkd4aElpPXM5Ni1jIiwiY2lkIjoxNzkyMTA0LCJpc3MiOiJodHRwczovL3NlY3VyZXRva2VuLmdvb2dsZS5jb20vYm9vZG1vLXRlc3QiLCJhdWQiOiJib29kbW8tdGVzdCIsImF1dGhfdGltZSI6MTY2NDM1MDcwNSwidXNlcl9pZCI6IjE3OTIxMDQiLCJzdWIiOiIxNzkyMTA0IiwiaWF0IjoxNjY0ODY4NzI1LCJleHAiOjE2NjQ4NzIzMjUsImVtYWlsIjoic2hhc2hpa2toZXRhbmlAZ21haWwuY29tIiwiZW1haWxfdmVyaWZpZWQiOnRydWUsInBob25lX251bWJlciI6Iis5MTc2MjMwMzg1NTYiLCJmaXJlYmFzZSI6eyJpZGVudGl0aWVzIjp7InBob25lIjpbIis5MTc2MjMwMzg1NTYiXSwiZW1haWwiOlsic2hhc2hpa2toZXRhbmlAZ21haWwuY29tIl19LCJzaWduX2luX3Byb3ZpZGVyIjoicGhvbmUifX0.FXBFHXs_TFGMp4vI2tzMgqYuFqgWhyEdQMwhhmGWJCXPMAIezQ7NxKVn3yUXpbg4L_OZVnJqkwcv75UNkI-DF2kPH_EKUlC9fNko1iThg9jnfvizC0kzoZUIR92bK8SfeXkKWmPQfhrqVGMlQeHCMOrgXFSUtU6SJiOrBI-3yr2FKUMxQpWacGgbdyKNHbQhEFrwG-JE5scZjFi3E5bZooI7Ed3m8YNNRx7o3h0WR5pBdBl7ZU3yEETEa-e5T12SkFxCT2XflkETQ0DPHXyTaNQrIzqJv-aNbErnaIlG8Z8c-mELlVvkuS7VEmkbWqSogabUYF_Lotfy1vwM3LB4Ow");
            request.Headers.TryAddWithoutValidation("X-Client-App", "web");
            request.Headers.TryAddWithoutValidation("Accept", "application/json, text/plain, */*");
            request.Headers.TryAddWithoutValidation("Referer", "https://boodmo.com/");
            request.Headers.TryAddWithoutValidation("X-Client-Id", "e3d9bcb2915ce83b40a3a90724e12b0b");
            request.Headers.TryAddWithoutValidation("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/105.0.0.0 Safari/537.36");
            request.Headers.TryAddWithoutValidation("X-Date", "2022-10-04T07:44:50.006Z");
            request.Headers.TryAddWithoutValidation("Accept-Version", "v1");
            request.Headers.TryAddWithoutValidation("X-Client-Build", "220928.1448");
            request.Headers.TryAddWithoutValidation("X-Boo-Sign", "6c83e42effccd58531ebffd1ee9bdb15");
            request.Headers.TryAddWithoutValidation("sec-ch-ua-platform", "\"Windows\"");
        }

        public void AddHeaders(Dictionary<string, string> _headers)
        {
            headers = _headers;
        }

        public async Task<JObject> Get(string url)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, url))
            {
                foreach(var header in headers)
                    request.Headers.TryAddWithoutValidation(header.Key, header.Value);
                
                var response = await _httpClient.SendAsync(request);

                return await GetResponse(response);
            }
        }

        public async Task<JObject> Post(string url, object data)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Post, url))
            {
                foreach (var header in headers)
                    request.Headers.TryAddWithoutValidation(header.Key, header.Value);

                var content = JsonSerializer.Serialize(data);
                var buffer = Encoding.UTF8.GetBytes(content);
                var byteContent = new ByteArrayContent(buffer);
                byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                request.Content = byteContent;

                var response = await _httpClient.SendAsync(request);

                return await GetResponse(response);
            }
        }

        public async Task<JObject> GetResponse(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
            {
                var responseText = await response.Content.ReadAsStringAsync();

                JObject result = JObject.Parse(responseText);

                return result;
            }
            else
                return null;
        }
    }
}
