using System;
using System.Collections.Generic;
using GridDominance.Levelfileformat.Blueprint;
using GridDominance.SAMScriptParser;
using Microsoft.Xna.Framework;

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

			DefineMethod("alias", DefineAlias);
			DefineMethod("define", DefineAlias);
			DefineMethod("include", IncludeSource);

			DefineMethod("init", InitLevel);
			DefineMethod("scale", SetScale);
			DefineMethod("setview", SetView);
			DefineMethod("setwrap", SetWrapMode);
			DefineMethod("config", SetConfig);

			DefineMethod("cannon", AddCannon);
			DefineMethod("laser", AddLaserCannon);

			DefineMethod("voidwall", AddVoidWallFull);
			DefineMethod("voidwall_h", AddVoidWallHorz);
			DefineMethod("voidwall_v", AddVoidWallVert);
			DefineMethod("voidwall_r", AddVoidWallRot);
			DefineMethod("voidcircle", AddVoidCircle);

			DefineMethod("glasswall", AddGlassWall);
			DefineMethod("glasswall_h", AddGlassWallHorz);
			DefineMethod("glasswall_v", AddGlassWallVert);
			DefineMethod("glasswall_r", AddGlassWallRot);
			DefineMethod("glassblock", AddGlassBlock);

			DefineMethod("mirrorwall", AddMirrorWall);
			DefineMethod("mirrorwall_h", AddMirrorWallHorz);
			DefineMethod("mirrorwall_v", AddMirrorWallVert);
			DefineMethod("mirrorwall_r", AddMirrorWallRot);
			DefineMethod("mirrorblock", AddMirrorBlock);
			DefineMethod("mirrorcircle", AddMirrorCircle);

			DefineMethod("blackhole", AddBlackHole);
			DefineMethod("whitehole", AddWhiteHole);

			DefineMethod("portal", AddPortal);


			DefineMethod("text", AddText);
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

			_result.BlueprintCannons.Add(new CannonBlueprint(posX, posY, size, player, rotation, (byte)cannonid));
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
			var pc1x = ExtractVec2fParameter(methodParameter, 0).Item1 * _scaleFactor;
			var pc1y = ExtractVec2fParameter(methodParameter, 0).Item2 * _scaleFactor;
			var pc2x = ExtractVec2fParameter(methodParameter, 1).Item1 * _scaleFactor;
			var pc2y = ExtractVec2fParameter(methodParameter, 1).Item2 * _scaleFactor;

			var tstart = ExtractNumberParameter(methodParameter, "trim_start", 0) * _scaleFactor;
			var tend = ExtractNumberParameter(methodParameter, "trim_end", 0) * _scaleFactor;
			var offsetX = ExtractVec2fParameter(methodParameter, "offset", Tuple.Create(0f, 0f)).Item1 * _scaleFactor;
			var offsetY = ExtractVec2fParameter(methodParameter, "offset", Tuple.Create(0f, 0f)).Item2 * _scaleFactor;
			var superrot = ExtractNumberParameter(methodParameter, "rot", 0);
			var offsetNorm = ExtractNumberParameter(methodParameter, "normal_offset", 0) * _scaleFactor;

			var vs = new Vector2(pc1x, pc1y);
			var ve = new Vector2(pc2x, pc2y);

			var nrm = ve - vs;
			nrm.Normalize();


			vs = vs + nrm * tstart;
			ve = ve - nrm * tend;

			var d = ve - vs;
			var c = (vs + ve) / 2;

			var onn = new Vector2(-nrm.Y, nrm.X) * offsetNorm;

			var r = (float)((Math.Atan2(d.Y, d.X) + Math.PI + Math.PI) % (Math.PI + Math.PI));
			r *= 360f / (float)(Math.PI + Math.PI);

			_result.BlueprintVoidWalls.Add(new VoidWallBlueprint(c.X + offsetX + onn.X, c.Y + offsetY + onn.Y, d.Length(), r + superrot));
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

		private void AddGlassWall(List<string> methodParameter)
		{
			var pc1x = ExtractVec2fParameter(methodParameter, 0).Item1 * _scaleFactor;
			var pc1y = ExtractVec2fParameter(methodParameter, 0).Item2 * _scaleFactor;
			var pc2x = ExtractVec2fParameter(methodParameter, 1).Item1 * _scaleFactor;
			var pc2y = ExtractVec2fParameter(methodParameter, 1).Item2 * _scaleFactor;

			var tstart     = ExtractNumberParameter(methodParameter, "trim_start", 0) * _scaleFactor;
			var tend       = ExtractNumberParameter(methodParameter, "trim_end", 0) * _scaleFactor;
			var offsetX    = ExtractVec2fParameter(methodParameter,  "offset", Tuple.Create(0f, 0f)).Item1 * _scaleFactor;
			var offsetY    = ExtractVec2fParameter(methodParameter,  "offset", Tuple.Create(0f, 0f)).Item2 * _scaleFactor;
			var superrot   = ExtractNumberParameter(methodParameter, "rot", 0);
			var width      = ExtractNumberParameter(methodParameter, "width", GlassBlockBlueprint.DEFAULT_WIDTH);
			var offsetNorm = ExtractNumberParameter(methodParameter, "normal_offset", 0) * _scaleFactor;

			var vs = new Vector2(pc1x, pc1y);
			var ve = new Vector2(pc2x, pc2y);

			var nrm = ve - vs;
			nrm.Normalize();


			vs = vs + nrm * tstart;
			ve = ve - nrm * tend;

			var d = ve - vs;
			var c = (vs + ve) / 2;

			var onn = new Vector2(-nrm.Y, nrm.X) * offsetNorm;

			var r = (float)((Math.Atan2(d.Y, d.X) + Math.PI + Math.PI) % (Math.PI + Math.PI));
			r *= 360f / (float)(Math.PI + Math.PI);

			_result.BlueprintGlassBlocks.Add(new GlassBlockBlueprint(c.X + offsetX + onn.X, c.Y + offsetY + onn.Y, d.Length(), width, r + superrot));
		}

		private void AddGlassWallHorz(List<string> methodParameter)
		{
			var pcx = ExtractVec2fParameter(methodParameter, 0).Item1 * _scaleFactor;
			var pcy = ExtractVec2fParameter(methodParameter, 0).Item2 * _scaleFactor;
			var len = ExtractNumberParameter(methodParameter, 1) * _scaleFactor;

			_result.BlueprintGlassBlocks.Add(new GlassBlockBlueprint(pcx, pcy, len, GlassBlockBlueprint.DEFAULT_WIDTH, 0));
		}

		private void AddGlassWallVert(List<string> methodParameter)
		{
			var pcx = ExtractVec2fParameter(methodParameter, 0).Item1 * _scaleFactor;
			var pcy = ExtractVec2fParameter(methodParameter, 0).Item2 * _scaleFactor;
			var len = ExtractNumberParameter(methodParameter, 1) * _scaleFactor;

			_result.BlueprintGlassBlocks.Add(new GlassBlockBlueprint(pcx, pcy, GlassBlockBlueprint.DEFAULT_WIDTH, len, 0));
		}

		private void AddGlassWallRot(List<string> methodParameter)
		{
			var pcx = ExtractVec2fParameter(methodParameter, 0).Item1 * _scaleFactor;
			var pcy = ExtractVec2fParameter(methodParameter, 0).Item2 * _scaleFactor;
			var len = ExtractNumberParameter(methodParameter, 1) * _scaleFactor;
			var rot = ExtractNumberParameter(methodParameter, 2);

			_result.BlueprintGlassBlocks.Add(new GlassBlockBlueprint(pcx, pcy, GlassBlockBlueprint.DEFAULT_WIDTH, len, rot));
		}

		private void AddGlassBlock(List<string> methodParameter)
		{
			var pcx = ExtractVec2fParameter(methodParameter, 0).Item1 * _scaleFactor;
			var pcy = ExtractVec2fParameter(methodParameter, 0).Item2 * _scaleFactor;
			var w   = ExtractNumberParameter(methodParameter, 1) * _scaleFactor;
			var h   = ExtractNumberParameter(methodParameter, 2) * _scaleFactor;
			var r   = ExtractNumberParameter(methodParameter, 3, 0f);

			_result.BlueprintGlassBlocks.Add(new GlassBlockBlueprint(pcx, pcy, w, h, r));
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

		private void AddPortal(List<string> methodParameter)
		{
			var grp = (short)ExtractIntegerParameter(methodParameter, 0);
			var sid = ExtractBooleanParameter(methodParameter, 1);
			var pcx = ExtractVec2fParameter(methodParameter, 2).Item1 * _scaleFactor;
			var pcy = ExtractVec2fParameter(methodParameter, 2).Item2 * _scaleFactor;
			var len = ExtractNumberParameter(methodParameter, 3) * _scaleFactor;
			var nrm = ExtractNumberParameter(methodParameter, 4);

			_result.BlueprintPortals.Add(new PortalBlueprint(pcx, pcy, len, nrm, grp, sid));
		}
		
		private void AddMirrorWallHorz(List<string> methodParameter)
		{
			var pcx = ExtractVec2fParameter(methodParameter, 0).Item1 * _scaleFactor;
			var pcy = ExtractVec2fParameter(methodParameter, 0).Item2 * _scaleFactor;
			var len = ExtractNumberParameter(methodParameter, 1) * _scaleFactor;

			_result.BlueprintMirrorBlocks.Add(new MirrorBlockBlueprint(pcx, pcy, len, MirrorBlockBlueprint.DEFAULT_WIDTH, 0));
		}

		private void AddMirrorWallVert(List<string> methodParameter)
		{
			var pcx = ExtractVec2fParameter(methodParameter, 0).Item1 * _scaleFactor;
			var pcy = ExtractVec2fParameter(methodParameter, 0).Item2 * _scaleFactor;
			var len = ExtractNumberParameter(methodParameter, 1) * _scaleFactor;

			_result.BlueprintMirrorBlocks.Add(new MirrorBlockBlueprint(pcx, pcy, len, MirrorBlockBlueprint.DEFAULT_WIDTH, 90));
		}

		private void AddMirrorWallRot(List<string> methodParameter)
		{
			var pcx = ExtractVec2fParameter(methodParameter, 0).Item1 * _scaleFactor;
			var pcy = ExtractVec2fParameter(methodParameter, 0).Item2 * _scaleFactor;
			var len = ExtractNumberParameter(methodParameter, 1) * _scaleFactor;
			var rot = ExtractNumberParameter(methodParameter, 2);

			_result.BlueprintMirrorBlocks.Add(new MirrorBlockBlueprint(pcx, pcy, len, MirrorBlockBlueprint.DEFAULT_WIDTH, rot));
		}

		private void AddMirrorBlock(List<string> methodParameter)
		{
			var pcx = ExtractVec2fParameter(methodParameter, 0).Item1 * _scaleFactor;
			var pcy = ExtractVec2fParameter(methodParameter, 0).Item2 * _scaleFactor;
			var w   = ExtractNumberParameter(methodParameter, 1) * _scaleFactor;
			var h   = ExtractNumberParameter(methodParameter, 2) * _scaleFactor;
			var rot = ExtractNumberParameter(methodParameter, 3, 0f);

			_result.BlueprintMirrorBlocks.Add(new MirrorBlockBlueprint(pcx, pcy, w, h, rot));
		}

		private void AddMirrorWall(List<string> methodParameter)
		{
			var pc1x = ExtractVec2fParameter(methodParameter, 0).Item1 * _scaleFactor;
			var pc1y = ExtractVec2fParameter(methodParameter, 0).Item2 * _scaleFactor;
			var pc2x = ExtractVec2fParameter(methodParameter, 1).Item1 * _scaleFactor;
			var pc2y = ExtractVec2fParameter(methodParameter, 1).Item2 * _scaleFactor;

			var tstart = ExtractNumberParameter(methodParameter, "trim_start", 0) * _scaleFactor;
			var tend   = ExtractNumberParameter(methodParameter, "trim_end", 0) * _scaleFactor;
			var offsetX = ExtractVec2fParameter(methodParameter, "offset", Tuple.Create(0f, 0f)).Item1 * _scaleFactor;
			var offsetY = ExtractVec2fParameter(methodParameter, "offset", Tuple.Create(0f, 0f)).Item2 * _scaleFactor;
			var superrot = ExtractNumberParameter(methodParameter, "rot", 0);
			var width = ExtractNumberParameter(methodParameter, "width", MirrorBlockBlueprint.DEFAULT_WIDTH);
			var offsetNorm = ExtractNumberParameter(methodParameter, "normal_offset", 0) * _scaleFactor;

			var vs = new Vector2(pc1x, pc1y);
			var ve = new Vector2(pc2x, pc2y);

			var nrm = ve - vs;
			nrm.Normalize();


			vs = vs + nrm * tstart;
			ve = ve - nrm * tend;

			var d = ve - vs;
			var c = (vs + ve) / 2;

			var onn = new Vector2(-nrm.Y, nrm.X) * offsetNorm;

			var r = (float)((Math.Atan2(d.Y, d.X) + Math.PI + Math.PI) % (Math.PI + Math.PI));
			r *= 360f / (float)(Math.PI + Math.PI);

			_result.BlueprintMirrorBlocks.Add(new MirrorBlockBlueprint(c.X + offsetX + onn.X, c.Y + offsetY + onn.Y, d.Length(), width, r + superrot));
		}
		
		private void AddMirrorCircle(List<string> methodParameter)
		{
			var pcx = ExtractVec2fParameter(methodParameter, 0).Item1 * _scaleFactor;
			var pcy = ExtractVec2fParameter(methodParameter, 0).Item2 * _scaleFactor;
			var dia = ExtractNumberParameter(methodParameter, 1) * _scaleFactor;

			_result.BlueprintMirrorCircles.Add(new MirrorCircleBlueprint(pcx, pcy, dia));
		}

		private void AddLaserCannon(List<string> methodParameter)
		{
			var size = ExtractNumberParameter(methodParameter, 0) * _scaleFactor;
			var player = ExtractIntegerParameter(methodParameter, 1);
			var posX = ExtractVec2fParameter(methodParameter, 2).Item1 * _scaleFactor;
			var posY = ExtractVec2fParameter(methodParameter, 2).Item2 * _scaleFactor;
			var rotation = ExtractNumberParameter(methodParameter, 3, -1);
			var cannonid = ExtractIntegerParameter(methodParameter, "id", _nextCannonID);

			_nextCannonID = Math.Max(cannonid + 1, _nextCannonID);

			_result.BlueprintLaserCannons.Add(new LaserCannonBlueprint(posX, posY, size, player, rotation, (byte)cannonid));
		}

		private void SetView(List<string> methodParameter)
		{
			var width  = ExtractNumberParameter(methodParameter, 0) * _scaleFactor;
			var height = ExtractNumberParameter(methodParameter, 1) * _scaleFactor;
			var ctrX   = ExtractVec2fParameter(methodParameter, 2).Item1 * _scaleFactor;
			var ctrY   = ExtractVec2fParameter(methodParameter, 2).Item2 * _scaleFactor;

			_result.LevelWidth  = width;
			_result.LevelHeight = height;
			_result.LevelViewX  = ctrX;
			_result.LevelViewY  = ctrY;
		}

		private void SetWrapMode(List<string> methodParameter)
		{
			var wm = (byte)ExtractIntegerParameter(methodParameter, 0);

			_result.WrapMode = wm;
		}

		private void SetConfig(List<string> methodParameter)
		{
			if (_result.ParseConfiguration == null) _result.ParseConfiguration = new Dictionary<int, float>();
			_result.ParseConfiguration[ExtractIntegerParameter(methodParameter, 0)] = ExtractNumberParameter(methodParameter, 1);
		}

		private void AddText(List<string> methodParameter)
		{
			var x = ExtractVec2fParameter(methodParameter, 0).Item1 * _scaleFactor;
			var y = ExtractVec2fParameter(methodParameter, 0).Item2 * _scaleFactor;
			var w = ExtractNumberParameter(methodParameter, 1) * _scaleFactor;
			var h = ExtractNumberParameter(methodParameter, 2) * _scaleFactor;
			var t = ExtractIntegerParameter(methodParameter, 3);
			var c = ExtractBitOptions16Parameter(methodParameter, 4, 0);
			var r = ExtractNumberParameter(methodParameter, 5, 0f);

			_result.BlueprintBackgroundText.Add(new BackgroundTextBlueprint(x, y, w, h, r, t, c));
		}
	}
}
