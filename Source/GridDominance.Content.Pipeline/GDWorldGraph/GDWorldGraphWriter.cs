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
			var start = output.BaseStream.Position;
			value.BinarySerialize(output);
			var length = output.BaseStream.Position - start;

			Console.WriteLine("Writing " + length + " byte long serialized file");
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
