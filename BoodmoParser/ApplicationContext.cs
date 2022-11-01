using BoodmoParser.Entities;
using Microsoft.EntityFrameworkCore;

namespace BoodmoParser
{
    public class ApplicationContext : DbContext
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
        {

        }

        public static ApplicationContext GetSqlLiteContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationContext>();

            options.UseSqlite($"DataSource=file:C:/Users/lifebookE/source/repos/BoodmoParser/BoodmoParser/DB/data_vin_vw.db?&cache=shared");

            var context = new ApplicationContext(options.Options);

            return context;
        }

        public static ApplicationContext GetMySqlContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationContext>();

            options.UseMySql(
                "Server=localhost;port=3306;database=tata;uid=root;password=1111",
                //"Server=38.242.248.12;port=3306;database=tata;uid=root;password=FN3P8H2FEUBEdsfffTN38du28",
                new MySqlServerVersion(new Version(8, 0, 28)));

            var context = new ApplicationContext(options.Options);

            return context;
        }


        public DbSet<Item> Item { get; set; }

        public DbSet<AftermarketReplacementPart> AftermarketReplacementParts { get; set; }

        public DbSet<OEMReplacementParts> OEMReplacementParts { get; set; }

        public DbSet<OffersProvided> OffersProvided { get; set; }

        public DbSet<Number> Numbers { get; set; }

    }
}
