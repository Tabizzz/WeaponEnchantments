﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using static WeaponEnchantments.WEPlayer;

namespace WeaponEnchantments.Effects {
    public class MaxMP : StatEffect, IVanillaStat {
        public MaxMP(float additive = 0f, float multiplicative = 1f, float flat = 0f, float @base = 0f) : base(additive, multiplicative, flat, @base) { }

        public override EnchantmentStat statType => EnchantmentStat.MaxMP;
        public override string DisplayName { get; } = "Max Mana";
    }
}
