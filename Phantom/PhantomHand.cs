using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Bluemagic.Phantom
{
	public class PhantomHand : ModNPC
	{
		private const float maxSpeed = 8f;

		public override void SetDefaults()
		{
			npc.name = "Phantom Hand";
			npc.displayName = "The Phantom";
			npc.aiStyle = -1;
			npc.lifeMax = 50000;
			npc.damage = 120;
			npc.defense = 50;
			npc.knockBackResist = 0f;
			npc.dontTakeDamage = true;
			npc.width = 80;
			npc.height = 80;
			npc.alpha = 70;
			npc.value = Item.buyPrice(0, 15, 0, 0);
			npc.npcSlots = 0f;
			npc.boss = true;
			npc.lavaImmune = true;
			npc.noGravity = true;
			npc.noTileCollide = true;
			npc.HitSound = SoundID.NPCHit1;
			npc.DeathSound = SoundID.NPCDeath6;
			for (int k = 0; k < npc.buffImmune.Length; k++)
			{
				npc.buffImmune[k] = true;
			}
		}

		public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
		{
			npc.lifeMax = (int)(npc.lifeMax * 0.7f * bossLifeScale);
			npc.damage = (int)(npc.damage * 0.7f);
		}

		public Phantom Head
		{
			get
			{
				return (Phantom)Main.npc[(int)npc.ai[0]].modNPC;
			}
		}

		public float Direction
		{
			get
			{
				return npc.ai[1];
			}
		}

		public float AttackID
		{
			get
			{
				return npc.ai[2];
			}
			set
			{
				npc.ai[2] = value;
			}
		}

		public float AttackTimer
		{
			get
			{
				return npc.ai[3];
			}
			set
			{
				npc.ai[3] = value;
			}
		}

		public float MaxAttackTimer
		{
			get
			{
				return 60f * (float)Head.npc.life / (float)Head.npc.lifeMax;
			}
		}

		public override void AI()
		{
			if (Head.Enraged)
			{
				npc.damage = npc.defDamage * 2;
				npc.defense = npc.defDefense * 2;
			}
			npc.direction = (int)Direction;
			npc.spriteDirection = (int)Direction;
			if (!npc.HasValidTarget)
			{
				npc.TargetClosest(false);
			}

			if (AttackID == 1 || AttackID == 4)
			{
				if (Direction == -1f)
				{
					HammerAttack();
				}
				else
				{
					WispAttack();
				}
			}
			else if (AttackID == 2 || AttackID == 5)
			{
				if (Direction == -1f)
				{
					BladeAttack();
				}
				else
				{
					HammerAttack();
				}
			}
			else if (AttackID == 3)
			{
				ChargeAttack();
			}
			else
			{
				IdleBehavior();
			}
			AttackTimer += 1f;
			if (AttackTimer >= MaxAttackTimer)
			{
				ChooseAttack();
			}
			else if (AttackTimer >= 0)
			{
				AttackID = 0f;
			}
		}

		private void ChooseAttack()
		{
			AttackID += 1f;
			if (AttackID >= 6f)
			{
				AttackID = 1f;
			}
			if (AttackID == 1f || AttackID == 4f)
			{
				if (Direction == -1f)
				{
					AttackTimer = -120f;
				}
				else
				{
					AttackTimer = -120f;
				}
			}
			else if (AttackID == 2f || AttackID == 5f)
			{
				if (Direction == -1f)
				{
					AttackTimer = -120f;
				}
				else
				{
					AttackTimer = -120f;
				}
			}
			else if (AttackID == 3f)
			{
				AttackTimer = -60f;
			}
			npc.TargetClosest(false);
			npc.netUpdate = true;
		}

		private void IdleBehavior()
		{
			Vector2 target = Head.npc.Bottom + new Vector2(Direction * 128f, 64f);
			Vector2 change = target - npc.Bottom;
			CapVelocity(ref change, maxSpeed * 2f);
			ModifyVelocity(change);
			CapVelocity(ref npc.velocity, maxSpeed * 2f);
		}

		private void HammerAttack()
		{
			Vector2 target = Main.player[npc.target].Center;
			Vector2 moveTarget = target + new Vector2(Direction * 240f, -240f);
			Vector2 offset = moveTarget - npc.Center;
			CapVelocity(ref offset, maxSpeed);
			ModifyVelocity(offset);
			CapVelocity(ref npc.velocity, maxSpeed);
		}

		private void BladeAttack()
		{
			Vector2 target = Main.player[npc.target].Center;
			Vector2 moveTarget = target + new Vector2(Direction * 240f, 0f);
			Vector2 offset = moveTarget - npc.Center;
			CapVelocity(ref offset, maxSpeed);
			ModifyVelocity(offset);
			CapVelocity(ref npc.velocity, maxSpeed);
		}

		private void WispAttack()
		{
			Vector2 target = Main.player[npc.target].Center;
			Vector2 moveTarget = target + new Vector2(Direction * 240f, 0f);
			Vector2 offset = moveTarget - npc.Center;
			CapVelocity(ref offset, maxSpeed);
			ModifyVelocity(offset);
			CapVelocity(ref npc.velocity, maxSpeed);
		}

		private void ChargeAttack()
		{
			if (AttackTimer < -40f)
			{
				Vector2 offset = Main.player[npc.target].Center - npc.Center;
				CapVelocity(ref offset, maxSpeed);
				ModifyVelocity(offset, 0.1f);
				CapVelocity(ref npc.velocity, maxSpeed);
			}
		}

		private void ModifyVelocity(Vector2 modify, float weight = 0.05f)
		{
			npc.velocity = Vector2.Lerp(npc.velocity, modify, weight);
		}

		private void CapVelocity(ref Vector2 velocity, float max)
		{
			if (velocity.Length() > max)
			{
				velocity.Normalize();
				velocity *= max;
			}
		}

		public override Color? GetAlpha(Color lightColor)
		{
			return Color.White * 0.8f;
		}
	}
}