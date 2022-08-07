﻿using System.Collections.Generic;
using Terraria.ID;
using WeaponEnchantments.Effects;

namespace WeaponEnchantments.Items.Enchantments.Utility
{
	public abstract class HunterEnchantment : Enchantment
	{
		public override EnchantmentEffect[] Effects => new EnchantmentEffect[] {
			new BuffEffect(BuffID.Hunter)
		};
		public override int EnchantmentValueTierReduction => -2;
		public override int LowestCraftableTier => 5;
		public override Dictionary<EItemType, float> AllowedList => new Dictionary<EItemType, float>() {
			{ EItemType.Weapon, 1f },
			{ EItemType.Armor, 1f },
			{ EItemType.Accessory, 1f }
		};

		public override string Artist => "Zorutan";
		public override string Designer => "andro951";
	}
	public class HunterEnchantmentUltraRare : HunterEnchantment { }
}
