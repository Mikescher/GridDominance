using MonoSAMFramework.Portable.MathHelper.FloatClasses;

namespace MonoSAMFramework.Portable.MathHelper.VectorPath
{
	public abstract class VectorPathSegment
	{
		public abstract float Length { get; }
		public abstract FRectangle Boundings { get; }


		public abstract FPoint Get(float len);
	}
}
