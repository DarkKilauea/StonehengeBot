using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Options;
using StonehengeBot.Configuration;

namespace StonehengeBot.Data
{
    /// <summary>
    /// Provides information about Undaunted Pledges
    /// </summary>
    public class PledgesRepository
    {
        private readonly PledgeOptions _options;

        /// <summary>
        /// Construct a repository of information about Undaunted Pledges
        /// </summary>
        /// <param name="options"></param>
        public PledgesRepository(IOptions<PledgeOptions> options)
        {
            _options = options.Value;
        }

        /// <summary>
        /// Get a list of pledges available for today
        /// </summary>
        public IEnumerable<PledgeInfo> GetPledgesForToday() => GetPledgesForDate(DateTime.Today);

        /// <summary>
        /// Get a list of pledges available for tomorrow
        /// </summary>
        public IEnumerable<PledgeInfo> GetPledgesForTomorrow() => GetPledgesForDate(DateTime.Today.AddDays(1));

        /// <summary>
        /// Get a list of pledges available on a particular date.
        /// </summary>
        /// <param name="date">Date to lookup pledges for.  MUST occur after the <see cref="PledgeOptions.StartDate"/></param>
        public IEnumerable<PledgeInfo> GetPledgesForDate(DateTimeOffset date)
        {
            var daysSinceStart = (int) (date - _options.StartDate).TotalDays;

            return _options.QuestGivers.Select(giver =>
                new PledgeInfo(giver.Name, giver.Dungeons[daysSinceStart % giver.Dungeons.Length])
            );
        }

        /// <summary>
        /// Get the title to use for the embed message
        /// </summary>
        public string GetTitle() => _options.Title;

        /// <summary>
        /// Get the url to a page describing pledges
        /// </summary>
        public Uri GetUrl() => _options.Url;
    }
}
