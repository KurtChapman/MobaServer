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
		  private PacketQueue packetQueue;
		  private ClientSocket socket;
		  private int id;

		  public Client(Socket socket)
		  {
				this.packetParser = new PacketParser();
				this.packetQueue = new PacketQueue(packetParser);
				this.socket = new ClientSocket(socket, packetParser);
				this.id = socket.GetHashCode();
		  }
	 }
}
