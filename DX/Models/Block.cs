using DX.Common;
using System;
using System.Text;

namespace DX.Models
{
	public class Block
	{
		// 4byte
		public byte[] BlockType;
		// 4byte
		public int BlockTotalLengthHead;
		/* variable length, aligned to 32 bits */
		public byte[] BlockBody;
		// 4byte
		public int BlockTotalLengthEnd;

		public Block(byte[] byteArray)
		{
			BlockType = Tools.GetPartialBytes(byteArray, 0, 4);
			BlockTotalLengthHead = Tools.BytesToInt(byteArray, 4);
			BlockBody = Tools.GetPartialBytes(byteArray, 8, BlockTotalLengthHead - 12);
			BlockTotalLengthEnd = BlockTotalLengthHead;
		}
	}

	public class BlockBody
	{
		// 4byte
		public int InterfaceID;
		// 4byte
		public int Timestamp_High;
		// 4byte
		public int Timestamp_Low;
		// 4byte
		public int CapturedLen;
		// 4byte
		public int PacketLen;

		public byte[] PacketData;

		public ulong Time;


		public BlockBody(byte[] BlockBody)
		{
			InterfaceID = Tools.BytesToInt(BlockBody, 0);

			Time = Tools.BytesToUlong3(BlockBody, 4);

			CapturedLen = Tools.BytesToInt(BlockBody, 12);
			PacketLen = BitConverter.ToInt32(BlockBody, 16);
			

			PacketData = Tools.GetPartialBytes(BlockBody, 20, CapturedLen);
		}
	}

	public class PacketData 
	{

		public int IP_Protocol { get; set; }
		public int[] IP_SourceAddress { get; set; }
		public int[] IP_DestinationAddress { get; set; }
		public int TCP_SourcePort { get; set; }
		public int TCP_DestinationPort { get; set; }
		public string HTTP { get; set; }
		public ulong Time { get; set; }


		public PacketData(byte[] PacketData) 
		{
			
			IP_Protocol = Convert.ToInt32(PacketData[23]);

			IP_SourceAddress = new int[4] { Convert.ToInt32(PacketData[26]), 
				                            Convert.ToInt32(PacketData[27]),
										    Convert.ToInt32(PacketData[28]),
										    Convert.ToInt32(PacketData[29]),};

			IP_DestinationAddress = new int[4] { Convert.ToInt32(PacketData[30]),
										         Convert.ToInt32(PacketData[31]),
										         Convert.ToInt32(PacketData[32]),
										         Convert.ToInt32(PacketData[33]),};

			TCP_SourcePort = Tools.Bytes2ToInt(PacketData[34], PacketData[35]);
			TCP_DestinationPort = Tools.Bytes2ToInt(PacketData[36], PacketData[37]);


			

			if (PacketData.Length>54)
            {
                
					HTTP = Encoding.ASCII.GetString(Tools.GetPartialBytes(PacketData, 54, Tools.Bytes2ToInt(PacketData[16], PacketData[17]) - 40));
				
                
				// int x = Tools.bytes2ToInt(PacketData[16], PacketData[17]) - 40;
				
            }
            else
            {
				HTTP = "";

			}
			
		}
	}

}
	
