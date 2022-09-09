using BoodmoParser;
using BoodmoParser.Entities;
using BoodmoParser.Parsers;
using Microsoft.EntityFrameworkCore;
using System.Net;
using Z.EntityFramework.Extensions;

var context = ApplicationContext.GetSqlLiteContext();

EntityFrameworkManager.ContextFactory = context => ApplicationContext.GetSqlLiteContext();

await context.Database.EnsureCreatedAsync();

if (!context.Numbers.Any())
    await context.Numbers.BulkInsertAsync(NumberSource.NumberList.Select(x => new Number { Name = x }));

 RequestManager requestManager = new RequestManager();

 IParser parser = new ItemParser(requestManager, context);

 await parser.Run();
