using System.Web.Http;

namespace VuBongBongWeb.Controllers
{
    public class MainController : ApiController
    {
        //public ProductManager productManager = new ProductManager();
        //public CategoryManager categoryManager = new CategoryManager();
        //public BillManager billManager = new BillManager();
        //public BaseManager baseManager = new BaseManager();

        // GET: api/Main
        //public IEnumerable<string> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}
        //[HttpGet]
        //public object Call(string requestObj)
        //{
        //    ResponseObject result = new ResponseObject();
        //    try
        //    {
        //        var request = JsonConvert.DeserializeObject<RequestObject>(requestObj);
        //        if (request == null) return null;
        //        Module module;
        //        string moduleName = request.ModuleName;
        //        Enum.TryParse(moduleName, true, out module);
        //        switch (module)
        //        {
        //            case Module.CoffeeManager:
        //                return productManager.ActionMethod(request);
        //            case Module.CategoryManager:
        //                return categoryManager.ActionMethod(request);
        //            case Module.BillManager:
        //                return billManager.ActionMethod(request);
        //            case Module.BaseManager:
        //                return baseManager.ActionMethod(request);
        //            default:
        //                result.Data = new Hashtable();
        //                result.Data.Add(Resource.K_RETURN_STATUS, false);
        //                result.Data.Add(Resource.K_RETURN_MESSAGE, "Incorrect Request method.");
        //                break;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        result.Data = new Hashtable();
        //        result.Data.Add(Resource.K_RETURN_STATUS, false);
        //        result.Data.Add(Resource.K_RETURN_MESSAGE, ex.Message);
        //        //throw ex;
        //    }
        //    return result;
        //}


        // GET: api/Main/5
        //public string Get(int id)
        //{
        //    return "value";
        //}

        //// POST: api/Main
        //public void Post([FromBody]string value)
        //{
        //}

        //// PUT: api/Main/5
        //public void Put(int id, [FromBody]string value)
        //{
        //}

        //// DELETE: api/Main/5
        //public void Delete(int id)
        //{
        //}
    }
}
