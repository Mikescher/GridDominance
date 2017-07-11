using GridDominance.Shared.Screens.NormalGameScreen.Entities;
using GridDominance.Shared.Screens.NormalGameScreen.Fractions;
using GridDominance.Shared.Screens.ScreenGame;
using Microsoft.Xna.Framework;
using MonoSAMFramework.Portable.Extensions;
using MonoSAMFramework.Portable.GameMath;
using MonoSAMFramework.Portable.LogProtocol;
using MonoSAMFramework.Portable.Network.Multiplayer;
using System.Linq;
using MonoSAMFramework.Portable.GameMath.Geometry;
using MonoSAMFramework.Portable.Input;
using MonoSAMFramework.Portable.Screens;

namespace GridDominance.Shared.Network.Multiplayer
{
	public abstract class GDMultiplayerCommon : SAMNetworkConnection
	{
		public static byte AREA_BULLETCANNONS    = 0xC0;
		public static byte AREA_BULLETS          = 0xC1;
		public static byte AREA_LASERCANNONS     = 0xC2;
		public static byte AREA_SHIELDPROJECTORS = 0xC3;
		public static byte AREA_MINIGUNS         = 0xC4;
		public static byte AREA_RELAYCANNONS     = 0xC5;
		public static byte AREA_TRISHOTCANNONS   = 0xC6;
		public static byte AREA_END              = 0x77;

		public static int PLEN_BULLETCANNON    =  6;
		public static int PLEN_RELAYCANNON     =  4;
		public static int PLEN_LASERCANNON     = 12;
		public static int PLEN_TRISHOTCANNON   =  6;
		public static int PLEN_MINIGUN         =  6;
		public static int PLEN_SHIELDPROJECTOR = 12;
		public static int PLEN_BULLETS         = 10;

		public static int REMOTE_BULLET_UPDATELESS_LIFETIME = 8;
		
		public static int PACKAGE_FORWARD_HEADER_SIZE = 10;

		public readonly long[] RecieveBigSeq = new long[32];

		public GDGameScreen Screen;

		protected int packageCount = 0;
		protected int packageModSize = 0;
		protected float lagBehindTime = 0f;

		protected GDMultiplayerCommon(INetworkMedium medium) : base(medium)
		{
		}

		public override void Update(SAMTime gameTime, InputState istate)
		{
			base.Update(gameTime, istate);

			if (Mode == ServerMode.CreatingNewGame) Screen = null;
			if (Mode == ServerMode.BeforeNewGame) Screen = null;
		}

		protected void ProcessStateData(byte[] d, byte euid)
		{
			// [8: CMD] [8:seq] [16: SessionID] [4: UserID] [12: SessionSecret] [32:time] [~: Payload]

			RecieveBigSeq[euid]++;
			var bseq = RecieveBigSeq[euid];

			var sendertime = NetworkDataTools.GetSingle(d[6], d[7], d[8], d[9]);

			if (sendertime - 0.05f > Screen.LevelTime)
			{
				SAMLog.Warning("GDMPC::FFWD", $"Fastforward level by {sendertime - Screen.LevelTime}s ({Screen.LevelTime} --> {sendertime})");
				Screen.FastForward(sendertime);
			}

			lagBehindTime = Screen.LevelTime - sendertime;

			int p = PACKAGE_FORWARD_HEADER_SIZE;
			for (;;)
			{
				byte cmd = d[p];
				p++;

				if (cmd == AREA_END) break;

				if (p >= MAX_PACKAGE_SIZE_BYTES)
				{
					SAMLog.Error("SNS-COMMON::OOB", "OOB: " + p);
					break;
				}
				else if (cmd == AREA_BULLETCANNONS)
				{
					ProcessForwardBulletCannons(ref p, d, bseq, sendertime);
				}
				else if (cmd == AREA_BULLETS)
				{
					ProcessForwardBullets(ref p, d, bseq, sendertime);
				}
				else if (cmd == AREA_LASERCANNONS)
				{
					ProcessForwardLaserCannons(ref p, d, bseq, sendertime);
				}
				else if (cmd == AREA_SHIELDPROJECTORS)
				{
					ProcessForwardShieldProjectors(ref p, d, bseq, sendertime);
				}
				else if (cmd == AREA_MINIGUNS)
				{
					ProcessForwardMiniguns(ref p, d, bseq, sendertime);
				}
				else if (cmd == AREA_TRISHOTCANNONS)
				{
					ProcessForwardTrishotCannons(ref p, d, bseq, sendertime);
				}
				else if (cmd == AREA_RELAYCANNONS)
				{
					ProcessForwardRelayCannons(ref p, d, bseq, sendertime);
				}
				else
				{
					SAMLog.Error("SNS-COMMON::UA", "Unknown AREA: " + cmd);
					break;
				}
			}
		}

