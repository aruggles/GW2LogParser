﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace GW2EIEvtcParser.EIData
{
    public class CombatReplayMap
    {

        public class MapItem
        {
            public string Link { get; internal set; }
            public long Start { get; internal set; }
            public long End { get; internal set; }
        }

        public IReadOnlyList<MapItem> Maps => _maps;
        private readonly List<MapItem> _maps = new List<MapItem>();
        private (int width, int height) _urlPixelSize;
        private (double topX, double topY, double bottomX, double bottomY) _rectInMap;
        //private (int topX, int topY, int bottomX, int bottomY) _fullRect;
        //private (int bottomX, int bottomY, int topX, int topY) _worldRect;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="link">Url to the image</param>
        /// <param name="urlPixelSize">Width and Height of the image in pixel</param>
        /// <param name="rectInMap">The map rectangle region corresponding to the image in map coordinates</param>
        internal CombatReplayMap(string link, (int width, int height) urlPixelSize, (double topX, double topY, double bottomX, double bottomY) rectInMap)
        {
            _maps.Add(new MapItem()
            {
                Link = link,
                Start = 0,
                End = long.MaxValue
            });
            _urlPixelSize = urlPixelSize;
            _rectInMap = rectInMap;
        }

        /*/internal CombatReplayMap(string link, (int width, int height) size, (int topX, int topY, int bottomX, int bottomY) rect, (int topX, int topY, int bottomX, int bottomY) fullRect, (int bottomX, int bottomY, int topX, int topY) worldRect)
        {
            _maps.Add(new MapItem()
            {
                Link = link,
                Start = -1,
                End = -1
            });
            _size = size;
            _rect = rect;
            _fullRect = fullRect;
            _worldRect = worldRect;
        }*/

        public (int width, int height) GetPixelMapSize()
        {
            double ratio = (double)_urlPixelSize.width / _urlPixelSize.height;
            const int pixelSize = 750;
            if (ratio > 1.0)
            {
                return (pixelSize, (int)Math.Round(pixelSize / ratio));
            }
            else if (ratio < 1.0)
            {
                return ((int)Math.Round(ratio * pixelSize), pixelSize);
            }
            else
            {
                return (pixelSize, pixelSize);
            }
        }

        internal void ComputeBoundingBox(ParsedEvtcLog log)
        {
            if (log.CanCombatReplay && _rectInMap.topX == _rectInMap.bottomX)
            {
                _rectInMap.topX = int.MaxValue;
                _rectInMap.topY = int.MaxValue;
                _rectInMap.bottomX = int.MinValue;
                _rectInMap.bottomY = int.MinValue;
                foreach (Player p in log.PlayerList)
                {
                    IReadOnlyList<Point3D> pos = p.GetCombatReplayPolledPositions(log);
                    if (pos.Count == 0)
                    {
                        continue;
                    }
                    _rectInMap.topX = Math.Min(Math.Floor(pos.Min(x => x.X)) - 250, _rectInMap.topX);
                    _rectInMap.topY = Math.Min(Math.Floor(pos.Min(x => x.Y)) - 250, _rectInMap.topY);
                    _rectInMap.bottomX = Math.Max(Math.Floor(pos.Max(x => x.X)) + 250, _rectInMap.bottomX);
                    _rectInMap.bottomY = Math.Max(Math.Floor(pos.Max(x => x.Y)) + 250, _rectInMap.bottomY);
                }
            }
        }

        internal (float x, float y) GetMapCoord(float realX, float realY)
        {
            (int width, int height) = GetPixelMapSize();
            double scaleX = (double)width / _urlPixelSize.width;
            double scaleY = (double)height / _urlPixelSize.height;
            double x = (realX - _rectInMap.topX) / (_rectInMap.bottomX - _rectInMap.topX);
            double y = (realY - _rectInMap.topY) / (_rectInMap.bottomY - _rectInMap.topY);
            return ((float)Math.Round(scaleX * _urlPixelSize.width * x, ParserHelper.CombatReplayDataDigit), (float)Math.Round(scaleY * (_urlPixelSize.height - _urlPixelSize.height * y), ParserHelper.CombatReplayDataDigit));
        }

        /// <summary>
        /// This assumes that all urls are of the same size (or at least size ratio) and that they have the same map rectangle
        /// </summary>
        /// <param name="urls"></param>
        /// <param name="phases"></param>
        /// <param name="fightEnd"></param>
        internal void MatchMapsToPhases(List<string> urls, List<PhaseData> phases, long fightEnd)
        {
            if (phases.Count - 1 > urls.Count)
            {
                return;
            }
            MapItem originalMap = _maps[0];
            originalMap.Start = 0;
            for (int i = 1; i < phases.Count; i++)
            {
                PhaseData phase = phases[i];
                _maps.Last().End = phase.Start;
                _maps.Add(new MapItem()
                {
                    Link = urls[i - 1],
                    Start = phase.Start
                });
            }
            _maps.Last().End = fightEnd;
            _maps.RemoveAll(x => x.End - x.Start <= 0);
        }

        public float GetInchToPixel()
        {
            float ratio = (float)(_rectInMap.bottomX - _rectInMap.topX) / GetPixelMapSize().width;
            return (float)Math.Round(1.0f / ratio, 3);
        }

        internal CombatReplayMap Translate(double x, double y)
        {
            _rectInMap.bottomX -= x;
            _rectInMap.topX -= x;
            _rectInMap.bottomY -= y;
            _rectInMap.topY -= y;
            return this;
        }

        internal CombatReplayMap Scale(double scale)
        {
            double centerX = (_rectInMap.bottomX + _rectInMap.topX) / 2.0;
            double halfWidth = scale *(_rectInMap.bottomX - centerX);
            double centerY = (_rectInMap.bottomY + _rectInMap.topY) / 2.0;
            double halfHeigth = scale * (_rectInMap.bottomY - centerY);
            _rectInMap.bottomX = centerX + halfWidth;
            _rectInMap.bottomY = centerY + halfHeigth;
            _rectInMap.topX = centerX - halfWidth;
            _rectInMap.topY = centerY - halfHeigth;
            return this;
        }

        internal CombatReplayMap AdjustForAspectRatio()
        {
            double ratio = (double)_urlPixelSize.width / _urlPixelSize.height;
            double centerY = (_rectInMap.bottomY + _rectInMap.topY) / 2.0;
            double halfHeigth = (_rectInMap.bottomY - centerY);
            double centerX = (_rectInMap.bottomX + _rectInMap.topX) / 2.0;
            double halfWidth = ratio * halfHeigth;
            _rectInMap.bottomX = centerX + halfWidth;
            _rectInMap.bottomY = centerY + halfHeigth;
            _rectInMap.topX = centerX - halfWidth;
            _rectInMap.topY = centerY - halfHeigth;
            return this;
        }

    }
}
