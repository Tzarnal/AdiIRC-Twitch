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
         * NOTICE is a normal irc message but due to how twitch sends them 
         * they don't arrive in the channel windows, but in the server.
         */
        public static bool Notice(IServer server, string rawMessage)
        {
            //Check if its a usable notice message
            var noticeRegex = @".+ :tmi.twitch.tv NOTICE (#.+) :(.+)";
            var noticeMatch = Regex.Match(rawMessage, noticeRegex);

            if (noticeMatch.Success)
            {
                var channel = noticeMatch.Groups[1].ToString();
                var message = noticeMatch.Groups[2].ToString();

                //Send a fake regular irc notice
                var notice = $":Twitch!Twitch@tmi.twitch.tv NOTICE {channel} :{message}";
                server.SendFakeRaw(notice);

                //Handled the NOTICE message.
                return true;
            }

            //Not recognized as a NOTICE message, didn't handle it.
            return false;
        }

        /* 
         * CLEARCHAT is a message used by twitch to Timeout/Ban people, and clear their
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

        /* 
         * USERNOTICE is a message used by twitch to by twitch to inform about (Re-)Subscriptions 
         * It can take two forms with or without a user message attached. 
         */
        public static bool Usernotice(IServer server, string rawMessage, bool showSubs, Dictionary<string, string> tags)
        {
            var subRegexMessage = @".*USERNOTICE (#\w+)\s*(:.+)?";                    
            var subMessageMatch = Regex.Match(rawMessage, subRegexMessage);

            if (subMessageMatch.Success)
            {
                if (!showSubs)
                {
                    //It is definitly a USERNOTICE message but the settings say not to display it.
                    return true;
                }
                
                //Grab the channel part of the message, its always included.
                var channel = subMessageMatch.Groups[1].ToString();
                var userMessage = "";

                //Check for the usermessage section, it has an included : as the first charcter so that needs to be removed.
                if (subMessageMatch.Groups.Count >= 3 && !string.IsNullOrWhiteSpace(subMessageMatch.Groups[2].ToString()) )
                {                   
                    userMessage = $" [ { subMessageMatch.Groups[2].ToString().TrimStart(':')} ]";
                }

                //Construct the notice, twitch includes a detailed message for us about the nature of the subscription in the tags
                var notice = $":Twitch!Twitch@tmi.twitch.tv NOTICE {channel} :{tags["system-msg"]}{userMessage}";

                server.SendFakeRaw(notice);
                return true;
            }

            //We did not find a USERNOTICE message, return false to indicate we did not handle the message
            return false;
        }

        //WHISPER is a message used by twitch to handle private messsages between users ( and bots )
        //But its not a normal IRC message type, so they have to be rewritten into PRIVMSG's
        public static bool WhisperReceived(IServer server, string rawMessage)
        {                            
            var whisperRegex = @".*(\x3A[^!@ ]+![^@ ]+@\S+) WHISPER (\S+) (\x3A.*)";
            var whisperMatch = Regex.Match(rawMessage, whisperRegex);
            
            if (whisperMatch.Success)
            {
                var sender = whisperMatch.Groups[1];
                var target = whisperMatch.Groups[2];
                var message = whisperMatch.Groups[3];

                //Construct and send a proper PRIVSMG instead of the WHISPER
                var privmsg = $"{sender} PRIVMSG {target} {message}";

                server.SendFakeRaw(privmsg);                
                return true;
            }

            //We did not find a WHISPER message, return false to indicate we did not handle the message
            return false;
        }
    }
}
