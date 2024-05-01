using GridDominance.Levelfileformat.Blueprint;
using Microsoft.Xna.Framework.Content.Pipeline;
using System;
using System.Linq;
using GridDominance.Levelfileformat;
using GridDominance.Shared.SCCM.PreCalculation;
using GridDominance.Common.TTFFonts;

namespace GridDominance.Content.Pipeline.TTFFont
{
	[ContentProcessor(DisplayName = "GridDominance TTF Processor")]
	public class TTFProcessor : ContentProcessor<TTFPackage, TTFFontData>
	{
		public override TTFFontData Process(TTFPackage input, ContentProcessorContext context)
		{
			return new TTFFontData(){BinaryData = input.Content};
		}
	}
}
