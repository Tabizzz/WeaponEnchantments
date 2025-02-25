﻿using MagicStorage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using WeaponEnchantments.Common;
using WeaponEnchantments.Common.Utility;
using WeaponEnchantments.Items;

namespace WeaponEnchantments.ModIntegration
{
    [ExtendsFromMod(MagicStorageIntegration.magicStorageName)]
    public class WEEnvironmentModule : EnvironmentModule
    {
        public override string Name => "WeaponEnchantments_EnvironmentModule";
		public override IEnumerable<Item> GetAdditionalItems(EnvironmentSandbox sandbox) {
            return Main.LocalPlayer.GetWEPlayer().enchantingTable.essenceItem;
        }
		public override void ModifyCraftingZones(EnvironmentSandbox sandbox, ref CraftingInformation information) {
            int highestTableTierUsed = Main.LocalPlayer.GetWEPlayer().highestTableTierUsed;
            int baseTableTier = ModContent.TileType<Tiles.WoodEnchantingTable>();
            int tableTier;
            if (highestTableTierUsed == 0) {
                tableTier = baseTableTier;
	        }
            else {
                tableTier = baseTableTier - 5 + highestTableTierUsed;
            }

            if (tableTier > -1)
                information.adjTiles[tableTier] = true;
		}
		public override void OnConsumeItemForRecipe(EnvironmentSandbox sandbox, Item item, int stack) {
			if (item.ModItem != null && item.ModItem is EnchantmentEssence) {
                int type0 = ModContent.ItemType<EnchantmentEssenceBasic>();
                Main.LocalPlayer.GetWEPlayer().enchantingTable.essenceItem[EnchantmentEssence.IDs.IndexOf(item.type)].stack -= stack;
            }//Will be done by magic storage next update
            
            WEMod.consumedItems.Add(item.Clone());
        }
	}
}
