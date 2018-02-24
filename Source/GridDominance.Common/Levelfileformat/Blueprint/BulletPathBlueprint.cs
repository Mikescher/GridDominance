using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;

namespace GridDominance.Levelfileformat.Blueprint
{
	public sealed class BulletPathBlueprint
	{
		public readonly int TargetCannonID;
		public readonly float CannonRotation; // radians

		public readonly Tuple<Vector2, Vector2>[] Rays; // <start, end>

		public readonly List<Vector2> PreviewBulletPath; // NULL in game

		public BulletPathBlueprint(int cid, float rot, Tuple<Vector2, Vector2>[] rays)
		{
			TargetCannonID = cid;
			CannonRotation = rot;
			Rays = rays;
			PreviewBulletPath = null;
		}

		public BulletPathBlueprint(int cid, float rot, Tuple<Vector2, Vector2>[] rays, List<Vector2> calculatedPreviewPath)
		{
			TargetCannonID = cid;
			CannonRotation = rot;
			Rays = rays;
			PreviewBulletPath = calculatedPreviewPath;
		}

		public BulletPathBlueprint AsCleaned()
		{
			if (Rays.Any(r => EpsEquals(r.Item1, r.Item2)))
			{
				return new BulletPathBlueprint(TargetCannonID, CannonRotation, Rays.Where(r => !EpsEquals(r.Item1, r.Item2)).ToArray(), PreviewBulletPath);
			}

			return this;
		}

		private bool EpsEquals(Vector2 v1, Vector2 v2)
		{
			const float eps = 0.001f;
			return Math.Abs(v1.X - v2.X) <= eps && Math.Abs(v1.Y - v2.Y) <= eps;
		}

		public void Serialize(BinaryWriter bw)
		{
			bw.Write(TargetCannonID);
			bw.Write(CannonRotation);
			bw.Write((short)Rays.Length);
			foreach (var ray in Rays)
			{
				bw.Write(ray.Item1.X);
				bw.Write(ray.Item1.Y);
				bw.Write(ray.Item2.X);
				bw.Write(ray.Item2.Y);
			}
		}

		public static BulletPathBlueprint Deserialize(BinaryReader br)
		{
			var cid = br.ReadInt32();

			var rot = br.ReadSingle();

			var cnt = br.ReadInt16();
			var ray = new Tuple<Vector2, Vector2>[cnt];
			for (int i = 0; i < cnt; i++)
			{
				var x1 = br.ReadSingle();
				var y1 = br.ReadSingle();
				var x2 = br.ReadSingle();
				var y2 = br.ReadSingle();

				ray[i] = Tuple.Create(new Vector2(x1, y1), new Vector2(x2, y2));
			}

			return new BulletPathBlueprint(cid, rot, ray);
		}
	}
}
