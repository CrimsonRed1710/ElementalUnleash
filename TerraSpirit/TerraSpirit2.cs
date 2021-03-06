using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using System.Reflection;
using Bluemagic.PuritySpirit;

namespace Bluemagic.TerraSpirit
{
	public class TerraSpirit2 : ModNPC
	{
		private const int size = 120;
		private const int particleSize = 12;
		public const int arenaWidth = 2400;
		public const int arenaHeight = 1600;

		internal int Stage
		{
			get
			{
				return (int)npc.ai[0];
			}
			set
			{
				npc.ai[0] = value;
			}
		}

		internal int Progress
		{
			get
			{
				return (int)npc.ai[1];
			}
			set
			{
				npc.ai[1] = value;
			}
		}

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Spirit of Purity");
			Main.npcFrameCount[npc.type] = 4;
			NPCID.Sets.MustAlwaysDraw[npc.type] = true;
			NPCID.Sets.NeedsExpertScaling[npc.type] = false;
		}

		public override void SetDefaults()
		{
			npc.aiStyle = -1;
			npc.lifeMax = 750000;
			npc.damage = 0;
			npc.defense = 0;
			npc.knockBackResist = 0f;
			npc.width = size;
			npc.height = size;
			npc.npcSlots = 1337f;
			npc.boss = true;
			npc.lavaImmune = true;
			npc.noGravity = true;
			npc.noTileCollide = true;
			npc.chaseable = false;
			npc.HitSound = new LegacySoundStyle(15, 0, Terraria.Audio.SoundType.Sound);
			npc.DeathSound = null;
			npc.alpha = 255;
			for (int k = 0; k < npc.buffImmune.Length; k++)
			{
				npc.buffImmune[k] = true;
			}
			music = MusicID.LunarBoss;
		}

		internal List<Bullet> bullets = new List<Bullet>();

		public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
		{
			scale = 2f;
			return null;
		}

		public override void AI()
		{
			int numPlayers = CountPlayers();
			npc.timeLeft = NPC.activeTime;
			if (Stage >= 0 && numPlayers == 0)
			{
				Stage = -1;
				Progress = 0;
			}
			if (Stage == -1)
			{
				RunAway();
			}
			else if (Stage == 0)
			{
				Initialize();
			}
			else if (Stage == 1)
			{
				Recover();
			}
			else if (Stage == 2)
			{
				Attack();
			}
			else if (Stage == 3)
			{
				End();
			}
			Progress++;
			Rectangle bounds = new Rectangle((int)npc.Center.X - arenaWidth / 2, (int)npc.Center.Y - arenaHeight / 2, arenaWidth, arenaHeight);
			for (int k = 0; k < bullets.Count; k++)
			{
				if (bullets[k].Update(null, bounds))
				{
					Player player = Main.player[Main.myPlayer];
					if (player.active && !player.dead && player.GetModPlayer<BluemagicPlayer>().terraLives > 0 && bullets[k].Collides(player.Hitbox))
					{
						player.GetModPlayer<BluemagicPlayer>().TerraKill();
					}
				}
				else
				{
					bullets.RemoveAt(k);
					k--;
				}
			}
		}

		private int CountPlayers()
		{
			int count = 0;
			for (int k = 0; k < 255; k++)
			{
				Player player = Main.player[k];
				if (player.active && !player.dead && player.GetModPlayer<BluemagicPlayer>().terraLives > 0)
				{
					count++;
				}
			}
			return count;
		}

		public void RunAway()
		{
			if (Progress >= 180)
			{
				npc.active = false;
				if (Main.netMode != 1)
				{
					BluemagicWorld.terraDeaths++;
					if (Main.netMode == 2)
					{
						NetMessage.SendData(MessageID.WorldData);
					}
				}
			}
		}

