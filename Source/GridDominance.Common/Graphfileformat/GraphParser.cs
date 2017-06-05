using System;
using System.Collections.Generic;
using GridDominance.Graphfileformat.Blueprint;
using GridDominance.SAMScriptParser;

namespace GridDominance.Graphfileformat
{
	public class GraphParser : SSParser
	{
		private INodeBlueprint _currentNode;
		private float _scaleFactor = 1f;
		private readonly string content;
		private readonly Func<string, string> includeFinderFunction;

		private GraphBlueprint _result = null;

		public GraphParser(string graphContent, Func<string, string> includeFunction)
		{
			content = graphContent;
			includeFinderFunction = includeFunction;

			DefineMethod("alias", DefineAlias);
			DefineMethod("include", IncludeSource);
			DefineMethod("scale", SetScale);

			DefineMethod("node", AddNode);
			DefineMethod("root", AddRootNode);
			DefineMethod("warp", AddWarpNode);
			DefineMethod("connect", AddPipe);
		}

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

		private void IncludeSource(List<string> methodParameter)
		{
			var fileName = ExtractStringParameter(methodParameter, 0);
			var fileContent = includeFinderFunction(fileName);

			if (fileContent == null) throw new Exception("Include not found: " + fileName);

			SubParse(fileName, fileContent);
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

		private void AddWarpNode(List<string> methodParameter)
		{
			var pos = ExtractVec2fParameter(methodParameter, 0);
			var target = ExtractGuidParameter(methodParameter, 1);

			var px = pos.Item1 * _scaleFactor;
			var py = pos.Item2 * _scaleFactor;

			var node = new WarpNodeBlueprint(px, py, target);

			_currentNode = node;
			_result.WarpNodes.Add(node);
		}

		private void AddRootNode(List<string> methodParameter)
		{
			var pos       = ExtractVec2fParameter(methodParameter, 0);
			var worldid   = ExtractGuidParameter(methodParameter, 1);

			var px = pos.Item1 * _scaleFactor;
			var py = pos.Item2 * _scaleFactor;

			var node = new RootNodeBlueprint(px, py, worldid);

			_currentNode = node;
			_result.RootNode = node;
		}

		private void AddPipe(List<string> methodParameter)
		{
			if (_currentNode == null) throw new Exception("Pipe without Node");

			var id = ExtractGuidParameter(methodParameter, 0);
			PipeBlueprint.Orientation o;
			byte p = 255;

			string orientation = ExtractValueParameter(methodParameter, 1).ToLower();

			if (orientation == "auto") o = PipeBlueprint.Orientation.Auto;
			else if (orientation == "lin") o = PipeBlueprint.Orientation.Direct;
			else if (orientation == "cw") o = PipeBlueprint.Orientation.Clockwise;
			else if (orientation == "ccw") o = PipeBlueprint.Orientation.Counterclockwise;
			else throw new Exception("Not definied PipeBlueprint.Orientation: " + orientation);
			
			if (ExtractIntegerParameter(methodParameter, 2, -1) > 0) p = (byte)ExtractIntegerParameter(methodParameter, 2);

			_currentNode.Pipes.Add(new PipeBlueprint(id, o, p));
		}

	}
}
