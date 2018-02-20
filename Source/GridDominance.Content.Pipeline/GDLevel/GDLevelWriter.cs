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
			value.BinarySerialize(output);
		}
		
		public override string GetRuntimeType(TargetPlatform targetPlatform)
		{
			return typeof(LevelBlueprint).AssemblyQualifiedName;
		}

		public override string GetRuntimeReader(TargetPlatform targetPlatform)
		{
			return typeof (GDLevelReader).AssemblyQualifiedName;
		}

		protected override bool ShouldCompressContent(TargetPlatform targetPlatform, object value)
		{
			return true;
		}
	}
}
