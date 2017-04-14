using GridDominance.Levelformat.Parser;
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
		public int UniqueID { get; private set; } = -999;

		private Dictionary<string, string> _aliasDict;
		private WGNode? _currentNode;
		private float _scaleFactor = 1f;

		public WorldGraphFile()
		{
			content = string.Empty;
		}

		public WorldGraphFile(string levelContent)
		{
			content = levelContent;
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
			_aliasDict = new Dictionary<string, string>();
			_currentNode = null;
			_scaleFactor = 1f;

			using (StringReader sr = new StringReader(fileContent))
			{
				string rawline;
				int lineNumber = 0;
				while ((rawline = sr.ReadLine()) != null)
				{
					lineNumber++;
					try
					{
						var line = Uncomment(rawline);
						if (line == string.Empty) continue;

						ParseLine(line);
					}
					catch (Exception e)
					{
						throw new ParsingException(fileName, lineNumber, e);
					}
				}
			}
		}

		private void ParseLine(string cntLine)
		{
			for (var m1 = REX_ALIAS.Match(cntLine); m1.Success;)
			{
				_aliasDict.Add(m1.Groups["idshort"].Value.ToLower(), m1.Groups["idlong"].Value);
				return;
			}
			
			for (var m2 = REX_NODE.Match(cntLine); m2.Success;)
			{
				_currentNode = new WGNode(_scaleFactor * float.Parse(m2.Groups["px"].Value), _scaleFactor * float.Parse(m2.Groups["py"].Value), Guid.Parse(DeRef(m2.Groups["id"].Value)));
				Nodes.Add(_currentNode.Value);
				return;
			}
			
			for (var m3 = REX_PIPE.Match(cntLine); m3.Success;)
			{
				if (_currentNode == null) throw new Exception("Pipe without Node");

				var o = WGPipe.Orientation.Auto;
				if (m3.Groups["orientation"] != null && m3.Groups["orientation"].Value.ToLower() == "cw")  o = WGPipe.Orientation.Clockwise;
				if (m3.Groups["orientation"] != null && m3.Groups["orientation"].Value.ToLower() == "ccw") o = WGPipe.Orientation.Counterclockwise;

				_currentNode.Value.OutgoingPipes.Add(new WGPipe(Guid.Parse(DeRef(m3.Groups["id"].Value)), o));
				return;
			}
			
			for (var m4 = REX_SCALE.Match(cntLine); m4.Success;)
			{
				_scaleFactor = float.Parse(m4.Groups["factor"].Value);
				return;
			}

			throw new Exception("Regex match failed");
		}

		private string DeRef(string r)
		{
			if (_aliasDict.ContainsKey(r.ToLower())) return DeRef(_aliasDict[r.ToLower()]);
			return r;
		}

		private string Uncomment(string line)
		{
			int start = -1;
			int end = -1;
			for (int i = 0; i < line.Length; i++)
			{
				if (line[i] == '#')
				{
					if (start < 0) return string.Empty;
					return line.Substring(start, end - start);
				}
				if (line[i] == ' ')
				{
					continue;
				}
				if (line[i] == '\t')
				{
					continue;
				}
				
				if (start == -1) start = i;
				end = i+1;
			}

			if (start == end) return string.Empty;
			return line.Substring(start, end - start);
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
