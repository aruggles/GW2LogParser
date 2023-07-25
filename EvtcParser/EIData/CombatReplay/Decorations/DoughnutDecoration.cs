﻿namespace GW2EIEvtcParser.EIData
{
    internal class DoughnutDecoration : FormDecoration
    {
        public int OuterRadius { get; }
        public int InnerRadius { get; }

        public DoughnutDecoration(bool fill, int growing, int innerRadius, int outerRadius, (int start, int end) lifespan, string color, Connector connector) : base(fill, growing, lifespan, color, connector)
        {
            InnerRadius = innerRadius;
            OuterRadius = outerRadius;
        }
        //

        public override GenericDecorationCombatReplayDescription GetCombatReplayDescription(CombatReplayMap map, ParsedEvtcLog log)
        {
            return new DoughnutDecorationCombatReplayDescription(log, this, map);
        }

    }
}
