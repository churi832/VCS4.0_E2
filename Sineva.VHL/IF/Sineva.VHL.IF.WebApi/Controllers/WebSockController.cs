using Newtonsoft.Json;
using Sineva.VHL.IF.WebApi.Models;
using Sineva.VHL.Library.Remoting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.WebSockets;

namespace Sineva.VHL.IF.WebApi.Controllers
{
    /// <summary>
    /// WebSocket推送
    /// </summary>
    public class WebSockController : ApiController
    {
        /// <summary>
        /// 推送消息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage LongSock()
        {
            if (HttpContext.Current.IsWebSocketRequest)
            {
                HttpContext.Current.AcceptWebSocketRequest(ProcessWSMsg);
            }
            return new HttpResponseMessage(HttpStatusCode.SwitchingProtocols);
        }
        CancellationToken cancellationToken = new CancellationToken();
        private static readonly object _bufferLock = new object();
        public static Dictionary<string, WebSocket> webSockPool = new Dictionary<string, WebSocket>();
        public static Dictionary<string, string> usernamemap = new Dictionary<string, string>();
        public async Task SendMsg(string msg)
        {
            byte[] msgByte = System.Text.Encoding.UTF8.GetBytes(msg);
            ArraySegment<byte> buffer = new ArraySegment<byte>(msgByte);
            foreach (var v in webSockPool)
            {
                if (v.Value.State == WebSocketState.Open)
                {
                    try
                    {
                        await v.Value.SendAsync(buffer, WebSocketMessageType.Text, true, cancellationToken);
                    }
                    catch
                    {
                        webSockPool.Remove(v.Key);
                        var temp = usernamemap.FirstOrDefault(o => o.Value == v.Key);
                        if (!string.IsNullOrEmpty(temp.Key))
                        {
                            usernamemap.Remove(temp.Key);
                        }
                    }
                }
                else
                {
                    webSockPool.Remove(v.Key);
                    var temp = usernamemap.FirstOrDefault(o => o.Value == v.Key);
                    if (!string.IsNullOrEmpty(temp.Key))
                    {
                        usernamemap.Remove(temp.Key);
                    }
                }
            }
        }
        public void SendMsg(KeyValuePair<string, WebSocket> v, string msg)
        {
            byte[] msgByte = System.Text.Encoding.UTF8.GetBytes(msg);
            ArraySegment<byte> buffer = new ArraySegment<byte>(msgByte);
            if (v.Value.State == WebSocketState.Open)
            {
                try
                {
                    v.Value.SendAsync(buffer, WebSocketMessageType.Text, true, cancellationToken);
                }
                catch
                {
                    try
                    {
                        webSockPool[v.Key].Dispose();
                    }
                    catch
                    { }
                    webSockPool.Remove(v.Key);
                    var temp = usernamemap.FirstOrDefault(o => o.Value == v.Key);
                    if (!string.IsNullOrEmpty(temp.Key))
                    {
                        usernamemap.Remove(temp.Key);
                    }
                }
            }
            else
            {
                try
                {
                    webSockPool[v.Key].Dispose();
                }
                catch
                { }
                webSockPool.Remove(v.Key);
                var temp = usernamemap.FirstOrDefault(o => o.Value == v.Key);
                if (!string.IsNullOrEmpty(temp.Key))
                {
                    usernamemap.Remove(temp.Key);
                }
            }
        }
        //private async Task ProcessWSMsg(AspNetWebSocketContext arg)
        //{
        //    WebSocket websock = arg.WebSocket;
        //    var unikey = arg.SecWebSocketKey;//arg.UserHostAddress;
        //    if (!webSockPool.ContainsKey(unikey))
        //    {
        //        webSockPool.Add(unikey, websock);
        //    }
        //    //SendMsg(unikey);
        //    while (true)
        //    {
        //        ArraySegment<byte> buffer = new ArraySegment<byte>(new byte[1024 * 10]);
        //        //WebSocketReceiveResult result = await websock.ReceiveAsync(buffer, CancellationToken.None);

        //        if (websock.State == WebSocketState.Open)
        //        {
        //            if (RemoteManager.TouchInstance.Remoting != null && RemoteManager.TouchInstance.Remoting.TouchGUI != null)
        //            {
        //                Debug.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(RemoteManager.TouchInstance.Remoting.TouchGUI));
        //                SendMsg(Newtonsoft.Json.JsonConvert.SerializeObject(RemoteManager.TouchInstance.Remoting.TouchGUI));
        //            }
        //        }
        //        await Task.Delay(200);
        //    }
        //}

        private async Task ProcessWSMsg(AspNetWebSocketContext arg)
        {
            WebSocket websock = arg.WebSocket;
            var unikey = arg.SecWebSocketKey;

            // 为每个连接创建独立的CancellationTokenSource
            var cts = new CancellationTokenSource();
            try
            {
                if (!webSockPool.ContainsKey(unikey))
                {
                    webSockPool.Add(unikey, websock);
                }

                while (websock.State == WebSocketState.Open)
                {
                    //ArraySegment<byte> receiveBuffer = new ArraySegment<byte>(new byte[1024]);
                    //WebSocketReceiveResult receiveResult = await websock.ReceiveAsync(
                    //    receiveBuffer, cts.Token);

                    //// 如果收到关闭消息，结束连接
                    //if (receiveResult.MessageType == WebSocketMessageType.Close)
                    //{
                    //    await websock.CloseAsync(WebSocketCloseStatus.NormalClosure,
                    //        "Closed by client", cts.Token);
                    //    break;
                    //}
                    try
                    {
                        if (RemoteManager.TouchInstance.Remoting?.TouchGUI != null)
                        {
                            var web = RemoteManager.TouchInstance.Remoting.TouchGUI.GetWebData();
                            if (web != null)
                            {
                                await SendMsg(JsonConvert.SerializeObject(web));
                                RemoteManager.TouchInstance.Remoting.TouchGUI.ResponseMsg = string.Empty;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        //Debug.WriteLine($"Remoting error!");
                        //// 可以选择发送错误信息给客户端
                        await SendMsgToSocket(websock, Newtonsoft.Json.JsonConvert.SerializeObject(new { Error = $"Error:{ex.Message}" }), cts.Token);
                        break;
                    }

                    // 3. 添加适当的延迟
                    await Task.Delay(100);
                }
            }
            catch (OperationCanceledException)
            {
                // 正常取消
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"WebSocket error: {ex.Message}");
            }
            finally
            {
                // 清理资源
                webSockPool.Remove(unikey);
                var temp = usernamemap.FirstOrDefault(o => o.Value == unikey);
                if (!string.IsNullOrEmpty(temp.Key))
                {
                    usernamemap.Remove(temp.Key);
                }
                websock?.Dispose();
                cts.Dispose();
            }
        }

        private async Task SendMsgToSocket(WebSocket socket, string message, CancellationToken ct)
        {
            if (socket?.State == WebSocketState.Open)
            {
                byte[] msgByte = Encoding.UTF8.GetBytes(message);
                ArraySegment<byte> buffer = new ArraySegment<byte>(msgByte);
                await socket.SendAsync(buffer, WebSocketMessageType.Text, true, ct);
            }
        }
    }
}
