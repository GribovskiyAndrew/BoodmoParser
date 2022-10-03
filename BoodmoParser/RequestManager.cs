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
            request.Headers.TryAddWithoutValidation("Authorization", "Bearer eyJhbGciOiJSUzI1NiIsImtpZCI6IjU4NWI5MGI1OWM2YjM2ZDNjOTBkZjBlOTEwNDQ1M2U2MmY4ODdmNzciLCJ0eXAiOiJKV1QifQ.eyJuYW1lIjoic2hhc2hpayBLaGV0YW5pIiwicGljdHVyZSI6Imh0dHBzOi8vbGgzLmdvb2dsZXVzZXJjb250ZW50LmNvbS9hL0FJdGJ2bWtGWC0yNWhTR1M0U1pvNE12UlpIWWpkelBtc1QxM1lFQkd4aElpPXM5Ni1jIiwiY2lkIjoxNzkyMTA0LCJpc3MiOiJodHRwczovL3NlY3VyZXRva2VuLmdvb2dsZS5jb20vYm9vZG1vLXRlc3QiLCJhdWQiOiJib29kbW8tdGVzdCIsImF1dGhfdGltZSI6MTY2NDM1MDcwNSwidXNlcl9pZCI6IjE3OTIxMDQiLCJzdWIiOiIxNzkyMTA0IiwiaWF0IjoxNjY0ODI4MzAwLCJleHAiOjE2NjQ4MzE5MDAsImVtYWlsIjoic2hhc2hpa2toZXRhbmlAZ21haWwuY29tIiwiZW1haWxfdmVyaWZpZWQiOnRydWUsInBob25lX251bWJlciI6Iis5MTc2MjMwMzg1NTYiLCJmaXJlYmFzZSI6eyJpZGVudGl0aWVzIjp7InBob25lIjpbIis5MTc2MjMwMzg1NTYiXSwiZW1haWwiOlsic2hhc2hpa2toZXRhbmlAZ21haWwuY29tIl19LCJzaWduX2luX3Byb3ZpZGVyIjoicGhvbmUifX0.cVF3Ftxd4T__vbOnCr7CQBJ_CO-1S-A9LuqpHxSGZqsbxD4tVN25YV54sX0HjVTA4cmF2dT-b-jCMksRm1GPXPEkgmJKqZJllRCwM4aME4Fs6oqm9_E8yzJIPgcNcTJH2FCWD78_EnrTN81FM308sHjKXkh_mQRMm_Q8YnYJRVm9mjSuiwxEaW1BmAxuB7IKNeE-Jd2bkUxidvDrQLpc3V0E0WsbP0j1hN4NHxj75OKlXk48MzsAGJziSO5SRakfRQmXgeO9gW5cQANtRW0gFdUz5QKzqOTXIF6_iR42xuwr5erTdVUaTdrKNdJFf5IPtoEkS-uXA9PDGvbn8L_LGA");
            request.Headers.TryAddWithoutValidation("X-Client-App", "web");
            request.Headers.TryAddWithoutValidation("Accept", "application/json, text/plain, */*");
            request.Headers.TryAddWithoutValidation("Referer", "https://boodmo.com/");
            request.Headers.TryAddWithoutValidation("X-Client-Id", "e3d9bcb2915ce83b40a3a90724e12b0b");
            request.Headers.TryAddWithoutValidation("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/105.0.0.0 Safari/537.36");
            request.Headers.TryAddWithoutValidation("X-Date", "2022-10-03T20:24:21.217Z");
            request.Headers.TryAddWithoutValidation("Accept-Version", "v2");
            request.Headers.TryAddWithoutValidation("X-Client-Build", "220928.1448");
            request.Headers.TryAddWithoutValidation("X-Boo-Sign", "72e12568422d1dc6a1bd4d8ca5cf1151");
            request.Headers.TryAddWithoutValidation("sec-ch-ua-platform", "\"Windows\"");
        }

        public async Task<JObject> Get(string url)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, url))
            {
                AddHeaders(request);

                var response = await _httpClient.SendAsync(request);

                return await GetResponse(response);
            }
        }

        public async Task<JObject> Post(string url, object data)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Post, url))
            {
                AddHeaders(request);

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
