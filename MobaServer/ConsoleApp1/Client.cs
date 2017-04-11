using MobaServer.Transport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MobaServer
{
	 class Client
	 {
		  private PacketParser packetParser;
		  private ClientSocket socket;

		  public Client(Socket socket)
		  {
				this.packetParser = new PacketParser();
				this.socket = new ClientSocket(socket, packetParser);
		  }
	 }
}
