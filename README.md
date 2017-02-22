Twitch @ AdiRC
=============
This is a .net plugin for AdiIRC.

Twitch @ AdiIRC aims to add improvements for using twitch chat through AdiIRC. 

* Adds Twitch Emote support
* Displays Cheers in chat
* Updates the channel topic with the game and status of the twitch channel periodicly
* Displays channels state changes such as slow mode, subscriber mode etc properly in channels
* Shows timeouts/bans as they happen. But does not clear chat.
* Cleans up the server tab spam

Installation
=============

Extract the files from your download. Copy both .dll's to '%localappdata%\AdiIRC\Plugins'

In order for this plugin to work properly you have to add the following to the Commands (Server->Server List->Commands) tab of your twitch server in the Server List.

```
CAP REQ :twitch.tv/tags
CAP REQ :twitch.tv/commands
```
And make sure "Run these commands on connect" is selected. Additionally if you wish the userlist to populate and to get messages for people joining and leaving add 

```
CAP REQ :twitch.tv/membership
```

Finally go to File->Plugins and select Twitch @ AdiIrc. Click Load. It should now say Loaded in Status and be working.