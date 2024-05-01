using GridDominance.Levelfileformat.Blueprint;
using Microsoft.Xna.Framework.Content.Pipeline;
using System;
using System.Linq;
using GridDominance.Levelfileformat;
using GridDominance.Shared.SCCM.PreCalculation;

namespace GridDominance.Content.Pipeline.GDLevel
{
	[ContentProcessor(DisplayName = "GridDominance Level Processor")]
	public class GDLevelProcessor : ContentProcessor<LevelPackage, LevelBlueprint>
	{
		public override LevelBlueprint Process(LevelPackage input, ContentProcessorContext context)
		{
			var parser = new LevelParser(input.Content, x => input.Includes.FirstOrDefault(p => LevelBlueprint.IsIncludeMatch(p.Key, x)).Value);

			var lf = parser.Parse();

			BlueprintPreprocessor.ProcessLevel(lf);

			Console.WriteLine("Parsing file with " + lf.BlueprintCannons.Count + " Cannon definitions");
			Console.WriteLine("Parsing file with " + lf.BlueprintVoidWalls.Count + " VoidWall definitions");
            Console.WriteLine("Parsing file with " + lf.BlueprintVoidCircles.Count + " VoidCircle definitions");
            Console.WriteLine("Parsing file with " + lf.BlueprintGlassBlocks.Count + " GlassBlock definitions");
            Console.WriteLine("Parsing file with " + lf.BlueprintBlackHoles.Count + " BlackHole definitions");
            Console.WriteLine("Parsing file with " + lf.BlueprintPortals.Count + " Portal definitions");
            Console.WriteLine("Parsing file with " + lf.BlueprintLaserCannons.Count + " LaserCannon definitions");
            Console.WriteLine("Parsing file with " + lf.BlueprintMirrorBlocks.Count + " MirrorBlock definitions");
            Console.WriteLine("Parsing file with " + lf.BlueprintMirrorCircles.Count + " MirrorCircle definitions");
            Console.WriteLine("Parsing file with " + lf.BlueprintBackgroundText.Count + " BackgroundText definitions");
            Console.WriteLine("Parsing file with " + lf.BlueprintMinigun.Count + " Minigun definitions");
            Console.WriteLine("Parsing file with " + lf.BlueprintShieldProjector.Count + " ShieldProjector definitions");
            Console.WriteLine("Parsing file with " + lf.BlueprintRelayCannon.Count + " RelayCannon definitions");
            Console.WriteLine("Parsing file with " + lf.BlueprintTrishotCannon.Count + " TrishotCannon definitions");


            return lf;
		}
	}
}
