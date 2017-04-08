using GridDominance.Graphfileformat.Parser;
using GridDominance.Graphfileformat.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using System;

namespace GridDominance.Content.Pipeline.GDWorldGraph
{
	[ContentTypeWriter]
	public class GDWorldGraphWriter : ContentTypeWriter<WorldGraphFile>
	{
		protected override void Write(ContentWriter output, WorldGraphFile value)
		{
			var start = output.BaseStream.Position;
			value.BinarySerialize(output);
			var length = output.BaseStream.Position - start;

			Console.WriteLine("Writing " + length + " byte long serialized file");
		}
		
		public override string GetRuntimeType(TargetPlatform targetPlatform)
		{
			return typeof(WorldGraphFile).AssemblyQualifiedName;
		}

		public override string GetRuntimeReader(TargetPlatform targetPlatform)
		{
			return typeof (GDWorldGraphReader).AssemblyQualifiedName;
		}
	}
}
