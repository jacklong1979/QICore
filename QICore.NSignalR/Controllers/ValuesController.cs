using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using QICore.NSignalR.Common;

namespace QICore.NSignalR.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        private readonly IHubContext<WeChatHub> _hub;

        public ValuesController(IHubContext<WeChatHub> hubContext)
        {
            _hub = hubContext;
        }

        // GET api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            string userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

            return new string[] { "value1", "value2" };
        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        [HttpPost("Login/{username?}/{userpwd?}")]
        public async Task<IActionResult> Login(string username, string userpwd)
        {
            var version = Request.QueryString.Value;
             username = Request.Form["username"];
             userpwd = Request.Form["userpwd"];
            #region
            //try
            //{
            //    //登陆授权
            //    //声明【类似于身份证中姓名，民族等定义】
            //    string userId = Guid.NewGuid().ToString().Replace("-", "");
            //    var claims = new List<Claim>()
            //{
            //    new Claim(ClaimTypes.Name,username),   //储存用户name
            //    new Claim(ClaimTypes.NameIdentifier,userId)  //储存用户id
            //};
            //    //身份【似身份证，多个声明（姓名，民族等）构成】
            //    var indentity = new ClaimsIdentity(claims, "formlogin");
            //    //证件所有者【类似身份证所有者】
            //    var principal = new ClaimsPrincipal(indentity);

            //    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            //    //验证是否授权成功
            //    if (principal.Identity.IsAuthenticated)
            //    {
            //        return Json(new { code = "success", msg = "登陆成功", userid = userId });
            //    }
            //    else
            //        return Json(new { code = "failed", msg = "登陆失败" });
            //}
            //catch (Exception e)
            //{
            //    return Json(new { code = "failed", msg = e.ToString() });
            //}
            #endregion
            var userid = Guid.NewGuid().ToString();
            //_hub.Groups.AddToGroupAsync(_hub.Clients.ConnectionId, "组名");
            var claimsIdentity = new ClaimsIdentity("Cookie");
            claimsIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, userid));
            claimsIdentity.AddClaim(new Claim(ClaimTypes.Name, username));
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);
            return Json(new { code = "success", msg = "登陆成功", userid = userid });
        }
        /// <summary>
        /// 登出
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> logOff()
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            }
            return Redirect("/Home/Index");
        }
        [HttpGet("SendAll")]
        public async Task<IActionResult> SendAll(string userId, string message)
        {
             await _hub.Clients.All.SendAsync("ReceiveAllMessage", userId, message);
            return Ok("推送全部人");
        }
        [HttpGet("SendOnly/{userId}/{message}")]
        public async Task<IActionResult> SendOnly(string userId, string message)
        {
            var claimNameIdentifier = User.Claims.FirstOrDefault(s => s.Type == ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(claimNameIdentifier))
            {
                return Ok(new { code =401, message = "用户未登陆！" });
            }
            await _hub.Clients.User(userId).SendAsync("ReceiveUserMessage", message);

            return Ok("推送当前登录用户");
        }
        /// <summary>
        /// 通知本组成员
        /// </summary>
        /// <returns></returns>
        public IActionResult Two(string usernmae)
        {    
            //如果分组成员不存在，不会报错
            _hub.Clients.Group(usernmae).SendAsync("hello", "本组成员好");
            return Content("执行完成");
        }
    }
}
