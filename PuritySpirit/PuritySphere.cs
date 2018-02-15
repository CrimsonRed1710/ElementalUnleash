using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Bluemagic.PuritySpirit
{
	public class PuritySphere : ModProjectile
	{
		public const float radius = 240f;
		public const int strikeTime = 20;
		private int timer = -60;
		public int maxTimer;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Purity Eye");
			Main.projFrames[projectile.type] = 4;
			ProjectileID.Sets.TrailingMode[projectile.type] = 2;
			ProjectileID.Sets.TrailCacheLength[projectile.type] = 20;
		}

		public override void SetDefaults()
		{
			projectile.width = 40;
			projectile.height = 40;
			projectile.penetrate = -1;
			projectile.magic = true;
			projectile.tileCollide = false;
			projectile.ignoreWater = true;
			projectile.alpha = 120;
			cooldownSlot = 1;
		}

		public override void SendExtraAI(BinaryWriter writer)
		{
			writer.Write(projectile.localAI[0]);
			writer.Write(projectile.localAI[1]);
			writer.Write(maxTimer);
		}

		public override void ReceiveExtraAI(BinaryReader reader)
		{
			projectile.localAI[0] = reader.ReadSingle();
			projectile.localAI[1] = reader.ReadSingle();
			maxTimer = reader.ReadInt32();
		}

		public override void AI()
		{
			if (projectile.velocity.X != 0f)
			{
				projectile.localAI[0] = projectile.velocity.X == -1f ? 0f : projectile.velocity.X;
				projectile.velocity.X = 0f;
			}
			if (projectile.velocity.Y != 0f)
			{
				projectile.localAI[1] = projectile.velocity.Y;
				projectile.velocity.Y = 0f;
			}
			if (projectile.knockBack != 0f)
			{
				maxTimer = (int)projectile.knockBack;
				projectile.knockBack = 0f;
			}
			if (timer < 0)
			{
				projectile.alpha = -timer * 3;
			}
			else
			{
				projectile.alpha = 0;
				projectile.hostile = true;
			}
			if (projectile.localAI[0] != 255f)
			{
				Player player = Main.player[(int)projectile.localAI[0]];
				if (!player.active || player.dead)
				{
					projectile.localAI[0] = 255f;
				}
			}
			Vector2 center = new Vector2(projectile.ai[0], projectile.ai[1]);
			if (timer < 0 && projectile.localAI[0] != 255f)
			{
				Vector2 newCenter = Main.player[(int)projectile.localAI[0]].Center;
				projectile.position += newCenter - center;
				projectile.ai[0] = newCenter.X;
				projectile.ai[1] = newCenter.Y;
				center = newCenter;
			}
			float rotateSpeed = 2f * (float)Math.PI / 60f / 4f * projectile.localAI[1];
			if (timer < maxTimer)
			{
				projectile.Center = projectile.Center.RotatedBy(rotateSpeed, center);
			}
			else
			{
				Vector2 offset = projectile.Center - center;
				offset.Normalize();
				offset *= radius * ((float)strikeTime + maxTimer - timer) / (float)strikeTime;
				projectile.Center = center + offset;
			}
			if (timer == maxTimer)
			{
				BluemagicPlayer modPlayer = Main.player[Main.myPlayer].GetModPlayer<BluemagicPlayer>(mod);
				if (modPlayer.heroLives > 0)
				{
					Main.PlaySound(2, -1, -1, 12);
				}
				else
				{
					Main.PlaySound(2, (int)projectile.Center.X, (int)projectile.Center.Y, 12);
				}
				projectile.hostile = true;
			}
			if (timer >= maxTimer + strikeTime)
			{
				projectile.Kill();
			}
			timer++;
			projectile.rotation += rotateSpeed * -5f * projectile.localAI[1];
			projectile.spriteDirection = projectile.localAI[1] < 0 ? -1 : 1;
			if (projectile.frame < 4)
			{
				projectile.frameCounter++;
				if (projectile.frameCounter >= 8)
				{
					projectile.frameCounter = 0;
					projectile.frame++;
					projectile.frame %= 4;
				}
			}
		}

		public override Color? GetAlpha(Color lightColor)
		{
			return Color.White * ((255 - projectile.alpha / 2) / 255f);
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			Texture2D texture = Main.projectileTexture[projectile.type];
			for (int k = projectile.oldPos.Length - 1; k >= 0; k--)
			{
				if (k % 4 == 0)
				{
					Color alpha = GetAlpha(lightColor).Value * (1f - (float)k / (float)projectile.oldPos.Length);
					spriteBatch.Draw(texture, projectile.oldPos[k] + projectile.Size / 2f - Main.screenPosition, new Rectangle(0, projectile.frame * texture.Height / 4, texture.Width, texture.Height / 4), alpha, projectile.oldRot[k], new Vector2(20, 20), 1f, SpriteEffects.None, 0f);
				}
			}
			return true;
		}
	}
}