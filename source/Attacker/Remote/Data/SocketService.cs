using System.Net.Sockets;
using System.Net;
using System.Text;

namespace Remote.Data
{
    public class SocketService
    {
        private TcpListener? _tcpListener;

        public void Start(string ip, int port)
        {
            _tcpListener = new TcpListener(IPAddress.Parse(ip), port);
            _tcpListener.Start();
            _tcpListener.BeginAcceptTcpClient(AsyncCallback, _tcpListener);
            Console.WriteLine("伺服器已啟動，開始監聽");
        }

        /// <summary>
        /// 非同步接收客戶端連線
        /// </summary>
        private void AsyncCallback(IAsyncResult asyncResult)
        {
            if (asyncResult.AsyncState is not TcpListener listener) return;
            TcpClient client = listener.EndAcceptTcpClient(asyncResult);
            Console.WriteLine("客戶端已連線");

            // 開始接收客戶端資料
            NetworkStream stream = client.GetStream();
            byte[] buffer = new byte[1024];
            var read = stream.Read(buffer, 0, buffer.Length);
            var receive = Encoding.UTF8.GetString(buffer, 0, read);
            Console.WriteLine($"接收到客戶端訊息：{receive}");

            // 開始傳送資料給客戶端
            string message = "嗨，我是伺服器";
            byte[] bytes = Encoding.UTF8.GetBytes(message);
            stream.Write(bytes, 0, bytes.Length);
            Console.WriteLine($"發送給客戶端的訊息: {message}");

            // 關閉連線
            stream.Close();
            client.Close();

            // 接收下一個訊息
            listener.BeginAcceptTcpClient(AsyncCallback, listener);
        }

        public void Close()
        {
            _tcpListener?.Stop();
        }
    }
}