using Database.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BoodmoParser.Database.Parsers
{
    public abstract class Parser
    {

        protected const string _path = "";

        protected readonly ApplicationContext _context;

        public static HttpClient HttpClient = GetClient();

        protected readonly object _lock = new object();

        public static HttpClient GetClient()
        {
            HttpClientHandler handler = new HttpClientHandler();
            handler.UseCookies = true;

            HttpClient client = new HttpClient(handler);

            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Host = "tgpecatsib.tatamotors.com";
            client.DefaultRequestHeaders.Referrer = new Uri("https://tgpecatsib.tatamotors.com/iPromisPrd/cgi/ipr_main_frame_data.cgi?SESSION_ID=d23fe8f76705d7dbefc8858862d17d99&Code_id=MM01");
            client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/99.0.4844.82 Safari/537.36");
            return client;
        }

        public Parser(ApplicationContext applicationContext)
        {
            _context = applicationContext;
        }

        public abstract Task Parse();

        protected async Task DownloadImg(string url, string name, string folder, HttpClient httpClient = null)
        {
            if (File.Exists(@$"{_path}/{folder}/" + name))
                return;

            if (httpClient is null)
            {
                httpClient = HttpClient;
            }

            byte[] buffer = null;
            try
            {
                HttpResponseMessage task = await httpClient.GetAsync(url);
                Stream task2 = await task.Content.ReadAsStreamAsync();
                using (MemoryStream ms = new MemoryStream())
                {
                    await task2.CopyToAsync(ms);
                    buffer = ms.ToArray();
                }
                await File.WriteAllBytesAsync(@$"{_path}/{folder}/" + name, buffer);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        protected async Task Save(BaseEntity baseEntity)
        {
            baseEntity.Done = true;

            await _context.SaveChangesAsync();
        }
    }
}
