using System;
using System.IO;
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

        private IPluginHost _myHost;
        private ITools _myTools;
        public IPluginHost Host
        {
            get { return _myHost; }
            set { _myHost = value; }
        }

        public ITools Tools
        {
            get { return _myTools; }
            set { _myTools = value; }
        }
        
        public void Initialize()
        {
            //Register Delegates. 
            _myHost.OnGetData += MyHostOnGetData;
            _myHost.OnRawData += MyHostOnOnRawData;
        }

        private void MyHostOnOnRawData(object sender, RawDataArgs rawDataArgs)
        {
            throw new NotImplementedException();
        }

        private void MyHostOnGetData(IServer Server, string Data, out EatData Return)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            //Nothing to handle on closing    
        }        
    }
}
