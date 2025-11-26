# LiteStreams

A [ResoniteModLoader](https://github.com/resonite-modding-group/ResoniteModLoader) mod for [Resonite](https://resonite.com/) that reduces outbound network traffic.

This is primarily beneficial for users with poor upload speed, metered internet, and/or users that achieve exceedingly high framerates (144+ fps).

## How it works

The majority of outbound traffic is caused by value streams and audio streams, the following tweaks are made when you join sessions:

### Value Streams

Value streams handle sending information like your visemes and tracking points. Each of these value streams contains a `Period` value, which determines how many engine updates to wait between sending updates.

The higher your frame rate, the more often your streams will update.

When you join a session, LiteStreams will double the period of every stream, which cuts the update frequency in half.

When the period is 0, it is left as is because streams with a period of 0 only update as needed instead of being updated at a constant rate.

### Voice Streams

Your voice stream doesn't send much traffic as is, but LiteStreams reduces the bitrate slightly by default to further reduce outbound traffic.

You can set it back to the default bitrate with the LiteStreams config, or you can reduce the bitrate even further.

Generally, the lower the quality your mic is, the lower bitrate you can get away with.

### Audio Streams

Audio streams by default will constantly send outbound traffic, even when the input audio is completely silent.

LiteStreams sets a minimum volume, which requires that a certain volume threshold must be met before sending packets.

This effectively stops outbound traffic when the stream is silent.

### How much does this help?

LiteStreams reduces your outbound traffic by about a third, not including audio streams.

LiteStreams has a greater effect for hosts as the host has to send your traffic to everyone else in the session.

## Requirements
- [ResoniteModLoader](https://github.com/resonite-modding-group/ResoniteModLoader)

## Installation
1. Install [ResoniteModLoader](https://github.com/resonite-modding-group/ResoniteModLoader).
2. Place [LiteStreams.dll](https://github.com/Raidriar796/LiteStreams/releases/latest/download/LiteStreams.dll) into your `rml_mods` folder. This folder should be at `C:\Program Files (x86)\Steam\steamapps\common\Resonite\rml_mods` for a default install. You can create it if it's missing, or if you launch the game once with ResoniteModLoader installed it will create the folder for you.
3. Start the game. If you want to verify that the mod is working you can check your Resonite logs. 
