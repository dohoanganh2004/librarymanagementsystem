using Microsoft.AspNetCore.Mvc;

namespace ELibrary.WebApp.Controllers
{
    public class BaseController : Controller
    {
       
        protected int ReaderId
        {
            get
            {
                var id = HttpContext.Session.GetInt32("readerId");
                if (id == null)
                {
                   
                    return -1; 
                }
                return id.Value;
            }
        }
    }
}
