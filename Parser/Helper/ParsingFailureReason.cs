﻿using Gw2LogParser.Exceptions;
using System;

namespace Gw2LogParser.Parser.Helper
{
    public class ParsingFailureReason
    {
        private Exception _reason { get; }

        public bool IsEvtcContentIssue => _reason is EvtcContentException;

        public bool IsSafeToIgnore => _reason is EINonFatalException;

        public bool IsParserBug => !(_reason is EIException);

        public string Reason => _reason.Message;

        internal ParsingFailureReason(Exception ex)
        {
            _reason = ParserHelper.GetFinalException(ex);
        }

        /// <summary>
        /// Throws the exception
        /// </summary>
        public void Throw()
        {
            throw _reason;
        }

        /// <summary>
        /// Throws the exception if reason is not an <see cref="EIException"/>
        /// </summary>
        public void ThrowIfUnknown()
        {
            if (IsParserBug)
            {
                throw _reason;
            }
        }
    }
}
