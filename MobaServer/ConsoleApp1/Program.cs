using MobaServer.Transport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;

namespace MobaServer
{
    class Program
    {
        static void Main(string[] args)
        {
            PacketParser parser = new PacketParser();
            SocketHandler socket = new SocketHandler(parser);

            while (true)
            {
            }
        }
    }
}
