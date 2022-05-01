﻿using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using WeaponEnchantments.Items;
using Terraria.ModLoader.IO;
using static WeaponEnchantments.Items.Enchantments;
using Terraria.ID;
using System;

namespace WeaponEnchantments.Common.Globals
{
    public class EnchantedItem : GlobalItem
    {
        public Item[] enchantments = new Item[EnchantingTable.maxEnchantments];//Track enchantment items on a weapon/armor/accessory item
        
        public int experience;//current experience of a weapon/armor/accessory item
        public float baseDamage;
        public float baseFireRate;
        public float baseSpeed;
        public float baseAnimation;
        public float origionalScale;
        public int origionalValue;
        public int levelBeforeBooster;
        public int level;
        public bool powerBoosterInstalled;//Tracks if Power Booster is installed on item +10 levels to spend on enchantments (Does not affect experience)
        public bool inEnchantingTable;
        public const int maxLevel = 40;
        public EnchantedItem()
        {
            for (int i = 0; i < EnchantingTable.maxEnchantments; i++) 
            {
                enchantments[i] = new Item();
            }
        }//Constructor
        public override bool InstancePerEntity => true;

        public override GlobalItem Clone(Item item, Item itemClone)
        {
            EnchantedItem clone = (EnchantedItem)base.Clone(item, itemClone);
            clone.enchantments = (Item[])enchantments.Clone();
            for (int i = 0; i < enchantments.Length; i++)
            {
               clone.enchantments[i] = enchantments[i].Clone();
            }//fixes enchantments being applied to all of an item instead of just the instance
            return clone;
        }
        public void UpdateLevel()//Optimize this into get next level?
        {
            int previous1 = 1;
            int previous2 = 0;
            int current;
            int total = 0;
            int l;
            for(l = 0; l <= maxLevel; l++)
            {
                current = previous1 + previous2;
                total += current;
                previous2 = previous1;
                previous1 = current;
                if(experience < total)
                {
                    level = l;
                    break;
                }
            }
            if(l >= maxLevel)
            {
                levelBeforeBooster = maxLevel;
                level = powerBoosterInstalled ? maxLevel + 10 : maxLevel;
            }
            else
            {
                levelBeforeBooster = l;
                level = powerBoosterInstalled ? l + 10 : l;
            }
        }
        public int GetLevelsAvailable()
        {
            UpdateLevel();
            int total = 0;
            for (int i = 0; i < enchantments.Length; i++)
            {
                if (!enchantments[i].IsAir) 
                { 
                    total += ((Enchantments)enchantments[i].ModItem).GetLevelCost();
                }
            }
            return level - total;
        }
        public override void LoadData(Item item, TagCompound tag)
        {
            for (int i = 0; i < EnchantingTable.maxEnchantments; i++)
            {
                if (tag.Get<Item>("enchantments" + i.ToString()) != null)
                {
                    if (!tag.Get<Item>("enchantments" + i.ToString()).IsAir)
                    {
                        enchantments[i] = tag.Get<Item>("enchantments" + i.ToString()).Clone();
                    }
                    else
                    {
                        enchantments[i] = new Item();
                    }
                }
            }//Load enchantment item tags
            experience = tag.Get<int>("experience");//Load experience tag
            powerBoosterInstalled = tag.Get<bool>("powerBooster");//Load status of powerBoosterInstalled
        }
        public override void SaveData(Item item, TagCompound tag)
        {
            if (enchantments != null)
            {
                for (int i = 0; i < EnchantingTable.maxEnchantments; i++)
                {
                    if (enchantments[i] != null)
                    {
                        if (!enchantments[i].IsAir)
                        {
                            tag["enchantments" + i.ToString()] = enchantments[i].Clone();
                        }
                    }
                }//Save enchantment item tags
            }
            tag["experience"] = experience;//Save experience tag
            tag["powerBooster"] = powerBoosterInstalled;//save status of powerBoosterInstalled
        }
        public override void ModifyWeaponDamage(Item item, Player player, ref StatModifier damage)
        {
            if (baseDamage == 0f)
            {
                baseDamage = damage.Base;
                baseFireRate = ContentSamples.ItemsByType[item.type].shootSpeed;
                baseSpeed = ContentSamples.ItemsByType[item.type].useTime;
                baseAnimation = ContentSamples.ItemsByType[item.type].useAnimation;
            }
            float modifier = 0f;
            float speedModifier = 0f;
            for (int i = 0; i < EnchantingTable.maxEnchantments; i++)
            {
                if (!enchantments[i].IsAir && ((Enchantments)enchantments[i].ModItem).enchantmentType == EnchantmentTypeIDs.Damage)
                {
                    modifier += ((Enchantments)enchantments[i].ModItem).enchantmentStrength * damage.Multiplicative;
                }
                else if(!enchantments[i].IsAir && ((Enchantments)enchantments[i].ModItem).enchantmentType == EnchantmentTypeIDs.Speed)
                {
                    speedModifier += ((Enchantments)enchantments[i].ModItem).enchantmentStrength;
                }
            }
            if (modifier > 0 && baseDamage * (1 + modifier) * damage.Multiplicative - baseDamage < 1)
            {
                damage += 1/baseDamage/damage.Multiplicative;
            }
            else
            {
                damage += modifier * damage.Multiplicative;
            }
            item.useTime = (int)(baseSpeed * 1 / (1 + speedModifier));
            item.useAnimation = (int)(baseSpeed * 1 / (1 + speedModifier));
            item.shootSpeed = baseFireRate * (1 + speedModifier);
        }
        public override void ModifyWeaponCrit(Item item, Player player, ref float crit)
        {
            for (int i = 0; i < EnchantingTable.maxEnchantments; i++)
            {
                if (!enchantments[i].IsAir && ((Enchantments)enchantments[i].ModItem).enchantmentType == EnchantmentTypeIDs.Critical)
                {
                    crit += (int)(((Enchantments)enchantments[i].ModItem).enchantmentStrength * 100);
                }
            }
        }
        public override void ModifyWeaponKnockback(Item item, Player player, ref StatModifier knockback)
        {
            if (origionalScale == 0)
            {
                origionalScale = item.scale;
                origionalValue = item.value;
            }
            float scale = 0f;
            int value = 0;
            for (int i = 0; i < EnchantingTable.maxEnchantments; i++)
            {
                if (!enchantments[i].IsAir && ((Enchantments)enchantments[i].ModItem).enchantmentType == EnchantmentTypeIDs.Size)
                {
                    knockback += (int)(((Enchantments)enchantments[i].ModItem).enchantmentStrength * 100);
                    scale += ((Enchantments)enchantments[i].ModItem).enchantmentStrength / 2;//Only do 50% of enchantmentStrength to size
                }
                value += enchantments[i].value;
            }
            item.scale = origionalScale * (1 + scale);//Update item size
            item.value = origionalValue + value;//Update items value based on enchantments installed
        }
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            bool enchantmentsToolTipAdded = false;
            bool enchantemntInstalled = false;
            UpdateLevel();
            for (int i = 0; i < EnchantingTable.maxEnchantments; i++)
            {
                if (!enchantments[i].IsAir)
                {
                    enchantemntInstalled = true;
                    break;
                }
            }
            if (experience > 0 || powerBoosterInstalled || inEnchantingTable || enchantemntInstalled)
            {
                if (powerBoosterInstalled)
                {
                    tooltips.Add(new TooltipLine(Mod, "level", "Level: " + levelBeforeBooster.ToString() + " Points available: " + GetLevelsAvailable().ToString() + " (Booster Installed)") { OverrideColor = Color.LightGreen });
                }
                else
                {
                    tooltips.Add(new TooltipLine(Mod, "level", "Level: " + levelBeforeBooster.ToString() + " Points available: " + GetLevelsAvailable().ToString()) { OverrideColor = Color.LightGreen });
                }
            }
            for (int i = 0; i < EnchantingTable.maxEnchantments; i++)
            {
                if (!enchantments[i].IsAir)
                {
                    if (!enchantmentsToolTipAdded)
                    {
                        tooltips.Add(new TooltipLine(Mod, "enchantmentsToolTip", "Enchantments:") { OverrideColor = Color.Violet});
                        enchantmentsToolTipAdded = true;
                    }//Enchantmenst: tooltip
                    if (((Enchantments)enchantments[i].ModItem).enchantmentType == EnchantmentTypeIDs.Size)
                    {
                        tooltips.Add(new TooltipLine(Mod, "enchantment" + i.ToString(), "+" + ((int)(((Enchantments)enchantments[i].ModItem).enchantmentStrength * 50)).ToString() + "% " + ((Enchantments)enchantments[i].ModItem).enchantmentTypeName)
                        {
                            OverrideColor = Enchantments.rarityColors[((Enchantments)enchantments[i].ModItem).enchantmentSize]
                        });
                        tooltips.Add(new TooltipLine(Mod, "enchantment" + i.ToString() + "-2", "+" + ((int)(((Enchantments)enchantments[i].ModItem).enchantmentStrength * 100)).ToString() + "% Knockback")
                        {
                            OverrideColor = Enchantments.rarityColors[((Enchantments)enchantments[i].ModItem).enchantmentSize]
                        });
                    }//Special tooltip if size enchantment installed
                    else
                    {
                        tooltips.Add(new TooltipLine(Mod, "enchantment" + i.ToString(), "+" + ((int)(((Enchantments)enchantments[i].ModItem).enchantmentStrength * 100)).ToString() + "% " + ((Enchantments)enchantments[i].ModItem).enchantmentTypeName)
                        {
                            OverrideColor = Enchantments.rarityColors[((Enchantments)enchantments[i].ModItem).enchantmentSize]
                        });
                    }//Default tooltip
                }
            }//Edit Tooltips
        }
        public override void OnHitNPC(Item item, Player player, NPC target, int damage, float knockBack, bool crit)
        {
            if (target.life <= 0) 
            {
                KillNPC(item, target);
            }
        }
        public void KillNPC(Item item, NPC target)
        {
            WEPlayer wePlayer = Main.LocalPlayer.GetModPlayer<WEPlayer>();
            if (target.type != NPCID.TargetDummy && !target.SpawnedFromStatue && !target.friendly && !target.townNPC)
            {
                int xpInt;
                float multiplier = (1f + ((float)((target.noGravity ? 2f : 0f) + (target.noTileCollide ? 2f : 0f)) + 2f * (1f - target.knockBackResist)) / 10f + (float)target.defDamage / 40f) / (target.boss ? 2f : 1f);
                float effDamage = (float)item.damage * (1f + (float)item.crit / 100f);
                float xp = (float)target.lifeMax * multiplier * effDamage/(effDamage - (float)target.defDefense / 2);
                xpInt = (int)xp;
                if(levelBeforeBooster < maxLevel)
                {
                    Main.NewText(wePlayer.Player.name + " recieved " + xpInt.ToString() + " xp from killing " + target.FullName + ".");
                    GainXP(item, xpInt);
                }
            }
        }
        public void GainXP(Item item, int xpInt)
        {
            WEPlayer wePlayer = Main.LocalPlayer.GetModPlayer<WEPlayer>();
            int currentLevel = levelBeforeBooster;
            experience += xpInt;
            if (levelBeforeBooster < maxLevel)
            {
                UpdateLevel();
                if (levelBeforeBooster > currentLevel)
                {
                    Main.NewText(wePlayer.Player.name + "'s " + item.Name + " reached level " + levelBeforeBooster.ToString() + ".");
                }
            }
        }
        public override void OnCreate(Item item, ItemCreationContext context)
        {
            WEPlayer wePlayer = Main.LocalPlayer.GetModPlayer<WEPlayer>();
            for (int j = 0; j < Main.recipe.Length; j++)
            {
                if(Main.recipe[j].createItem.type == item.type)
                {
                    foreach(Item requiredItem in Main.recipe[j].requiredItem)
                    {
                        if(requiredItem.damage > 0)
                        {
                            for(int i = 0; i < 91; i++)
                            {
                                if(wePlayer.inventoryItemRecord[i].type == requiredItem.type)
                                {
                                    experience += wePlayer.inventoryItemRecord[i].GetGlobalItem<EnchantedItem>().experience;
                                    if (wePlayer.inventoryItemRecord[i].GetGlobalItem<EnchantedItem>().powerBoosterInstalled)
                                    {
                                        powerBoosterInstalled = true;
                                    }
                                    WEModSystem.playerInventoryUpdated = false;
                                    WEModSystem.enchantingTableInventoryUpdated = false;
                                    WEModSystem.previousChest = -1;
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
