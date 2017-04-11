using Microsoft.VisualStudio.TestTools.UnitTesting;
using MobaServer.Transport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobaServer.Transport.Tests
{
    [TestClass()]
    public class PacketParserTests
    {
        private byte[] GeneratePacket(string packetContents)
        {
            byte[] helloWorld = Encoding.ASCII.GetBytes(packetContents);
            byte[] intBytes = BitConverter.GetBytes((ushort)packetContents.Length);
            Array.Reverse(intBytes);
            var packet = intBytes.Concat(helloWorld).ToArray();
            return packet;
        }

        [TestMethod()]
        public void OnDataCompletePacketTest()
        {
            //setup
            PacketParser parser = new PacketParser();
            string testData = "testData";
            //exec
            parser.OnData(GeneratePacket(testData));
            //verify
            var packets = parser.ParsedPackets;
            Assert.AreEqual(packets.Count, 1);
            Assert.AreEqual(packets[0].data, testData);
        }

        [TestMethod()]
        public void OnDataTwoPacketsTest()
        {
            //setup
            PacketParser parser = new PacketParser();
            string testData = "testData";
            var byteArray = GeneratePacket(testData);
            byteArray = byteArray.Concat(GeneratePacket(testData)).ToArray();
            //exec
            parser.OnData(byteArray);
            //verify
            var packets = parser.ParsedPackets;
            Assert.AreEqual(packets.Count, 2);
            Assert.AreEqual(packets[0].data, testData);
            Assert.AreEqual(packets[1].data, testData);
        }

        [TestMethod()]
        public void OnDataTruncatedPacketTest()
        {
            //setup
            PacketParser parser = new PacketParser();
            string testData = "testData";
            var byteArray = GeneratePacket(testData);
            byte[] intBytes = BitConverter.GetBytes((ushort)testData.Length);
            Array.Reverse(intBytes);
            byteArray = byteArray.Concat(intBytes).ToArray();
            //exec
            parser.OnData(byteArray);
            //verify
            var packets = parser.ParsedPackets;
            Assert.AreEqual(packets.Count, 1);
            Assert.AreEqual(packets[0].data, testData);
        }

        [TestMethod()]
        public void OnDataMultipleReadsPacketTest()
        {
            //setup
            PacketParser parser = new PacketParser();
            string testData = "testData";
            var byteArray = GeneratePacket(testData);
            byte[] intBytes = BitConverter.GetBytes((ushort)testData.Length);
            Array.Reverse(intBytes);
            byteArray = byteArray.Concat(intBytes).ToArray();
            //exec
            parser.OnData(byteArray);
            byteArray = Encoding.ASCII.GetBytes(testData);
            parser.OnData(byteArray);
            //verify
            var packets = parser.ParsedPackets;
            Assert.AreEqual(packets.Count, 2);
            Assert.AreEqual(packets[0].data, testData);
            Assert.AreEqual(packets[1].data, testData);
        }
    }
}