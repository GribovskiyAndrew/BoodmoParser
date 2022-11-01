using Newtonsoft.Json.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Policy;
using System.Text;
using System.Text.Json;

namespace BoodmoParser
{
    public class RequestManager
    {
        private readonly HttpClient _httpClient;

        private Dictionary<string, string> headers = new Dictionary<string, string>();

        public int Count = 0;
        public RequestManager()
        {
            var handler = new HttpClientHandler();
            handler.UseCookies = false;

            handler.Proxy = new WebProxy()
            {
                Address = new Uri($"http://94.177.134.106:8800"),
                BypassProxyOnLocal = false,
                UseDefaultCredentials = false,

                Credentials = new NetworkCredential(userName: "162752", password: "4X8AqZaE8")
            };

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
            request.Headers.TryAddWithoutValidation("sec-ch-ua", "\"Google Chrome\";v=\"107\", \"Chromium\";v=\"107\", \"Not=A?Brand\";v=\"24\"");
            request.Headers.TryAddWithoutValidation("X-Client-Version", "4.6.0");
            request.Headers.TryAddWithoutValidation("sec-ch-ua-mobile", "?0");
            request.Headers.TryAddWithoutValidation("X-Api", "CustomerAPI");
            request.Headers.TryAddWithoutValidation("X-Client-App", "web");
            request.Headers.TryAddWithoutValidation("Authorization", "Bearer eyJhbGciOiJSUzI1NiIsImtpZCI6ImRjMzdkNTkzNjVjNjIyOGI4Y2NkYWNhNTM2MGFjMjRkMDQxNWMxZWEiLCJ0eXAiOiJKV1QifQ.eyJuYW1lIjoiSmFjayBsaWFtIiwicGljdHVyZSI6Imh0dHBzOi8vbGgzLmdvb2dsZXVzZXJjb250ZW50LmNvbS9hL0FMbTV3dTE0Vk5LTnNjbHFGV1lTaTBDY25wWG1UOHZMX2NMOGxMRHRZVlFIPXM5Ni1jIiwiY2lkIjozMTU1NTkyLCJpc3MiOiJodHRwczovL3NlY3VyZXRva2VuLmdvb2dsZS5jb20vYm9vZG1vLXRlc3QiLCJhdWQiOiJib29kbW8tdGVzdCIsImF1dGhfdGltZSI6MTY2NzI0NzAxMywidXNlcl9pZCI6IjZTMmM3S2Q0OThRNFZGN3JmNTdxWkNBdFkxVDIiLCJzdWIiOiI2UzJjN0tkNDk4UTRWRjdyZjU3cVpDQXRZMVQyIiwiaWF0IjoxNjY3MjgyMjU0LCJleHAiOjE2NjcyODU4NTQsImVtYWlsIjoiamFja2xpYW0yNTIwQGdtYWlsLmNvbSIsImVtYWlsX3ZlcmlmaWVkIjp0cnVlLCJmaXJlYmFzZSI6eyJpZGVudGl0aWVzIjp7Imdvb2dsZS5jb20iOlsiMTE4Mzc2MTkzMzg3MjUzNjg5MzM0Il0sImVtYWlsIjpbImphY2tsaWFtMjUyMEBnbWFpbC5jb20iXX0sInNpZ25faW5fcHJvdmlkZXIiOiJwYXNzd29yZCJ9fQ.MSEPteh7BriuHJ4xbT5jXNu3xjypjtAqQw-7JaA8duNEcW6HpSnHlRzRPMjNAwVKZbkRh8q1Kexf2pLLYz1rsHiTt0ow6c-LBp5ZO6eIg2FFp3LJTcCjLii7igi7h8QCpWmc_rAVD_AaqBz7hnVQXxaRLJdnBgwr2wpAxUTue5UWuzecQGLbu45gmBlidYePgeDA-CC_-1VWyi2lU2MX-KOtWgng_jElRpQcPM7C91kLy1gOgc8KxZ-xQOsRv45ivtmo94cc0j2cJyUSfO3PATB1FirG4kzy4q3oYiwbL7R3z8eILfE6OB4fo0OlYS1PJNhGFeJR98us_mmY5A8ucw");
            request.Headers.TryAddWithoutValidation("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/107.0.0.0 Safari/537.36");
            request.Headers.TryAddWithoutValidation("Accept", "application/json, text/plain, */*");
            request.Headers.TryAddWithoutValidation("Referer", "https://boodmo.com/");
            request.Headers.TryAddWithoutValidation("X-Client-Id", "55f5bfcb48e712f66cba594157d277a1");
            request.Headers.TryAddWithoutValidation("X-Boo-Sign", "8db2abd51d684e9d6e16ce5593ecbf1b");
            request.Headers.TryAddWithoutValidation("X-Date", "2022-11-01T06:24:46.659Z");
            request.Headers.TryAddWithoutValidation("Accept-Version", "v1");
            request.Headers.TryAddWithoutValidation("X-Client-Build", "221026.1443");
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
                //foreach(var header in headers)
                //    request.Headers.TryAddWithoutValidation(header.Key, header.Value);

                AddHeaders(request);

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

                //AddHeaders(request);

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
