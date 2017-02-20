Twitch @ AdiRC
=============
This is a .net plugin for AdiIRC.

Twitch @ AdiIRC aims to add improvements for using twitch chat through AdiIRC. 

* Adds Twitch Emote support
* Displays Cheers in chat
* Displays channels state changes such as slow mode, subscriber mode etc properly in channels
* Shows timeouts/bans as they happen. But does not clear chat.
* Cleans up the server tab spam



In order for this plugin to work properly you have to add the following to the Commands tab of your twitch server in the Server List.

```
CAP REQ :twitch.tv/tags
CAP REQ :twitch.tv/commands
```

And make sure "Run these commands on connect" is selected.