using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using AdiIRCAPI;

namespace TwitchAdiIRC
{
    public class TwitchAdiIrc : IPlugin
    {
        public string Description => "Provides simple additional features like emotes for twitch chat integration.";
        public string Author => "Xesyto";
        public string Name => "Twitch @ AdiIRC";
        public string Version => "1";
        public string Email => "";

        public IPluginHost Host { get; set; }
        public ITools Tools { get; set; }
        
        private readonly string _emoteDirectory =  Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\AdiIRC\TwitchEmotes";        
        private List<string> _handledEmotes;
        private IServer _twitchServer;

        public void Initialize()
        {
            //Register Delegates. 
            Host.OnRawData += MyHostOnOnRawData;
            _handledEmotes = new List<string>();

            if (!Directory.Exists(_emoteDirectory))
            {
                Directory.CreateDirectory(_emoteDirectory);
            }
        }

        private void MyHostOnOnRawData(object sender, RawDataArgs rawDataArgs)
        {
            var dataString = System.Text.Encoding.UTF8.GetString(rawDataArgs.Bytes);

            if (!dataString.Contains("twitch.tv") || string.IsNullOrEmpty(dataString))
                return; //Only work on twitch.tv messages, early exit as soon as possible.

            //We'll need a window to send a command too later so we are grabbing the twitch server here
            if (_twitchServer == null)
            {
                foreach (var server in ((IEnumerable<IServer>)Host.GetServers))
                {
                    if (server.Network.Contains("twitch"))
                    {
                        _twitchServer = server;
                    }
                }
                //If that did not work grab the first.               
                if (_twitchServer == null)
                    _twitchServer = ((IEnumerable<IServer>) Host.GetServers).First();
            }

            //Process Message, looking for emotes.
            if (dataString.Contains("PRIVMSG ")) 
            {
                TwitchIrcMessage twitchMessage;

                try
                {
                    twitchMessage = new TwitchIrcMessage(dataString);
                }
                catch (Exception)
                {
                    return;
                }

                if (twitchMessage.HasEmotes)
                {                    
                    RegisterEmotes(twitchMessage.Emotes);
                }

                return;
            }

            //Pricess Message, redirect notices to proper channel tab
            if (dataString.Contains("NOTICE "))
            {
                //Check if its a usable notiec message
                var noticeRegex = @".+ :tmi.twitch.tv NOTICE (#.+) :(.+)";
                var noticeMatch = Regex.Match(dataString, noticeRegex);

                if (noticeMatch.Success)
                {
                    var channel = noticeMatch.Groups[1].ToString();
                    var message = noticeMatch.Groups[2].ToString();

                    //Send a fake regular irc notice instead
                    
                    var notice = $":Twitch!Twitch@Twitch.tv NOTICE {channel} :{message}";
                    _twitchServer.SendFakeRaw(notice);

                    //Eat message.
                    rawDataArgs.Bytes = null;
                    return;
                }
            }

            if (dataString.Contains("CLEARCHAT ") )
            {
                var clearChatRegex = @"@ban-duration=(.*?);ban-reason=(.*?);.+ :tmi.twitch.tv CLEARCHAT (#.+) :(.+)";
                var clearChatMatch = Regex.Match(dataString, clearChatRegex);

                if (clearChatMatch.Success)
                {
                    var channel = clearChatMatch.Groups[3].ToString();
                    var message = clearChatMatch.Groups[2].ToString().Replace("\\s"," ");
                    var time = clearChatMatch.Groups[1].ToString();
                    var target = clearChatMatch.Groups[4].ToString();

                    if (string.IsNullOrEmpty(time))
                        time = "∞";

                    if (string.IsNullOrEmpty(message))
                        message = "No Reason Given";

                    var notice = $":Twitch!Twitch@Twitch.tv NOTICE {channel} :{target} was banned: {message} [{time} seconds]";
                    _twitchServer.SendFakeRaw(notice);

                    //Eat message.
                    rawDataArgs.Bytes = null;
                    return;
                }                
            }

            if (dataString.Contains("ROOMSTATE ") || dataString.Contains("USERSTATE ") || dataString.Contains("USERNOTICE "))
            {
                //Silently eat these messages and do nothing. They only cause empty * lines to appear in the server tab and Twitch@AdiIRC does not use them
                rawDataArgs.Bytes = null;
            }                                    
        }

        public void RegisterEmote(TwitchEmote emote)
        {
            if (_handledEmotes.Contains(emote.Name))
                return;

            var emoteFile = $"{_emoteDirectory}\\{emote.Id}.png";

            if (File.Exists(emoteFile))
            {
                
                //Actually register the emote with AdiIRC
                var command = $"Emoticons Emoticon_{emote.Name} {emoteFile}";
                Host.SendCommand(_twitchServer, ".setoption", command);

                _handledEmotes.Add(emote.Name);
                return;
            }

            if (DownloadEmote(emote))
            {
                //Actually register the emote with AdiIRC
                var command = $"Emoticons Emoticon_{emote.Name} {emoteFile}";
                Host.SendCommand(_twitchServer, ".setoption", command);                

                _handledEmotes.Add(emote.Name);
            }
        }

        public void RegisterEmotes(IEnumerable<TwitchEmote> emotes)
        {
            foreach (var emote in emotes)
            {
                RegisterEmote(emote);
            }
        }

        private bool DownloadEmote(TwitchEmote emote)
        {
            var filePath = $"{_emoteDirectory}\\{emote.Id}.png";

            try
            {
                WebClient wc = new WebClient();
                wc.DownloadFile(emote.URL, filePath);
            }
            catch (Exception)
            {                
                return false;
            }

            return true;
        }

        public void Dispose()
        {        
        }        
    }
}
