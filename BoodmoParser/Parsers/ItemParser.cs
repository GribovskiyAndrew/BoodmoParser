using BoodmoParser.Entities;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System.Collections.Concurrent;

namespace BoodmoParser.Parsers
{
    public class ItemParser : BaseParser, IParser
    {

        private readonly ConcurrentQueue<dynamic> _queue;

        public ItemParser(RequestManager requestManager, ApplicationContext context) : base(requestManager, context)
        {
            _queue = new ConcurrentQueue<dynamic>(
                _context.Numbers
                .Where(x => !x.Done)
                .Select(x => new { x.Name, x.Id })
                .ToList());
        }

        public async Task Run()
        {

            async Task Parse()
            {
                while (!_queue.IsEmpty)
                {
                    var ok = _queue.TryDequeue(out dynamic part);

                    if (ok)
                    {
                        if (part.Name.Length < 11)
                            continue;

                        try
                        {
                            var search = await _requestManager.Get($"https://boodmo.com/api/v1/customer/api/pim/part/search?searchQuery={part.Name}&sort=new&page%5Boffset%5D=1&page%5Blimit%5D=48");

                            var id = search["items"].First()["id"].ToString();

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
                            };

                            var imgName = link1["image"].ToString();

                            if (imgName != null && imgName.Length != 0)
                            {
                                imgName = imgName.Substring(0, imgName.LastIndexOf('.')) + ".png";

                                await _requestManager.SaveImage(imgName);

                                item.ImageName = imgName.Substring(imgName.LastIndexOf('/') + 1);
                            }

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

                            var link3 = await _requestManager.Get($"https://boodmo.com/api/v2/customer/api/pim/part/{id}/cross-link/list?filter%5Btype%5D=isReplacement&page%5Boffset%5D=1&page%5Blimit%5D=8");

                            List<AftermarketReplacementPart> aftermarkets = link3["items"].Select(
                                x => new AftermarketReplacementPart
                                {
                                    PartsBrand = x["brandName"].ToString(),
                                    Title = x["name"].ToString(),
                                    Price = Convert.ToDouble(x["offerPrice"].ToString()) / 100,
                                    PartNumber = x["number"].ToString(),
                                    Discount = Convert.ToInt32(x["offerSafePercent"].ToString()),
                                    OriginalPrice = Convert.ToDouble(x["offerMrp"].ToString()) / 100,
                                    ItemId = item.Id,
                                }
                                ).ToList();

                            foreach (var aftermarket in aftermarkets)
                            {
                                var sparePart = await _requestManager.Get($"https://boodmo.com/api/v1/customer/api/catalog/part/{link3["items"][aftermarkets.IndexOf(aftermarket)]["id"]}");

                                aftermarket.PartNumber = sparePart["number"].ToString();
                            }

                            var link4 = await _requestManager.Get($"https://boodmo.com/api/v2/customer/api/pim/part/{id}/cross-link/list?filter%5Btype%5D=isOemReplacement&page%5Boffset%5D=1&page%5Blimit%5D=8");

                            List<OEMReplacementParts> details = link4["items"].Select(
                                x => new OEMReplacementParts
                                {
                                    PartsBrand = x["brandName"].ToString(),
                                    Title = x["name"].ToString(),
                                    PartNumber = x["number"].ToString(),
                                    Price = Convert.ToDouble(x["offerPrice"].ToString()) / 100,
                                    ItemId = item.Id,
                                }
                            )
                            .ToList();

                            foreach (var detail in details)
                            {
                                var sparePart = await _requestManager.Get($"https://boodmo.com/api/v1/customer/api/catalog/part/{link4["items"][details.IndexOf(detail)]["id"]}");

                                detail.PartNumber = sparePart["number"].ToString();
                            }

                            item.OEMReplacementParts = details;
                            item.AftermarketReplacementParts = aftermarkets;
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
                        }
                    }
                }
            }

            await Task.WhenAll(
                Parse(),
                Parse(),
                Parse(),
                Parse()
                );
        }
    }
}
