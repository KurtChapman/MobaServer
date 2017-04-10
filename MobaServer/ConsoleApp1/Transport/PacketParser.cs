using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobaServer.Transport
{
    public class Packet
    {
        public string data;
    }


    class PacketParser
    {
        const int PACKET_SIZE = 2;
        private byte[] buffer;

        public PacketParser()
        {
            buffer = new byte[0];
        }

        public void OnData(byte[] dataBuffer)
        {
            dataBuffer = MergeDataBuffers(dataBuffer);

            List<Packet> packets = new List<Packet>();
            //extract the size
            ushort dataSize = GetSizeOfCurrentPacket(dataBuffer);
            while (dataSize > 0)
            {
                if (dataSize > dataBuffer.Length)
                {
                    buffer = new byte[dataBuffer.Length];
                    buffer = dataBuffer;
                    break;
                }

                var packet = ReadBytesFromBuffer(dataSize, dataBuffer);
                packets.Add(packet);
                //move onto the next packet in the buffer
                Array.Clear(dataBuffer, 0, PACKET_SIZE + dataSize);
                dataSize = GetSizeOfCurrentPacket(dataBuffer);
            }
            HandlePackets(packets);
        }

        private byte[] MergeDataBuffers(byte[] receivedBuffer)
        {
            //add any left over buffer to the end of the array
            receivedBuffer = buffer.Concat(receivedBuffer).ToArray();
            //clear the internal buffer
            Array.Clear(buffer, 0, buffer.Length);
            return receivedBuffer;
        }

        private ushort GetSizeOfCurrentPacket(byte[] dataBuffer)
        {
            ushort dataSize = BitConverter.ToUInt16(new byte[2] { (byte)dataBuffer[1], (byte)dataBuffer[0] }, 0);
            return dataSize;
        }

        private Packet ReadBytesFromBuffer(ushort count, byte[] dataBuffer)
        {
            //get the data
            string packetString = System.Text.Encoding.UTF8.GetString(dataBuffer, PACKET_SIZE, count);
            //generate a packet from the data
            Packet packet = new Packet();
            packet.data = packetString;
            return packet;
        }

        private void HandlePackets(List<Packet> packets)
        {
            //print the recv'd packets
            foreach (var p in packets)
            {
                Console.WriteLine(p.data);
            }
        }
    }
}
