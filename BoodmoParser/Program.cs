using BoodmoParser;
using BoodmoParser.Entities;
using Microsoft.EntityFrameworkCore;
using System.Net;

var context = ApplicationContext.GetSqlLiteContext();

await context.Database.EnsureCreatedAsync();

if (!context.Numbers.Any())
    await context.Numbers.BulkInsertAsync(NumberSource.NumberList.Select(x => new Number { Name = x }));

