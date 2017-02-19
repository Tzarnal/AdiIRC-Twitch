using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
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
        
        private string _emoteDirectory =  Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\AdiIRC\TwitchEmotes";        
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
                Host.SendCommand(_twitchServer, ".setoption", $"Emoticons Emoticon_{emote.Name} {emoteFile}");

                _handledEmotes.Add(emote.Name);
                return;
            }

            if (DownloadEmote(emote))
            {
                //Actually register the emote with AdiIRC
                Host.SendCommand(_twitchServer, ".setoption", $"Emoticons Emoticon_{emote.Name} {emoteFile}");

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
            catch (Exception e)
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
