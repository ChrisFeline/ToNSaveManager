using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text.Json;

namespace ToNSaveManager.Utils
{
    internal enum XSOMessageType : int
    {
        Default = 0,
        NotificationPopup = 1,
        MediaPlayerInfo = 2
    }

    internal struct XSOMessage
    {
        public XSOMessageType messageType { get; set; } = XSOMessageType.NotificationPopup;
        public int index { get; set; } = 0; // Only used for Media Player, changes the icon on the wrist.
        public float timeout { get; set; } = 2f;
        public string title { get; set; }
        public string? content { get; set; }
        public string? icon { get; set; }
        public bool useBase64Icon { get; set; } = true;
        public string sourceApp { get; } = "ToNSaveManager";

        public XSOMessage(string title)
        {
            this.title = title;
        }

        public byte[] ToByteArray()
        {
            return JsonSerializer.SerializeToUtf8Bytes(this);
        }

        public override string ToString()
        {
            return title;
        }
    }

    internal class XSOverlay
    {
        public const int DefaultPort = 42069;
        public readonly Socket BroadcastSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        public readonly IPEndPoint EndPoint;

        private readonly string? Base64Icon;

        public XSOverlay() {
            IPAddress broadcastIP = IPAddress.Parse("127.0.0.1"); // localhost
            EndPoint = new IPEndPoint(broadcastIP, DefaultPort);

            Stream? stream = Program.GetEmbededResource("xs_icon.png");
            if (stream == null) return;

            using (MemoryStream memoryStream = new MemoryStream())
            {
                stream.CopyTo(memoryStream);
                byte[] byteArray = memoryStream.ToArray();
                Base64Icon = Convert.ToBase64String(byteArray);
            }

            stream.Dispose();
        }

        public void SetPort(int port)
        {
            EndPoint.Port = port;
        }

        private void Send(XSOMessage message)
        {
            message.icon = Base64Icon;

            try
            {
                BroadcastSocket.SendTo(message.ToByteArray(), EndPoint);
            } catch (Exception ex)
            {
                Debug.WriteLine("[XSOverlay] Exception:\n" + ex);
            }
        }
        public void Send(string title)
        {
            Send(new XSOMessage(title));
        }
        public void Send(string title, float timeout)
        {
            Send(new XSOMessage(title) { timeout = timeout });
        }
    }
}
