using GridDominance.Content.Pipeline.GDLevel;
using Microsoft.Xna.Framework.Content.Pipeline;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GridDominance.Content.Pipeline.TTFFont
{
    [ContentImporter(".ttf", DefaultProcessor = "TTFProcessor", DisplayName = "GridDominance TTF Font Importer")]
    public class TTFImporter : ContentImporter<TTFPackage>
    {
        public override TTFPackage Import(string filename, ContentImporterContext context)
        {
            var content = File.ReadAllBytes(filename);

            Console.WriteLine("Reading {0} bytes long ttf file", content.Length);

            return new TTFPackage
            {
                Content = content,
            };
        }
    }
}
