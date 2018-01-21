using System;
using System.Collections.Generic;

namespace StonehengeBot.Data
{
    /// <summary>
    /// Contains information about an active pledge quest
    /// </summary>
    public struct PledgeInfo : IEquatable<PledgeInfo>
    {
        /// <summary>
        /// Name of the NPC giving the quest
        /// </summary>
        public string QuestGiver { get; }

        /// <summary>
        /// Name of the dungeon the quest is for
        /// </summary>
        public string DungeonName { get; }

        /// <summary>
        /// Create a container representing a single pledge quest
        /// </summary>
        /// <param name="questGiver">Name of the NPC giving the quest</param>
        /// <param name="dungeonName">Name of the dungeon the quest is for</param>
        public PledgeInfo(string questGiver, string dungeonName)
        {
            QuestGiver = questGiver;
            DungeonName = dungeonName;
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return obj is PledgeInfo info && Equals(info);
        }

        /// <inheritdoc />
        public bool Equals(PledgeInfo other)
        {
            return QuestGiver == other.QuestGiver &&
                DungeonName == other.DungeonName;
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            var hashCode = 23651936;
            hashCode = hashCode * -1521134295 + base.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(QuestGiver);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(DungeonName);
            return hashCode;
        }

        public static bool operator ==(PledgeInfo info1, PledgeInfo info2)
        {
            return info1.Equals(info2);
        }

        public static bool operator !=(PledgeInfo info1, PledgeInfo info2)
        {
            return !(info1 == info2);
        }
    }
}
