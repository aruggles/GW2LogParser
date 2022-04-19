using Gw2LogParser.Parser.Data;
using Gw2LogParser.Parser.Data.Agents;
using Gw2LogParser.Parser.Data.Skills;

namespace Gw2LogParser.Parser.Extensions
{
    public abstract class AbstractExtensionHandler
    {
        public uint Signature { get; }
        public uint Revision { get; protected set; }
        public string Name { get; } = "Unknown";
        public string Version { get; protected set; } = "Unknown";

        internal AbstractExtensionHandler(uint sig, string name)
        {
            Signature = sig;
            Name = name;
        }

        internal abstract bool HasTime(Combat c);
        internal abstract bool SrcIsAgent(Combat c);
        internal abstract bool DstIsAgent(Combat c);

        internal abstract bool IsDamage(Combat c);

        internal abstract void InsertEIExtensionEvent(Combat c, AgentData agentData, SkillData skillData);

        internal abstract void AttachToCombatData(CombatData combatData, ParserController operation, ulong gw2Build);

        internal abstract void AdjustCombatEvent(Combat combatItem, AgentData agentData);
    }
}
