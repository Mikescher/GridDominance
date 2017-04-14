using System;
using System.Collections.Generic;
using System.IO;

namespace GridDominance.Graphfileformat.Parser
{
	public class WorldGraphFile
	{
		public readonly List<WGNode> Nodes = new List<WGNode>();

		public WorldGraphFile()
		{
			//
		}
		
		public void BinarySerialize(BinaryWriter bw)
		{
			bw.Write((byte)0x66);
			bw.Write((byte)Nodes.Count);

			for (int i = 0; i < Nodes.Count; i++)
			{
				bw.Write(Nodes[i].X);
				bw.Write(Nodes[i].Y);
				bw.Write(Nodes[i].LevelID.ToByteArray());
				bw.Write((byte)Nodes[i].OutgoingPipes.Count);
				for (int j = 0; j < Nodes[i].OutgoingPipes.Count; j++)
				{
					bw.Write(Nodes[i].OutgoingPipes[j].Target.ToByteArray());
					bw.Write((byte)Nodes[i].OutgoingPipes[j].PipeOrientation);
				}
			}

			bw.Write((byte)0xAA);
			bw.Write((byte)0x08);
			bw.Write((byte)0xFF);
			bw.Write((byte)0x26);
		}

		public void BinaryDeserialize(BinaryReader br)
		{
			var header = br.ReadByte();
			if (header != 0x66) throw new Exception("Missing header");

			int ncount = br.ReadByte();

			Nodes.Clear();
			for (int i = 0; i < ncount; i++)
			{
				var nx = br.ReadSingle();
				var ny = br.ReadSingle();
				var nid = new Guid(br.ReadBytes(16));
				
				var node = new WGNode(nx, ny, nid);

				int pcount = br.ReadByte();
				for (int j = 0; j < pcount; j++)
				{
					var b = br.ReadBytes(16);
					var pid = new Guid(b);
					var por = (WGPipe.Orientation)br.ReadByte();
					node.OutgoingPipes.Add(new WGPipe(pid, por));
				}
				Nodes.Add(node);
			}

			if (br.ReadByte() != 0xAA) throw new Exception("Missing footer byte 1");
			if (br.ReadByte() != 0x08) throw new Exception("Missing footer byte 2");
			if (br.ReadByte() != 0xFF) throw new Exception("Missing footer byte 3");
			if (br.ReadByte() != 0x26) throw new Exception("Missing footer byte 4");
		}
	}
}
