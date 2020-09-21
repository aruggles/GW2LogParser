using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gw2LogParser.Parser.Data.Events.MetaData
{
    public class ErrorEvent : AbstractMetaDataEvent
    {
        public string Message { get; }
        internal ErrorEvent(Combat evtcItem) : base(evtcItem)
        {
            byte[] bytes = new byte[32 * 1]; // 32 * sizeof(char), char as in C not C#
            int offset = 0;
            // 8 bytes
            foreach (byte bt in BitConverter.GetBytes(evtcItem.Time))
            {
                bytes[offset++] = bt;
            }
            // 8 bytes
            foreach (byte bt in BitConverter.GetBytes(evtcItem.SrcAgent))
            {
                bytes[offset++] = bt;
            }
            // 8 bytes
            foreach (byte bt in BitConverter.GetBytes(evtcItem.DstAgent))
            {
                bytes[offset++] = bt;
            }
            // 4 bytes
            foreach (byte bt in BitConverter.GetBytes(evtcItem.Value))
            {
                bytes[offset++] = bt;
            }
            // 4 bytes
            foreach (byte bt in BitConverter.GetBytes(evtcItem.BuffDmg))
            {
                bytes[offset++] = bt;
            }
            Message = System.Text.Encoding.UTF8.GetString(bytes);
        }
    }
}
