using System;
using System.IO;
using AdiIRCAPIv2.Arguments.Aliasing;
using AdiIRCAPIv2.Arguments.Channel;
using Timer = System.Threading.Timer;
using AdiIRCAPIv2.Interfaces;
using Twitch___AdiIRC.TwitchApi;

namespace Twitch___AdiIRC
{
    //Inerit form IPlugin to be an AdiIRC plugin.
    public class TwitchAdiIrc : IPlugin
    {
        //Mandatory information fields.
        public string PluginDescription => "Provides simple additional features like emotes for twitch chat integration.";
        public string PluginAuthor => "Xesyto";
        public string PluginName => "Twitch @ AdiIRC";
        public string PluginVersion => "6";
        public string PluginEmail => "s.oudenaarden@gmail.com";

        private IPluginHost _host;
        
        private Timer _topicTimer;
                
        private Settings _settings;        
        private SettingsForm _settingsForm;

        public void Initialize(IPluginHost host)
        {
            //Store the host in a private field, we want to be able to access it later
            _host = host;

            //Fetch the Config folder and attach the correct path to 
            //Twitch @AdiIRC's config file
            var settingsPath = _host.ConfigFolder + @"\Plugins\TwitchConfig\Config.json";

            //Either Load an existing config file or create a new one with default values.
            if (File.Exists(settingsPath))
            {
                _settings = Settings.Load(settingsPath);
            }
            else
            {
                _settings = new Settings {Path = settingsPath};
                _settings.Save();
            }

            //Create the settings form
            _settingsForm = new SettingsForm(_settings);

            //Register a command to show the settings form
            _host.HookCommand("/twitch@");

            //Register Delegates
            _host.OnChannelJoin += OnChannelJoin;
            _host.OnRegisteredCommand += OnCommand;

            //Start a timer to update all channel topics regularly
            _topicTimer = new Timer(state => TopicUpdate(), true, TimeSpan.FromMinutes(15), TimeSpan.FromMinutes(10));
        }

        private void OnCommand(RegisteredCommandArgs argument)
        {
            _settingsForm.Show();
        }


        private void OnChannelJoin(ChannelJoinArgs argument)
        {
            //Check if this event was fired on twitch, if not this plugin should 
            //never touch it so fires an early return.
            if (!IsTwitchServer(argument.Server))
            {
                return;
            }
                            
            var server = argument.Server;
            var channelName = argument.Channel.Name;
            var userName = argument.Channel.Name.TrimStart('#');
            string topicData;

            //Check if this event fired on the client joining the channel or 
            //someone else joining, we only need to set the topic of a channel
            //when we join a channel.
            if (argument.User.Nick != argument.Server.Nick)
            {
                return;
            }

            //TwitchApiTools connects to the web, disk or web IO is unreliable 
            //so handle it in a try / catch block
            try
            {                
                topicData = TwitchApiTools.GetSimpleChannelInformationByName(userName);
                
            }
            catch (Exception)
            {
                topicData = $"Twitch@AdiIRC: Could not find channel topic data for {userName}.";
            }

            //Finally set the topic title through a raw IRC message.
            var topicMessage = $":Twitch!Twitch@Twitch.tv TOPIC {channelName} :{topicData}";
            server.SendFakeRaw(topicMessage);
        }

        private void TopicUpdate()
        {
            //Find any twitch server connections in the serverlist. there might be
            //more than one and there might be none so storing it statically is impractical
            foreach (IServer server in _host.GetServers)
            {
                if (IsTwitchServer(server))
                {
                    var channels = server.GetChannels;

                    //Iterate over all channels, updating the topic.
                    foreach (IChannel channel in channels)
                    {
                        string topicData;
                        var userName = channel.Name.TrimStart('#');

                        //TwitchApiTools connects to the web, disk or web IO is unreliable 
                        //so handle it in a try / catch block
                        try
                        {                           
                            topicData = TwitchApiTools.GetSimpleChannelInformationByName(userName);
                        }
                        catch (Exception)
                        {
                            topicData = $"Twitch@AdiIRC: Could not find channel topic data for {userName}.";                            
                        }

                        //AdiIRC will let you set a topic to the same thing, this avoids repetitive topic updates. 
                        if (channel.Topic != topicData)
                        {
                            var topicMessage = $":Twitch!Twitch@Twitch.tv TOPIC #{userName} :{topicData}";
                            server.SendFakeRaw(topicMessage);
                        }
                    }
                }
            }
        }


        private bool IsTwitchServer(IServer server)
        {
            return server != null &&
                   server.Network.ToLower().Contains("twitch") 
                   && server.NetworkLabel.ToLower().Contains("twitch");
        }

        public void Dispose()
        {
            _settings.Save();        
        }        
    }
}
