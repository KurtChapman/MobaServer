using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobaServer.Transport
{
	 public class WebsocketFrame
	 {
		  public WebsocketFrame()
		  {

		  }

		  //this way is a bit garbage, do it better.
		  Int64 GetDataLength(Byte[] bytes)
		  {
				var size = bytes[1] - 128;
				if (size > 0 && size <= 125)
				{
					 return size;
				}
				else if (size <= 126 && size <= 65535)
				{
					 size = bytes[1] << bytes[2];
					 return size;
				}
				else
				{
					 size = bytes[1] << bytes[2] << bytes[3] << bytes[4] << bytes[5] << bytes[6] << bytes[7] << bytes[8];
					 return size;
				}
		  }

		  public byte[] FrameData(byte[] bytes, int bytesRead)
		  {
				//have less magic numbers here and do it properly.
				var size = GetDataLength(bytes);
				Byte[] decoded = new Byte[size];
				Byte[] encoded = new Byte[size];
				Array.Copy(bytes, 2 + 4, encoded, 0, bytesRead - 6);
				Byte[] key = new Byte[4];
				Array.Copy(bytes, 2, key, 0, 4);
				for (int i = 0; i < encoded.Length; i++)
				{
					 decoded[i] = (Byte)(encoded[i] ^ key[i % 4]);
				}
				return decoded;
		  }
	 }
}
