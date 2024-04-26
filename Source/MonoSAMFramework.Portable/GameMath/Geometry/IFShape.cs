using Microsoft.Xna.Framework;
using System.Diagnostics.Contracts;

namespace MonoSAMFramework.Portable.GameMath.Geometry
{
	public interface IFShape
	{
		[Pure]
		IFShape AsTranslated(float offsetX, float offsetY);

		[Pure]
		IFShape AsTranslated(Vector2 offset);

		[Pure]
		bool Contains(float x, float y);

		[Pure]
		bool Contains(FPoint p);
		
		[Pure]
		bool Contains(Vector2 v);

		[Pure]
		bool Overlaps(IFShape other);
	}
}
