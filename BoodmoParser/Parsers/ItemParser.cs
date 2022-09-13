using BoodmoParser.Entities;
using BoodmoParser.Parsers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoodmoParser.Parsers
{
    public class ItemParser : BaseParser, IParser
    {
        public ItemParser(RequestManager requestManager, ApplicationContext context) : base(requestManager, context)
        {
        }

        public async Task Run()
        {
            var numbers = await _context.Numbers.Where(x => !x.Done).ToListAsync();

            foreach (var number in numbers)
            {
                try
                {
                    var search = await _requestManager.Get($"https://boodmo.com/api/v1/customer/api/pim/part/search?searchQuery={number.Name}&sort=new&page%5Boffset%5D=1&page%5Blimit%5D=48");

                    var id = search["items"][0]["id"].ToString();

                    var link1 = await _requestManager.Get($"https://boodmo.com/api/v1/customer/api/catalog/part/{id}");

                    Item item = new()
                    {
                        Id = Guid.NewGuid(),
                        PartsBrand = link1["brand"]["name"].ToString(),
                        Title = link1["name"].ToString(),
                        SoldBy = "",
                        Price = default,
                        PartNumber = link1["number"].ToString(),
                        Origin = Convert.ToBoolean(link1["brand"]["oem"].ToString()) ? "OEM" : "Aftermarket",
                        Class = link1["family"]["name"].ToString(),
                        Description = link1["custom_attributes"]["gmc_title"].ToString(),
                    };

                    //https://boodmo.com/api/v1/customer/api/sales/part-offers/{id}?filter%5Bpin%5D=370040

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
                        //link3["items"][aftermarkets.IndexOf(aftermarket)]["id"]

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
                        //link4["items"][aftermarkets.IndexOf(aftermarket)]["id"]

                        var sparePart = await _requestManager.Get($"https://boodmo.com/api/v1/customer/api/catalog/part/{link4["items"][details.IndexOf(detail)]["id"]}");

                        detail.PartNumber = sparePart["number"].ToString();
                    }

                    item.OEMReplacementParts = details;
                    item.AftermarketReplacementParts = aftermarkets;
                    item.Offers = offers;

                    item.SoldBy = link2["items"][0]["seller"]["name"].ToString();
                    item.Price = Convert.ToDouble(link2["items"][0]["price"].ToString()) / 100;

                    await _context.Item.SingleInsertAsync(item, (x) => { x.IncludeGraph = true; x.InsertKeepIdentity = true; });

                    number.Done = true;
                    number.ItemId = item.Id;
                    await _context.SaveChangesAsync();


                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

        }


    }
}