		public void Initialize()
		{
			if (Progress == Main.netMode)
			{
				bullets.Clear();
				if (Main.netMode == 0)
				{
					Main.NewText("The Spirit of Purity is losing control!");
				}
				else if (Main.netMode == 2)
				{
					NetMessage.BroadcastChatMessage(NetworkText.FromKey("Mods.Bluemagic.TerraSpiritExpert"), Color.White);
				}
				npc.life = 1;
				if (Main.netMode == 0)
				{
					Main.player[Main.myPlayer].GetModPlayer<BluemagicPlayer>().terraLives += 3;
					Main.NewText("You have been granted 3 extra lives!");
				}
				if (Main.netMode == 2)
				{
					NetMessage.BroadcastChatMessage(NetworkText.FromKey("Mods.Bluemagic.ExtraLives"), Color.White);
					ModPacket packet = mod.GetPacket();
					packet.Write((byte)MessageType.ExtraLives);
					packet.Send();
				}
			}
			if (Progress >= 120)
			{
				Stage++;
				Progress = -1;
			}
		}

		private void Recover()
		{
			npc.life = 1 + 4321 * Progress;
			if (npc.life >= npc.lifeMax)
			{
				npc.life = npc.lifeMax;
				Stage++;
				Progress = -1;
			}
		}

		private void Attack()
		{
			Progress %= 300;
			if (Progress == 0 || (Main.expertMode && Progress == 150))
			{
				/*Player target = null;
				float distance = 0f;
				for (int k = 0; k < 255; k++)
				{
					if (Main.player[k].active && !Main.player[k].dead)
					{
						float temp = Vector2.Distance(Main.player[k].Center, npc.Center);
						if (target == null || temp < distance)
						{
							target = Main.player[k];
							distance = temp;
						}
					}
				}
				if (target != null)
				{
					NPC.NewNPC((int)npc.position.X + npc.width / 2, (int)npc.position.Y + npc.height - 20, mod.NPCType("NegativeBlob"), 0, npc.whoAmI, (target.Center - npc.Center).ToRotation());
				}*/
				NPC.NewNPC((int)npc.position.X + npc.width / 2, (int)npc.position.Y + npc.height - 20, mod.NPCType("NegativeBlob"), 0, npc.whoAmI, Main.rand.NextFloat() * MathHelper.TwoPi);
			}
			npc.life += Main.expertMode ? 5 : 1;
			if (npc.life > npc.lifeMax)
			{
				npc.life = npc.lifeMax;
			}
		}

