using GridDominance.SAMScriptParser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace GridDominance.Graphfileformat.Parser
{
	public class WorldGraphFile
	{
		private static readonly Regex REX_ALIAS = new Regex(@"^alias\s+(?<idshort>[^\s]*)\s*=>\s*(?<idlong>[^\s]*)$");
		private static readonly Regex REX_SCALE = new Regex(@"^scale\s+(?<factor>[0-9\.]+)$");
		private static readonly Regex REX_NODE  = new Regex(@"^node\[\s*(?<px>[\-\+]?[0-9\.]+)\s*,\s*(?<py>[\-\+]?[0-9\.]+)\s*\]\s*:\s*(?<id>[^\s]*)$");
		private static readonly Regex REX_PIPE  = new Regex(@"^\s*\->\s*(?<id>[^\s]*)(\s*\(\s*(?<orientation>CW|CCW)\s*\))?");

		private readonly string content;

		public readonly List<WGNode> Nodes = new List<WGNode>();

		private SSParser parser = new SSParser();
		private WGNode? _currentNode;
		private float _scaleFactor = 1f;

		public WorldGraphFile()
		{
			content = string.Empty;

			Init();
		}

		public WorldGraphFile(string levelContent)
		{
			content = levelContent;

			Init();
		}

		private void Init()
		{
			parser.DefineMethod("alias", DefineAlias);
			parser.DefineMethod("scale", SetScale);
			parser.DefineMethod("node", AddNode);
			parser.DefineMethod("connect", AddPipe);
		}

		#region Parsing

		public void Parse()
		{
			Parse("__root__", content);
			//TODO Define Root and final node
			//TODO test for loops (forbidden - breaks some algos in map)
			//TODO test all nodes reachable
		}

		public void Parse(string fileName, string fileContent)
		{
			_scaleFactor = 1f;
			_currentNode = null;
			parser.Parse(fileName, fileContent);
		}

		private void DefineAlias(List<string> methodParameter)
		{
			var key = parser.ExtractValueParameter(methodParameter, 0);
			var val = parser.ExtractValueParameter(methodParameter, 1);

			parser.AddAlias(key, val);
		}

		private void SetScale(List<string> methodParameter)
		{
			_scaleFactor = parser.ExtractNumberParameter(methodParameter, 0);
		}

		private void AddNode(List<string> methodParameter)
		{
			var pos = parser.ExtractVec2fParameter(methodParameter, 0);
			var id = parser.ExtractGuidParameter(methodParameter, 1);

			var px = pos.Item1 * _scaleFactor;
			var py = pos.Item2 * _scaleFactor;

			_currentNode = new WGNode(px, py, id);
			Nodes.Add(_currentNode.Value);
		}

		private void AddPipe(List<string> methodParameter)
		{
			if (_currentNode == null) throw new Exception("Pipe without Node");

			var id = parser.ExtractGuidParameter(methodParameter, 0);
			var o = WGPipe.Orientation.Auto;
			if (parser.ExtractValueParameter(methodParameter, 1, "").ToLower() == "cw") o = WGPipe.Orientation.Clockwise;
			if (parser.ExtractValueParameter(methodParameter, 1, "").ToLower() == "ccw") o = WGPipe.Orientation.Counterclockwise;

			_currentNode.Value.OutgoingPipes.Add(new WGPipe(id, o));
		}

		#endregion
		
		#region Pipeline Serialize

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

		#endregion
	}
}
