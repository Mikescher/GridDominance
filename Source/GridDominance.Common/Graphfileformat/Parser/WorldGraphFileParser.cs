using GridDominance.SAMScriptParser;
using System;
using System.Collections.Generic;

namespace GridDominance.Graphfileformat.Parser
{
	public class WorldGraphFileParser : SSParser
	{
		private WGNode? _currentNode;
		private float _scaleFactor = 1f;
		private readonly string content;

		private WorldGraphFile _result = null;

		public WorldGraphFileParser(string graphContent)
		{
			content = graphContent;

			DefineMethod("alias", DefineAlias);
			DefineMethod("scale", SetScale);
			DefineMethod("node", AddNode);
			DefineMethod("connect", AddPipe);
		}

		//TODO Define Root and final node
		//TODO test for loops (forbidden - breaks some algos in map)
		//TODO test all nodes reachable
		public WorldGraphFile Parse(string fileName = "__root__")
		{
			_result = new WorldGraphFile();
			_scaleFactor = 1f;
			_currentNode = null;

			StartParse(fileName, content);

			return _result;
		}

		private void DefineAlias(List<string> methodParameter)
		{
			var key = ExtractValueParameter(methodParameter, 0);
			var val = ExtractValueParameter(methodParameter, 1);

			AddAlias(key, val);
		}

		private void SetScale(List<string> methodParameter)
		{
			_scaleFactor = ExtractNumberParameter(methodParameter, 0);
		}

		private void AddNode(List<string> methodParameter)
		{
			var pos = ExtractVec2fParameter(methodParameter, 0);
			var id = ExtractGuidParameter(methodParameter, 1);

			var px = pos.Item1 * _scaleFactor;
			var py = pos.Item2 * _scaleFactor;

			_currentNode = new WGNode(px, py, id);
			_result.Nodes.Add(_currentNode.Value);
		}

		private void AddPipe(List<string> methodParameter)
		{
			if (_currentNode == null) throw new Exception("Pipe without Node");

			var id = ExtractGuidParameter(methodParameter, 0);
			var o = WGPipe.Orientation.Auto;
			if (ExtractValueParameter(methodParameter, 1, "").ToLower() == "cw") o = WGPipe.Orientation.Clockwise;
			if (ExtractValueParameter(methodParameter, 1, "").ToLower() == "ccw") o = WGPipe.Orientation.Counterclockwise;

			_currentNode.Value.OutgoingPipes.Add(new WGPipe(id, o));
		}

	}
}
