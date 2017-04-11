using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobaServer.Transport
{
	 class PacketQueue
	 {
		  private List<Packet> processingPackets;

		  public PacketQueue(PacketParser parser)
		  {
				processingPackets = new List<Packet>();
				parser.OnReadData += OnPacket;
		  }

		  private void OnPacket(List<Packet> packets)
		  {
				foreach(var p in packets)
				{
					 Console.WriteLine("Queuing for processing: " + p.data);
					 processingPackets.Add(p);
					 //process in a thread?
				}
		  }
	 }
}
