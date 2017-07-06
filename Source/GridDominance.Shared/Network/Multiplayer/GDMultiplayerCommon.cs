using GridDominance.Shared.Screens.NormalGameScreen.Entities;
using GridDominance.Shared.Screens.NormalGameScreen.Fractions;
using GridDominance.Shared.Screens.ScreenGame;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.Extensions;
using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.LogProtocol;
using MonoSAMFramework.Portable.Network.Multiplayer;
using System;
using MonoSAMFramework.Portable.GameMath.Geometry;

namespace GridDominance.Shared.Network.Multiplayer
{
	public abstract class GDMultiplayerCommon : SAMNetworkConnection
	{
		public static int BYTE_SIZE = 61;

		public static byte AREA_BCANNONS = 0xC0;
		public static byte AREA_BULLETS = 0xC1;
		public static byte AREA_END = 0x77;

		public static int SIZE_BCANNON_DEF = 4;
		public static int SIZE_BULLET_DEF = 10;

		public static int REMOTE_BULLET_UPDATELESS_LIFETIME = 8;
		
		public static int PACKAGE_FORWARD_HEADER_SIZE = 10;

		public readonly long[] RecieveBigSeq = new long[32];

		public GDGameScreen Screen;

		protected int packageCount = 0;
		protected int packageModSize = 0;

		protected GDMultiplayerCommon(INetworkMedium medium) : base(medium)
		{
		}

		protected void ProcessStateData(byte[] d, byte euid)
		{
			// [8: CMD] [8:seq] [16: SessionID] [4: UserID] [12: SessionSecret] [32:time] [~: Payload]

			RecieveBigSeq[euid]++;
			var bseq = RecieveBigSeq[euid];

			int p = PACKAGE_FORWARD_HEADER_SIZE;
			for (;;)
			{
				byte cmd = d[p];
				p++;

				if (p >= BYTE_SIZE)
				{
					SAMLog.Error("SNS-COMMON::OOB", "OOB: " + p);
					break;
				}
				else if (cmd == AREA_BCANNONS)
				{
					ProcessForwardBulletCannons(ref p, d, bseq);
				}
				else if (cmd == AREA_BULLETS)
				{
					ProcessForwardBullets(ref p, d, bseq);
				}
				else if (cmd == AREA_END)
				{
					break;
				}
				else
				{
					SAMLog.Error("SNS-COMMON::UA", "Unknown AREA: " + cmd);
					break;
				}
			}
		}

		private void ProcessForwardBulletCannons(ref int p, byte[] d, long bseq)
		{
			int count = d[p];
			p++;

			for (int i = 0; i < count; i++)
			{
				var id = NetworkDataTools.GetByte(d[p + 0]);
				var frac = Screen.GetFractionByID(NetworkDataTools.GetHighBits(d[p + 1], 3));
				var boost = NetworkDataTools.GetLowBits(d[p + 1], 5);
				var rot = NetworkDataTools.ConvertToRadians(NetworkDataTools.GetByte(d[p + 2]), 8);
				var hp = NetworkDataTools.GetByte(d[p + 3]) / 255f;

				Cannon c;
				if (Screen.CannonMap.TryGetValue(id, out c))
				{
					BulletCannon bc = c as BulletCannon;
					if (bc != null && ShouldRecieveData(frac, bc))
					{
						if (ShouldRecieveRotationData(frac, bc))
						{
							bc.Rotation.Set(rot);
						}

						if (ShouldRecieveStateData(frac, bc))
						{
							if (bc.Fraction != frac) bc.SetFraction(frac);
							bc.CannonHealth.Set(hp);
							bc.ManualBoost = boost;
						}
					}
				}

				p += SIZE_BCANNON_DEF;
			}
		}