		private void ProcessForwardBulletCannons(ref int p, byte[] d, long bseq, float sendertime)
		{
			int count = d[p];
			p++;

			for (int i = 0; i < count; i++)
			{
				var id = NetworkDataTools.GetByte(d[p + 0]);
				var frac = Screen.GetFractionByID(NetworkDataTools.GetHighBits(d[p + 1], 3));
				var boost = NetworkDataTools.GetLowBits(d[p + 1], 5);
				var rotA = NetworkDataTools.ConvertToRadians(NetworkDataTools.GetByte(d[p + 2]), 8);
				var rotT = NetworkDataTools.ConvertToRadians(NetworkDataTools.GetByte(d[p + 3]), 8);
				var hp = NetworkDataTools.GetByte(d[p + 4]) / 255f;
				var chrg = NetworkDataTools.GetByte(d[p + 5]) / 255f;

				Cannon c;
				if (Screen.CannonMap.TryGetValue(id, out c))
				{
					BulletCannon bc = c as BulletCannon;
					if (bc != null && ShouldRecieveData(frac, bc))
					{
						if (ShouldRecieveRotationData(frac, bc))
						{
							bc.RemoteRotationUpdate(rotA, rotT, sendertime);
						}

						if (ShouldRecieveStateData(frac, bc))
						{
							bc.RemoteUpdate(frac, hp, boost, chrg, sendertime);
						}
					}
				}

				p += PLEN_BULLETCANNON;
			}
		}

		private void ProcessForwardRelayCannons(ref int p, byte[] d, long bseq, float sendertime)
		{
			int count = d[p];
			p++;

			for (int i = 0; i < count; i++)
			{
				var id = NetworkDataTools.GetByte(d[p + 0]);
				var frac = Screen.GetFractionByID(NetworkDataTools.GetHighBits(d[p + 1], 3));
				var boost = NetworkDataTools.GetLowBits(d[p + 1], 5);
				var rotA = NetworkDataTools.ConvertToRadians(NetworkDataTools.GetByte(d[p + 2]), 8);
				var rotT = NetworkDataTools.ConvertToRadians(NetworkDataTools.GetByte(d[p + 3]), 8);

				Cannon c;
				if (Screen.CannonMap.TryGetValue(id, out c))
				{
					RelayCannon rc = c as RelayCannon;
					if (rc != null && ShouldRecieveData(frac, rc))
					{
						if (ShouldRecieveRotationData(frac, rc))
						{
							rc.RemoteRotationUpdate(rotA, rotT, sendertime);
						}

						if (ShouldRecieveStateData(frac, rc))
						{
							rc.RemoteUpdate(frac, sendertime);
						}
					}
				}

				p += PLEN_RELAYCANNON;
			}
		}

