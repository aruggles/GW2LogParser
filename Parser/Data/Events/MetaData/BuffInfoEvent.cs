﻿using Gw2LogParser.Parser.Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gw2LogParser.Parser.Data.Events.MetaData
{
    public class BuffInfoEvent : AbstractMetaDataEvent
    {
        public long BuffID { get; }

        public bool ProbablyInvul { get; private set; }

        public bool ProbablyInvert { get; private set; }

        public ArcDPSEnums.BuffCategory Category { get; private set; }

        public byte StackingTypeByte { get; private set; }
        public ArcDPSEnums.BuffStackType StackingType { get; private set; }

        public bool ProbablyResistance { get; private set; }

        public ushort MaxStacks { get; private set; }
        public uint DurationCap { get; private set; }
        public List<BuffFormula> Formulas { get; } = new List<BuffFormula>();

        internal BuffInfoEvent(Combat evtcItem) : base(evtcItem)
        {
            BuffID = evtcItem.SkillID;
            CompleteBuffInfoEvent(evtcItem);
        }

        internal void CompleteBuffInfoEvent(Combat evtcItem)
        {
            if (evtcItem.SkillID != BuffID)
            {
                throw new InvalidOperationException("Non matching buff id in BuffDataEvent complete method");
            }
            switch (evtcItem.IsStateChange)
            {
                case ArcDPSEnums.StateChange.BuffFormula:
                    BuildFromBuffFormula(evtcItem);
                    break;
                case ArcDPSEnums.StateChange.BuffInfo:
                    BuildFromBuffInfo(evtcItem);
                    break;
                default:
                    throw new InvalidDataException("Invalid combat event in BuffDataEvent complete method");
            }
        }

        private void BuildFromBuffInfo(Combat evtcItem)
        {
            ProbablyInvul = evtcItem.IsFlanking > 0;
            ProbablyInvert = evtcItem.IsShields > 0;
            Category = ArcDPSEnums.GetBuffCategory(evtcItem.IsOffcycle);
            MaxStacks = evtcItem.SrcMasterInstid;
            DurationCap = evtcItem.OverstackValue;
            StackingTypeByte = evtcItem.Pad1;
            StackingType = ArcDPSEnums.GetBuffStackType(StackingTypeByte);
            ProbablyResistance = evtcItem.Pad2 > 0;
        }

        internal void AdjustBuffInfo(Dictionary<byte, ArcDPSEnums.BuffAttribute> solved)
        {
            Formulas.Sort((x, y) => (x.SortKey).CompareTo(y.SortKey));
            if (solved.Count == 0)
            {
                return;
            }
            foreach (BuffFormula formula in Formulas)
            {
                formula.AdjustUnknownFormulaAttributes(solved);
            }
        }

        private void BuildFromBuffFormula(Combat evtcItem)
        {
            Formulas.Add(new BuffFormula(evtcItem, this));
        }
    }
}
