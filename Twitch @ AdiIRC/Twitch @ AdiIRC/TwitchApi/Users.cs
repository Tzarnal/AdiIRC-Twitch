using System.Collections.Generic;

namespace Twitch___AdiIRC.TwitchApi
{
    class TwitchUsers
    {
        public int _total;
        public List<TwitchUser> users;
    }

    class TwitchUser
    {
        public string _id;
        public string bio;
        public string created_at;
        public string display_name;
        public string logo;
        public string name;
        public string type;
        public string updated_at;
    }
}
