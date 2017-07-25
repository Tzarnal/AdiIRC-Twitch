using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using AdiIRCAPIv2.Arguments.ChannelMessages;

namespace Twitch___AdiIRC
{
    public class TwitchIrcMessage
    {
        public string Message;
        public string Channel;
        public string UserName;
        public string UserMask;
        public string BadgeList;
        public bool HasEmotes;
        public bool HasBadges;
        public Dictionary<string, string> Tags;
        public List<TwitchEmote> Emotes;

        private string EmoteRegex = @"((\d+):(\d+)-(\d+))";

        public TwitchIrcMessage(ChannelNormalMessageArgs argument)
        {
            //Assign information we care about to fields.
            Message = argument.Message;
            Tags = (Dictionary<string,string>) argument.MessageTags;
            UserMask = argument.User.Host;
            UserName = argument.User.Nick;

            //Parse the Tags for emotes
            //I'm being lazy on int.parse security so i'm throwing in a
            //catchall Exception 
            try
            {
                HasEmotes = ExtractEmotes();
            }
            catch (Exception)
            {
                HasEmotes = false;
            }
        }

        private void ExtractBadges()
        {
            if (!Tags.ContainsKey("badges"))
            {
                HasBadges = false;
                return;
            }

            var badges = Tags["badges"];

            if (badges.Contains("broadcaster/1"))
            {
                BadgeList += "📺";
            }

            if (badges.Contains("staff/1"))
            {
                BadgeList += "🔧";
            }

            if (badges.Contains("admin/1"))
            {
                BadgeList += "🛡️";
            }

            if (badges.Contains("global_mod/1"))
            {
                BadgeList += "⚔️";
            }

            if (badges.Contains("moderator/1"))
            {
                BadgeList += "🗡️";
            }

            if (badges.Contains("subscriber/"))
            {
                BadgeList += "⭐";
            }

            if (badges.Contains("turbo/1"))
            {
                BadgeList += "⚡";
            }

            if (badges.Contains("prime/1"))
            {
                BadgeList += "👑";
            }

            HasBadges = !string.IsNullOrWhiteSpace(BadgeList);
        }

        private bool ExtractEmotes()
        {
            //Early exit if the tags does not contain any emotes.
            if (!Tags.ContainsKey("emotes"))
            {
                return false;
            }

            Emotes = new List<TwitchEmote>();

            var emoteMatches = Regex.Matches(Tags["emotes"], EmoteRegex);

            //Early exit if no emotes were actually matched.
            if (emoteMatches.Count <= 0)
            {
                return false;
            }

            //Emotes are received as an id and the partof the message they match too
            //So we substring the actual name of the mote out of the message.
            foreach (Match match in emoteMatches)
            {
                var emoteId = int.Parse(match.Groups[2].ToString());
                var startIndex = int.Parse(match.Groups[3].ToString());
                var endIndex = int.Parse(match.Groups[4].ToString());

                var emoteName = Message.Substring(startIndex, endIndex - startIndex + 1);

                var emote = new TwitchEmote { Id = emoteId, Name = emoteName };

                Emotes.Add(emote);
            }

            return true;
        }
    }
}