		private void ProcessForwardLaserCannons(ref int p, byte[] d, long bseq, float sendertime)
		{
			int count = d[p];
			p++;

			for (int i = 0; i < count; i++)
			{
				var id = NetworkDataTools.GetByte(d[p + 0]);
				var frac = Screen.GetFractionByID(NetworkDataTools.GetHighBits(d[p + 1], 3));
				var boost = NetworkDataTools.GetLowBits(d[p + 1], 5);
				var rotA = NetworkDataTools.GetSingle(d[p + 2], d[p + 3], d[p + 4], d[p + 5]);
				var rotT = NetworkDataTools.GetSingle(d[p + 6], d[p + 7], d[p + 8], d[p + 9]);
				var hp = NetworkDataTools.GetByte(d[p + 10]) / 255f;
				var ct = (NetworkDataTools.GetByte(d[p + 11]) / 255f) * Cannon.LASER_CHARGE_COOLDOWN_MAX;

				Cannon c;
				if (Screen.CannonMap.TryGetValue(id, out c))
				{
					LaserCannon bc = c as LaserCannon;
					if (bc != null && ShouldRecieveData(frac, bc))
					{
						if (ShouldRecieveRotationData(frac, bc))
						{
							bc.RemoteRotationUpdate(rotA, rotT, sendertime);
						}

						if (ShouldRecieveStateData(frac, bc))
						{
							bc.RemoteUpdate(frac, hp, boost, ct, sendertime);
						}
					}
				}

				p += PLEN_LASERCANNON;
			}
		}

		private void ProcessForwardTrishotCannons(ref int p, byte[] d, long bseq, float sendertime)
		{
			int count = d[p];
			p++;

			for (int i = 0; i < count; i++)
			{
				var id = NetworkDataTools.GetByte(d[p + 0]);
				var frac = Screen.GetFractionByID(NetworkDataTools.GetHighBits(d[p + 1], 3));
				var boost = NetworkDataTools.GetLowBits(d[p + 1], 5);
				var rotA = NetworkDataTools.ConvertToRadians(NetworkDataTools.GetByte(d[p + 2]), 8);
				var rotT = NetworkDataTools.ConvertToRadians(NetworkDataTools.GetByte(d[p + 3]), 8);
				var hp = NetworkDataTools.GetByte(d[p + 4]) / 255f;
				var chrg = NetworkDataTools.GetByte(d[p + 5]) / 255f;

				Cannon c;
				if (Screen.CannonMap.TryGetValue(id, out c))
				{
					TrishotCannon bc = c as TrishotCannon;
					if (bc != null && ShouldRecieveData(frac, bc))
					{
						if (ShouldRecieveRotationData(frac, bc))
						{
							bc.RemoteRotationUpdate(rotA, rotT, sendertime);
						}

						if (ShouldRecieveStateData(frac, bc))
						{
							bc.RemoteUpdate(frac, hp, boost, chrg, sendertime);
						}
					}
				}

				p += PLEN_TRISHOTCANNON;
			}
		}

		private void ProcessForwardMiniguns(ref int p, byte[] d, long bseq, float sendertime)
		{
			int count = d[p];
			p++;

			for (int i = 0; i < count; i++)
			{
				var id = NetworkDataTools.GetByte(d[p + 0]);
				var frac = Screen.GetFractionByID(NetworkDataTools.GetHighBits(d[p + 1], 3));
				var boost = NetworkDataTools.GetLowBits(d[p + 1], 5);
				var rotA = NetworkDataTools.ConvertToRadians(NetworkDataTools.GetByte(d[p + 2]), 8);
				var rotT = NetworkDataTools.ConvertToRadians(NetworkDataTools.GetByte(d[p + 3]), 8);
				var hp = NetworkDataTools.GetByte(d[p + 4]) / 255f;
				var chrg = NetworkDataTools.GetByte(d[p + 5]) / 255f;

				Cannon c;
				if (Screen.CannonMap.TryGetValue(id, out c))
				{
					MinigunCannon bc = c as MinigunCannon;
					if (bc != null && ShouldRecieveData(frac, bc))
					{
						if (ShouldRecieveRotationData(frac, bc))
						{
							bc.RemoteRotationUpdate(rotA, rotT, sendertime);
						}

						if (ShouldRecieveStateData(frac, bc))
						{
							bc.RemoteUpdate(frac, hp, boost, chrg, sendertime);
						}
					}
				}

				p += PLEN_MINIGUN;
			}
		}

