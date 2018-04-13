using System;
using System.Net;

namespace Twitch___AdiIRC
{
    public class TwitchEmote
    {
        public int Id;
        public string Name;
        public string URL => $"http://static-cdn.jtvnw.net/emoticons/v1/{Id}/1.0";

        public bool DownloadEmote(string filepath)
        {
            
            try
            {
                var wc = new WebClient();
                wc.DownloadFile(URL, filepath);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }
    }
}
