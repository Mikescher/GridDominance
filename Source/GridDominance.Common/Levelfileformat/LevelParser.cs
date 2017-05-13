using System;
using System.Collections.Generic;
using GridDominance.Levelfileformat.Blueprint;
using GridDominance.SAMScriptParser;

namespace GridDominance.Levelfileformat
{
	public class LevelParser : SSParser
	{
		private readonly string content;
		private readonly Func<string, string> includeFinderFunction;
		private float _scaleFactor = 1f;
		private int _nextCannonID = 1;

		private LevelBlueprint _result = null;

		public LevelParser(string levelContent, Func<string, string> includeFunction)
		{
			content = levelContent;
			includeFinderFunction = includeFunction;

			DefineMethod("init", InitLevel);
			DefineMethod("scale", SetScale);
			DefineMethod("define", DefineAlias);
			DefineMethod("include", IncludeSource);

			DefineMethod("cannon", AddCannon);

			DefineMethod("voidwall", AddVoidWallFull);
			DefineMethod("voidwall_h", AddVoidWallHorz);
			DefineMethod("voidwall_v", AddVoidWallVert);
			DefineMethod("voidwall_r", AddVoidWallRot);

			DefineMethod("glasswall_h", AddGlassWallHorz);
			DefineMethod("glasswall_v", AddGlassWallVert);
			DefineMethod("glassblock", AddGlassBlock);

			DefineMethod("voidcircle", AddVoidCircle);

			DefineMethod("blackhole", AddBlackHole);
			DefineMethod("whitehole", AddWhiteHole);

		}