		private void ProcessForwardBullets(ref int p, byte[] d, long bseq)
		{
			int count = d[p];
			p++;

			for (int i = 0; i < count; i++)
			{
				var id = NetworkDataTools.GetSplitBits(d[p + 0], d[p + 1], 8, 4);
				var state = (RemoteBullet.RemoteBulletState)NetworkDataTools.GetLowBits(d[p + 1], 4);

				var ipx = NetworkDataTools.GetUInt16(d[p + 2], d[p + 3]);
				var ipy = NetworkDataTools.GetUInt16(d[p + 4], d[p + 5]);
				Screen.DoubleByteToPosition(ipx, ipy, out float px, out float py);

				var rot = NetworkDataTools.ConvertToRadians(NetworkDataTools.GetSplitBits(d[p + 6], d[p + 7], 8, 2), 10);
				var len = NetworkDataTools.GetSplitBits(d[p + 7], d[p + 8], 6, 5) / 8f;
				var veloc = new Vector2(len, 0).Rotate(rot);

				var fraction = Screen.GetFractionByID(NetworkDataTools.GetLowBits(d[p + 8], 3));
				var scale = 16 * (d[p + 9] / 255f);

				var bullet = Screen.RemoteBulletMapping[id];

				switch (state)
				{
					case RemoteBullet.RemoteBulletState.Normal:
						if (bullet != null)
						{
							bullet.RemoteUpdate(state, px, py, veloc, fraction, scale, bseq);
						}
						else
						{
							Screen.RemoteBulletMapping[id] = new RemoteBullet(Screen, new FPoint(px, py), veloc, id, scale, fraction, bseq);
							Screen.RemoteBulletMapping[id].RemoteState = state;
							Screen.Entities.AddEntity(Screen.RemoteBulletMapping[id]);
						}
						break;
					case RemoteBullet.RemoteBulletState.Dying_Explosion:
					case RemoteBullet.RemoteBulletState.Dying_ShrinkSlow:
					case RemoteBullet.RemoteBulletState.Dying_ShrinkFast:
					case RemoteBullet.RemoteBulletState.Dying_Fade:
					case RemoteBullet.RemoteBulletState.Dying_Instant:
						if (bullet != null && bullet.RemoteState == RemoteBullet.RemoteBulletState.Normal)
						{
							bullet.RemoteKill(state);
						}
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
				
				p +=SIZE_BULLET_DEF;
			}

			for (int i = 0; i < GDGameScreen.MAX_BULLET_ID; i++)
			{
				if (Screen.RemoteBulletMapping[i] != null && bseq - Screen.RemoteBulletMapping[i].LastUpdateBigSeq > REMOTE_BULLET_UPDATELESS_LIFETIME)
				{
					SAMLog.Debug("Mercykill Bullet: " + i);
					Screen.RemoteBulletMapping[i].Alive = false;
				}
			}
		}

		protected void SendAndReset(ref int idx)
		{
			SetSequenceCounter(ref MSG_FORWARD[1]);

			MSG_FORWARD[idx] = AREA_END;
			_medium.Send(MSG_FORWARD);
			packageModSize = idx;

			idx = PACKAGE_FORWARD_HEADER_SIZE;

			packageCount++;
		}

		protected void SendForwardBulletCannons(ref int idx)
		{
			if (idx + 2 >= BYTE_SIZE) SendAndReset(ref idx);

			MSG_FORWARD[idx] = AREA_BCANNONS;
			idx++;

			byte arrsize = (byte)((BYTE_SIZE - idx - 2) / SIZE_BCANNON_DEF);

			int posSize = idx;

			MSG_FORWARD[posSize] = 0xFF;
			idx++;

			int i = 0;
			foreach (var cannon in Screen.GetEntities<BulletCannon>())
			{
				if (!ShouldSendData(cannon)) continue;

				// [8: ID] [3: Fraction] [5: Boost] [8: Rotation] [8: Health]

				NetworkDataTools.SetByte(out MSG_FORWARD[idx + 0], cannon.BlueprintCannonID);
				NetworkDataTools.SetSplitByte(out MSG_FORWARD[idx + 1], Screen.GetFractionID(cannon.Fraction), cannon.IntegerBoost, 3, 5, 3, 5);
				NetworkDataTools.SetByte(out MSG_FORWARD[idx + 2], NetworkDataTools.ConvertFromRadians(cannon.Rotation.TargetValue, 8));
				NetworkDataTools.SetByteFloor(out MSG_FORWARD[idx + 3], FloatMath.Clamp(cannon.CannonHealth.TargetValue, 0f, 1f) * 255);

				idx += SIZE_BCANNON_DEF;

				i++;
				if (i >= arrsize)
				{
					MSG_FORWARD[posSize] = (byte)i;
					SendAndReset(ref idx);
					MSG_FORWARD[idx] = AREA_BCANNONS;
					idx++;
					i -= arrsize;
					arrsize = (byte)((BYTE_SIZE - idx - 2) / SIZE_BCANNON_DEF);
					posSize = idx;
					MSG_FORWARD[posSize] = 0xFF;
					idx++;
				}
			}
			MSG_FORWARD[posSize] = (byte)i;
		}

		protected void SendForwardBullets(ref int idx)
		{
			if (idx + 2 >= BYTE_SIZE) SendAndReset(ref idx);

			MSG_FORWARD[idx] = AREA_BULLETS;
			idx++;

			byte arrsize = (byte)((BYTE_SIZE - idx - 2) / SIZE_BULLET_DEF);

			int posSize = idx;

			MSG_FORWARD[posSize] = 0xFF;
			idx++;

			int i = 0;
			for (int bid = 0; bid < GDGameScreen.MAX_BULLET_ID; bid++)
			{
				if (Screen.BulletMapping[bid].Bullet == null) continue;
				if (Screen.BulletMapping[bid].State != RemoteBullet.RemoteBulletState.Normal && Screen.BulletMapping[bid].RemainingPostDeathTransmitions <= 0)
				{
					Screen.BulletMapping[bid].Bullet = null; // for GC
					continue;
				}

				Screen.BulletMapping[bid].RemainingPostDeathTransmitions--;

				// [12: ID] [4: State] [16: PosX] [16: PosY] [10: VecRot] [11: VecLen] [3: Fraction] [8: Scale]

				var b = Screen.BulletMapping[bid].Bullet;
				var state = Screen.BulletMapping[bid].State;
				var veloc = b.Velocity;
				ushort px, py;
				Screen.PositionTo2Byte(b.Position, out px, out py);
				ushort rot = (ushort)((FloatMath.NormalizeAngle(veloc.ToAngle()) / FloatMath.TAU) * 1024); // 10bit
				ushort len = (ushort)FloatMath.IClamp(FloatMath.Round(veloc.Length() * 8), 0, 2048); // 11bit (fac=8)
				byte frac = Screen.GetFractionID(b.Fraction);


				NetworkDataTools.SetByteWithHighBits(out MSG_FORWARD[idx + 0], bid, 12);
				NetworkDataTools.SetSplitByte(out MSG_FORWARD[idx + 1], bid, (int)state, 12, 4, 4, 4);
				NetworkDataTools.SetUInt16(out MSG_FORWARD[idx + 2], out MSG_FORWARD[idx + 3], px);
				NetworkDataTools.SetUInt16(out MSG_FORWARD[idx + 4], out MSG_FORWARD[idx + 5], py);
				NetworkDataTools.SetByteWithHighBits(out MSG_FORWARD[idx + 6], rot, 10);
				NetworkDataTools.SetSplitByte(out MSG_FORWARD[idx + 7], rot, len, 10, 11, 2, 6);
				NetworkDataTools.SetSplitByte(out MSG_FORWARD[idx + 8], len, frac, 11, 3, 5, 3);
				NetworkDataTools.SetByteClamped(out MSG_FORWARD[idx + 9], (int)((b.Scale / 16f) * 255));

				idx += SIZE_BULLET_DEF;

				i++;
				if (i >= arrsize)
				{
					MSG_FORWARD[posSize] = (byte)i;
					SendAndReset(ref idx);
					MSG_FORWARD[idx] = AREA_BULLETS;
					idx++;
					i -= arrsize;
					arrsize = (byte)((BYTE_SIZE - idx - 2) / SIZE_BULLET_DEF);
					posSize = idx;
					MSG_FORWARD[posSize] = 0xFF;
					idx++;
				}
			}
			MSG_FORWARD[posSize] = (byte)i;
		}

		protected abstract bool ShouldRecieveData(Fraction f, BulletCannon c);
		protected abstract bool ShouldRecieveRotationData(Fraction f, BulletCannon c);
		protected abstract bool ShouldRecieveStateData(Fraction f, BulletCannon c);
		protected abstract bool ShouldSendData(BulletCannon c);
	}
}