		private void End()
		{
			npc.dontTakeDamage = true;
			bullets.Clear();
			if (Progress == 1)
			{
				Main.PlaySound(29, -1, -1, 92);
			}
			if (Progress >= 420)
			{
				BluemagicWorld.downedTerraSpirit = true;
				if (Main.netMode == 2)
				{
					NetMessage.SendData(MessageID.WorldData);
				}
				Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("PuriumCoin"), Main.expertMode ? Main.rand.Next(20, 25) : Main.rand.Next(10, 13));
				Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("RainbowStar"));
				if (Main.expertMode)
				{
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("BlushieCharm"));
				}
				npc.active = false;
			}
		}

		public override bool CheckDead()
		{
			Stage = 3;
			Progress = 0;
			npc.life = npc.lifeMax;
			npc.dontTakeDamage = true;
			return false;
		}

		public override bool? CanBeHitByItem(Player player, Item item)
		{
			return false;
		}

		public override bool? CanBeHitByProjectile(Projectile projectile)
		{
			return false;
		}

		public override void FindFrame(int frameHeight)
		{
			npc.frameCounter += 1;
			npc.frameCounter %= 40;
			int frame = (int)npc.frameCounter / 10;
			npc.frame.Y = frame * frameHeight;
		}

		public override Color? GetAlpha(Color lightColor)
		{
			return Color.White;
		}

		public override void PostDraw(SpriteBatch spriteBatch, Color drawColor)
		{
			const int blockSize = 16;
			int centerX = (int)npc.Center.X;
			int centerY = (int)npc.Center.Y;
			Texture2D outlineTexture = mod.GetTexture("TerraSpirit/TerraBlockOutline");
			Texture2D blockTexture = mod.GetTexture("TerraSpirit/TerraBlock");
			Texture2D outlineSpike = mod.GetTexture("TerraSpirit/TerraSpikeOutline");
			Texture2D spike = mod.GetTexture("TerraSpirit/TerraSpike");
			float spikeAlpha = 1f;
			if (Stage == 0)
			{
				spikeAlpha = (Progress % 80f) / 40f;
				if (spikeAlpha > 1f)
				{
					spikeAlpha = 2f - spikeAlpha;
				}
			}
			Vector2 half = new Vector2(8, 8);
			for (int x = centerX - (arenaWidth + blockSize) / 2; x <= centerX + (arenaWidth + blockSize) / 2; x += blockSize)
			{
				int y = centerY - (arenaHeight + blockSize) / 2;
				Vector2 drawPos = new Vector2(x - blockSize / 2, y - blockSize / 2) - Main.screenPosition;
				spriteBatch.Draw(outlineTexture, drawPos, Color.White);
				spriteBatch.Draw(blockTexture, drawPos, Color.White * 0.75f);
				spriteBatch.Draw(outlineSpike, drawPos + new Vector2(0f, blockSize) + half, null, Color.White * spikeAlpha, MathHelper.Pi, half, 1f, SpriteEffects.None, 0f);
				spriteBatch.Draw(spike, drawPos + new Vector2(0f, blockSize) + half, null, Color.White * spikeAlpha * 0.75f, MathHelper.Pi, half, 1f, SpriteEffects.None, 0f);
				drawPos.Y += arenaHeight + blockSize;
				spriteBatch.Draw(outlineTexture, drawPos, Color.White);
				spriteBatch.Draw(blockTexture, drawPos, Color.White * 0.75f);
				spriteBatch.Draw(outlineSpike, drawPos + new Vector2(0f, -blockSize), Color.White * spikeAlpha);
				spriteBatch.Draw(spike, drawPos + new Vector2(0f, -blockSize), Color.White * spikeAlpha * 0.75f);
			}
			for (int y = centerY - (arenaHeight + blockSize) / 2; y <= centerY + (arenaHeight + blockSize) / 2; y += blockSize)
			{
				int x = centerX - (arenaWidth + blockSize) / 2;
				Vector2 drawPos = new Vector2(x - blockSize / 2, y - blockSize / 2) - Main.screenPosition;
				spriteBatch.Draw(outlineTexture, drawPos, Color.White);
				spriteBatch.Draw(blockTexture, drawPos, Color.White * 0.75f);
				spriteBatch.Draw(outlineSpike, drawPos + new Vector2(blockSize, 0f) + half, null, Color.White * spikeAlpha, MathHelper.PiOver2, half, 1f, SpriteEffects.None, 0f);
				spriteBatch.Draw(spike, drawPos + new Vector2(blockSize, 0f) + half, null, Color.White * spikeAlpha * 0.75f, MathHelper.PiOver2, half, 1f, SpriteEffects.None, 0f);
				drawPos.X += arenaWidth + blockSize;
				spriteBatch.Draw(outlineTexture, drawPos, Color.White);
				spriteBatch.Draw(blockTexture, drawPos, Color.White * 0.75f);
				spriteBatch.Draw(outlineSpike, drawPos + new Vector2(-blockSize, 0f) + half, null, Color.White * spikeAlpha, -MathHelper.PiOver2, half, 1f, SpriteEffects.None, 0f);
				spriteBatch.Draw(spike, drawPos + new Vector2(-blockSize, 0f) + half, null, Color.White * spikeAlpha * 0.75f, -MathHelper.PiOver2, half, 1f, SpriteEffects.None, 0f);
			}

			foreach (Bullet bullet in bullets)
			{
				bullet.Draw(spriteBatch);
			}
		}
	}
}