		public LevelBlueprint Parse(string fileName = "__root__")
		{
			_result = new LevelBlueprint();

			_scaleFactor = 1f;
			_nextCannonID = 1;

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

		private void AddCannon(List<string> methodParameter)
		{
			var size     = ExtractNumberParameter(methodParameter, 0) * _scaleFactor;
			var player   = ExtractIntegerParameter(methodParameter, 1);
			var posX     = ExtractVec2fParameter(methodParameter, 2).Item1 * _scaleFactor;
			var posY     = ExtractVec2fParameter(methodParameter, 2).Item2 * _scaleFactor;
			var rotation = ExtractNumberParameter(methodParameter, 3, -1);
			var cannonid = ExtractIntegerParameter(methodParameter, "id", _nextCannonID);

			_nextCannonID = Math.Max(cannonid + 1, _nextCannonID);

			_result.BlueprintCannons.Add(new CannonBlueprint(posX, posY, size, player, rotation, cannonid));
		}

		private void IncludeSource(List<string> methodParameter)
		{
			var fileName = ExtractStringParameter(methodParameter, 0);
			var fileContent = includeFinderFunction(fileName);

			if (fileContent == null) throw new Exception("Include not found: " + fileName);

			SubParse(fileName, fileContent);
		}

		private void InitLevel(List<string> methodParameter)
		{
			var levelname = ExtractStringParameter(methodParameter,  0);
			var leveldesc = ExtractStringParameter(methodParameter,  1);
			var levelguid = ExtractGuidParameter(methodParameter,    2);
			var levelki   = ExtractIntegerParameter(methodParameter, 3);

			_result.Name = levelname;
			_result.FullName = leveldesc;
			_result.UniqueID = levelguid;
			_result.KIType = (byte)levelki;
		}

		private void SetScale(List<string> methodParameter)
		{
			_scaleFactor = ExtractNumberParameter(methodParameter, 0);
		}

		private void AddVoidWallFull(List<string> methodParameter)
		{
			var x1 = ExtractVec2fParameter(methodParameter, 0).Item1 * _scaleFactor;
			var y1 = ExtractVec2fParameter(methodParameter, 0).Item2 * _scaleFactor;
			var x2 = ExtractVec2fParameter(methodParameter, 1).Item1 * _scaleFactor;
			var y2 = ExtractVec2fParameter(methodParameter, 1).Item2 * _scaleFactor;

			var pcx = (x1 + x2) / 2f;
			var pcy = (y1 + y2) / 2f;
			var len = (float)Math.Sqrt(Math.Pow((x2 - x1), 2) + Math.Pow((y2 - y1), 2));
			var rot = (float)(360 + Math.Atan2(y2 - y1, x2 - x1) * (180 / Math.PI)) % 360;

			_result.BlueprintVoidWalls.Add(new VoidWallBlueprint(pcx, pcy, len, rot));
		}

		private void AddVoidWallHorz(List<string> methodParameter)
		{
			var pcx = ExtractVec2fParameter(methodParameter, 0).Item1 * _scaleFactor;
			var pcy = ExtractVec2fParameter(methodParameter, 0).Item2 * _scaleFactor;
			var len = ExtractNumberParameter(methodParameter, 1) * _scaleFactor;
			var rot = 0;

			_result.BlueprintVoidWalls.Add(new VoidWallBlueprint(pcx, pcy, len, rot));
		}

		private void AddVoidWallVert(List<string> methodParameter)
		{
			var pcx = ExtractVec2fParameter(methodParameter, 0).Item1 * _scaleFactor;
			var pcy = ExtractVec2fParameter(methodParameter, 0).Item2 * _scaleFactor;
			var len = ExtractNumberParameter(methodParameter, 1) * _scaleFactor;
			var rot = 90;

			_result.BlueprintVoidWalls.Add(new VoidWallBlueprint(pcx, pcy, len, rot));
		}

		private void AddVoidWallRot(List<string> methodParameter)
		{
			var pcx = ExtractVec2fParameter(methodParameter, 0).Item1 * _scaleFactor;
			var pcy = ExtractVec2fParameter(methodParameter, 0).Item2 * _scaleFactor;
			var len = ExtractNumberParameter(methodParameter, 1) * _scaleFactor;
			var rot = ExtractNumberParameter(methodParameter, 2);

			_result.BlueprintVoidWalls.Add(new VoidWallBlueprint(pcx, pcy, len, rot));
		}

		private void AddVoidCircle(List<string> methodParameter)
		{
			var pcx = ExtractVec2fParameter(methodParameter, 0).Item1 * _scaleFactor;
			var pcy = ExtractVec2fParameter(methodParameter, 0).Item2 * _scaleFactor;
			var dia = ExtractNumberParameter(methodParameter, 1) * _scaleFactor;

			_result.BlueprintVoidCircles.Add(new VoidCircleBlueprint(pcx, pcy, dia));
		}

		private void AddGlassWallHorz(List<string> methodParameter)
		{
			var pcx = ExtractVec2fParameter(methodParameter, 0).Item1 * _scaleFactor;
			var pcy = ExtractVec2fParameter(methodParameter, 0).Item2 * _scaleFactor;
			var len = ExtractNumberParameter(methodParameter, 1) * _scaleFactor;

			_result.BlueprintGlassBlocks.Add(new GlassBlockBlueprint(pcx, pcy, len, GlassBlockBlueprint.DEFAULT_WIDTH));
		}

		private void AddGlassWallVert(List<string> methodParameter)
		{
			var pcx = ExtractVec2fParameter(methodParameter, 0).Item1 * _scaleFactor;
			var pcy = ExtractVec2fParameter(methodParameter, 0).Item2 * _scaleFactor;
			var len = ExtractNumberParameter(methodParameter, 1) * _scaleFactor;

			_result.BlueprintGlassBlocks.Add(new GlassBlockBlueprint(pcx, pcy, GlassBlockBlueprint.DEFAULT_WIDTH, len));
		}

		private void AddGlassBlock(List<string> methodParameter)
		{
			var pcx = ExtractVec2fParameter(methodParameter, 0).Item1 * _scaleFactor;
			var pcy = ExtractVec2fParameter(methodParameter, 0).Item2 * _scaleFactor;
			var w = ExtractNumberParameter(methodParameter, 1) * _scaleFactor;
			var h = ExtractNumberParameter(methodParameter, 2) * _scaleFactor;

			_result.BlueprintGlassBlocks.Add(new GlassBlockBlueprint(pcx, pcy, w, h));
		}

		private void AddBlackHole(List<string> methodParameter)
		{
			var pcx = ExtractVec2fParameter(methodParameter, 0).Item1 * _scaleFactor;
			var pcy = ExtractVec2fParameter(methodParameter, 0).Item2 * _scaleFactor;
			var dia = ExtractNumberParameter(methodParameter, 1) * _scaleFactor;
			var power = ExtractNumberParameter(methodParameter, 2, dia * BlackHoleBlueprint.DEFAULT_POWER_FACTOR);

			_result.BlueprintBlackHoles.Add(new BlackHoleBlueprint(pcx, pcy, dia, -power));
		}

		private void AddWhiteHole(List<string> methodParameter)
		{
			var pcx = ExtractVec2fParameter(methodParameter, 0).Item1 * _scaleFactor;
			var pcy = ExtractVec2fParameter(methodParameter, 0).Item2 * _scaleFactor;
			var dia = ExtractNumberParameter(methodParameter, 1) * _scaleFactor;
			var power = ExtractNumberParameter(methodParameter, 2, dia * BlackHoleBlueprint.DEFAULT_POWER_FACTOR);

			_result.BlueprintBlackHoles.Add(new BlackHoleBlueprint(pcx, pcy, dia, +power));
		}
	}
}
