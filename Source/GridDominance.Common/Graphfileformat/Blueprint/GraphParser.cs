using GridDominance.SAMScriptParser;
using System;
using System.Collections.Generic;

namespace GridDominance.Graphfileformat.Blueprint
{
	public class GraphParser : SSParser
	{
		private INodeBlueprint _currentNode;
		private float _scaleFactor = 1f;
		private readonly string content;

		private GraphBlueprint _result = null;

		public GraphParser(string graphContent)
		{
			content = graphContent;

			DefineMethod("alias", DefineAlias);
			DefineMethod("scale", SetScale);
			DefineMethod("node", AddNode);
			DefineMethod("root", AddRootNode);
			DefineMethod("connect", AddPipe);
		}

		//TODO Define Root and final node
		//TODO test for loops (forbidden - breaks some algos in map)
		//TODO test all nodes reachable
		public GraphBlueprint Parse(string fileName = "__root__")
		{
			_result = new GraphBlueprint();

			_scaleFactor = 1f;
			_currentNode = null;

			StartParse(fileName, content);

			_result.ValidOrThrow();

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

			var node = new NodeBlueprint(px, py, id);

			_currentNode = node;
			_result.Nodes.Add(node);
		}

		private void AddRootNode(List<string> methodParameter)
		{
			var pos = ExtractVec2fParameter(methodParameter, 0);

			var px = pos.Item1 * _scaleFactor;
			var py = pos.Item2 * _scaleFactor;

			var node = new RootNodeBlueprint(px, py);

			_currentNode = node;
			_result.RootNode = node;
		}

		private void AddPipe(List<string> methodParameter)
		{
			if (_currentNode == null) throw new Exception("Pipe without Node");

			var id = ExtractGuidParameter(methodParameter, 0);
			var o = PipeBlueprint.Orientation.Auto;
			if (ExtractValueParameter(methodParameter, 1, "").ToLower() == "cw") o = PipeBlueprint.Orientation.Clockwise;
			if (ExtractValueParameter(methodParameter, 1, "").ToLower() == "ccw") o = PipeBlueprint.Orientation.Counterclockwise;

			_currentNode.Pipes.Add(new PipeBlueprint(id, o));
		}

	}
}
