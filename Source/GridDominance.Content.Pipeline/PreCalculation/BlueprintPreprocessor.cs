using GridDominance.Graphfileformat.Blueprint;
using GridDominance.Levelfileformat.Blueprint;

namespace GridDominance.Content.Pipeline.PreCalculation
{
	public static class BlueprintPreprocessor
	{
		public static void ProcessLevel(LevelBlueprint lvl)
		{
			//TODO KI Improv: Sim: Config: Scatter_Trust_Angle
			//TODO KI Improv: Trace: Laser: Terminate on self coll
			//TODO KI Improv: Trace: Laser: Config no corner hit
			//TODO KI Improv: Sim: Stepsize
			//TODO KI Improv: Sim: min no coll distance to other stuff

			if (lvl.KIType == LevelBlueprint.KI_TYPE_RAYTRACE)    new LevelKITracer().Precalc(lvl);
			if (lvl.KIType == LevelBlueprint.KI_TYPE_PRECALC)     new LevelKITracer().Precalc(lvl);
			if (lvl.KIType == LevelBlueprint.KI_TYPE_PRESIMULATE) new LevelKISimulator().Precalc(lvl);
		}

		public static void ProcessGraph(GraphBlueprint graph)
		{
			// Nothing
		}
	}
}
