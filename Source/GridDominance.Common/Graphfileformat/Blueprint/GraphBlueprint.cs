using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace GridDominance.Graphfileformat.Blueprint
{
	public class GraphBlueprint
	{
		public IEnumerable<INodeBlueprint> AllNodes => new INodeBlueprint[] {RootNode}.Concat(Nodes.Cast<INodeBlueprint>());

		public readonly List<NodeBlueprint> Nodes = new List<NodeBlueprint>();
		public RootNodeBlueprint RootNode = new RootNodeBlueprint(float.NaN, float.NaN);

		public GraphBlueprint()
		{
			//
		}
		
		public void BinarySerialize(BinaryWriter bw)
		{
			bw.Write((byte)0x66);

			bw.Write(RootNode.X);
			bw.Write(RootNode.Y);
			bw.Write((byte)RootNode.OutgoingPipes.Count);
			for (int j = 0; j < RootNode.OutgoingPipes.Count; j++)
			{
				bw.Write(RootNode.OutgoingPipes[j].Target.ToByteArray());
				bw.Write((byte)RootNode.OutgoingPipes[j].PipeOrientation);
				bw.Write(RootNode.OutgoingPipes[j].Priority);
			}

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
					bw.Write(Nodes[i].OutgoingPipes[j].Priority);
				}
			}

			bw.Write((byte)0xB1);
			bw.Write((byte)0x6B);
			bw.Write((byte)0x00);
			bw.Write((byte)0xB5);
		}

		public void BinaryDeserialize(BinaryReader br)
		{
			var header = br.ReadByte();
			if (header != 0x66) throw new Exception("Missing header");

			{
				float nx = br.ReadSingle();
				float ny = br.ReadSingle();
				RootNode = new RootNodeBlueprint(nx, ny);
				int pcount = br.ReadByte();
				for (int j = 0; j < pcount; j++)
				{
					var pid = new Guid(br.ReadBytes(16));
					var por = (PipeBlueprint.Orientation)br.ReadByte();
					var pri = br.ReadByte();
					RootNode.OutgoingPipes.Add(new PipeBlueprint(pid, por, pri));
				}
			}

			int ncount = br.ReadByte();

			Nodes.Clear();
			for (int i = 0; i < ncount; i++)
			{
				var nx = br.ReadSingle();
				var ny = br.ReadSingle();
				var nid = new Guid(br.ReadBytes(16));
				
				var node = new NodeBlueprint(nx, ny, nid);

				int pcount = br.ReadByte();
				for (int j = 0; j < pcount; j++)
				{
					var pid = new Guid(br.ReadBytes(16));
					var por = (PipeBlueprint.Orientation)br.ReadByte();
					var pri = br.ReadByte();
					node.OutgoingPipes.Add(new PipeBlueprint(pid, por, pri));
				}
				Nodes.Add(node);
			}

			if (br.ReadByte() != 0xB1) throw new Exception("Missing footer byte 1");
			if (br.ReadByte() != 0x6B) throw new Exception("Missing footer byte 2");
			if (br.ReadByte() != 0x00) throw new Exception("Missing footer byte 3");
			if (br.ReadByte() != 0xB5) throw new Exception("Missing footer byte 4");
		}

		public void ValidOrThrow()
		{
			// ======== 1 ========

			if (float.IsNaN(RootNode.X) || float.IsNaN(RootNode.Y))
				throw new Exception("Every World needs a root node");

			// ======== 2 ========

			foreach (var pipe in Nodes.SelectMany(n => n.OutgoingPipes).Concat(RootNode.OutgoingPipes))
			{
				if (Nodes.Count(n => n.LevelID == pipe.Target) != 1) throw new Exception("Non-unique pipe target: " + pipe.Target);
			}

			// ======== 3 ========

			foreach (var p in RootNode.OutgoingPipes) WalkGraphAndFindLoops(Nodes.Single(n => n.LevelID == p.Target), new List<NodeBlueprint>());

			// ======== 4 ========

			var nl = Nodes.ToList();
			var ns = new Stack<NodeBlueprint>();
			foreach (var p in RootNode.OutgoingPipes) ns.Push(Nodes.Single(n => n.LevelID == p.Target));
			while (ns.Any())
			{
				var bp = ns.Pop();
				nl.Remove(bp);
				foreach (var p in bp.OutgoingPipes) ns.Push(Nodes.Single(n => n.LevelID == p.Target));
			}
			if (nl.Any()) throw new Exception("Graph has unreachable nodes");
		}

		private void WalkGraphAndFindLoops(NodeBlueprint me, List<NodeBlueprint> history)
		{
			if (history.Contains(me)) throw new Exception("Graph has a directed cycle");

			foreach (var p in me.OutgoingPipes)
				WalkGraphAndFindLoops(Nodes.Single(n => n.LevelID == p.Target), history.Concat(new List<NodeBlueprint>{me}).ToList());
		}
	}
}
