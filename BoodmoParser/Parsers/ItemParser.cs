using BoodmoParser.Entities;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Collections.Concurrent;

namespace BoodmoParser.Parsers
{
    public class ItemParser : BaseParser, IParser
    {
        private string brand = "VAG (VW, AUDI, SKODA)";

        private readonly ConcurrentQueue<dynamic> _queue;

        private static object locker = new object();

        public ItemParser(RequestManager requestManager, ApplicationContext context) : base(requestManager, context)
        {
            _queue = new ConcurrentQueue<dynamic>(
                _context.Numbers
                .Where(x => x.Id > 18240)
                .Where(x => !x.Done)
                .Select(x => new { x.Name, x.Id })
                .ToList());
        }

        public async Task Run()
        {

            Dictionary<string, string> _headers;

            async Task Parse()
            {
                while (!_queue.IsEmpty)
                {
                    var ok = _queue.TryDequeue(out dynamic part);

                    if (ok)
                    {
                        if (part.Name.Length < 7)
                            continue;

                        try
                        {
                            var search = await _requestManager.Get($"https://boodmo.com/api/v1/customer/api/pim/part/search?searchQuery={part.Name}&sort=new&page%5Boffset%5D=1&page%5Blimit%5D=48");

                            if (Convert.ToInt32(search["list"]["size"].ToString()) < 1)
                                continue;

                            string id = search["items"].First()["id"].ToString();

                            if (Convert.ToInt32(search["list"]["size"].ToString()) > 1)
                            {
                                foreach (var i in search["items"])
                                {
                                    if (i["brand"]["name"].ToString() == brand)
                                    {
                                        id = i["id"].ToString();
                                        break;
                                    }
                                }
                            }

                            var link1 = await _requestManager.Get($"https://boodmo.com/api/v1/customer/api/catalog/part/{id}");

                            Item item = new()
                            {
                                PartsBrand = link1["brand"]["name"].ToString(),
                                Title = link1["name"].ToString(),
                                SoldBy = "",
                                Price = default,
                                PartNumber = link1["number"].ToString(),
                                Origin = Convert.ToBoolean(link1["brand"]["oem"].ToString()) ? "OEM" : "Aftermarket",
                                Class = link1["family"]["name"].ToString(),
                                Description = link1["custom_attributes"]["gmc_title"].ToString(),
                                ImageName = link1["image"].ToString(),
                            };

                            if (link1["categories"] != null)
                                foreach (var i in link1["categories"])
                                {
                                    item.Path += "/" + i["name"].ToString();
                                }

                            item.Path += "/" + link1["name"].ToString();

                            //var imgName = link1["image"].ToString();

                            //if (imgName != null && imgName.Length != 0)
                            //{
                            //    imgName = imgName.Substring(0, imgName.LastIndexOf('.')) + ".png";

                            //    //await _requestManager.SaveImage(imgName);

                            //    item.ImageName = imgName.Substring(imgName.LastIndexOf('/') + 1);
                            //}

                            var link2 = await _requestManager.Get($"https://boodmo.com/api/v1/customer/api/sales/part-offers/{id}");

                            List<OffersProvided> offers = link2["items"].Select(
                                 x => new OffersProvided
                                 {
                                     SoldBy = x["seller"]["name"].ToString(),
                                     Price = Convert.ToDouble(x["price"].ToString()) / 100,
                                     DeliveryCharge = Convert.ToDouble(x["delivery_price"].ToString()),
                                     ItemId = item.Id,
                                 }
                                ).ToList();

                            var count = await _requestManager.Get($"https://boodmo.com/api/v1/customer/api/pim/part/{id}/cross-link/count");

                            int limitReplacement = Convert.ToInt32(count["isReplacementCount"].ToString());

                            if (limitReplacement > 0)
                            {
                                List<AftermarketReplacementPart> aftermarkets = new List<AftermarketReplacementPart>();

                                int i = 1;

                                while (limitReplacement > 0)
                                {
                                    var link3 = await _requestManager.Get($"https://boodmo.com/api/v2/customer/api/pim/part/{id}/cross-link/list?filter%5Btype%5D=isReplacement&page%5Boffset%5D=1&page%5Blimit%5D=48");

                                    foreach (var aftermaket in link3["items"])
                                    {
                                        aftermarkets.Add(
                                            new AftermarketReplacementPart
                                            {
                                                PartsBrand = aftermaket["brandName"].ToString(),
                                                Title = aftermaket["name"].ToString(),
                                                Price = Convert.ToDouble(aftermaket["offerPrice"].ToString()) / 100,
                                                PartNumber = aftermaket["number"].ToString(),
                                                Discount = Convert.ToInt32(aftermaket["offerSafePercent"].ToString()),
                                                OriginalPrice = Convert.ToDouble(aftermaket["offerMrp"].ToString()) / 100,
                                                ItemId = item.Id,
                                                StoreId = Convert.ToInt32(aftermaket["id"].ToString()),
                                            });
                                    }

                                    limitReplacement -= 48;
                                    i++;
                                }

                                foreach (var aftermarket in aftermarkets)
                                {
                                    var sparePart = await _requestManager.Get($"https://boodmo.com/api/v1/customer/api/catalog/part/{aftermarket.StoreId.ToString()}");

                                    aftermarket.PartNumber = sparePart["number"].ToString();
                                }

                                item.AftermarketReplacementParts = aftermarkets;
                            }

                            int limitOemReplacement = Convert.ToInt32(count["isOemReplacementCount"].ToString());

                            if (limitOemReplacement > 0)
                            {
                                List<OEMReplacementParts> details = new List<OEMReplacementParts>();

                                int i = 1;

                                while (limitOemReplacement > 0)
                                {
                                    var link4 = await _requestManager.Get($"https://boodmo.com/api/v2/customer/api/pim/part/{id}/cross-link/list?filter%5Btype%5D=isOemReplacement&page%5Boffset%5D={i}&page%5Blimit%5D=48");

                                    foreach (var oem in link4["items"])
                                    {
                                        details.Add(new OEMReplacementParts
                                        {
                                            PartsBrand = oem["brandName"].ToString(),
                                            Title = oem["name"].ToString(),
                                            PartNumber = oem["number"].ToString(),
                                            Price = Convert.ToDouble(oem["offerPrice"].ToString()) / 100,
                                            ItemId = item.Id,
                                            StoreId = Convert.ToInt32(oem["id"].ToString()),
                                        });
                                    }

                                    limitOemReplacement -= 48;
                                    i++;
                                }

                                foreach (var detail in details)
                                {
                                    var sparePart = await _requestManager.Get($"https://boodmo.com/api/v1/customer/api/catalog/part/{detail.StoreId.ToString()}");

                                    detail.PartNumber = sparePart["number"].ToString();
                                }

                                item.OEMReplacementParts = details;
                            }

                            item.Offers = offers;

                            if (link2["items"] != null && ((JArray)link2["items"]).Count != 0)
                                item.SoldBy = link2["items"].First()["seller"]["name"].ToString();

                            if (link2["items"] != null && ((JArray)link2["items"]).Count != 0)
                                item.Price = Convert.ToDouble(link2["items"].First()["price"].ToString()) / 100;

                            using var context = ApplicationContext.GetSqlLiteContext();
                            context.Item.Add(item);
                            context.SaveChanges();

                            await context.Database.ExecuteSqlRawAsync($@"UPDATE Numbers SET Done = 1,
                                                                         ItemId = {item.Id} Where Id = {part.Id}");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);

                            //lock (locker)
                            //{
                            //    _headers = new Dictionary<string, string>();

                            //    ChromeOptions options = new ChromeOptions();
                            //    //options.AddArguments(new List<string>() { "--headless", "--no-sandbox", "--disable-dev-shm-usage" });
                            //    options.AcceptInsecureCertificates = true;
                            //    options.LeaveBrowserRunning = false;
                            //    options.AddArgument("--disable-blink-features=AutomationControlled");
                            //    options.SetLoggingPreference(LogType.Performance, LogLevel.All);

                            //    ChromeDriver driver = new ChromeDriver(options);
                            //    driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);

                            //    driver.Navigate().GoToUrl("https://boodmo.com/u/signin/");

                            //    driver.FindElement(By.CssSelector("[type='email']")).SendKeys("rahimkayte@gmail.com");
                            //    driver.FindElement(By.CssSelector("button.btn.btn-block")).Click();
                            //    driver.FindElement(By.CssSelector("[type='password']")).SendKeys("rahim@2000");
                            //    driver.FindElement(By.CssSelector("button.btn.btn-block")).Click();

                            //    driver.Navigate().GoToUrl("https://boodmo.com/catalog/part-41602m75j12-36619363/");

                            //    var el = driver.FindElement(By.ClassName("p-dataview-content"));

                            //    driver.FindElement(By.CssSelector("h2.part-info-heading"));

                            //    var logs = driver.Manage().Logs;

                            //    var perf = logs.GetLog(LogType.Performance);

                            //    var item = perf.Select(x => x.Message).Where(x => x.Contains("Network.requestWillBeSent") && x.Contains("X-Boo-Sign") && x.Contains("X-Date") && x.Contains("X-Client-Id")).First();

                            //    JObject result = JObject.Parse(item);

                            //    var headers = result["message"]["params"]["request"]["headers"];

                            //    foreach (JProperty prop in headers.OfType<JProperty>())
                            //    {
                            //        _headers.Add(prop.Name, prop.Value.ToString());
                            //    }

                            //    driver.Dispose();

                            //    _requestManager.AddHeaders(_headers);
                            //}

                        }
                    }
                }
            }

            await Task.WhenAll(
                //Parse(),
                //Parse(),
                Parse(),
                Parse()
                );
        }
    }
}
