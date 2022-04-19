﻿using Gw2LogParser.Parser.Data.El.Buffs;
using Gw2LogParser.Parser.Helper;
using System;
using System.Collections.Generic;
using static Gw2LogParser.Parser.Helper.ArcDPSEnums.BuffAttribute;

namespace Gw2LogParser.Parser.Data.Events.MetaData
{
    public class BuffFormula
    {
        private static string GetAttributeString(ArcDPSEnums.BuffAttribute attribute)
        {
            switch (attribute)
            {
                case Power:
                    return "Power";
                case Precision:
                    return "Precision";
                case Toughness:
                    return "Toughness";
                case Vitality:
                    return "Vitality";
                case Ferocity:
                    return "Ferocity";
                case Healing:
                    return "Healing Power";
                case Condition:
                    return "Condition Damage";
                case Concentration:
                    return "Concentration";
                case Expertise:
                    return "Expertise";
                case Armor:
                    return "Armor";
                case Agony:
                    return "Agony";
                case StatInc:
                    return "Stat Increase";
                case FlatInc:
                    return "Flat Increase";
                case PhysInc:
                    return "Outgoing Strike Damage";
                case CondInc:
                    return "Outgoing Condition Damage";
                case SiphonInc:
                    return "Outgoing Life Leech Damage";
                case SiphonRec:
                    return "Incoming Life Leech Damage";
                case CondRec:
                    return "Incoming Condition Damage";
                case CondRec2:
                    return "Incoming Condition Damage (Mult)";
                case PhysRec:
                    return "Incoming Strike Damage";
                case PhysRec2:
                    return "Incoming Strike Damage (Mult)";
                case AttackSpeed:
                    return "Attack Speed";
                case ConditionDurationInc:
                    return "Outgoing Condition Duration";
                case DamageFormulaSquaredLevel:
                case DamageFormula:
                    return "Damage Formula";
                case GlancingBlow:
                    return "Glancing Blow";
                case CriticalChance:
                    return "Critical Chance";
                case StrikeDamageToHP:
                    return "Strike Damage to Health";
                case ConditionDamageToHP:
                    return "Condition Damage to Health";
                case SkillActivationDamageFormula:
                    return "Damage Formula on Skill Activation";
                case MovementActivationDamageFormula:
                    return "Damage Formula based on Movement";
                case EnduranceRegeneration:
                    return "Endurance Regeneration";
                case HealingEffectivenessRec:
                    return "Incoming Healing Effectiveness";
                case HealingEffectivenessConvInc:
                case HealingEffectivenessFlatInc:
                    return "Outgoing Healing Effectiveness";
                case HealingOutputFormula:
                    return "Healing Formula";
                case ExperienceFromKills:
                    return "Experience From Kills";
                case ExperienceFromAll:
                    return "Experience From All";
                case GoldFind:
                    return "Gold Find";
                case MovementSpeed:
                    return "Movement Speed";
                case KarmaBonus:
                    return "Karma Bonus";
                case SkillRechargeSpeedIncrease:
                    return "Skill Recharge Speed Increase";
                case MagicFind:
                    return "Magic Find";
                case WXP:
                    return "WXP";
                case Unknown:
                    return "Unknown";
                default:
                    return "";
            }
        }

        private static string GetVariableStat(ArcDPSEnums.BuffAttribute attribute, int type)
        {
            switch (attribute)
            {
                case DamageFormulaSquaredLevel:
                case DamageFormula:
                case SkillActivationDamageFormula:
                case MovementActivationDamageFormula:
                    return type > 10 ? "Power" : "Condition Damage";
                case HealingOutputFormula:
                    return "Healing Power";
                case Unknown:
                    return "Unknown";
                default:
                    return "";
            }
        }

