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

                    Item _item = new Item()
                    {
                        Aftermarkets = null,
                        OEMs = null,
                        Offers = null,

                        PartsBrand = link1["brand"]["name"].ToString(),
                        Title = link1["name"].ToString(),
                        SoldBy = default,
                        Price = default,
                        PartNumber = number.ToString(),
                        Origin = default,
                        Class = link1["family"]["name"].ToString(),
                        Description = link1["custom_attributes"]["gmc_title"].ToString(),
                    };

                    var link2 = await _requestManager.Get($"https://boodmo.com/api/v1/customer/api/sales/part-offers/{id}");

                    List<OffersProvided> offers = link2["items"].Select(
                         x => new OffersProvided
                         {
                             SoldBy = x["items"]["seller"]["name"].ToString(),
                             Price = Convert.ToDouble(x["items"]["price"]),
                             DeliveryCharge = Convert.ToDouble(x["items"]["delivery_price"]),
                             ItemId = _item.Id,
                         }
                        ).ToList();

                    var link3 = await _requestManager.Get($"https://boodmo.com/api/v2/customer/api/pim/part/{id}/cross-link/list?filter%5Btype%5D=isReplacement&page%5Boffset%5D=1&page%5Blimit%5D=8");

                    List<AftermarketReplacementParts> aftermarkets = link3[""].Select(
                        x => new AftermarketReplacementParts
                        {
                            PartsBrand = x["brandName"].ToString(),
                            Title = x["name"].ToString(),
                            Price = Convert.ToDouble(x["offerPrice"]),
                            ShortNumber = x["number"].ToString(),
                            Discount = Convert.ToInt32(x["offerSafePercent"]),
                            OriginalPrice = Convert.ToDouble(x["offerMrp"]),
                            ItemId = _item.Id,
                        }
                        ).ToList();

                    var link4 = await _requestManager.Get($"https://boodmo.com/api/v2/customer/api/pim/part/{id}/cross-link/list?filter%5Btype%5D=isOemReplacement&page%5Boffset%5D=1&page%5Blimit%5D=8");

                    List<OEMReplacementParts> details = link4["items"].Select(
                        x => new OEMReplacementParts
                        {
                            PartsBrand = x["brandName"].ToString(),
                            Title = x["name"].ToString(),
                            ShortNumber = x["number"].ToString(),
                            Price = Convert.ToDouble(x["offerPrice"]),
                            ItemId = _item.Id,
                        }
                    )
                    .ToList();

                    _item.OEMs = details;
                    _item.Aftermarkets = aftermarkets;
                    _item.Offers = offers;

                    number.Done = true;
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
