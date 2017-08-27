using System;
using System.Net;


namespace Twitch___AdiIRC
{
    class TwitchBit
    {
        public string Amount;
        public string Name => $"cheer{Amount}";       
        public string URL => $"https://static-cdn.jtvnw.net/bits/light/static/{Color}/1";

        public string Color
        {
            get {

                var color = "gray";
                try
                {
                    var ibitCount = int.Parse(Amount);
                    if (ibitCount > 10000)
                    {
                        color = "red";
                    }
                    else if (ibitCount > 5000)
                    {
                        color = "blue";
                    }
                    else if (ibitCount > 1000)
                    {
                        color = "green";
                    }
                    else if (ibitCount > 100)
                    {
                        color = "purple";
                    }
                }
                catch (Exception)
                {
                    color = "gray";
                }

                return color;
            }
        }

        public bool DownloadBit(string filepath)
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