        private static string GetPercent(ArcDPSEnums.BuffAttribute attribute1, ArcDPSEnums.BuffAttribute attribute2)
        {
            if (attribute2 != Unknown && attribute2 != None)
            {
                return "%";
            }
            switch (attribute1)
            {
                case FlatInc:
                case PhysInc:
                case CondInc:
                case CondRec:
                case CondRec2:
                case PhysRec:
                case PhysRec2:
                case AttackSpeed:
                case ConditionDurationInc:
                case GlancingBlow:
                case CriticalChance:
                case StrikeDamageToHP:
                case ConditionDamageToHP:
                case EnduranceRegeneration:
                case HealingEffectivenessRec:
                case SiphonInc:
                case SiphonRec:
                case HealingEffectivenessConvInc:
                case HealingEffectivenessFlatInc:
                case ExperienceFromKills:
                case ExperienceFromAll:
                case GoldFind:
                case MovementSpeed:
                case KarmaBonus:
                case SkillRechargeSpeedIncrease:
                case MagicFind:
                case WXP:
                    return "%";
                case MovementActivationDamageFormula:
                    return " adds";
                case SkillActivationDamageFormula:
                    return " replaces";
                case Unknown:
                    return "Unknown";
                default:
                    return "";
            }
        }
        // Effect type
        public int Type { get; }
        // Effect attributes
        public byte ByteAttr1 { get; }
        public ArcDPSEnums.BuffAttribute Attr1 { get; private set; }
        public byte ByteAttr2 { get; }
        public ArcDPSEnums.BuffAttribute Attr2 { get; private set; }
        // Effect parameters
        public float ConstantOffset { get; }
        public float LevelOffset { get; }
        public float Variable { get; }
        // Effect Condition
        public int TraitSrc { get; }
        public int TraitSelf { get; }
        public int BuffSrc { get; }
        public int BuffSelf { get; }
        internal long SortKey => TraitSrc + TraitSelf + BuffSrc + BuffSelf;
        public bool IsConditional => SortKey > 0;
        // Meta data
        private bool Npc { get; }
        private bool Player { get; }
        private bool Break { get; }
        // Extra number
        private byte ExtraNumberState { get; }
        private uint ExtraNumber { get; }
        private bool IsExtraNumberBuffID => ExtraNumberState == 2;
        private bool IsExtraNumberNone => ExtraNumberState == 0;
        private bool IsExtraNumberSomething => ExtraNumberState == 1;

        private bool IsFlippedFormula => Attr1 == PhysRec2 || Attr1 == CondRec2;

        private string _solvedDescription = null;

        private readonly BuffInfoEvent _buffInfoEvent;

        private int Level => (_buffInfoEvent.Category == ArcDPSEnums.BuffCategory.Food || _buffInfoEvent.Category == ArcDPSEnums.BuffCategory.Enhancement) ? 0 : (Attr1 == DamageFormulaSquaredLevel ? 6400 : 80);

        internal BuffFormula(Combat evtcItem, BuffInfoEvent buffInfoEvent)
        {
            _buffInfoEvent = buffInfoEvent;
            Npc = evtcItem.IsFlanking == 0;
            Player = evtcItem.IsShields == 0;
            Break = evtcItem.IsOffcycle > 0;
            byte[] formulaBytes = new byte[10 * sizeof(float)];
            int offset = 0;
            // 2 
            foreach (byte bt in BitConverter.GetBytes(evtcItem.Time))
            {
                formulaBytes[offset++] = bt;
            }
            // 2
            foreach (byte bt in BitConverter.GetBytes(evtcItem.SrcAgent))
            {
                formulaBytes[offset++] = bt;
            }
            // 2
            foreach (byte bt in BitConverter.GetBytes(evtcItem.DstAgent))
            {
                formulaBytes[offset++] = bt;
            }
            // 1
            foreach (byte bt in BitConverter.GetBytes(evtcItem.Value))
            {
                formulaBytes[offset++] = bt;
            }
            // 1
            foreach (byte bt in BitConverter.GetBytes(evtcItem.BuffDmg))
            {
                formulaBytes[offset++] = bt;
            }
            // 0.5
            foreach (byte bt in BitConverter.GetBytes(evtcItem.SrcInstid))
            {
                formulaBytes[offset++] = bt;
            }
            // 0.5
            foreach (byte bt in BitConverter.GetBytes(evtcItem.DstInstid))
            {
                formulaBytes[offset++] = bt;
            }
            // 0.5
            foreach (byte bt in BitConverter.GetBytes(evtcItem.SrcMasterInstid))
            {
                formulaBytes[offset++] = bt;
            }
            // 0.5
            foreach (byte bt in BitConverter.GetBytes(evtcItem.DstMasterInstid))
            {
                formulaBytes[offset++] = bt;
            }
            //
            float[] formulaFloats = new float[10];
            Buffer.BlockCopy(formulaBytes, 0, formulaFloats, 0, formulaBytes.Length);
            //
            Type = (int)formulaFloats[0];
            ByteAttr1 = (byte)formulaFloats[1];
            ByteAttr2 = (byte)formulaFloats[2];
            Attr1 = ArcDPSEnums.GetBuffAttribute(ByteAttr1);
            Attr2 = ArcDPSEnums.GetBuffAttribute(ByteAttr2);
            ConstantOffset = formulaFloats[3];
            LevelOffset = formulaFloats[4];
            Variable = formulaFloats[5];
            TraitSrc = (int)formulaFloats[6];
            TraitSelf = (int)formulaFloats[7];
            BuffSrc = (int)formulaFloats[8];
            BuffSelf = (int)formulaFloats[9];
            ExtraNumber = evtcItem.OverstackValue;
            ExtraNumberState = evtcItem.Pad1;
        }

