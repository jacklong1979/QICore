using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace QICore.NSignalR.Common
{
    public class ChatHub : Hub
    {
       // public const string ChatName = "找工作-.-";
        public static ConcurrentDictionary<string, OnlineClient> OnlineClients { get; set; }

        private static readonly object SyncObj = new object();

        static ChatHub()
        {
            OnlineClients = new ConcurrentDictionary<string, OnlineClient>();
        }
        public override async Task OnConnectedAsync()
        {
            var http = Context.GetHttpContext();
            var userId=http.Request.Query["userId"].FirstOrDefault();
            var userName = http.Request.Query["userName"].FirstOrDefault();
            var nickName = http.Request.Query["nickName"].FirstOrDefault();
          
            lock (SyncObj)
            {
                var currClient = OnlineClients.Where(x =>x.Key== userId).Select(x => x.Value).FirstOrDefault();
                if (currClient != null)
                {
                    currClient.UserId = userId;
                    currClient.UserName = userName;
                    currClient.NickName = nickName;
                    currClient.ConnectionId = Context.ConnectionId;
                    OnlineClients[userId] = currClient;
                }
                else
                {
                    var client = new OnlineClient()
                    {
                        NickName = nickName,
                        UserId = userId,
                        UserName = userName,
                        ConnectionId = Context.ConnectionId
                     };
                    OnlineClients.AddOrUpdate(userId, client, (oldkey, oldvalue) => client); // .AddOrUpdate(key, value, (oldkey, oldvalue) => value);
                }
               
            }
            await base.OnConnectedAsync();
            await Groups.AddToGroupAsync(Context.ConnectionId, userId);
           // await Clients.GroupExcept(userId, new[] { Context.ConnectionId }).SendAsync("ReceiveAllMessage", $"用户{userId}加入了群聊");
            await Clients.Client(Context.ConnectionId).SendAsync("ReceiveAllMessage", $"{userName}",$"我加入了组：{Context.ConnectionId}");

        }
       
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await base.OnDisconnectedAsync(exception);
            bool isRemoved=false;
            OnlineClient client;
            client = OnlineClients.Where(x => x.Value.ConnectionId == Context.ConnectionId).Select(x => x.Value).FirstOrDefault();
            lock (SyncObj)
            {
                if (client != null)
                {
                    isRemoved = OnlineClients.TryRemove(client.UserId, out client);
                }
            }
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, client.UserId);

            if (isRemoved)
            {
              //  await Clients.GroupExcept(ChatName, new[] { Context.ConnectionId }).SendAsync("ReceiveAllMessage", $"用户{client.NickName}加入了群聊");
            }
        }

        //public async Task SendMessage(string msg)
        //{
        //    var client = OnlineClients.Where(x => x.Key == Context.ConnectionId).Select(x => x.Value).FirstOrDefault();
        //    if (client == null)
        //    {
        //        await Clients.Client(Context.ConnectionId).SendAsync("ReceiveAllMessage", "您已不在聊天室,请重新加入");
        //    }
        //    else
        //    {
        //        await Clients.GroupExcept(ChatName, new[] { Context.ConnectionId }).SendAsync("ReceiveMessage", new { msg, nickName = client.NickName, userId = client.UserId });

        //    }
        //}
    }
     
    public class OnlineClient
    {
        public string NickName { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string ConnectionId { get; set; }
        /// <summary>
        /// 用户所加入的组
        /// </summary>
        public List<string> GroupNameList { get; set; }
    }
}
