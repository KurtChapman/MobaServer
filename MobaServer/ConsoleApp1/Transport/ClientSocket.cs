using System;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace MobaServer.Transport
{
    class ClientSocket
    {
        private static int READ_BUFFER_SIZE = 1024;
        private TcpClient tcpClient;
		  private Socket socket;
		  private PacketParser packetParser;
        private byte[] readBuffer = new byte[READ_BUFFER_SIZE];
		  bool open = false;

		  public ClientSocket(TcpClient tcpClient, PacketParser parser)
        {
				this.tcpClient = tcpClient;
				this.packetParser = parser;
				HandleHandshake(tcpClient);
		  }

		  private void HandleHandshake(TcpClient tcpClient)
		  {
				Console.WriteLine("Performing websocket handshake");
				NetworkStream stream = tcpClient.GetStream();
				//enter to an infinite cycle to be able to handle every change in stream
				while (true)
				{
					 while (!stream.DataAvailable) ;

					 Byte[] bytes = new Byte[tcpClient.Available];

					 stream.Read(bytes, 0, bytes.Length);
					 String data = Encoding.UTF8.GetString(bytes);
					 //translate bytes of request to string
					 Console.WriteLine(data);

					 if (new Regex("^GET").IsMatch(data))
					 {
						  Byte[] response = Encoding.UTF8.GetBytes("HTTP/1.1 101 Switching Protocols" + Environment.NewLine
						  + "Connection: Upgrade" + Environment.NewLine
						  + "Upgrade: websocket" + Environment.NewLine
						  + "Sec-WebSocket-Accept: " + Convert.ToBase64String(
								SHA1.Create().ComputeHash(Encoding.UTF8.GetBytes(new Regex("Sec-WebSocket-Key: (.*)").Match(data).Groups[1].Value.Trim() + "258EAFA5-E914-47DA-95CA-C5AB0DC85B11"))) + Environment.NewLine + Environment.NewLine);

						  stream.Write(response, 0, response.Length);
						  socket = tcpClient.Client;
						  socket.BeginReceive(readBuffer, 0, READ_BUFFER_SIZE, 0, new AsyncCallback(OnRead), null);
					 }
					 else
					 {
						  //need to handle partial data here! Could get skewed upgrade header from the client.
					 }
				}

		  }

		  private void OnRead(IAsyncResult result)
		  {
				//Read data from the client socket.
				int bytesRead = socket.EndReceive(result);
				if (bytesRead > 0)
				{
					 WebsocketFrame frame = new WebsocketFrame();
					 var frameData = frame.FrameData(readBuffer, bytesRead);
					 packetParser.OnData(frameData, 0);
					 Array.Clear(readBuffer, 0, readBuffer.Length);
				}

				//start listening again.
				socket.BeginReceive(readBuffer, 0, READ_BUFFER_SIZE, 0, new AsyncCallback(OnRead), null);
		  }
	 }
}
