﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using WeaponEnchantments.Common;
using WeaponEnchantments.Common.Utility;

namespace WeaponEnchantments.Items
{
	public abstract class EnchantmentEssence : ModItem
	{
		public int essenceRarity = -1;
		public static string[] rarity = new string[5] { "Basic", "Common", "Rare", "SuperRare", "UltraRare" };
		public static int[] IDs = new int[rarity.Length];
		public const int maxStack = 9999;
		public static float[] values = new float[rarity.Length];
		public static float[] xpPerEssence = new float[rarity.Length];
		public static float valuePerXP;
		public override string Texture => (GetType().Namespace + ".Sprites." + Name + (WEMod.clientConfig.ColorBlindMode ? "CB" : "")).Replace('.', '/');

		public virtual string Artist { private set; get; } = "Kiroto";
		public virtual string Designer { private set; get; } = "andro951";
		private int entitySize = 20;

		public abstract Color glowColor { get; }
		public abstract int animationFrames { get; }

		public override void SetStaticDefaults()
        {
			Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(5, animationFrames));
			ItemID.Sets.AnimatesAsSoul[Item.type] = true;
			ItemID.Sets.ItemIconPulse[Item.type] = true;
			ItemID.Sets.ItemNoGravity[Item.type] = true;

			for (int i = 0; i < rarity.Length; i++)
            {
				values[i] = (float)(25 * Math.Pow(8, i));
				xpPerEssence[i] = (float)(400 * Math.Pow(4, i));
			}
			valuePerXP = (values[rarity.Length - 1] / xpPerEssence[rarity.Length - 1]);
			GetDefaults();
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 25;
			Tooltip.SetDefault(Enchantment.displayRarity[essenceRarity].AddSpaces() + " material for crafting and upgrading enchantments.\nCan be converted to " + xpPerEssence[essenceRarity] + " experience in an enchanting table.");
			if (!WEMod.clientConfig.UseOldRarityNames)
				DisplayName.SetDefault(StringManipulation.AddSpaces(Name.Substring(0, Name.IndexOf(rarity[essenceRarity])) + Enchantment.displayRarity[essenceRarity]));

			if (LogModSystem.printListOfContributors) {
				LogModSystem.UpdateContributorsList(this);
				WEMod.clientConfig.ColorBlindMode = !WEMod.clientConfig.ColorBlindMode;
				LogModSystem.UpdateContributorsList(this);
				WEMod.clientConfig.ColorBlindMode = !WEMod.clientConfig.ColorBlindMode;
			}
		}

        public override void PostUpdate()
		{
			// Turn the alpha of the color into it's brightness (0-1)
			float intensity = glowColor.A / 255f;
			Lighting.AddLight(Item.Center, glowColor.ToVector3() * intensity * Main.essScale);	
		}

		public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
		{
			// Add glow mask
			Texture2D texture = TextureAssets.Item[Item.type].Value;
			int currentFrame = Main.itemFrameCounter[whoAmI];
			Rectangle frame = Main.itemAnimations[Item.type] is not null ? Main.itemAnimations[Item.type].GetFrame(texture, currentFrame) : texture.Frame();
			spriteBatch.Draw
			(
				texture,
				new Vector2
				(
					Item.position.X - Main.screenPosition.X + Item.width * 0.5f,
					Item.position.Y - Main.screenPosition.Y + Item.height * 0.5f
				),
				frame,
				Color.White,
				rotation,
				new Vector2
				(
					texture.Width,
					texture.Height / animationFrames
				) * 0.5f,
				scale,
				SpriteEffects.None,
				0f
			);
		}

		private void GetDefaults()
        {
			for (int i = 0; i < rarity.Length; i++)
			{
				if (rarity[i] == Name.Substring(Name.IndexOf("Essence") + 7))
				{
					essenceRarity = i;
					break;
				}
			}
		}
		public override void SetDefaults()
		{
			GetDefaults();
			Item.value = (int)values[essenceRarity];
			Item.maxStack = maxStack;
			Item.width = entitySize;
			Item.height = entitySize;
		}

		public override void AddRecipes()
		{
			for (int i = 0; i < rarity.Length; i++)
			{
				if (essenceRarity > -1)
				{
					//Dont sell basic/common/rare with NPC!!!
					Recipe recipe = CreateRecipe();
					if (essenceRarity > 0)
					{
						recipe.AddIngredient(Mod, "EnchantmentEssence" + rarity[essenceRarity - 1], 8 - i);
						recipe.AddTile(Mod, WoodEnchantingTable.enchantingTableNames[i] + "EnchantingTable"); //Put this inside if(essenceRarity >0) when not testing
						recipe.Register(); //Put this inside if(essenceRarity >0) when not testing
					}
					

					if (essenceRarity < rarity.Length - 1)
					{
						recipe = CreateRecipe();
						recipe.AddIngredient(Mod, "EnchantmentEssence" + rarity[essenceRarity + 1], 1);
						recipe.createItem.stack = 2 + i / 2;
						recipe.AddTile(Mod, WoodEnchantingTable.enchantingTableNames[i] + "EnchantingTable");
						recipe.Register();
					}
					IDs[essenceRarity] = this.Type;
				}
			}
		}
    }
	public class EnchantmentEssenceBasic: EnchantmentEssence {
		public override int animationFrames => 8;
		public override Color glowColor => Color.FromNonPremultiplied(0x2E, 0x7F, 0x4C, 0x80);  // #2e7f4c
	}
	public class EnchantmentEssenceCommon : EnchantmentEssence {
		public override int animationFrames => 8;
		public override Color glowColor => Color.FromNonPremultiplied(0x1F, 0xD4, 0xDA, 0x84);  // #1fd4da
	}
	public class EnchantmentEssenceRare : EnchantmentEssence {
		public override int animationFrames => 6;
		public override Color glowColor => Color.FromNonPremultiplied(0x67, 0x26, 0xA1, 0x87);  // #6726a1
	}
	public class EnchantmentEssenceSuperRare : EnchantmentEssence {
		public override int animationFrames => 10;
		public override Color glowColor => Color.FromNonPremultiplied(0xF9, 0x00, 0x23, 0x89);  // #f90023
	}
	public class EnchantmentEssenceUltraRare : EnchantmentEssence {
		public override int animationFrames => 16;
		public override Color glowColor => Color.FromNonPremultiplied(0xD7, 0x54, 0x09, 0x8a);  // #d75409
	}
}
