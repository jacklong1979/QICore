using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using QICore.NSignalR.Common;
using System.IO;
namespace QICore.NSignalR.Controllers
{
    
    [Route("api/[controller]")]
    public class ChatController : Controller
    {
        private readonly IHubContext<ChatHub> _hub;
      
        public ChatController(IHubContext<ChatHub> hubContext)
        {
            _hub = hubContext;
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
       
        [HttpGet("SendAll")]
        public async Task<IActionResult> SendAll(string userId, string message)
        {
             await _hub.Clients.All.SendAsync("ReceiveAllMessage", userId, message);
            return Ok("推送全部人");
        }
        [HttpPost("senduser")]
        public async Task<IActionResult> SendUser()
        {
            var online = ChatHub.OnlineClients;
            Message msg = new Message();
            msg.SenderId = Request.Form["senderId"].ToString();
            msg.SenderName = Request.Form["senderName"].ToString();
            msg.ReceiveId = Request.Form["receiveId"].ToString();
            msg.ReceiveName = Request.Form["receiveName"].ToString();
            msg.Msg = Request.Form["msg"].ToString();
            msg.MsgType =int.Parse(Request.Form["msgType"].ToString());
            var senderClient = ChatHub.OnlineClients.Where(x => x.Key == msg.SenderId).Select(x => x.Value).FirstOrDefault();
            var receiveClient = ChatHub.OnlineClients.Where(x => x.Key == msg.ReceiveId).Select(x => x.Value).FirstOrDefault();
            if (senderClient != null)
            {
                await AddGroup(senderClient.ConnectionId,msg.SenderId);//【接收人】加入【发送人】
                await AddGroup(senderClient.ConnectionId, msg.ReceiveId);//【发送人】加入【接收人】
            }
            if (receiveClient != null)
            {
                await AddGroup(receiveClient.ConnectionId, msg.SenderId);//【接收人】加入【发送人】
                await AddGroup(receiveClient.ConnectionId, msg.ReceiveId);//【发送人】加入【接收人】
            }
            // await _hub.Clients.User(receiveId).SendAsync("ReceiveMessage",msg);
            await _hub.Clients.Group(msg.SenderId).SendAsync("ReceiveMessage", msg);

            return Ok("推送当前登录用户");
        }
        private async Task AddGroup(string connectionId, string groupName)
        {
            await _hub.Groups.AddToGroupAsync(connectionId, groupName);
        }        
    }
    public class Message
    {
        public string SenderId { get; set; }
        public string SenderName { get; set; }
        public string ReceiveId { get; set; }
        public string ReceiveName { get; set; }
        public string Msg { get; set; }
        public int MsgType { get; set; }
    }
}
