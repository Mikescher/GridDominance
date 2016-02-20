using Microsoft.Xna.Framework.Content.Pipeline;
using System;
using System.IO;
using System.Text;

namespace GridDominance.Content.Pipeline
{
	[ContentImporter(".gslevel", DefaultProcessor = "GDLevelProcessor", DisplayName = "GridDominance Level Importer")]
	public class GDLevelImporter : ContentImporter<string>
	{
		public override string Import(string filename, ContentImporterContext context)
		{
			var text = File.ReadAllText(filename, Encoding.UTF8);

			Console.WriteLine("Reading " + text.Length + " character long file");

			return text;
		}
	}
}
