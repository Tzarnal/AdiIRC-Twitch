Twitch @ AdiRC
=============
This is a .net plugin for AdiIRC.

Twitch @ AdiIRC aims to add improvements for using twitch chat through AdiIRC. 

* Adds Twitch Emote support
* Some simple configuration options accesible through /twitch@
* Displays Cheers in chat
* Updates the channel topic with the game and status of the twitch channel periodicly
* Displays channels state changes such as slow mode, subscriber mode etc properly in channels
* Shows timeouts/bans as they happen. But does not clear chat.
* Shows Subscriptions, Resubscriptions, Prime Subscriptions, etc.
* Cleans up the server tab spam

Usage
=============
Download and install properly, connect to twitch chat, thats it, it should be working. 
A very basic set of configuation options can be accessed through the '/twitch@' command.

Download
=============
Download the Lastest Version [here](https://github.com/Xesyto/AdiIRC-Twitch/releases/latest)

Installation
=============

Extract the files from your download. Copy both .dll's to '%localappdata%\AdiIRC\Plugins'

Make sure you have twitch chat setup as a server in AdiIRC, twitch has a general IRC Guide [here](https://help.twitch.tv/customer/portal/articles/1302780-twitch-irc)

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
