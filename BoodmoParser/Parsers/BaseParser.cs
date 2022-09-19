
namespace BoodmoParser.Parsers
{
    public abstract class BaseParser
    {

        protected readonly RequestManager _requestManager;
        protected readonly ApplicationContext _context;

        protected BaseParser(RequestManager requestManager, ApplicationContext context)
        {
            _requestManager = requestManager;
            _context = context;
        }
    }
}
