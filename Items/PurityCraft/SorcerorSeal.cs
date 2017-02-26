using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Bluemagic.Items.PurityCraft
{
	public class SorcerorSeal : ModItem
	{
		public override void SetDefaults()
		{
			item.name = "Sorceror Seal";
			item.toolTip = "25% increased magic damage";
			item.width = 24;
			item.height = 24;
			item.accessory = true;
			item.rare = 11;
			item.value = Item.sellPrice(0, 25, 0, 0);
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
			player.magicDamage += 0.25f;
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.SorcererEmblem);
			recipe.AddIngredient(null, "InfinityCrystal");
			recipe.AddTile(null, "ElementalPurge");
			recipe.SetResult(this);
			recipe.AddRecipe();

			recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.AvengerEmblem);
			recipe.AddIngredient(null, "InfinityCrystal");
			recipe.AddTile(null, "ElementalPurge");
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}
}