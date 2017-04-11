using System;
using System.Net.Sockets;

namespace MobaServer.Transport
{
    class ClientSocket
    {
        private static int READ_BUFFER_SIZE = 1024;
        private Socket socket;
		  private PacketParser packetParser;
        private byte[] readBuffer = new byte[READ_BUFFER_SIZE];

		  public ClientSocket(Socket socket, PacketParser parser)
        {
            this.socket = socket;
				this.packetParser = parser;
				this.socket.BeginReceive(readBuffer, 0, READ_BUFFER_SIZE, 0, new AsyncCallback(OnRead), null);
        }

		  private void OnRead(IAsyncResult result)
		  {
				// Read data from the client socket.   
				int bytesRead = socket.EndReceive(result);
				if (bytesRead > 0)
				{
					 packetParser.OnData(readBuffer, 0);
					 Array.Clear(readBuffer, 0, readBuffer.Length);
				}

				//start listening again.
				socket.BeginReceive(readBuffer, 0, READ_BUFFER_SIZE, 0, new AsyncCallback(OnRead), null);
		  }
	 }
}
