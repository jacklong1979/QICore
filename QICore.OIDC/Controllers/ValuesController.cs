using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace QICore.OIDC.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ValuesController : Controller
    {
        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            var user=this.User;
            return new string[] { "value1", "value2" };
        }
        // GET api/values
        [HttpGet("userinfo")]      
        public IActionResult GetUserinfo()
        {
            // var user = this.User.Claims.ToList();
            var user = from c in User.Claims select new { type = c.Type, value = c.Value };
            return Json(user);
        }
    }
}
