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


    public class PacketParser
    {
        const int PACKET_SIZE = 2;
        private byte[] unprocessedDataBuffer;
        private List<Packet> parsedPackets;

        public List<Packet> ParsedPackets { get => parsedPackets; }

        public PacketParser()
        {
            unprocessedDataBuffer = new byte[0];
            parsedPackets = new List<Packet>();
        }

        public void OnData(byte[] dataBuffer, int id)
        {
            Console.WriteLine(id.ToString());
            dataBuffer = MergeDataBuffers(dataBuffer);

            List<Packet> packets = new List<Packet>();
            //extract the size
            ushort dataSize = GetSizeOfCurrentPacket(dataBuffer);
            while (dataSize > 0)
            {
                if (dataSize > dataBuffer.Length)
                {
                    unprocessedDataBuffer = new byte[dataBuffer.Length];
                    unprocessedDataBuffer = dataBuffer;
                    break;
                }

                var packet = ReadBytesFromBuffer(dataSize, dataBuffer);
                packets.Add(packet);
                //move onto the next packet in the buffer
                dataBuffer = RemoveRangeFromBuffer(dataBuffer, 0, PACKET_SIZE + dataSize);
                dataSize = GetSizeOfCurrentPacket(dataBuffer);
            }
            HandlePackets(packets);
        }

        private byte[] MergeDataBuffers(byte[] receivedBuffer)
        {
            //add any data wating to be processed to the end of the receieved buffer
            receivedBuffer = unprocessedDataBuffer.Concat(receivedBuffer).ToArray();
            //clear the internal buffer
            Array.Clear(unprocessedDataBuffer, 0, unprocessedDataBuffer.Length);
            return receivedBuffer;
        }

        private ushort GetSizeOfCurrentPacket(byte[] dataBuffer)
        {
            if (dataBuffer.Length < 2)
            {
                return 0;
            }
            else
            {
                ushort dataSize = BitConverter.ToUInt16(new byte[2] { (byte)dataBuffer[1], (byte)dataBuffer[0] }, 0);
                return dataSize;
            }
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
            parsedPackets = parsedPackets.Concat(packets).ToList();

            //print the recv'd packets
            foreach (var p in packets)
            {
                Console.WriteLine("Parsed Packet: " + p.data);
            }
        }

        private byte[] RemoveRangeFromBuffer(byte[] dataBuffer, int start, int end)
        {
            var tempList = dataBuffer.ToList();
            tempList.RemoveRange(start, end);
            return tempList.ToArray();
        }
    }
}
