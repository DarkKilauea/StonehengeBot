using System;
using Microsoft.Extensions.Options;
using StonehengeBot.Configuration;
using StonehengeBot.Data;
using Xunit;

namespace StonehengeBot.Tests
{
    public class PledgesRepositoryTests
    {
        private readonly PledgeOptions _defaultOptions = new PledgeOptions
        {
            StartDate = new DateTimeOffset(2018, 01, 10, 02, 00, 00, TimeSpan.FromHours(-4)),
            QuestGivers = new []
            {
                new PledgeOptions.QuestGiver
                {
                    Name = "Maj al-Ragath",
                    Dungeons = new []
                    {
                        "Spindleclutch II",
                        "Banished Cells I",
                        "Fungal Grotto II",
                        "Spindleclutch I",
                        "Darkshade Caverns II",
                        "Elden Hollow I",
                        "Wayrest Sewers II",
                        "Fungal Grotto I",
                        "Banished Cells II",
                        "Darkshade Caverns I",
                        "Elden Hollow II",
                        "Wayrest Sewers I"
                    }
                },
                new PledgeOptions.QuestGiver
                {
                    Name = "Glirion the Redbeard",
                    Dungeons = new []
                    {
                        "Direfrost Keep",
                        "Vaults of Madness",
                        "Crypt of Hearts II",
                        "City of Ash I",
                        "Tempest Island",
                        "Blackheart Haven",
                        "Arx Corinium",
                        "Selene's Web",
                        "City of Ash II",
                        "Crypt of Hearts I",
                        "Volenfell",
                        "Blessed Crucible"
                    }
                },
                new PledgeOptions.QuestGiver
                {
                    Name = "Urgarlag Chief-bane",
                    Dungeons = new []
                    {
                        "Ruins of Mazzatun",
                        "White-Gold Tower",
                        "Cradle of Shadows",
                        "Imperial City Prison",
                        "Bloodroot Forge"
                    }
                }
            }
        };

        [Fact]
        public void Should_Return_The_Correct_Pledges_On_The_Start_Date()
        {
            var targetDate = _defaultOptions.StartDate;
            var expectedResults = new[]
            {
                new PledgeInfo("Maj al-Ragath", "Spindleclutch II"),
                new PledgeInfo("Glirion the Redbeard", "Direfrost Keep"),
                new PledgeInfo("Urgarlag Chief-bane", "Ruins of Mazzatun")
            };

            var repository = new PledgesRepository(Options.Create(_defaultOptions));
            var results = repository.GetPledgesForDate(targetDate);

            Assert.Equal(expectedResults, results);
        }

        [Fact]
        public void Should_Return_The_Correct_Pledges_After_Start_Date()
        {
            var targetDate = new DateTimeOffset(2018, 01, 20, 02, 00, 00, TimeSpan.FromHours(-4));
            var expectedResults = new[]
            {
                new PledgeInfo("Maj al-Ragath", "Elden Hollow II"),
                new PledgeInfo("Glirion the Redbeard", "Volenfell"),
                new PledgeInfo("Urgarlag Chief-bane", "Ruins of Mazzatun")
            };

            var repository = new PledgesRepository(Options.Create(_defaultOptions));
            var results = repository.GetPledgesForDate(targetDate);

            Assert.Equal(expectedResults, results);
        }
    }
}
