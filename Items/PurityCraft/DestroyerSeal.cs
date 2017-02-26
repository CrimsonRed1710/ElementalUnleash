using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Bluemagic.Items.PurityCraft
{
	public class DestroyerSeal : ModItem
	{
		public override void SetDefaults()
		{
			item.name = "Avenger Seal";
			item.toolTip = "20% increased damage";
			item.toolTip2 = "15% critical strike chance";
			item.width = 24;
			item.height = 24;
			item.accessory = true;
			item.rare = 11;
			item.value = Item.sellPrice(0, 30, 0, 0);
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			player.meleeDamage += 0.2f;
			player.rangedDamage += 0.2f;
			player.magicDamage += 0.2f;
			player.minionDamage += 0.2f;
			player.thrownDamage += 0.2f;
			player.meleeCrit += 15;
			player.rangedCrit += 15;
			player.magicCrit += 15;
			player.thrownCrit += 15;
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(null, "AvengerSeal");
			recipe.AddIngredient(ItemID.DestroyerEmblem);
			recipe.AddIngredient(null, "InfinityCrystal");
			recipe.AddTile(null, "ElementalPurge");
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}
}