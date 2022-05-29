﻿using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Default;
using WeaponEnchantments.Items;

namespace WeaponEnchantments.Common
{
    public class OldItemManager
    {
        private enum OldItemContext
        {
            firstWordNames,
            searchWordNames,
            wholeNameReplaceWithItem,
            wholeNameReplaceWithCoins
        }
        private static Dictionary<string, int> firstWordNames = new Dictionary<string, int> { { "Critical", (int)EnchantmentTypeID.CriticalStrikeChance }, {"Size", (int)EnchantmentTypeID.Scale }, { "ManaCost", (int)EnchantmentTypeID.Mana } };
        private static Dictionary<string, int> searchWordNames = new Dictionary<string, int> { { "SuperRare", 3 }, { "UltraRare", 4 }, { "Rare", 2 } };
        private static Dictionary<string, int> wholeNameReplaceWithItem = new Dictionary<string, int> { { "ContainmentFragment", ItemID.GoldBar } };
        private static Dictionary<string, int> wholeNameReplaceWithCoins = new Dictionary<string, int>();// { { "ContainmentFragment", 2000 } };
        public static void ReplaceAllOldItems()
        {
            ReplaceOldItems(Main.LocalPlayer.inventory);
            ReplaceOldItems(Main.LocalPlayer.bank.item);
            ReplaceOldItems(Main.LocalPlayer.bank2.item);
            ReplaceOldItems(Main.LocalPlayer.bank3.item);
            ReplaceOldItems(Main.LocalPlayer.bank4.item);
            foreach (Chest chest in Main.chest)
            {
                if (chest != null)
                    ReplaceOldItems(chest.item);
                else
                    break;
            }
        }
        private static void ReplaceOldItems(Item[] inventory)
        {
            for(int i = 0; i < inventory.Length; i++)
            {
                 ReplaceOldItem(ref inventory[i]);
            }
        }
        public static void ReplaceOldItem(ref Item item)
        {
            if (item.type == 5163)
            {
                bool replaced = TryReplaceItem(ref item, firstWordNames, OldItemContext.firstWordNames);
                replaced = !replaced ? TryReplaceItem(ref item, searchWordNames, OldItemContext.searchWordNames) : replaced;//Not tested
                replaced = !replaced ? TryReplaceItem(ref item, wholeNameReplaceWithItem, OldItemContext.wholeNameReplaceWithItem) : replaced;
                replaced = !replaced ? TryReplaceItem(ref item, wholeNameReplaceWithCoins, OldItemContext.wholeNameReplaceWithCoins) : replaced;
            }
        }
        private static bool TryReplaceItem(ref Item item, Dictionary<string, int> dict, OldItemContext context)
        {
            string name = ((UnloadedItem)item.ModItem).ItemName;
            string key = null;
            foreach (string k in dict.Keys)
            {
                switch (context) 
                {
                    case OldItemContext.firstWordNames:
                        if (name.Length >= k.Length)
                        {
                            string keyCheck = name.Substring(0, k.Length);
                            if (keyCheck == k)
                            {
                                key = k;
                            }
                        }
                        break;
                    case OldItemContext.searchWordNames://Not tested
                        int index = name.IndexOf(k);
                        if (index > -1)
                        {
                            key = k;
                            name = name.Substring(0, index - 1) + AllForOneEnchantmentBasic.rarity[dict[key]] + name.Substring(index);
                        }
                        break;
                    default:
                        if(k == name)
                            key = k;
                        break;
                }
                if (key != null)
                    break;
            }
            if (key != null)
            {
                switch (context)
                {
                    case OldItemContext.firstWordNames:
                        foreach (ModItem modItem in ModContent.GetInstance<WEMod>().GetContent<ModItem>())
                        {
                            if (modItem is AllForOneEnchantmentBasic enchantment)
                            {
                                if (enchantment.EnchantmentType == dict[key])
                                {
                                    int typeOffset = AllForOneEnchantmentBasic.GetEnchantmentSize(name);
                                    ReplaceItem(ref item, enchantment.Item.type + typeOffset);
                                    return true;
                                }
                            }
                        }
                        break;
                    case OldItemContext.searchWordNames://Not tested
                        foreach (ModItem modItem in ModContent.GetInstance<WEMod>().GetContent<ModItem>())
                        {
                            if (modItem is AllForOneEnchantmentBasic enchantment)
                            {
                                if(enchantment.Name == name)
                                {
                                    ReplaceItem(ref item, enchantment.Item.type);
                                    return true;
                                }
                            }
                        }
                        break;
                    case OldItemContext.wholeNameReplaceWithItem:
                        ReplaceItem(ref item, dict[key]);
                        return true;
                    case OldItemContext.wholeNameReplaceWithCoins:
                        ReplaceItem(ref item, dict[key], true);
                        return true;
                }
            }//firstWordNames
            return false;
        }
        public static void ReplaceItem(ref Item item, int type, bool replaceWithCoins = false)
        {
            int stack = item.stack;
            item.TurnToAir();
            if (replaceWithCoins)
            {
                UtilityMethods.SpawnCoins(type * stack, true);//type is coins when replaceWithCoins is true
            }
            else
            {
                item = new Item(type, stack);
            }
        }
    }
}
