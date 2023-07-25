﻿using GW2EIEvtcParser.EIData;
using Gw2LogParser.EvtcParserExtensions;
using System.Collections.Generic;

namespace Gw2LogParser.GW2EIBuilders
{
    internal static class JsonActorCombatReplayDataBuilder
    {
        public static JsonActorCombatReplayData BuildJsonActorCombatReplayDataBuilder(AbstractSingleActor actor, ParsedLog log, RawFormatSettings settings)
        {
            CombatReplayMap map = log.FightData.Logic.GetCombatReplayMap(log);
            AbstractSingleActorCombatReplayDescription description = actor.GetCombatReplayDescription(map, log);
            var actorCombatReplayData = new JsonActorCombatReplayData()
            {
                IconURL = description.Img,
                Start = description.Start,
                End = description.End
            };
            if (settings.RawFormatTimelineArrays)
            {
                //
                var jsonPositions = new List<float[]>();
                for (int i = 0; i < description.Positions.Count; i += 2)
                {
                    jsonPositions.Add(new float[2] { description.Positions[i], description.Positions[i + 1] });
                }
                actorCombatReplayData.Positions = jsonPositions;
                //
                if (description.Dead != null)
                {
                    var jsonDeads = new List<long[]>();
                    for (int i = 0; i < description.Dead.Count; i += 2)
                    {
                        jsonDeads.Add(new long[2] { description.Dead[i], description.Dead[i + 1] });
                    }
                    actorCombatReplayData.Dead = jsonDeads;
                }
                if (description.Dc != null)
                {
                    var jsonDcs = new List<long[]>();
                    for (int i = 0; i < description.Dc.Count; i += 2)
                    {
                        jsonDcs.Add(new long[2] { description.Dc[i], description.Dc[i + 1] });
                    }
                    actorCombatReplayData.Dc = jsonDcs;
                }
                if (description.Down != null)
                {
                    var jsonDowns = new List<long[]>();
                    for (int i = 0; i < description.Down.Count; i += 2)
                    {
                        jsonDowns.Add(new long[2] { description.Down[i], description.Down[i + 1] });
                    }
                    actorCombatReplayData.Down = jsonDowns;
                }
                //
                IReadOnlyList<GenericDecoration> decorations = actor.GetCombatReplayDecorations(log);
                foreach (GenericDecoration decoration in decorations)
                {
                    GenericDecorationCombatReplayDescription decDescription = decoration.GetCombatReplayDescription(map, log);
                    if (decDescription is FacingDecorationCombatReplayDescription facingDescription)
                    {
                        actorCombatReplayData.Orientations = facingDescription.FacingData;
                    }
                }
            }

            //
            return actorCombatReplayData;
        }
    }
}
