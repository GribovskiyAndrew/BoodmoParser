using BoodmoParser.Database.Parsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoodmoParser.Parsers
{
    public class ItemParser : Parser
    {
        public ItemParser(ApplicationContext applicationContext) : base(applicationContext)
        {
        }

        public async Task<string> Get(string url, HttpClient httpClient)
        {
            while (true)
                try
                {
                    return await httpClient.GetStringAsync(url);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
        }

        public override async Task Parse()
        {


        }

    }
}
