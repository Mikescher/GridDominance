using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GridDominance.Graphfileformat.Blueprint
{
	public class GraphBlueprint
	{
		public IEnumerable<INodeBlueprint> AllNodes => new INodeBlueprint[] {RootNode}.Concat(LevelNodes.Cast<INodeBlueprint>()).Concat(WarpNodes.Cast<INodeBlueprint>());

		public readonly List<NodeBlueprint> LevelNodes = new List<NodeBlueprint>();
		public readonly List<WarpNodeBlueprint> WarpNodes = new List<WarpNodeBlueprint>();
		public RootNodeBlueprint RootNode = new RootNodeBlueprint(float.NaN, float.NaN, Guid.Empty);

		public Guid ID => RootNode.WorldID;

		public GraphBlueprint()
		{
			//
		}
		
		public void BinarySerialize(BinaryWriter bw)
		{
			bw.Write((byte)0x66);

			bw.Write(RootNode.X);
			bw.Write(RootNode.Y);
			bw.Write(RootNode.WorldID.ToByteArray());
			bw.Write((byte)RootNode.OutgoingPipes.Count);
			for (int j = 0; j < RootNode.OutgoingPipes.Count; j++)
			{
				bw.Write(RootNode.OutgoingPipes[j].Target.ToByteArray());
				bw.Write((byte)RootNode.OutgoingPipes[j].PipeOrientation);
				bw.Write(RootNode.OutgoingPipes[j].Priority);
			}

			bw.Write((byte)LevelNodes.Count);
			for (int i = 0; i < LevelNodes.Count; i++)
			{
				bw.Write(LevelNodes[i].X);
				bw.Write(LevelNodes[i].Y);
				bw.Write(LevelNodes[i].LevelID.ToByteArray());
				bw.Write((byte)LevelNodes[i].OutgoingPipes.Count);
				for (int j = 0; j < LevelNodes[i].OutgoingPipes.Count; j++)
				{
					bw.Write(LevelNodes[i].OutgoingPipes[j].Target.ToByteArray());
					bw.Write((byte)LevelNodes[i].OutgoingPipes[j].PipeOrientation);
					bw.Write(LevelNodes[i].OutgoingPipes[j].Priority);
				}
			}

			bw.Write((byte)WarpNodes.Count);
			for (int i = 0; i < WarpNodes.Count; i++)
			{
				bw.Write(WarpNodes[i].X);
				bw.Write(WarpNodes[i].Y);
				bw.Write(WarpNodes[i].TargetWorld.ToByteArray());
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
				var rid = new Guid(br.ReadBytes(16));
				RootNode = new RootNodeBlueprint(nx, ny, rid);
				int pcount = br.ReadByte();
				for (int j = 0; j < pcount; j++)
				{
					var pid = new Guid(br.ReadBytes(16));
					var por = (PipeBlueprint.Orientation)br.ReadByte();
					var pri = br.ReadByte();
					RootNode.OutgoingPipes.Add(new PipeBlueprint(pid, por, pri));
				}
			}

			int ncount1 = br.ReadByte();
			LevelNodes.Clear();
			for (int i = 0; i < ncount1; i++)
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
				LevelNodes.Add(node);
			}

			int ncount2 = br.ReadByte();
			WarpNodes.Clear();
			for (int i = 0; i < ncount2; i++)
			{
				var nx = br.ReadSingle();
				var ny = br.ReadSingle();
				var nid = new Guid(br.ReadBytes(16));

				var node = new WarpNodeBlueprint(nx, ny, nid);

				WarpNodes.Add(node);
			}

			if (br.ReadByte() != 0xB1) throw new Exception("Missing footer byte 1");
			if (br.ReadByte() != 0x6B) throw new Exception("Missing footer byte 2");
			if (br.ReadByte() != 0x00) throw new Exception("Missing footer byte 3");
			if (br.ReadByte() != 0xB5) throw new Exception("Missing footer byte 4");
		}

		public static bool IsIncludeMatch(string a, string b)
		{
			const StringComparison icic = StringComparison.CurrentCultureIgnoreCase;

			if (string.Equals(a, b, icic)) return true;

			if (a.LastIndexOf('.') > 0) a = a.Substring(0, a.LastIndexOf('.'));
			if (b.LastIndexOf('.') > 0) b = b.Substring(0, b.LastIndexOf('.'));

			return string.Equals(a, b, icic);
		}

		public void ValidOrThrow()
		{
			// ======== 1 ========

			if (float.IsNaN(RootNode.X) || float.IsNaN(RootNode.Y))
				throw new Exception("Every World needs a root node");

			if (ID == Guid.Empty)
				throw new Exception("Non valid ID");

			// ======== 2 ========

			foreach (var pipe in LevelNodes.SelectMany(n => n.OutgoingPipes).Concat(RootNode.OutgoingPipes))
			{
				if (AllNodes.Count(n => n.ConnectionID == pipe.Target) < 0) throw new Exception("pipe target not found: " + pipe.Target);
				if (AllNodes.Count(n => n.ConnectionID == pipe.Target) > 1) throw new Exception("Non-unique pipe target: " + pipe.Target);
			}

			// ======== 3 ========

			foreach (var p in RootNode.OutgoingPipes) WalkGraphAndFindLoops(LevelNodes.Single(n => n.LevelID == p.Target), new List<INodeBlueprint>());

			// ======== 4 ========

			var nl = LevelNodes.Cast<INodeBlueprint>().Concat(WarpNodes.Cast<INodeBlueprint>()).ToList();
			var ns = new Stack<INodeBlueprint>();
			foreach (var p in RootNode.OutgoingPipes) ns.Push(LevelNodes.Single(n => n.LevelID == p.Target));
			while (ns.Any())
			{
				var bp = ns.Pop();
				nl.Remove(bp);
				foreach (var p in bp.Pipes) ns.Push(AllNodes.Single(n => n.ConnectionID == p.Target));
			}
			if (nl.Any()) throw new Exception("Graph has unreachable nodes");
		}

		private void WalkGraphAndFindLoops(INodeBlueprint me, List<INodeBlueprint> history)
		{
			if (history.Contains(me)) throw new Exception("Graph has a directed cycle");

			foreach (var p in me.Pipes)
				WalkGraphAndFindLoops(AllNodes.Single(n => n.ConnectionID == p.Target), history.Concat(new List<INodeBlueprint> {me}).ToList());
		}
	}
}
