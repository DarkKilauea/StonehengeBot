using System;
using JetBrains.Annotations;

namespace StonehengeBot.Configuration
{
    /// <summary>
    /// Configuration and information about pledges in ESO
    /// </summary>
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class PledgeOptions
    {
        /// <summary>
        /// Title to use for the embed message
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Url to a page describing pledges
        /// </summary>
        public Uri Url { get; set; }

        /// <summary>
        /// Date to use as a reference when computing which pledges are available for a particular date
        /// Also provides the time that the pledges reset
        /// </summary>
        public DateTimeOffset StartDate { get; set; }

        /// <summary>
        /// List of NPCs that give pledges and the dungeons they provide quests for
        /// in the order those quests are given
        /// </summary>
        public QuestGiver[] QuestGivers { get; set; }

        /// <summary>
        /// Represents an NPC that hands out pledges
        /// </summary>
        public class QuestGiver
        {
            /// <summary>
            /// Name of the NPC
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// List of dungeons they give quests for, in order
            /// </summary>
            public string[] Dungeons { get; set; }
        }
    }
}
