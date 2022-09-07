using BoodmoParser;
using Microsoft.EntityFrameworkCore;
using System.Net;

var context = ApplicationContext.GetSqlLiteContext();

await context.Database.EnsureCreatedAsync();



