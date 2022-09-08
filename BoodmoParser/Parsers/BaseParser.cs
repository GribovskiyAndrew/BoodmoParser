using Entities;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BoodmoParser.Parsers
{
    public abstract class BaseParser
    {

        protected readonly RequestManager _requestManager;
        protected readonly ApplicationContext _context;

        protected const string START_URL = "https://snaponepc.com/epc-services/datasets/dec9913a-c05a-5535-e043-60d416acaf35/navigations/" +
            "filterRequest/am9iSWQ9MXxkYXRhU2V0SWQ9ZGVjOTkxM2EtYzA1YS01NTM1LWUwNDMtNjBkNDE2YWNhZjM1fG1hbnVhbEZpbHRlcnNFbmFibGVkPXRydWV8bG" +
            "9jYWxlPWVuLVVTfGJ1c1JlZz1JTkR8cHJpY2VCb29rSWQ9OTFmMTA0OTEtY2RlYi00ZDIyLThkMzctMWQ4MzI1MzBiNjZjfHVzZXJJZD1jMDBiYjBmZC1jYWM3LT" +
            "Q0N2EtYjBjMy0yNWU2ZmU1MDBmMDc=";

        protected const string URL_PART = "/filterRequest/am9iSWQ9MXxkYXRhU2V0SWQ9ZGVjOTkxM2EtYzA1YS01NTM1LWUwNDMtNjBkNDE2YWNhZjM1fG1hbnV" +
            "hbEZpbHRlcnNFbmFibGVkPXRydWV8bG9jYWxlPWVuLVVTfGJ1c1JlZz1JTkR8cHJpY2VCb29rSWQ9OTFmMTA0OTEtY2RlYi00ZDIyLThkMzctMWQ4MzI1MzBiNjZ" +
            "jfHVzZXJJZD1jMDBiYjBmZC1jYWM3LTQ0N2EtYjBjMy0yNWU2ZmU1MDBmMDc=";

        protected const string API_PART = "https://snaponepc.com/epc-services/datasets/dec9913a-c05a-5535-e043-60d416acaf35/navigations/";

        protected string GetLink(JToken item)
        {
            if ((bool)item["leafNode"] == true)
            {
                var image_id = item["imageId"];
                return "https://snaponepc.com/epc-services/datasets/dec9913a-c05a-5535-e043-60d416acaf35/pages/parts/" +
                    item["serializedPath"] + URL_PART + $"?imageId={image_id}";
            }
            else
            {
                return API_PART + item["serializedPath"] + URL_PART;
            }
        }

        protected string GetLink(string serializedPath)
        {
            return API_PART + serializedPath + URL_PART;
        }

        protected BaseParser(RequestManager requestManager, ApplicationContext context)
        {
            _requestManager = requestManager;
            _context = context;
        }
    }
}
