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
		  private PacketParser parser;
		  private string testData;

        private byte[] GeneratePacket(string packetContents)
        {
            byte[] helloWorld = Encoding.ASCII.GetBytes(packetContents);
            byte[] intBytes = BitConverter.GetBytes((ushort)packetContents.Length);
            Array.Reverse(intBytes);
            var packet = intBytes.Concat(helloWorld).ToArray();
            return packet;
        }

		  List<Packet> packets;
		  private void OnRead(List<Packet> packets)
		  {
				this.packets = packets;
		  }


		  [TestInitialize()]
		  public void Init()
		  {
				parser = new PacketParser();
				parser.OnReadData += OnRead;
				testData = "testData";
		  }


        [TestMethod()]
        public void OnDataCompletePacketTest()
        {            
            //exec
            parser.OnData(GeneratePacket(testData), 0);
            //verify
            Assert.AreEqual(packets.Count, 1);
            Assert.AreEqual(packets[0].data, testData);
        }

        [TestMethod()]
        public void OnDataTwoPacketsTest()
        {
            //setup
            var byteArray = GeneratePacket(testData);
            byteArray = byteArray.Concat(GeneratePacket(testData)).ToArray();
            //exec
            parser.OnData(byteArray, 0);
				//verify
				Assert.AreEqual(packets.Count, 2);
            Assert.AreEqual(packets[0].data, testData);
            Assert.AreEqual(packets[1].data, testData);
        }

        [TestMethod()]
        public void OnDataTruncatedPacketTest()
        {
            //setup
            var byteArray = GeneratePacket(testData);
            byte[] intBytes = BitConverter.GetBytes((ushort)testData.Length);
            Array.Reverse(intBytes);
            byteArray = byteArray.Concat(intBytes).ToArray();
            //exec
            parser.OnData(byteArray, 0);
				//verify
				Assert.AreEqual(packets.Count, 1);
            Assert.AreEqual(packets[0].data, testData);
        }

        [TestMethod()]
        public void OnDataMultipleReadsPacketTest()
        {
            //setup
            var byteArray = GeneratePacket(testData);
            byte[] intBytes = BitConverter.GetBytes((ushort)testData.Length);
            Array.Reverse(intBytes);
            byteArray = byteArray.Concat(intBytes).ToArray();
            //exec1 (add the first packet with some truncated data)
            parser.OnData(byteArray, 0);
				//verify1
				Assert.AreEqual(packets.Count, 1);
				Assert.AreEqual(packets[0].data, testData);
				//exec2 (add another partial packet)
				byteArray = Encoding.ASCII.GetBytes(testData);
            parser.OnData(byteArray, 0);
				//verify
				Assert.AreEqual(packets.Count, 1);
            Assert.AreEqual(packets[0].data, testData);
        }
    }
}