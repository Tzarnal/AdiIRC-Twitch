namespace TwitchAdiIRC
{
    public class TwitchEmote
    {
        public int Id;
        public string Name;
        public string URL => $"https://static-cdn.jtvnw.net/emoticons/v1/{Id}/1.0";
    }
}
