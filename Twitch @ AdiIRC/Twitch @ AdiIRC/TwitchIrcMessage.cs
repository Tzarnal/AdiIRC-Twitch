using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace TwitchAdiIRC
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

        private string MessageRegex = @"@(.+?) :((.+)!.+?) PRIVMSG (#.+?) :(.+)";
        private string TagsRegex = @"(.*?=.*?);";
        private string EmoteRegex = @"((\d+):(\d+)-(\d+))";

        public TwitchIrcMessage(string message)
        {
            var messageMatch = Regex.Match(message, MessageRegex);

            if (!messageMatch.Success)
            {
                throw new ArgumentException("Not a valid IRCv3 with tags message.");
            }

            //Simple assignments.    
            UserMask = messageMatch.Groups[2].ToString();
            UserName = messageMatch.Groups[3].ToString();
            Channel = messageMatch.Groups[4].ToString();
            Message = messageMatch.Groups[5].ToString();

            //Tags
            Tags = ParseTagsString(messageMatch.Groups[1].ToString());

            //Parse for emotes
            //Cointains IntParses and other general number and substring unpleasantness.

            try
            {
                ExtractEmotes();
            }
            catch (Exception)
            {
                HasEmotes = false;
            }

            ExtractBadges();
        }

        private Dictionary<string, string> ParseTagsString(string tagsString)
        {
            var tags = new Dictionary<string, string>();

            var tagsMatches = Regex.Matches(tagsString, TagsRegex);

            if (tagsMatches.Count > 0)
            {
                foreach (Match match in tagsMatches)
                {
                    var kvPair = match.Groups[1].ToString().Split('=');
                    if (!string.IsNullOrEmpty(kvPair[1]))
                    {
                        tags.Add(kvPair[0], kvPair[1]);
                    }
                }
            }
            return tags;
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

        private void ExtractEmotes()
        {
            if (!Tags.ContainsKey("emotes"))
            {
                HasEmotes = false;
                return;
            }

            Emotes = new List<TwitchEmote>();

            var emoteMatches = Regex.Matches(Tags["emotes"], EmoteRegex);

            if (emoteMatches.Count > 0)
            {
                foreach (Match match in emoteMatches)
                {
                    var emoteId = int.Parse(match.Groups[2].ToString());
                    var startIndex = int.Parse(match.Groups[3].ToString());
                    var endIndex = int.Parse(match.Groups[4].ToString());

                    var emoteName = Message.Substring(startIndex, endIndex - startIndex + 1);

                    var emote = new TwitchEmote { Id = emoteId, Name = emoteName };

                    Emotes.Add(emote);
                }
            }

            HasEmotes = true;
        }
    }
}
