using GridDominance.Graphfileformat.Blueprint;
using GridDominance.Levelfileformat.Blueprint;

namespace GridDominance.Content.Pipeline.PreCalculation
{
	public static class BlueprintPreprocessor
	{
		public static void ProcessLevel(LevelBlueprint lvl)
		{
			if (lvl.KIType == LevelBlueprint.KI_TYPE_RAYTRACE)    LevelBulletPathTracer.Precalc(lvl);
			if (lvl.KIType == LevelBlueprint.KI_TYPE_PRECALC)     LevelBulletPathTracer.Precalc(lvl);
			if (lvl.KIType == LevelBlueprint.KI_TYPE_PRESIMULATE) LevelBulletPathSimulator.Precalc(lvl);
		}

		public static void ProcessGraph(GraphBlueprint graph)
		{
			// Nothing
		}
	}
}
