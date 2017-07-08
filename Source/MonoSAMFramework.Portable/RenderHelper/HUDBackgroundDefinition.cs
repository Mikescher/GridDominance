using System;
using Microsoft.Xna.Framework;

namespace MonoSAMFramework.Portable.RenderHelper
{
	public enum HUDBackgroundDefinitionType { None, Simple, Rounded, RoundedBlur, SimpleBlur, SimpleOutline, }

	public struct HUDBackgroundDefinition: IEquatable<HUDBackgroundDefinition>
	{
		public static HUDBackgroundDefinition NONE  = new HUDBackgroundDefinition(HUDBackgroundDefinitionType.None, Color.Transparent, Color.Transparent, 0, 0, false, false, false, false);
		public static HUDBackgroundDefinition DUMMY = CreateSimple(Color.Magenta);

		public readonly HUDBackgroundDefinitionType Type;
		public readonly Color Color;

		public readonly Color OutlineColor;
		public readonly float OutlineThickness;

		public readonly float CornerSize;
		public readonly bool RoundedCornerTL;
		public readonly bool RoundedCornerTR;
		public readonly bool RoundedCornerBL;
		public readonly bool RoundedCornerBR;

		private HUDBackgroundDefinition(HUDBackgroundDefinitionType t, Color c, Color oc, float ot, float cs, bool tl, bool tr, bool bl, bool br)
		{
			Type = t;
			Color = c;
			OutlineColor = oc;
			OutlineThickness = ot;
			CornerSize = cs;
			RoundedCornerTL = tl;
			RoundedCornerTR = tr;
			RoundedCornerBL = bl;
			RoundedCornerBR = br;
		}

		public static HUDBackgroundDefinition CreateSimple(Color c)
		{
			return new HUDBackgroundDefinition(HUDBackgroundDefinitionType.Simple, c, Color.Transparent, 0, 0, false, false, false, false);
		}

		public static HUDBackgroundDefinition CreateSimpleOutline(Color c, Color line, float lineWidth)
		{
			return new HUDBackgroundDefinition(HUDBackgroundDefinitionType.SimpleOutline, c, line, lineWidth, 0, false, false, false, false);
		}

		public static HUDBackgroundDefinition CreateSimpleBlur(Color c, float cornerSize)
		{
			return new HUDBackgroundDefinition(HUDBackgroundDefinitionType.SimpleBlur, c, Color.Transparent, 0, cornerSize, false, false, false, false);
		}

		public static HUDBackgroundDefinition CreateRounded(Color c, float cornerSize, bool tl = true, bool tr = true, bool bl = true, bool br = true)
		{
			return new HUDBackgroundDefinition(HUDBackgroundDefinitionType.Rounded, c, Color.Transparent, 0, cornerSize, tl, tr, bl, br);
		}

		public static HUDBackgroundDefinition CreateRoundedBlur(Color c, float cornerSize, bool tl = true, bool tr = true, bool bl = true, bool br = true)
		{
			return new HUDBackgroundDefinition(HUDBackgroundDefinitionType.RoundedBlur, c, Color.Transparent, 0, cornerSize, tl, tr, bl, br);
		}

		public HUDBackgroundDefinition WithColor(Color c)
		{
			return new HUDBackgroundDefinition(Type, c, OutlineColor, OutlineThickness, CornerSize, RoundedCornerTL, RoundedCornerTR, RoundedCornerBL, RoundedCornerBR);
		}

		public bool Equals(HUDBackgroundDefinition other)
		{
			return 
				Type == other.Type && 
				Color.Equals(other.Color) && 
				OutlineColor.Equals(other.OutlineColor) && 
				OutlineThickness.Equals(other.OutlineThickness) && 
				CornerSize.Equals(other.CornerSize) && 
				RoundedCornerTL == other.RoundedCornerTL && 
				RoundedCornerTR == other.RoundedCornerTR && 
				RoundedCornerBL == other.RoundedCornerBL && 
				RoundedCornerBR == other.RoundedCornerBR;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			return obj is HUDBackgroundDefinition && Equals((HUDBackgroundDefinition) obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = (int) Type;
				hashCode = (hashCode * 397) ^ Color.GetHashCode();
				hashCode = (hashCode * 397) ^ OutlineColor.GetHashCode();
				hashCode = (hashCode * 397) ^ OutlineThickness.GetHashCode();
				hashCode = (hashCode * 397) ^ CornerSize.GetHashCode();
				hashCode = (hashCode * 397) ^ RoundedCornerTL.GetHashCode();
				hashCode = (hashCode * 397) ^ RoundedCornerTR.GetHashCode();
				hashCode = (hashCode * 397) ^ RoundedCornerBL.GetHashCode();
				hashCode = (hashCode * 397) ^ RoundedCornerBR.GetHashCode();
				return hashCode;
			}
		}

		public static bool operator ==(HUDBackgroundDefinition a, HUDBackgroundDefinition b)
		{
			return a.Equals(b);
		}

		public static bool operator !=(HUDBackgroundDefinition a, HUDBackgroundDefinition b)
		{
			return !a.Equals(b);
		}
	}
}
