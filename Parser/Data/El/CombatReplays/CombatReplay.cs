﻿using Gw2LogParser.Parser.Data.El.CombatReplays.Decorations;
using Gw2LogParser.Parser.Data.El.Statistics;
using Gw2LogParser.Parser.Helper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Gw2LogParser.Parser.Data.El.CombatReplays
{
    public class CombatReplay
    {
        internal List<Point3D> Positions { get; } = new List<Point3D>();
        internal List<Point3D> PolledPositions { get; private set; } = new List<Point3D>();
        internal List<Point3D> Velocities { get; private set; } = new List<Point3D>();
        internal List<Point3D> Rotations { get; } = new List<Point3D>();
        internal List<Point3D> PolledRotations { get; private set; } = new List<Point3D>();
        private long _start = -1;
        private long _end = -1;
        internal (long start, long end) TimeOffsets => (_start, _end);
        // actors
        internal List<GenericDecoration> Decorations { get; } = new List<GenericDecoration>();

        internal CombatReplay(ParsedLog log)
        {
            _start = 0;
            _end = log.FightData.FightDuration;
        }

        internal void Trim(long start, long end)
        {
            PolledPositions.RemoveAll(x => x.Time < start || x.Time > end);
            PolledRotations.RemoveAll(x => x.Time < start || x.Time > end);
            _start = Math.Max(start, _start);
            _end = Math.Max(_start, Math.Min(end, _end));
        }

        private static int UpdateVelocityIndex(List<Point3D> velocities, int time, int currentIndex)
        {
            if (!velocities.Any())
            {
                return -1;
            }
            int res = Math.Max(currentIndex, 0);
            Point3D cuvVelocity = velocities[res];
            while (res < velocities.Count && cuvVelocity.Time < time)
            {
                res++;
                if (res < velocities.Count)
                {
                    cuvVelocity = velocities[res];
                }
            }
            return res - 1;
        }

        private void PositionPolling(int rate, long fightDuration)
        {
            if (Positions.Count == 0)
            {
                Positions.Add(new Point3D(int.MinValue, int.MinValue, 0, 0));
            }
            int positionTablePos = 0;
            int velocityTablePos = 0;
            //
            for (int i = (int)Math.Min(0, rate * ((Positions[0].Time / rate) - 1)); i < fightDuration; i += rate)
            {
                Point3D pt = Positions[positionTablePos];
                if (i <= pt.Time)
                {
                    PolledPositions.Add(new Point3D(pt.X, pt.Y, pt.Z, i));
                }
                else
                {
                    if (positionTablePos == Positions.Count - 1)
                    {
                        PolledPositions.Add(new Point3D(pt.X, pt.Y, pt.Z, i));
                    }
                    else
                    {
                        Point3D ptn = Positions[positionTablePos + 1];
                        if (ptn.Time < i)
                        {
                            positionTablePos++;
                            i -= rate;
                        }
                        else
                        {
                            Point3D last = PolledPositions.Last().Time > pt.Time ? PolledPositions.Last() : pt;
                            velocityTablePos = UpdateVelocityIndex(Velocities, i, velocityTablePos);
                            Point3D velocity = null;
                            if (velocityTablePos >= 0 && velocityTablePos < Velocities.Count)
                            {
                                velocity = Velocities[velocityTablePos];
                            }
                            if (velocity == null || (Math.Abs(velocity.X) <= 1e-1 && Math.Abs(velocity.Y) <= 1e-1))
                            {
                                PolledPositions.Add(new Point3D(last.X, last.Y, last.Z, i));
                            }
                            else
                            {
                                float ratio = (float)(i - last.Time) / (ptn.Time - last.Time);
                                PolledPositions.Add(new Point3D(last, ptn, ratio, i));
                            }

                        }
                    }
                }
            }
            PolledPositions = PolledPositions.Where(x => x.Time >= 0).ToList();
        }
        /// <summary>
        /// The method exists only to have the same amount of rotation as positions, it's easier to do it here than
        /// in javascript
        /// </summary>
        /// <param name="rate"></param>
        /// <param name="fightDuration"></param>
        /// <param name="forceInterpolate"></param>
        private void RotationPolling(int rate, long fightDuration)
        {
            if (Rotations.Count == 0)
            {
                return;
            }
            int rotationTablePos = 0;
            for (int i = (int)Math.Min(0, rate * ((Rotations[0].Time / rate) - 1)); i < fightDuration; i += rate)
            {
                Point3D pt = Rotations[rotationTablePos];
                if (i <= pt.Time)
                {
                    PolledRotations.Add(new Point3D(pt.X, pt.Y, pt.Z, i));
                }
                else
                {
                    if (rotationTablePos == Rotations.Count - 1)
                    {
                        PolledRotations.Add(new Point3D(pt.X, pt.Y, pt.Z, i));
                    }
                    else
                    {
                        Point3D ptn = Rotations[rotationTablePos + 1];
                        if (ptn.Time < i)
                        {
                            rotationTablePos++;
                            i -= rate;
                        }
                        else
                        {
                            PolledRotations.Add(new Point3D(pt.X, pt.Y, pt.Z, i));
                        }
                    }
                }
            }
            PolledRotations = PolledRotations.Where(x => x.Time >= 0).ToList();
        }

        internal void PollingRate(long fightDuration)
        {
            PositionPolling(ParserHelper.CombatReplayPollingRate, fightDuration);
            RotationPolling(ParserHelper.CombatReplayPollingRate, fightDuration);
        }
    }
}
