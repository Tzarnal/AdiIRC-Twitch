using System.Collections.Generic;
using System.Text.RegularExpressions;
using AdiIRCAPIv2.Interfaces;

namespace Twitch___AdiIRC
{
    class TwitchRawEventHandlers
    {
        /*
         * Twitch includes irv3 tags in most of its messsages, wand we want to be able to use those        
         * Here is an example of such a tagset in a message
         * @badges=global_mod/1,turbo/1;color=#0D4200;display-name=dallas;emotes=25:0-4,12-16/1902:6-10;mod=0;room-id=1337;subscriber=0;turbo=1;user-id=1337;user-type=global_mod :ronni!ronni@ronni.tmi.twitch.tv PRIVMSG #dallas :Kappa Keepo Kappa
         */
        public static Dictionary<string, string> ParseTagsFromString(string rawMessage)
        {            
            var tags = new Dictionary<string,string>();

            //Grab the Tag section of the Message
            var tagsGroupRegex = @"^@(.+?) :.+ [A-Z]+";
            var tagsGroupMatch = Regex.Match(rawMessage, tagsGroupRegex);

            if (tagsGroupMatch.Success)
            {
                var tagsString = tagsGroupMatch.Groups[1].ToString();

                //Split into seperate key=value pairs
                var tagPairs = tagsString.Split(';');
                foreach (var tagPair in tagPairs)
                {
                    //Seperate key from value
                    var data = tagPair.Split('=');

                    //Twitch uses \s to indicate a space in a Tag Value
                    var tagContent = data[1].Replace(@"\s", " ");
                    tags.Add(data[0], tagContent);
                }
            }

            return tags;
        }

        /* 
         * CLEARCHAT is a message used by twitter to Timeout/Ban people, and clear their
         * Text lines, We won't clear the text but will display the ban information
         */
        public static bool ClearChat(IServer server, string rawMessage, bool showTimeOut, Dictionary<string, string> tags)
        {
            var clearChatRegex = @".+ :tmi.twitch.tv CLEARCHAT (#.+) :(.+)";
            var clearChatMatch = Regex.Match(rawMessage, clearChatRegex);

            if (clearChatMatch.Success)
            {
                if (!showTimeOut)
                {
                    //It is definitly a CLEARCHAT message but the settings say not to display it.
                    return true;
                }

                var time = "∞";
                var message = "No Reason Given";
                var channel = clearChatMatch.Groups[1].ToString();                
                var target = clearChatMatch.Groups[2].ToString();

                //Display Ban-Duration if there was any in the tags.
                if(tags.ContainsKey("ban-duration"))
                {
                    if (!string.IsNullOrWhiteSpace(tags["ban-duration"]))
                    {
                        time = tags["ban-duration"];
                    }
                }

                //Display Ban-Message if there was any in the tags.
                if (tags.ContainsKey("ban-reason"))
                {
                    if (!string.IsNullOrWhiteSpace(tags["ban-reason"]))
                    {
                        message = tags["ban-reason"];
                    }
                }

                //Construct and send a tranditional irc NOTICE Message
                var notice = $":Twitch!Twitch@tmi.twitch.tv NOTICE {channel} :{target} was banned: {message} [ {time} seconds ]";
                server.SendFakeRaw(notice);

                //We found a CLEARCHAT message and turned it into a NOTICE, return true to indicate we handled the message
                return true;
            }

            //We did not find a CLEARCHAT message, return false to indicate we did not handle the message
            return false;
        }
    }
}
