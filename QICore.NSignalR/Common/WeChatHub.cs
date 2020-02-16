﻿using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace QICore.NSignalR.Common
{
    public class WeChatHub:Hub
    {
        public static Dictionary<string, ChatModel> HubList = new Dictionary<string, ChatModel>();
        public async Task InitUser(string userid, string username)
        {
           var connectionId= this.Context.ConnectionId;
            if (!HubList.ContainsKey(userid))
            {
                var user = new ChatModel() { UserId = userid, UserName = username, ConnectionId = connectionId, GroupNameList = new List<string>() };
                if (!user.GroupNameList.Contains(userid))
                {
                    user.GroupNameList.Add(userid);
                }
                HubList.Add(userid, user);
            }
            else
            {
                var user = HubList[userid] as ChatModel;
                user.ConnectionId = connectionId;
                if (!user.GroupNameList.Contains(userid))
                {
                    user.GroupNameList.Add(userid);
                }
                HubList[userid] = user;
            }
            Console.WriteLine($"{userid} 组：{connectionId}");
            await AddGroup(connectionId, userid);
        }
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task SendAll(string userName, string message)
        {
            await Clients.All.SendAsync("ReceiveAllMessage", userName, message);
        }

        //发送消息--发送给指定用户
        public async Task SendOnly(string userId, string message)
        {
             await Clients.User(userId).SendAsync("ReceiveUserMessage", message);
        }

        //发送消息--发送给指定组
        public async Task SendUser(string senderId,string receiveId, string message)
        {
            var senderName = string.Empty;
            var receiveName= string.Empty;
            #region 发送人
            if (HubList.ContainsKey(senderId))
            {
                var user = HubList[senderId] as ChatModel;
                var connectionId = user.ConnectionId;
                if (!user.GroupNameList.Contains(senderId))
                {
                    user.GroupNameList.Add(senderId);
                }
                senderName = user.UserName;
                Console.WriteLine($"发送消息{senderId} 组：{connectionId}");
                await AddGroup(connectionId, receiveId);//【接收人】加入【发送人】的组
            }
            #endregion
            #region 发送人
            if (HubList.ContainsKey(receiveId))
            {
                var user = HubList[receiveId] as ChatModel;
                var connectionId = user.ConnectionId;
                if (!user.GroupNameList.Contains(receiveId))
                {
                    user.GroupNameList.Add(receiveId);
                }
                receiveName = user.UserName;
                Console.WriteLine($"发送消息{senderId} 组：{connectionId}");
                await AddGroup(connectionId, senderId);//【发送人】加入【接收人】的组
            }
            #endregion
            var msg = new {senderId= senderId, senderName= senderName, receiveId = receiveId, receiveName= receiveName,message = message };
            await Clients.Group(receiveId).SendAsync("ReceiveMessage", msg);

        }
        /// <summary>
        /// 加入指定组并向组推送消息
        /// </summary>
        /// <param name="groupName"></param>
        /// <returns></returns>
        public async Task JoinGroup(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            await Clients.Group(groupName).SendAsync("ReceiveMessage", $"{Context.ConnectionId}",$"我加入了:[{groupName}]组");
        }
        public async Task AddGroup(string connectionId,string groupName)
        {
            await Groups.AddToGroupAsync(connectionId, groupName);
        }
        /// <summary>
        /// 退出指定组并向组推送消息
        /// </summary>
        /// <param name="groupName"></param>
        /// <returns></returns>
        public async Task LeaveGroup(string groupName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
            await Clients.Group(groupName).SendAsync("ReceiveMessage", $"{Context.ConnectionId} left {groupName}");
        }
        /// <summary>
        /// 向指定Id推送消息
        /// </summary>
        /// <param name="userid">要推送消息的对象</param>
        /// <param name="message"></param>
        /// <returns></returns>
        public Task Echo(string userid, string message)
        {
            return Clients.Client(Context.ConnectionId).SendAsync("ReceiveMessage", $"{Context.ConnectionId}: {message}");
        }
        public override Task OnConnectedAsync()
        {
            var userid = Context.GetHttpContext().Request.Query["userid"].ToString();
            var id = Context.GetHttpContext().Request.Query["id"].ToString();
            //var version = Context.QueryString["contosochatversion"];
            Console.WriteLine("哇，有人进来了：{0}", this.Context.ConnectionId);
           // var id = this.Context.ConnectionId;
            ////添加用户登录
            //this.Context.User.AddIdentity();
            ////获取用户登录
            //this.Clients.User()
           // this.Groups.AddToGroupAsync(Context.ConnectionId,"组名");
            
            var claimNameIdentifier = this.Context.User.Claims.FirstOrDefault(s => s.Type == ClaimTypes.NameIdentifier)?.Value;
            SignalRModel.SignalRList.Add(id, claimNameIdentifier);
            if (SignalRModel.StaticList.Any(s => s.Key.Equals(claimNameIdentifier)))
            {
                SignalRModel.StaticList.Remove(claimNameIdentifier);
            }
            if (claimNameIdentifier != null)
            {
                if (!SignalRModel.StaticList.ContainsKey(claimNameIdentifier))
                    SignalRModel.StaticList.Add(claimNameIdentifier, SignalRStatus.Open);
            }
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            Console.WriteLine("靠，有人跑路了：{0}", this.Context.ConnectionId);
            var id = this.Context.ConnectionId;
           // this.Groups.RemoveFromGroupAsync(Context.ConnectionId, "组名");
            var claimNameIdentifier = this.Context.User.Claims.FirstOrDefault(s => s.Type == ClaimTypes.NameIdentifier)?.Value;
            SignalRModel.SignalRList.Remove(id);
            if (claimNameIdentifier != null)
            {
                if(SignalRModel.StaticList.ContainsKey(claimNameIdentifier))
                SignalRModel.StaticList.Remove(claimNameIdentifier);
            }
           
            return base.OnDisconnectedAsync(exception);
        }
        
        //通知除了本组之外的其他链接
        public async Task ReceiveInfo(string content)
        {
            string id = this.Context.ConnectionId;
            await this.Clients.AllExcept(id)
                 .SendAsync("ReceiveMessage", id + "已经上线，" + content);
            // this.Clients.GroupExcept()
        }
        
    }
   
    public class SignalRModel
    {
        public static Dictionary<string, SignalRStatus> StaticList = new Dictionary<string, SignalRStatus>();
        public static Dictionary<string, string> SignalRList { get; set; } = new Dictionary<string, string>();
    }
    public class ChatModel
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string ConnectionId { get; set; }
        public List<string> GroupNameList { get; set; }
    }
    public enum SignalRStatus
    {
        Open=1,
        Close =0
    }
}
