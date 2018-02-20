using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.RenderHelper.TimesheetAnimation;

namespace GridDominance.Shared.Resources
{
	public static class Animations
	{
		private static TimesheetAnimation _bfbAnimation = null;
		public static TimesheetAnimation AnimationBlackForestBytesLogo => _bfbAnimation ?? (_bfbAnimation = CreateBFBAnimation());

		public static TimesheetAnimation CreateBFBAnimation()
		{
			var tsa = new TimesheetAnimation();
			tsa.Duration = 15f;
			tsa.Width  = 1000;
			tsa.Height = 1000;
			tsa.Loop = true;

			tsa.SpeedFactor = 1;

			tsa.AddImage(Textures.TexCircle, 0, 0, 1000, 1000, Color.Black);
			
			var rect_trunk_1_s = FRectangle.CreateWithOffset(300, 730, - 27, + 83,  54,   0);
			var rect_inner_1_s = FRectangle.CreateWithOffset(300, 730,    0, -399,   0, 345);
			var rect_outer_1_s = FRectangle.CreateWithOffset(300, 730,    0, -558,   0, 558);
			var rect_trunk_2_s = FRectangle.CreateWithOffset(500, 694, - 27, + 83,  54,   0);
			var rect_inner_2_s = FRectangle.CreateWithOffset(500, 694,    0, -399,   0, 345);
			var rect_outer_2_s = FRectangle.CreateWithOffset(500, 694,    0, -558,   0, 558);
			var rect_trunk_3_s = FRectangle.CreateWithOffset(700, 730, - 27, + 83,  54,   0);
			var rect_inner_3_s = FRectangle.CreateWithOffset(700, 730,    0, -399,   0, 345);
			var rect_outer_3_s = FRectangle.CreateWithOffset(700, 730,    0, -558,   0, 558);
			
			var rect_trunk_1_e = FRectangle.CreateWithOffset(300, 730, - 27, -135,  54, 218);
			var rect_inner_1_e = FRectangle.CreateWithOffset(300, 730, -124, -399, 248, 345);
			var rect_outer_1_e = FRectangle.CreateWithOffset(300, 730, -200, -558, 400, 558);
			var rect_trunk_2_e = FRectangle.CreateWithOffset(500, 694, - 27, - 35,  54, 118);
			var rect_inner_2_e = FRectangle.CreateWithOffset(500, 694, -124, -399, 248, 345);
			var rect_outer_2_e = FRectangle.CreateWithOffset(500, 694, -200, -558, 400, 558);
			var rect_trunk_3_e = FRectangle.CreateWithOffset(700, 730, - 27, -135,  54, 218);
			var rect_inner_3_e = FRectangle.CreateWithOffset(700, 730, -124, -399, 248, 345);
			var rect_outer_3_e = FRectangle.CreateWithOffset(700, 730, -200, -558, 400, 558);

			//MIDDLE
			var outer2 = tsa.AddImage(Textures.TexTriangle, rect_outer_2_s, Color.White);
			var trunk2 = tsa.AddImage(Textures.TexPixel,    rect_trunk_2_s, Color.White);
			var inner2 = tsa.AddImage(Textures.TexTriangle, rect_inner_2_s, Color.Black);


			//LEFT
			var outer1 = tsa.AddImage(Textures.TexTriangle, rect_outer_1_s, Color.White);
			var inner1 = tsa.AddImage(Textures.TexTriangle, rect_inner_1_s, Color.Black);
			var trunk1 = tsa.AddImage(Textures.TexPixel,    rect_trunk_1_s, Color.White);
			
			
			//RIGHT
			var outer3 = tsa.AddImage(Textures.TexTriangle, rect_outer_3_s, Color.White);
			var inner3 = tsa.AddImage(Textures.TexTriangle, rect_inner_3_s, Color.Black);
			var trunk3 = tsa.AddImage(Textures.TexPixel,    rect_trunk_3_s, Color.White);


			trunk1.AddBoundingTransformation(1.00f, rect_trunk_1_s, 2.00f, rect_trunk_1_e, FloatMath.FunctionEaseInOutQuad);
			trunk2.AddBoundingTransformation(1.25f, rect_trunk_2_s, 2.25f, rect_trunk_2_e, FloatMath.FunctionEaseInOutQuad);
			trunk3.AddBoundingTransformation(1.50f, rect_trunk_3_s, 2.50f, rect_trunk_3_e, FloatMath.FunctionEaseInOutQuad);

			outer2.AddBoundingTransformation(2.75f, rect_outer_2_s, 3.75f, rect_outer_2_e, FloatMath.FunctionEaseOutQuint);
			outer1.AddBoundingTransformation(3.00f, rect_outer_1_s, 4.00f, rect_outer_1_e, FloatMath.FunctionEaseOutQuint);
			outer3.AddBoundingTransformation(3.25f, rect_outer_3_s, 4.25f, rect_outer_3_e, FloatMath.FunctionEaseOutQuint);

			inner2.AddBoundingTransformation(2.90f, rect_inner_2_s, 3.75f, rect_inner_2_e, FloatMath.FunctionEaseOutQuart);
			inner1.AddBoundingTransformation(3.15f, rect_inner_1_s, 4.00f, rect_inner_1_e, FloatMath.FunctionEaseOutQuart);
			inner3.AddBoundingTransformation(3.40f, rect_inner_3_s, 4.25f, rect_inner_3_e, FloatMath.FunctionEaseOutQuart);

			trunk1.AddColorTransformation(12, Color.White, 15, Color.Transparent, FloatMath.FunctionEaseInQuart);
			trunk2.AddColorTransformation(12, Color.White, 15, Color.Transparent, FloatMath.FunctionEaseInQuart);
			trunk3.AddColorTransformation(12, Color.White, 15, Color.Transparent, FloatMath.FunctionEaseInQuart);

			outer1.AddColorTransformation(12, Color.White, 15, Color.Transparent, FloatMath.FunctionEaseInQuart);
			outer2.AddColorTransformation(12, Color.White, 15, Color.Transparent, FloatMath.FunctionEaseInQuart);
			outer3.AddColorTransformation(12, Color.White, 15, Color.Transparent, FloatMath.FunctionEaseInQuart);

			return tsa;
		}
	}
}