        internal void AdjustUnknownFormulaAttributes(Dictionary<byte, ArcDPSEnums.BuffAttribute> solved)
        {
            if (Attr1 == Unknown && solved.TryGetValue(ByteAttr1, out ArcDPSEnums.BuffAttribute solvedAttr))
            {
                Attr1 = solvedAttr;
            }
            if (Attr2 == Unknown && solved.TryGetValue(ByteAttr2, out solvedAttr))
            {
                Attr2 = solvedAttr;
            }
        }

        public string GetDescription(bool authorizeUnknowns, IReadOnlyDictionary<long, Buff> buffsByIds)
        {
            if (!authorizeUnknowns && (Attr1 == Unknown || Attr2 == Unknown))
            {
                return "";
            }
            if (_solvedDescription != null)
            {
                return _solvedDescription;
            }
            _solvedDescription = "";
            if (Attr1 == None)
            {
                return _solvedDescription;
            }
            string stat1 = GetAttributeString(Attr1);
            if (Attr1 == Unknown)
            {
                stat1 += " " + ByteAttr1;
            }
            if (IsExtraNumberBuffID)
            {
                if (buffsByIds.TryGetValue(ExtraNumber, out Buff buff))
                {
                    stat1 += " (" + buff.Name + ")";
                }
            }
            string stat2 = GetAttributeString(Attr2);
            if (Attr2 == Unknown)
            {
                stat2 += " " + ByteAttr2;
            }
            _solvedDescription += stat1;
            double variable = Math.Round(Variable, 6);
            double totalOffset = Math.Round(Level * LevelOffset + ConstantOffset, 6);
            bool addParenthesis = totalOffset != 0 && Variable != 0;
            if (Attr2 != None)
            {
                _solvedDescription += " from " + stat2;
                totalOffset *= 100.0;
                variable *= 100.0;
            }
            if (IsFlippedFormula)
            {
                variable = variable - 100.0;
                totalOffset = totalOffset - 100.0;
            }
            _solvedDescription += ": ";
            if (addParenthesis)
            {
                _solvedDescription += "(";
            }
            bool prefix = false;
            if (Variable != 0)
            {
                _solvedDescription += variable + " * " + GetVariableStat(Attr1, Type);
                prefix = true;
            }
            if (totalOffset != 0)
            {
                _solvedDescription += (Math.Sign(totalOffset) < 0 ? " -" : " +") + (prefix ? " " : "") + Math.Abs(totalOffset);
            }
            if (addParenthesis)
            {
                _solvedDescription += ")";
            }
            _solvedDescription += GetPercent(Attr1, Attr2);
            if (Npc && !Player)
            {
                _solvedDescription += ", on NPCs";
            }
            if (!Npc && Player)
            {
                _solvedDescription += ", on Players";
            }
            if (TraitSelf > 0)
            {
                _solvedDescription += ", using " + TraitSelf;
            }
            if (TraitSrc > 0)
            {
                _solvedDescription += ", source using " + TraitSrc;
            }
            if (BuffSelf > 0)
            {
                _solvedDescription += ", under " + BuffSelf;
            }
            if (BuffSrc > 0)
            {
                _solvedDescription += ", source under " + BuffSrc;
            }
            return _solvedDescription;
        }
    }
}