		private void ProcessForwardShieldProjectors(ref int p, byte[] d, long bseq, float sendertime)
		{
			int count = d[p];
			p++;

			for (int i = 0; i < count; i++)
			{
				var id = NetworkDataTools.GetByte(d[p + 0]);
				var frac = Screen.GetFractionByID(NetworkDataTools.GetHighBits(d[p + 1], 3));
				var boost = NetworkDataTools.GetLowBits(d[p + 1], 5);
				var rotA = NetworkDataTools.GetSingle(d[p + 2], d[p + 3], d[p + 4], d[p + 5]);
				var rotT = NetworkDataTools.GetSingle(d[p + 6], d[p + 7], d[p + 8], d[p + 9]);
				var hp = NetworkDataTools.GetByte(d[p + 10]) / 255f;
				var ct = (NetworkDataTools.GetByte(d[p + 11]) / 255f) * Cannon.SHIELD_CHARGE_COOLDOWN_MAX;

				Cannon c;
				if (Screen.CannonMap.TryGetValue(id, out c))
				{
					ShieldProjectorCannon sc = c as ShieldProjectorCannon;
					if (sc != null && ShouldRecieveData(frac, sc))
					{
						if (ShouldRecieveRotationData(frac, sc))
						{
							sc.RemoteRotationUpdate(rotA, rotT, sendertime);
						}

						if (ShouldRecieveStateData(frac, sc))
						{
							sc.RemoteUpdate(frac, hp, boost, ct, sendertime);
						}
					}
				}

				p += PLEN_SHIELDPROJECTOR;
			}
		}

