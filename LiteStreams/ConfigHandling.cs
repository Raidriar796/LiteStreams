using ResoniteModLoader;
using FrooxEngine;
using Elements.Assets;

namespace LiteStreams;

public partial class LiteStreams : ResoniteMod
{
    private static void UpdateValueStreams()
    {
        // iterate over every world
        foreach (World world in Engine.Current.WorldManager.Worlds)
        {
            world.RunSynchronously(() =>
            {
                // iterate over every stream on the user
                foreach (FrooxEngine.Stream stream in world.LocalUser.Streams)
                {
                    // Only change the period of streams if they are implicit streams
                    if (stream is ImplicitStream implicitStream)
                    {
                        // Depending on if the mod was enabled or disabled, double or half the stream period
                        if (Config!.GetValue(Enable))
                        {
                            implicitStream.SetUpdatePeriod(implicitStream.Period * 2, implicitStream.Phase);
                        }
                        else
                        {
                            implicitStream.SetUpdatePeriod((uint)(implicitStream.Period * 0.5), implicitStream.Phase);
                        }
                    }
                    // Only change the bitrate of the streams if it's the user's voice stream
                    else if (stream is OpusStream<MonoSample> voiceStream)
                    {
                        // See if the stream is a mono opus stream, incase a stream is named
                        // voice when it's not the user's voice for whatever reason
                        if (stream.Name == "Voice")
                        {
                            // Depending on if the mod was enabled or disabled, set the voice bitrate to the original or the configured bitrate
                            if (Config!.GetValue(Enable))
                            {
                                voiceStream.BitRate.Value = (int)Config!.GetValue(voiceQuality);
                            }
                            else
                            {
                                voiceStream.BitRate.Value = (int)VoiceQuality.Default;
                            }
                        }
                    }
                    // Only change the minimum volume of the streams if they're stereo opus streams
                    else if (stream is OpusStream<StereoSample> audioStream)
                    {
                        // Depending on if the mod was enabled or disabled, set or unset a minimum volume
                        if (Config!.GetValue(Enable))
                        {
                            audioStream.MinimumVolume.Value = 0.005f;
                        }
                        else
                        {
                            audioStream.MinimumVolume.Value = 0f;
                        }
                    }
                }
            });
        }
    }

    private static void UpdateVoiceStream()
    {
        if (Config!.GetValue(Enable))
        {
            foreach (World world in Engine.Current.WorldManager.Worlds)
            {
                world.RunSynchronously(() =>
                {
                    // Runs on every stream for the local user
                    foreach (FrooxEngine.Stream stream in world.LocalUser.Streams)
                    {
                        // Check for the user's voice audio stream
                        if (stream.Name == "Voice")
                        {
                            // See if the stream is a mono opus stream, incase a stream is named
                            // voice when it's not the user's voice for whatever reason
                            if (stream is OpusStream<MonoSample> voiceStream)
                            {
                                // Reduces the bitrate, default value is 25000
                                voiceStream.BitRate.Value = (int)Config!.GetValue(voiceQuality);
                                // Break the loop early, as there is no need to search anymore
                                break;
                            }
                        }
                    }
                });
            }
        }
    }
}
