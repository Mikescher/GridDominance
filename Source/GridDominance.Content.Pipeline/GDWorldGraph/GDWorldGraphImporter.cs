using Microsoft.Xna.Framework.Content.Pipeline;
using System.IO;
using System.Text;

namespace GridDominance.Content.Pipeline.GDWorldGraph
{
	[ContentImporter(".gsgraph", DefaultProcessor = "GDWorldGraphProcessor", DisplayName = "GridDominance WorldGraph Importer")]
	public class GDWorldGraphImporter : ContentImporter<string>
	{
		public override string Import(string filename, ContentImporterContext context)
		{
			return File.ReadAllText(filename, Encoding.UTF8);
		}
	}
}
