using GridDominance.Graphfileformat.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using System;
using GridDominance.Graphfileformat.Blueprint;

namespace GridDominance.Content.Pipeline.GDWorldGraph
{
	[ContentTypeWriter]
	public class GDWorldGraphWriter : ContentTypeWriter<GraphBlueprint>
	{
		protected override void Write(ContentWriter output, GraphBlueprint value)
		{
			value.BinarySerialize(output);
		}
		
		public override string GetRuntimeType(TargetPlatform targetPlatform)
		{
			return typeof(GraphBlueprint).AssemblyQualifiedName;
		}

		public override string GetRuntimeReader(TargetPlatform targetPlatform)
		{
			return typeof (GDWorldGraphReader).AssemblyQualifiedName;
		}
	}
}
