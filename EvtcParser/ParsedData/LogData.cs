﻿using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Extensions;

namespace GW2EIEvtcParser.ParsedData
{
    public class LogData
    {
        private const string DefaultTimeValue = "MISSING";

        // Fields
        public string ArcVersion => "EVTC" + EvtcVersion;
        public long EvtcVersion { get; } = -1;
        public string Language { get; } = "N/A";
        public LanguageEvent.LanguageEnum LanguageID { get; }
        public ulong GW2Build { get; } = 0;
        public AgentItem PoV { get; private set; } = null;
        public string PoVAccount { get; private set; } = "N/A";
        public string PoVName { get; private set; } = "N/A";
        private readonly string _dateFormat = "yyyy-MM-dd HH:mm:ss zz";
        public double LogStartRaw { get; private set; } = 0;
        public double LogEndRaw { get; private set; } = 0;
        private readonly string _dateFormatStd = "yyyy-MM-dd HH:mm:ss zzz";
        public string LogStart { get; private set; } = DefaultTimeValue;
        public string LogEnd { get; private set; } = DefaultTimeValue;
        public string LogStartStd { get; private set; } = DefaultTimeValue;
        public string LogEndStd { get; private set; } = DefaultTimeValue;

        public IReadOnlyList<AbstractExtensionHandler> UsedExtensions { get; }

        public IReadOnlyList<string> LogErrors => _logErrors;
        private readonly List<string> _logErrors = new List<string>();

        // Constructors
        internal LogData(long evtcVersion, CombatData combatData, long evtcLogDuration, List<Player> playerList, IReadOnlyDictionary<uint, AbstractExtensionHandler> extensions, ParserController operation)
        {
            EvtcVersion = evtcVersion;
            double unixStart = 0;
            double unixEnd = 0;
            //
            PointOfViewEvent povEvt = combatData.GetPointOfViewEvent();
            if (povEvt != null)
            {
                SetPOV(povEvt.PoV, playerList);
            }
            operation.UpdateProgressWithCancellationCheck("Parsing: PoV " + PoVName);
            //
            BuildEvent buildEvt = combatData.GetBuildEvent();
            if (buildEvt != null)
            {
                GW2Build = buildEvt.Build;
            }
            operation.UpdateProgressWithCancellationCheck("Parsing: GW2 Build " + GW2Build);
            //
            LanguageEvent langEvt = combatData.GetLanguageEvent();
            if (langEvt != null)
            {
                Language = langEvt.ToString();
                LanguageID = langEvt.Language;
            }
            operation.UpdateProgressWithCancellationCheck("Parsing: Language " + Language);
            //
            LogStartEvent logStr = combatData.GetLogStartEvent();
            if (logStr != null)
            {
                SetLogStart(logStr.ServerUnixTimeStamp);
                SetLogStartStd(logStr.ServerUnixTimeStamp);
                unixStart = logStr.ServerUnixTimeStamp;
            }
            //
            LogEndEvent logEnd = combatData.GetLogEndEvent();
            if (logEnd != null)
            {
                SetLogEnd(logEnd.ServerUnixTimeStamp);
                SetLogEndStd(logEnd.ServerUnixTimeStamp);
                unixEnd = logEnd.ServerUnixTimeStamp;
            }
            // log end event is missing, log start is present
            if (LogEnd == DefaultTimeValue && LogStart != DefaultTimeValue)
            {
                operation.UpdateProgressWithCancellationCheck("Parsing: Missing Log End Event");
                double dur = Math.Round(evtcLogDuration / 1000.0, 3);
                SetLogEnd(dur + unixStart);
                SetLogEndStd(dur + unixStart);
            }
            // log start event is missing, log end is present
            if (LogEnd != DefaultTimeValue && LogStart == DefaultTimeValue)
            {
                operation.UpdateProgressWithCancellationCheck("Parsing: Missing Log Start Event");
                double dur = Math.Round(evtcLogDuration / 1000.0, 3);
                SetLogStart(unixEnd - dur);
                SetLogStartStd(unixEnd - dur);
            }
            operation.UpdateProgressWithCancellationCheck("Parsing: Log Start " + LogStartStd);
            operation.UpdateProgressWithCancellationCheck("Parsing: Log End " + LogEndStd);
            //
            foreach (ErrorEvent evt in combatData.GetErrorEvents())
            {
                operation.UpdateProgressWithCancellationCheck("Parsing: Error " + evt.Message);
                _logErrors.Add(evt.Message);
            }
            //
            UsedExtensions = extensions.Values.ToList();
        }

        // Setters
        private void SetPOV(AgentItem pov, List<Player> playerList)
        {
            PoV = pov;
            Player povPlayer = playerList.Find(x => x.AgentItem == pov);
            if (povPlayer != null)
            {
                PoVName = povPlayer.Character;
                PoVAccount = povPlayer.Account;
            }
        }

        private string GetDateTime(double unixSeconds)
        {
            var dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixSeconds).ToLocalTime();
            return dtDateTime.ToString(_dateFormat);
        }

        private string GetDateTimeStd(double unixSeconds)
        {
            var dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixSeconds).ToLocalTime();
            return dtDateTime.ToString(_dateFormatStd);
        }

        private void SetLogStart(double unixSeconds)
        {
            LogStart = GetDateTime(unixSeconds);
            LogStartRaw = unixSeconds;
        }

        private void SetLogEnd(double unixSeconds)
        {
            LogEnd = GetDateTime(unixSeconds);
            LogEndRaw = unixSeconds;
        }

        private void SetLogStartStd(double unixSeconds)
        {
            LogStartStd = GetDateTimeStd(unixSeconds);
        }

        private void SetLogEndStd(double unixSeconds)
        {
            LogEndStd = GetDateTimeStd(unixSeconds);
        }
    }
}
