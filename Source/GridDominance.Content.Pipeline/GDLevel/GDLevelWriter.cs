using GridDominance.Levelfileformat.Blueprint;
using GridDominance.Levelfileformat.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using System;

namespace GridDominance.Content.Pipeline.GDLevel
{
	[ContentTypeWriter]
	public class GDLevelWriter : ContentTypeWriter<LevelBlueprint>
	{
		protected override void Write(ContentWriter output, LevelBlueprint value)
		{
			var start = output.BaseStream.Position;
			value.BinarySerialize(output);
			var length = output.BaseStream.Position - start;

			Console.WriteLine("Writing " + length + " byte long serialized file");
		}
		
		public override string GetRuntimeType(TargetPlatform targetPlatform)
		{
			return typeof(LevelBlueprint).AssemblyQualifiedName;
		}

		public override string GetRuntimeReader(TargetPlatform targetPlatform)
		{
			return typeof (GDLevelReader).AssemblyQualifiedName;
		}
	}
}
