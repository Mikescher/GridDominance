using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.GameMath;

namespace MonoSAMFramework.Portable.ColorHelper
{
	public static class ColorMath
	{
		public static readonly Dictionary<string, Color> COLOR_MAP = new Dictionary<string, Color>
		{
			{"color.transparentblack", Color.TransparentBlack},
			{"color.transparent", Color.Transparent},
			{"color.aliceblue", Color.AliceBlue},
			{"color.antiquewhite", Color.AntiqueWhite},
			{"color.aqua", Color.Aqua},
			{"color.aquamarine", Color.Aquamarine},
			{"color.azure", Color.Azure},
			{"color.beige", Color.Beige},
			{"color.bisque", Color.Bisque},
			{"color.black", Color.Black},
			{"color.blanchedalmond", Color.BlanchedAlmond},
			{"color.blue", Color.Blue},
			{"color.blueviolet", Color.BlueViolet},
			{"color.brown", Color.Brown},
			{"color.burlywood", Color.BurlyWood},
			{"color.cadetblue", Color.CadetBlue},
			{"color.chartreuse", Color.Chartreuse},
			{"color.chocolate", Color.Chocolate},
			{"color.coral", Color.Coral},
			{"color.cornflowerblue", Color.CornflowerBlue},
			{"color.cornsilk", Color.Cornsilk},
			{"color.crimson", Color.Crimson},
			{"color.cyan", Color.Cyan},
			{"color.darkblue", Color.DarkBlue},
			{"color.darkcyan", Color.DarkCyan},
			{"color.darkgoldenrod", Color.DarkGoldenrod},
			{"color.darkgray", Color.DarkGray},
			{"color.darkgreen", Color.DarkGreen},
			{"color.darkkhaki", Color.DarkKhaki},
			{"color.darkmagenta", Color.DarkMagenta},
			{"color.darkolivegreen", Color.DarkOliveGreen},
			{"color.darkorange", Color.DarkOrange},
			{"color.darkorchid", Color.DarkOrchid},
			{"color.darkred", Color.DarkRed},
			{"color.darksalmon", Color.DarkSalmon},
			{"color.darkseagreen", Color.DarkSeaGreen},
			{"color.darkslateblue", Color.DarkSlateBlue},
			{"color.darkslategray", Color.DarkSlateGray},
			{"color.darkturquoise", Color.DarkTurquoise},
			{"color.darkviolet", Color.DarkViolet},
			{"color.deeppink", Color.DeepPink},
			{"color.deepskyblue", Color.DeepSkyBlue},
			{"color.dimgray", Color.DimGray},
			{"color.dodgerblue", Color.DodgerBlue},
			{"color.firebrick", Color.Firebrick},
			{"color.floralwhite", Color.FloralWhite},
			{"color.forestgreen", Color.ForestGreen},
			{"color.fuchsia", Color.Fuchsia},
			{"color.gainsboro", Color.Gainsboro},
			{"color.ghostwhite", Color.GhostWhite},
			{"color.gold", Color.Gold},
			{"color.goldenrod", Color.Goldenrod},
			{"color.gray", Color.Gray},
			{"color.green", Color.Green},
			{"color.greenyellow", Color.GreenYellow},
			{"color.honeydew", Color.Honeydew},
			{"color.hotpink", Color.HotPink},
			{"color.indianred", Color.IndianRed},
			{"color.indigo", Color.Indigo},
			{"color.ivory", Color.Ivory},
			{"color.khaki", Color.Khaki},
			{"color.lavender", Color.Lavender},
			{"color.lavenderblush", Color.LavenderBlush},
			{"color.lawngreen", Color.LawnGreen},
			{"color.lemonchiffon", Color.LemonChiffon},
			{"color.lightblue", Color.LightBlue},
			{"color.lightcoral", Color.LightCoral},
			{"color.lightcyan", Color.LightCyan},
			{"color.lightgoldenrodye", Color.LightGoldenrodYellow},
			{"color.lightgray", Color.LightGray},
			{"color.lightgreen", Color.LightGreen},
			{"color.lightpink", Color.LightPink},
			{"color.lightsalmon", Color.LightSalmon},
			{"color.lightseagreen", Color.LightSeaGreen},
			{"color.lightskyblue", Color.LightSkyBlue},
			{"color.lightslategray", Color.LightSlateGray},
			{"color.lightsteelblue", Color.LightSteelBlue},
			{"color.lightyellow", Color.LightYellow},
			{"color.lime", Color.Lime},
			{"color.limegreen", Color.LimeGreen},
			{"color.linen", Color.Linen},
			{"color.magenta", Color.Magenta},
			{"color.maroon", Color.Maroon},
			{"color.mediumaquamarine", Color.MediumAquamarine},
			{"color.mediumblue", Color.MediumBlue},
			{"color.mediumorchid", Color.MediumOrchid},
			{"color.mediumpurple", Color.MediumPurple},
			{"color.mediumseagreen", Color.MediumSeaGreen},
			{"color.mediumslateblue", Color.MediumSlateBlue},
			{"color.mediumspringgree", Color.MediumSpringGreen},
			{"color.mediumturquoise", Color.MediumTurquoise},
			{"color.mediumvioletred", Color.MediumVioletRed},
			{"color.midnightblue", Color.MidnightBlue},
			{"color.mintcream", Color.MintCream},
			{"color.mistyrose", Color.MistyRose},
			{"color.moccasin", Color.Moccasin},
			{"color.monogameorange", Color.MonoGameOrange},
			{"color.navajowhite", Color.NavajoWhite},
			{"color.navy", Color.Navy},
			{"color.oldlace", Color.OldLace},
			{"color.olive", Color.Olive},
			{"color.olivedrab", Color.OliveDrab},
			{"color.orange", Color.Orange},
			{"color.orangered", Color.OrangeRed},
			{"color.orchid", Color.Orchid},
			{"color.palegoldenrod", Color.PaleGoldenrod},
			{"color.palegreen", Color.PaleGreen},
			{"color.paleturquoise", Color.PaleTurquoise},
			{"color.palevioletred", Color.PaleVioletRed},
			{"color.papayawhip", Color.PapayaWhip},
			{"color.peachpuff", Color.PeachPuff},
			{"color.peru", Color.Peru},
			{"color.pink", Color.Pink},
			{"color.plum", Color.Plum},
			{"color.powderblue", Color.PowderBlue},
			{"color.purple", Color.Purple},
			{"color.red", Color.Red},
			{"color.rosybrown", Color.RosyBrown},
			{"color.royalblue", Color.RoyalBlue},
			{"color.saddlebrown", Color.SaddleBrown},
			{"color.salmon", Color.Salmon},
			{"color.sandybrown", Color.SandyBrown},
			{"color.seagreen", Color.SeaGreen},
			{"color.seashell", Color.SeaShell},
			{"color.sienna", Color.Sienna},
			{"color.silver", Color.Silver},
			{"color.skyblue", Color.SkyBlue},
			{"color.slateblue", Color.SlateBlue},
			{"color.slategray", Color.SlateGray},
			{"color.snow", Color.Snow},
			{"color.springgreen", Color.SpringGreen},
			{"color.steelblue", Color.SteelBlue},
			{"color.tan", Color.Tan},
			{"color.teal", Color.Teal},
			{"color.thistle", Color.Thistle},
			{"color.tomato", Color.Tomato},
			{"color.turquoise", Color.Turquoise},
			{"color.violet", Color.Violet},
			{"color.wheat", Color.Wheat},
			{"color.white", Color.White},
			{"color.whitesmoke", Color.WhiteSmoke},
			{"color.yellow", Color.Yellow},
			{"color.yellowgreen", Color.YellowGreen},

			{"flatcolors.turquoise", FlatColors.Turquoise},
			{"flatcolors.greensea", FlatColors.GreenSea},
			{"flatcolors.sunflower", FlatColors.SunFlower},
			{"flatcolors.orange", FlatColors.Orange},
			{"flatcolors.emerald", FlatColors.Emerald},
			{"flatcolors.nephritis", FlatColors.Nephritis},
			{"flatcolors.carrot", FlatColors.Carrot},
			{"flatcolors.pumpkin", FlatColors.Pumpkin},
			{"flatcolors.peterriver", FlatColors.PeterRiver},
			{"flatcolors.belizehole", FlatColors.BelizeHole},
			{"flatcolors.alizarin", FlatColors.Alizarin},
			{"flatcolors.pomegranate", FlatColors.Pomegranate},
			{"flatcolors.amethyst", FlatColors.Amethyst},
			{"flatcolors.wisteria", FlatColors.Wisteria},
			{"flatcolors.clouds", FlatColors.Clouds},
			{"flatcolors.silver", FlatColors.Silver},
			{"flatcolors.wetasphalt", FlatColors.WetAsphalt},
			{"flatcolors.midnightblue", FlatColors.MidnightBlue},
			{"flatcolors.concrete", FlatColors.Concrete},
			{"flatcolors.asbestos", FlatColors.Asbestos},
		};

		public static Color Darken(this Color c, float perc = 0.5f)
		{
			return new Color(
				perc*(c.R / 255f),
				perc*(c.G / 255f),
				perc*(c.B / 255f),
				c.A);
		}

		public static Color Lighten(this Color c, float perc = 0.5f)
		{
			return new Color(
				1f - perc*(1f - c.R/255f),
				1f - perc*(1f - c.G/255f),
				1f - perc*(1f - c.B/255f),
				c.A);
		}

		public static Color Fade(Color c, float a)
		{
			if (a < 1)
			{
				return new Color(c.R, c.G, c.B, (c.A / 255f) * a);
			}
			else
			{
				return c;
			}
		}

		public static Color BlendTo(this Color c, Color other, float perc = 0.5F)
		{
			return Blend(c, other, perc);
		}

		public static Color Blend(Color a, Color b, float perc)
		{
			if (FloatMath.IsZero(perc)) return a;
			if (FloatMath.IsOne(perc)) return b;

			var cr = (1 - perc)*a.R + perc*b.R;
			var cg = (1 - perc)*a.G + perc*b.G;
			var cb = (1 - perc)*a.B + perc*b.B;

			return new Color((int)cr, (int)cg, (int)cb);
		}
	}
}
