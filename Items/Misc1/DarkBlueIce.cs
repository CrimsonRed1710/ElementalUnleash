using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Bluemagic.Items.Misc1
{
	public class DarkBlueIce : ModItem
	{
		public override void SetDefaults()
		{
			item.name = "Dark Blue Ice Block";
			item.width = 12;
			item.height = 12;
			item.maxStack = 999;
			item.useStyle = 1;
			item.useTurn = true;
			item.useAnimation = 15;
			item.useTime = 10;
			item.autoReuse = true;
			item.consumable = true;
			item.createTile = mod.TileType("DarkBlueIce");
		}
	}
}