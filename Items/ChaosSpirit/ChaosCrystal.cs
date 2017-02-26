using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Bluemagic.Items.ChaosSpirit
{
	public class ChaosCrystal : ModItem
	{
		public override void SetDefaults()
		{
			item.name = "Chaos Crystal";
			item.width = 28;
			item.height = 28;
			item.maxStack = 99;
			item.rare = 11;
			item.value = Item.sellPrice(1, 50, 0, 0);
			item.consumable = true;
			item.useStyle = 4;
			item.useTime = 30;
			item.useAnimation = 30;
			item.UseSound = SoundID.Item29;
		}

		public override DrawAnimation GetAnimation()
		{
			return new DrawAnimationVertical(5, 4);
		}

		public override bool CanUseItem(Player player)
		{
			return true;
		}

		public override bool UseItem(Player player)
		{
			player.GetModPlayer<BluemagicPlayer>(mod).chaosPoints++;
			return true;
		}

		public override Color? GetAlpha(Color lightColor)
		{
			return Color.White;
		}
	}
}