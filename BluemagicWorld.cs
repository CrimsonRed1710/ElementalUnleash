﻿using System.IO;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Bluemagic
{
	public class BluemagicWorld : ModWorld
	{
		private const int saveVersion = 0;
		public static bool eclipsePassed = false;
		public static bool pumpkinMoonPassed = false;
		public static bool snowMoonPassed = false;
		public static bool downedPhantom = false;
		public static bool downedAbomination = false;
		public static int downedAbomination2 = 0;
		public static bool downedPuritySpirit = false;
		public static bool downedChaosSpirit = false;

		public override void Initialize()
		{
			eclipsePassed = false;
			pumpkinMoonPassed = false;
			snowMoonPassed = false;
			downedPhantom = false;
			downedAbomination = false;
			downedAbomination2 = 0;
			downedPuritySpirit = false;
			downedChaosSpirit = false;
		}

		public override TagCompound Save()
		{
			TagCompound tag = new TagCompound();
			tag["eclipsePassed"] = eclipsePassed;
			tag["pumpkinMoonPassed"] = pumpkinMoonPassed;
			tag["snowMoonPassed"] = snowMoonPassed;
			tag["downedPhantom"] = downedPhantom;
			tag["downedAbomination"] = downedAbomination;
			tag["downedAbomination2"] = downedAbomination2;
			tag["downedPuritySpirit"] = downedPuritySpirit;
			tag["downedChaosSpirit"] = downedChaosSpirit;
			return tag;
		}

		public override void Load(TagCompound tag)
		{
			eclipsePassed = tag.GetBool("eclipsePassed");
			pumpkinMoonPassed = tag.GetBool("pumpkinMoonPassed");
			snowMoonPassed = tag.GetBool("snowMoonPassed");
			downedPhantom = tag.GetBool("downedPhantom");
			downedAbomination = tag.GetBool("downedAbomination");
			downedAbomination2 = tag.GetInt("downedAbomination2");
			downedPuritySpirit = tag.GetBool("downedPuritySpirit");
			downedChaosSpirit = tag.GetBool("downedChaosSpirit");
		}

		public override void NetSend(BinaryWriter writer)
		{
			byte flags = 0;
			if (eclipsePassed)
			{
				flags |= 1;
			}
			if (pumpkinMoonPassed)
			{
				flags |= 2;
			}
			if (snowMoonPassed)
			{
				flags |= 4;
			}
			if (downedPhantom)
			{
				flags |= 8;
			}
			if (downedAbomination)
			{
				flags |= 16;
			}
			if (downedPuritySpirit)
			{
				flags |= 32;
			}
			if (downedChaosSpirit)
			{
				flags |= 64;
			}
			writer.Write(flags);
			writer.Write(downedAbomination2);
		}

		public override void NetReceive(BinaryReader reader)
		{
			byte flags = reader.ReadByte();
			eclipsePassed = ((flags & 1) == 1);
			pumpkinMoonPassed = ((flags & 2) == 2);
			snowMoonPassed = ((flags & 4) == 4);
			downedPhantom = ((flags & 8) == 8);
			downedAbomination = ((flags & 16) == 16);
			downedPuritySpirit = ((flags & 32) == 32);
			downedChaosSpirit = ((flags & 64) == 64);
			downedAbomination2 = reader.ReadInt32();
		}

		public override void LoadLegacy(BinaryReader reader)
		{
			reader.ReadInt32();
			byte flags = reader.ReadByte();
			downedAbomination = ((flags & 1) == 1);
			downedPuritySpirit = ((flags & 2) == 2);
			downedChaosSpirit = ((flags & 4) == 4);
		}

		public override void ResetNearbyTileEffects()
		{
			BluemagicPlayer modPlayer = Main.player[Main.myPlayer].GetModPlayer<BluemagicPlayer>(mod);
			modPlayer.voidMonolith = false;
		}

		public override void PostUpdate()
		{
			Bluemagic.UpdatePureColor();
		}

		public static void GenPurium()
		{
			if (Main.netMode == 1 || WorldGen.noTileActions || WorldGen.gen || !NPC.downedMoonlord)
			{
				return;
			}
			downedAbomination2 += 1;
			for (double k = 0; k < (Main.maxTilesX - 200) * (Main.maxTilesY - 150 - (int)Main.rockLayer) / 10000.0 / (double)downedAbomination2; k += 1.0)
			{
				WorldGen.OreRunner(WorldGen.genRand.Next(100, Main.maxTilesX - 100), WorldGen.genRand.Next((int)Main.rockLayer, Main.maxTilesY - 150), (double)WorldGen.genRand.Next(4, 8), WorldGen.genRand.Next(4, 8), (ushort)Bluemagic.Instance.TileType("PuriumOre"));
			}
			if (Main.netMode == 0)
			{
				Main.NewText("The elements have been unleashed!", 100, 220, 100);
			}
			else if (Main.netMode == 2)
			{
				NetMessage.SendData(MessageID.ChatText, -1, -1, "The elements have been unleashed!", 255, 100, 220, 100);
				NetMessage.SendData(MessageID.WorldInfo);
			}
		}
	}
}
