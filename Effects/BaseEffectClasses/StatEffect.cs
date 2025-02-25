﻿using Terraria.ModLoader;
using WeaponEnchantments.Common;
using WeaponEnchantments.Common.Utility;
using static WeaponEnchantments.WEPlayer;

namespace WeaponEnchantments.Effects {
    public abstract class StatEffect : EnchantmentEffect, IEnchantmentStat {
        protected StatEffect(EStatModifier sm, bool playerStatOnWeapon = false) {
            EStatModifier = sm;
        }

        protected StatEffect(float additive = 0f, float multiplicative = 1f, float flat = 0f, float @base = 0f) {
            EStatModifier = new EStatModifier(statName, additive, multiplicative, flat, @base);
		}
        public StatEffect(DifficultyStrength additive, DifficultyStrength multiplicative, DifficultyStrength flat, DifficultyStrength @base) {
            EStatModifier = new EStatModifier(statName, additive, multiplicative, flat, @base);
        }
        public EStatModifier EStatModifier { set; get; }
        public override float EffectStrength => EStatModifier.Strength;
		public override float CombinedMultiplier {
            get => EStatModifier.EfficiencyMultiplier; 
            protected set => EStatModifier.EfficiencyMultiplier = value;
        }

        public abstract EnchantmentStat statName { get; }
        public override string Tooltip => $"{EStatModifier.SmartTooltip} {DisplayName}";
    }
}
