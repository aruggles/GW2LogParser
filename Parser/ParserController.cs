﻿using System;
using System.Collections.Generic;
using System.IO;

namespace Gw2LogParser.Parser
{
    public abstract class ParserController
    {
        protected List<string> StatusList { get; }

        protected ParserController()
        {
            StatusList = new List<string>();
        }

        protected virtual void ThrowIfCanceled()
        {

        }

        public void WriteLogMessages(StreamWriter sw)
        {
            foreach (string str in StatusList)
            {
                sw.WriteLine(str);
            }
        }

        public virtual void Reset()
        {
            StatusList.Clear();
        }

        public virtual void UpdateProgressWithCancellationCheck(string status)
        {
            UpdateProgress(status);
            ThrowIfCanceled();
        }
        public void UpdateProgress(string status)
        {
            StatusList.Add(status);
        }
    }
}
