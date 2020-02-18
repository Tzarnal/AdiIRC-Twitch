Unsuported
=============
I've stopped using irc in general and as such I'm no longer supporting this plugin.

Twitch @ AdiRC
=============
This is a .net plugin for AdiIRC.

Twitch @ AdiIRC aims to add improvements for using twitch chat through AdiIRC. 

* Adds Twitch Emote support
* Some simple configuration options accesible through /twitch@ or the Commands menu
* Displays Cheers in chat
* Updates the channel topic with the game and status of the twitch channel periodicly
* Displays channels state changes such as slow mode, subscriber mode etc properly in channels
* Shows timeouts/bans as they happen. But does not clear chat.
* Shows Subscriptions, Resubscriptions, Prime Subscriptions, etc.
* Shows Twitch Badges such as Broadcaster, Moderator, Subscriber etc as part of irc Username
* Cleans up the server tab spam
* Supports twitch Whispers through normal Query / Private message interface

Usage
=============
Download and install properly, connect to twitch chat, thats it, it should be working. 

A very basic set of configuation options can be accessed through the '/twitch@' command.

Commands
=============
As noted on the twitch Irc Guide:

Most normal chat commands like /timeout, /ban, /clear are sent with periods in place of the forward slash. For example, to ban the user "xangold", you would send ".ban xangold" to the server (minus the quotes). Programs like mIRC will not work with unknown / commands (which is why you use a period.)

Download
=============
Download the Lastest Version [here](https://github.com/Xesyto/AdiIRC-Twitch/releases/latest)

Installation
=============

Extract the files from your download. Copy both .dll's to '%localappdata%\AdiIRC\Plugins'

Go into the Properties dialog ( rightclick, select "Properties") for both .dll files and if present click the "unblock" button. If you fail to do this you will likely encounter an error while trying to load the plugin that looks like [this]( http://i.imgur.com/x9ETiDD.jpg). Or read this [article](https://blogs.msdn.microsoft.com/drew/2009/12/23/xunit-and-td-net-fixing-the-attempt-was-made-to-load-an-assembly-from-a-network-location-problem/).

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

This plugins uses AdiIRC own emoticon system to show twitch emotes. As such they must actually be enabled. You can find the option Through File-Options Selecting the "Emoticons" entry in the list and ticking the "Use Emoticons" box. 
