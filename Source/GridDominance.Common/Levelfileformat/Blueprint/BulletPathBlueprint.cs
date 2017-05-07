using System;
using System.IO;

namespace GridDominance.Levelfileformat.Blueprint
{
	public sealed class BulletPathBlueprint
	{
		public readonly int TargetCannonID;
		public readonly float CannonRotation; // radians

		public readonly Tuple<float, float>[] Rays; // <ray_endX, ray_endY>

		public BulletPathBlueprint(int cid, float rot, Tuple<float, float>[] rays)
		{
			TargetCannonID = cid;
			CannonRotation = rot;
			Rays = rays;
		}

		public void Serialize(BinaryWriter bw)
		{
			bw.Write(TargetCannonID);
			bw.Write(CannonRotation);
			bw.Write((short)Rays.Length);
			foreach (var ray in Rays)
			{
				bw.Write(ray.Item1);
				bw.Write(ray.Item2);
			}
		}

		public static BulletPathBlueprint Deserialize(BinaryReader br)
		{
			var cid = br.ReadInt32();

			var rot = br.ReadSingle();

			var cnt = br.ReadInt16();
			var ray = new Tuple<float, float>[cnt];
			for (int i = 0; i < cnt; i++)
			{
				var x = br.ReadSingle();
				var y = br.ReadSingle();

				ray[i] = Tuple.Create(x, y);
			}

			return new BulletPathBlueprint(cid, rot, ray);
		}
	}
}
