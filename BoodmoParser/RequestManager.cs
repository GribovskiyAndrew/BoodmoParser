using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace BoodmoParser
{
    public class RequestManager
    {
        private readonly HttpClient _httpClient;

        public RequestManager()
        {
            var handler = new HttpClientHandler();
            handler.UseCookies = false;

            handler.AutomaticDecompression = ~DecompressionMethods.None;

            _httpClient = new HttpClient(handler);
        }

        public async Task SavePartImage(string name)
        {
            if (string.IsNullOrEmpty(name))
                return;

            string url = "https://snaponepc.com/epc-services/datasets/dec9913a-c05a-5535-e043-60d416acaf35/pages/images/" + name;

            string file = "C:/Users/hi/source/repos/SnapParser/PartImages/" + name + ".png";

            await SaveImage(url, file);
        }

        public async Task SaveImage(string name)
        {
            if (string.IsNullOrEmpty(name))
                return;

            string file = "C:/Users/hi/source/repos/SnapParser/Images/" + name + ".png";

            string url = "https://snaponepc.com/epc-services/datasets/dec9913a-c05a-5535-e043-60d416acaf35/" +
                "navigations/eJxzCg329HMNDo4PcnX39PeLzrQ1sTAwNJbLsC0pKk2NlfX1d3H1iQ8MdfTxdPN0DQLKG5nLZdv6Zj" +
                "lW-gbK5cEYObapebqhwVD1IFOAigJD_VyCQk1DfQzcXAINwvzDnJ2cQ9zC3HwMo9xCs02dfAydAkJysoGmEKsSZg0ASos17w/thumbnails/"
                + name;

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
            request.Headers.TryAddWithoutValidation("X-Client-Version", "4.3.1");
            request.Headers.TryAddWithoutValidation("sec-ch-ua-mobile", "?0");
            request.Headers.TryAddWithoutValidation("X-Api", "CustomerAPI");
            request.Headers.TryAddWithoutValidation("Authorization", "Bearer eyJhbGciOiJSUzI1NiIsImtpZCI6IjJkMjNmMzc0MDI1ZWQzNTNmOTg0YjUxMWE3Y2NlNDlhMzFkMzFiZDIiLCJ0eXAiOiJKV1QifQ.eyJuYW1lIjoic2hhc2hpayBLaGV0YW5pIiwicGljdHVyZSI6Imh0dHBzOi8vbGgzLmdvb2dsZXVzZXJjb250ZW50LmNvbS9hL0FJdGJ2bWtGWC0yNWhTR1M0U1pvNE12UlpIWWpkelBtc1QxM1lFQkd4aElpPXM5Ni1jIiwiY2lkIjoxNzkyMTA0LCJpc3MiOiJodHRwczovL3NlY3VyZXRva2VuLmdvb2dsZS5jb20vYm9vZG1vLXRlc3QiLCJhdWQiOiJib29kbW8tdGVzdCIsImF1dGhfdGltZSI6MTY2MzA3OTM2NSwidXNlcl9pZCI6IjE3OTIxMDQiLCJzdWIiOiIxNzkyMTA0IiwiaWF0IjoxNjYzMTA0MTE1LCJleHAiOjE2NjMxMDc3MTUsImVtYWlsIjoic2hhc2hpa2toZXRhbmlAZ21haWwuY29tIiwiZW1haWxfdmVyaWZpZWQiOnRydWUsInBob25lX251bWJlciI6Iis5MTc2MjMwMzg1NTYiLCJmaXJlYmFzZSI6eyJpZGVudGl0aWVzIjp7InBob25lIjpbIis5MTc2MjMwMzg1NTYiXSwiZW1haWwiOlsic2hhc2hpa2toZXRhbmlAZ21haWwuY29tIl19LCJzaWduX2luX3Byb3ZpZGVyIjoicGhvbmUifX0.B6dOOkoigQa3v20h-aYY7rhBXXGA0C5MNUAJeVvJJ-LmFHvfb85khTaOxP4MUgti5L4D2XETYsoJvBj1jSD834D-pez2-e4JKWT2QT753X89yHb-42Kig1idj_IKh6Lq2WePVhLoCh2k3J5KteyBnpkyBPJxJHuXXnqQ5pSRzawrPXE99m-Qd9zYaOPvWKZens7ywVT3RooxHy2MGMc6gTRuXOIkqVqyN33dMbJrt7kq0H3eq-fQ67Dn2A0SmnBP7hisVJNIpgveBseGEY7_krRFESrHNLzM-_USZXNsLExYCaD0DyAkmFmvhB11mpAg7WgNUdzEHOojpSyNwbnJdw");
            request.Headers.TryAddWithoutValidation("X-Client-App", "web");
            request.Headers.TryAddWithoutValidation("Accept", "application/json, text/plain, */*");
            request.Headers.TryAddWithoutValidation("Referer", "https://boodmo.com/");
            request.Headers.TryAddWithoutValidation("X-Client-Id", "e3d9bcb2915ce83b40a3a90724e12b0b");
            request.Headers.TryAddWithoutValidation("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/105.0.0.0 Safari/537.36");
            request.Headers.TryAddWithoutValidation("X-Date", "2022-09-13T21:33:51.140Z");
            request.Headers.TryAddWithoutValidation("Accept-Version", "v2");
            request.Headers.TryAddWithoutValidation("X-Client-Build", "220912.1615");
            request.Headers.TryAddWithoutValidation("X-Boo-Sign", "9caf157b62465a8f8784802c39dba43a");
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
