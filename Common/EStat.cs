﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace WeaponEnchantments.Common
{
    public struct EStat
    {
        public float Initial;
        public float Base;
        public float Additive { get; private set; }
        public float Multiplicative { get; private set; }
        public string StatName { get; private set; }
        public float Flat;
        public EStat(float additive, float multiplicative, string statName, float flat = 0f, float @base = 0f, float intital = 1f)
        {
            Additive = additive;
            Multiplicative = multiplicative;
            Flat = flat;
            Base = @base;
            Initial = intital;
            StatName = statName;
        }
        public override bool Equals(object obj)
        {
            if(obj is not EStat s)
                return false;
            return this == s;
        }
        public override int GetHashCode()
        {
            int hashCode = 1713062080;
            hashCode = hashCode * -1521134295 + Additive.GetHashCode();
            hashCode = hashCode * -1521134295 + Multiplicative.GetHashCode();
            hashCode = hashCode * -1521134295 + Flat.GetHashCode();
            hashCode = hashCode * -1521134295 + Base.GetHashCode();
            return hashCode;
        }
        public static EStat operator + (EStat s, float add) 
            => new EStat(s.Additive + add, s.Multiplicative, s.StatName, s.Flat, s.Base);
        public static EStat operator -(EStat s, float sub) 
            => new EStat(s.Additive - sub, s.Multiplicative, s.StatName, s.Flat, s.Base);
        public static EStat operator *(EStat s, float mul)
            => new EStat(s.Additive, s.Multiplicative * mul, s.StatName, s.Flat, s.Base);

        public static EStat operator /(EStat s, float div)
            => new EStat(s.Additive, s.Multiplicative / div, s.StatName, s.Flat, s.Base);

        public static EStat operator +(float add, EStat s)
            => s + add;

        public static EStat operator *(float mul, EStat s)
            => s * mul;

        public static bool operator ==(EStat s1, EStat s2)
            => s1.Additive == s2.Additive && s1.Multiplicative == s2.Multiplicative && s1.Flat == s2.Flat && s1.Base == s2.Base;

        public static bool operator !=(EStat s1, EStat s2)
            => s1.Additive != s2.Additive || s1.Multiplicative != s2.Multiplicative || s1.Flat != s2.Flat || s1.Base != s2.Base;

        public float ApplyTo(float baseValue) =>
            (baseValue + Base) * Additive * Multiplicative + Flat;

        public EStat CombineWith(EStat s)
            => new EStat(Additive + s.Additive - 1, Multiplicative * s.Multiplicative, s.StatName, Flat + s.Flat, Base + s.Base);

        public EStat Scale(float scale)
            => new EStat(1 + (Additive - 1) * scale, 1 + (Multiplicative - 1) * scale, StatName, Flat * scale, Base * scale);
    }
}