		private void ProcessForwardBullets(ref int p, byte[] d, long bseq, float sendertime)
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
							bullet.RemoteUpdate(state, px, py, veloc, fraction, scale, bseq, sendertime);
						}
						else
						{
							Screen.RemoteBulletMapping[id] = new RemoteBullet(Screen, new FPoint(px, py), veloc, id, scale, fraction, bseq);
							Screen.RemoteBulletMapping[id].RemoteState = state;
							Screen.Entities.AddEntity(Screen.RemoteBulletMapping[id]);

							Screen.RemoteBulletMapping[id].RemoteUpdate(state, px, py, veloc, fraction, scale, bseq, sendertime);
							Screen.RemoteBulletMapping[id].ClientPredictionMiss = false;
						}
						break;
					case RemoteBullet.RemoteBulletState.Dying_Explosion:
					case RemoteBullet.RemoteBulletState.Dying_ShrinkSlow:
					case RemoteBullet.RemoteBulletState.Dying_ShrinkFast:
					case RemoteBullet.RemoteBulletState.Dying_Fade:
					case RemoteBullet.RemoteBulletState.Dying_Instant:
					case RemoteBullet.RemoteBulletState.Dying_FadeSlow:
						if (bullet != null && bullet.RemoteState != state)
						{
							bullet.RemoteUpdate(state, px, py, veloc, fraction, scale, bseq, sendertime);
						}
						break;
					default:
						SAMLog.Error("GDMC::EnumSwitch_PFB", "Unknown enum value: " + state);
						break;
				}
				
				p +=PLEN_BULLETS;
			}

			for (int i = 0; i < GDGameScreen.MAX_BULLET_ID; i++)
			{
				if (Screen.RemoteBulletMapping[i] != null && bseq - Screen.RemoteBulletMapping[i].LastUpdateBigSeq > REMOTE_BULLET_UPDATELESS_LIFETIME)
				{
					if (Screen.RemoteBulletMapping[i].RemoteState == RemoteBullet.RemoteBulletState.Normal)
					{
						SAMLog.Debug("Mercykill Bullet: " + i);
						Screen.RemoteBulletMapping[i].Alive = false;
					}
					else
					{
						// all ok - its dying
					}
				}
			}
		}

		protected void SendAndReset(ref int idx)
		{
			SetSequenceCounter(ref MSG_FORWARD[1]);

			MSG_FORWARD[idx] = AREA_END;
			Send(MSG_FORWARD);
			packageModSize = idx;

			idx = PACKAGE_FORWARD_HEADER_SIZE;

			packageCount++;
		}

		protected void SendForwardRelayCannons(ref int idx)
		{
			var data = Screen.GetEntities<RelayCannon>().ToList();
			if (data.Count == 0) return;

			if (idx + 2 >= MAX_PACKAGE_SIZE_BYTES) SendAndReset(ref idx);

			MSG_FORWARD[idx] = AREA_BULLETCANNONS;
			idx++;

			byte arrsize = (byte)((MAX_PACKAGE_SIZE_BYTES - idx - 2) / PLEN_RELAYCANNON);

			int posSize = idx;

			MSG_FORWARD[posSize] = 0xFF;
			idx++;

			int i = 0;
			foreach (var cannon in data)
			{
				if (!ShouldSendData(cannon)) continue;

				// [8: ID] [3: Fraction] [5: Boost] [8: RotationActual] [8: RotationTarget]

				NetworkDataTools.SetByte(out MSG_FORWARD[idx + 0], cannon.BlueprintCannonID);
				NetworkDataTools.SetSplitByte(out MSG_FORWARD[idx + 1], Screen.GetFractionID(cannon.Fraction), cannon.IntegerBoost, 3, 5, 3, 5);
				NetworkDataTools.SetByte(out MSG_FORWARD[idx + 2], NetworkDataTools.ConvertFromRadians(cannon.Rotation.ActualValue, 8));
				NetworkDataTools.SetByte(out MSG_FORWARD[idx + 3], NetworkDataTools.ConvertFromRadians(cannon.Rotation.TargetValue, 8));

				idx += PLEN_RELAYCANNON;

				i++;
				if (i >= arrsize)
				{
					MSG_FORWARD[posSize] = (byte)i;
					SendAndReset(ref idx);
					MSG_FORWARD[idx] = AREA_RELAYCANNONS;
					idx++;
					i -= arrsize;
					arrsize = (byte)((MAX_PACKAGE_SIZE_BYTES - idx - 2) / PLEN_RELAYCANNON);
					posSize = idx;
					MSG_FORWARD[posSize] = 0xFF;
					idx++;
				}
			}
			MSG_FORWARD[posSize] = (byte)i;
		}

		protected void SendForwardBulletCannons(ref int idx)
		{
			var data = Screen.GetEntities<BulletCannon>().ToList();
			if (data.Count == 0) return;

			if (idx + 2 >= MAX_PACKAGE_SIZE_BYTES) SendAndReset(ref idx);

			MSG_FORWARD[idx] = AREA_BULLETCANNONS;
			idx++;

			byte arrsize = (byte)((MAX_PACKAGE_SIZE_BYTES - idx - 2) / PLEN_BULLETCANNON);

			int posSize = idx;

			MSG_FORWARD[posSize] = 0xFF;
			idx++;

			int i = 0;
			foreach (var cannon in data)
			{
				if (!ShouldSendData(cannon)) continue;

				// [8: ID] [3: Fraction] [5: Boost] [8: RotationActual] [8: RotationTarget] [8: Health] [8:Charge]

				NetworkDataTools.SetByte(out MSG_FORWARD[idx + 0], cannon.BlueprintCannonID);
				NetworkDataTools.SetSplitByte(out MSG_FORWARD[idx + 1], Screen.GetFractionID(cannon.Fraction), cannon.IntegerBoost, 3, 5, 3, 5);
				NetworkDataTools.SetByte(out MSG_FORWARD[idx + 2], NetworkDataTools.ConvertFromRadians(cannon.Rotation.ActualValue, 8));
				NetworkDataTools.SetByte(out MSG_FORWARD[idx + 3], NetworkDataTools.ConvertFromRadians(cannon.Rotation.TargetValue, 8));
				NetworkDataTools.SetByteFloor(out MSG_FORWARD[idx + 4], FloatMath.Clamp(cannon.CannonHealth.TargetValue, 0f, 1f) * 255);
				NetworkDataTools.SetByteFloor(out MSG_FORWARD[idx + 5], FloatMath.Clamp(cannon.BarrelCharge, 0f, 1f) * 255);

				idx += PLEN_BULLETCANNON;

				i++;
				if (i >= arrsize)
				{
					MSG_FORWARD[posSize] = (byte)i;
					SendAndReset(ref idx);
					MSG_FORWARD[idx] = AREA_BULLETCANNONS;
					idx++;
					i -= arrsize;
					arrsize = (byte)((MAX_PACKAGE_SIZE_BYTES - idx - 2) / PLEN_BULLETCANNON);
					posSize = idx;
					MSG_FORWARD[posSize] = 0xFF;
					idx++;
				}
			}
			MSG_FORWARD[posSize] = (byte)i;
		}

		protected void SendForwardTrishotCannons(ref int idx)
		{
			var data = Screen.GetEntities<TrishotCannon>().ToList();
			if (data.Count == 0) return;

			if (idx + 2 >= MAX_PACKAGE_SIZE_BYTES) SendAndReset(ref idx);

			MSG_FORWARD[idx] = AREA_TRISHOTCANNONS;
			idx++;

			byte arrsize = (byte)((MAX_PACKAGE_SIZE_BYTES - idx - 2) / PLEN_TRISHOTCANNON);

			int posSize = idx;

			MSG_FORWARD[posSize] = 0xFF;
			idx++;

			int i = 0;
			foreach (var cannon in data)
			{
				if (!ShouldSendData(cannon)) continue;

				// [8: ID] [3: Fraction] [5: Boost] [8: RotationActual] [8: RotationTarget] [8: Health] [8:Charge]

				NetworkDataTools.SetByte(out MSG_FORWARD[idx + 0], cannon.BlueprintCannonID);
				NetworkDataTools.SetSplitByte(out MSG_FORWARD[idx + 1], Screen.GetFractionID(cannon.Fraction), cannon.IntegerBoost, 3, 5, 3, 5);
				NetworkDataTools.SetByte(out MSG_FORWARD[idx + 2], NetworkDataTools.ConvertFromRadians(cannon.Rotation.ActualValue, 8));
				NetworkDataTools.SetByte(out MSG_FORWARD[idx + 3], NetworkDataTools.ConvertFromRadians(cannon.Rotation.TargetValue, 8));
				NetworkDataTools.SetByteFloor(out MSG_FORWARD[idx + 4], FloatMath.Clamp(cannon.CannonHealth.TargetValue, 0f, 1f) * 255);
				NetworkDataTools.SetByteFloor(out MSG_FORWARD[idx + 5], FloatMath.Clamp(cannon.BarrelCharge, 0f, 1f) * 255);

				idx += PLEN_TRISHOTCANNON;

				i++;
				if (i >= arrsize)
				{
					MSG_FORWARD[posSize] = (byte)i;
					SendAndReset(ref idx);
					MSG_FORWARD[idx] = AREA_TRISHOTCANNONS;
					idx++;
					i -= arrsize;
					arrsize = (byte)((MAX_PACKAGE_SIZE_BYTES - idx - 2) / PLEN_TRISHOTCANNON);
					posSize = idx;
					MSG_FORWARD[posSize] = 0xFF;
					idx++;
				}
			}
			MSG_FORWARD[posSize] = (byte)i;
		}

		protected void SendForwardMiniguns(ref int idx)
		{
			var data = Screen.GetEntities<MinigunCannon>().ToList();
			if (data.Count == 0) return;

			if (idx + 2 >= MAX_PACKAGE_SIZE_BYTES) SendAndReset(ref idx);

			MSG_FORWARD[idx] = AREA_MINIGUNS;
			idx++;

			byte arrsize = (byte)((MAX_PACKAGE_SIZE_BYTES - idx - 2) / PLEN_MINIGUN);

			int posSize = idx;

			MSG_FORWARD[posSize] = 0xFF;
			idx++;

			int i = 0;
			foreach (var cannon in data)
			{
				if (!ShouldSendData(cannon)) continue;

				// [8: ID] [3: Fraction] [5: Boost] [8: RotationActual] [8: RotationTarget] [8: Health] [8:Charge]

				NetworkDataTools.SetByte(out MSG_FORWARD[idx + 0], cannon.BlueprintCannonID);
				NetworkDataTools.SetSplitByte(out MSG_FORWARD[idx + 1], Screen.GetFractionID(cannon.Fraction), cannon.IntegerBoost, 3, 5, 3, 5);
				NetworkDataTools.SetByte(out MSG_FORWARD[idx + 2], NetworkDataTools.ConvertFromRadians(cannon.Rotation.ActualValue, 8));
				NetworkDataTools.SetByte(out MSG_FORWARD[idx + 3], NetworkDataTools.ConvertFromRadians(cannon.Rotation.TargetValue, 8));
				NetworkDataTools.SetByteFloor(out MSG_FORWARD[idx + 4], FloatMath.Clamp(cannon.CannonHealth.TargetValue, 0f, 1f) * 255);
				NetworkDataTools.SetByteFloor(out MSG_FORWARD[idx + 5], FloatMath.Clamp(cannon.BarrelCharge, 0f, 1f) * 255);

				idx += PLEN_MINIGUN;

				i++;
				if (i >= arrsize)
				{
					MSG_FORWARD[posSize] = (byte)i;
					SendAndReset(ref idx);
					MSG_FORWARD[idx] = AREA_MINIGUNS;
					idx++;
					i -= arrsize;
					arrsize = (byte)((MAX_PACKAGE_SIZE_BYTES - idx - 2) / PLEN_MINIGUN);
					posSize = idx;
					MSG_FORWARD[posSize] = 0xFF;
					idx++;
				}
			}
			MSG_FORWARD[posSize] = (byte)i;
		}

		protected void SendForwardLaserCannons(ref int idx)
		{
			var data = Screen.GetEntities<LaserCannon>().ToList();
			if (data.Count == 0) return;

			if (idx + 2 >= MAX_PACKAGE_SIZE_BYTES) SendAndReset(ref idx);

			MSG_FORWARD[idx] = AREA_LASERCANNONS;
			idx++;

			byte arrsize = (byte)((MAX_PACKAGE_SIZE_BYTES - idx - 2) / PLEN_LASERCANNON);

			int posSize = idx;

			MSG_FORWARD[posSize] = 0xFF;
			idx++;

			int i = 0;
			foreach (var cannon in data)
			{
				if (!ShouldSendData(cannon)) continue;

				// [8: ID] [3: Fraction] [5: Boost] [32: RotationActual] [32: RotationTarget] [8: Health] [8:ChargeTime]

				NetworkDataTools.SetByte(out MSG_FORWARD[idx + 0], cannon.BlueprintCannonID);
				NetworkDataTools.SetSplitByte(out MSG_FORWARD[idx + 1], Screen.GetFractionID(cannon.Fraction), cannon.IntegerBoost, 3, 5, 3, 5);
				NetworkDataTools.SetSingle(out MSG_FORWARD[idx + 2], out MSG_FORWARD[idx + 3], out MSG_FORWARD[idx + 4], out MSG_FORWARD[idx + 5], cannon.Rotation.ActualValue);
				NetworkDataTools.SetSingle(out MSG_FORWARD[idx + 6], out MSG_FORWARD[idx + 7], out MSG_FORWARD[idx + 8], out MSG_FORWARD[idx + 9], cannon.Rotation.TargetValue);
				NetworkDataTools.SetByteFloor(out MSG_FORWARD[idx + 10], FloatMath.Clamp(cannon.CannonHealth.TargetValue, 0f, 1f) * 255);
				NetworkDataTools.SetByteFloor(out MSG_FORWARD[idx + 11], FloatMath.Clamp(cannon.ChargeTime / Cannon.LASER_CHARGE_COOLDOWN_MAX, 0f, 1f) * 255);

				idx += PLEN_LASERCANNON;

				i++;
				if (i >= arrsize)
				{
					MSG_FORWARD[posSize] = (byte)i;
					SendAndReset(ref idx);
					MSG_FORWARD[idx] = AREA_LASERCANNONS;
					idx++;
					i -= arrsize;
					arrsize = (byte)((MAX_PACKAGE_SIZE_BYTES - idx - 2) / PLEN_LASERCANNON);
					posSize = idx;
					MSG_FORWARD[posSize] = 0xFF;
					idx++;
				}
			}
			MSG_FORWARD[posSize] = (byte)i;
		}

		protected void SendForwardShieldProjectors(ref int idx)
		{
			var data = Screen.GetEntities<ShieldProjectorCannon>().ToList();
			if (data.Count == 0) return;

			if (idx + 2 >= MAX_PACKAGE_SIZE_BYTES) SendAndReset(ref idx);

			MSG_FORWARD[idx] = AREA_SHIELDPROJECTORS;
			idx++;

			byte arrsize = (byte)((MAX_PACKAGE_SIZE_BYTES - idx - 2) / PLEN_SHIELDPROJECTOR);

			int posSize = idx;

			MSG_FORWARD[posSize] = 0xFF;
			idx++;

			int i = 0;
			foreach (var cannon in data)
			{
				if (!ShouldSendData(cannon)) continue;

				// [8: ID] [3: Fraction] [5: Boost] [32: RotationActual] [32: RotationTarget] [8: Health] [8:ChargeTime]

				NetworkDataTools.SetByte(out MSG_FORWARD[idx + 0], cannon.BlueprintCannonID);
				NetworkDataTools.SetSplitByte(out MSG_FORWARD[idx + 1], Screen.GetFractionID(cannon.Fraction), cannon.IntegerBoost, 3, 5, 3, 5);
				NetworkDataTools.SetSingle(out MSG_FORWARD[idx + 2], out MSG_FORWARD[idx + 3], out MSG_FORWARD[idx + 4], out MSG_FORWARD[idx + 5], cannon.Rotation.ActualValue);
				NetworkDataTools.SetSingle(out MSG_FORWARD[idx + 6], out MSG_FORWARD[idx + 7], out MSG_FORWARD[idx + 8], out MSG_FORWARD[idx + 9], cannon.Rotation.TargetValue);
				NetworkDataTools.SetByteFloor(out MSG_FORWARD[idx + 10], FloatMath.Clamp(cannon.CannonHealth.TargetValue, 0f, 1f) * 255);
				NetworkDataTools.SetByteFloor(out MSG_FORWARD[idx + 11], FloatMath.Clamp(cannon.ChargeTime / Cannon.SHIELD_CHARGE_COOLDOWN_MAX, 0f, 1f) * 255);

				idx += PLEN_LASERCANNON;

				i++;
				if (i >= arrsize)
				{
					MSG_FORWARD[posSize] = (byte)i;
					SendAndReset(ref idx);
					MSG_FORWARD[idx] = AREA_SHIELDPROJECTORS;
					idx++;
					i -= arrsize;
					arrsize = (byte)((MAX_PACKAGE_SIZE_BYTES - idx - 2) / PLEN_SHIELDPROJECTOR);
					posSize = idx;
					MSG_FORWARD[posSize] = 0xFF;
					idx++;
				}
			}
			MSG_FORWARD[posSize] = (byte)i;
		}

		protected void SendForwardBullets(ref int idx)
		{
			if (idx + 2 >= MAX_PACKAGE_SIZE_BYTES) SendAndReset(ref idx);

			MSG_FORWARD[idx] = AREA_BULLETS;
			idx++;

			byte arrsize = (byte)((MAX_PACKAGE_SIZE_BYTES - idx - 2) / PLEN_BULLETS);

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

				idx += PLEN_BULLETS;

				i++;
				if (i >= arrsize)
				{
					MSG_FORWARD[posSize] = (byte)i;
					SendAndReset(ref idx);
					MSG_FORWARD[idx] = AREA_BULLETS;
					idx++;
					i -= arrsize;
					arrsize = (byte)((MAX_PACKAGE_SIZE_BYTES - idx - 2) / PLEN_BULLETS);
					posSize = idx;
					MSG_FORWARD[posSize] = 0xFF;
					idx++;
				}
			}
			MSG_FORWARD[posSize] = (byte)i;
		}

		protected abstract bool ShouldRecieveData(Fraction f, Cannon c);
		protected abstract bool ShouldRecieveRotationData(Fraction f, Cannon c);
		protected abstract bool ShouldRecieveStateData(Fraction f, Cannon c);
		protected abstract bool ShouldSendData(Cannon c);
	}
}
