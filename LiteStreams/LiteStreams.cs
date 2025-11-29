using ResoniteModLoader;
using FrooxEngine;
using Elements.Assets;

namespace LiteStreams;

public partial class LiteStreams : ResoniteMod
{
    public override string Name => "LiteStreams";
    public override string Author => "Raidriar796";
    public override string Version => "1.1.0";
    public override string Link => "https://github.com/Raidriar796/LiteStreams";
    private static ModConfiguration? Config;

    public override void OnEngineInit()
    {
        Config = GetConfiguration();
        Config?.Save(true);

        // Subscribe setup and cleanup methods
        Engine.Current.WorldManager.WorldAdded += RegisterWorlds;
        Engine.Current.WorldManager.WorldRemoved += CleanDictionary;

        // Update streams when config values change
        Enable.OnChanged += (value) => UpdateValueStreams();
        voiceQuality.OnChanged += (value) => UpdateVoiceStream();
    }

    // Predefined bitrates for simplified config options
    public enum VoiceQuality
    {
        Bad = 10000,
        Low = 15000,
        Reduced = 20000,
        Default = 25000
    }

    // For tracking each world and once you've focused in for the first time
    private static readonly Dictionary<World, bool> firstFocusList = [];

    // Assigns worlds to the dictionary and subscribes logic for when you initially focus
    private static void RegisterWorlds(World world)
    {
        firstFocusList.Add(world, new());
        firstFocusList[world] = false;
        world.WorldManager.WorldFocused += OnFirstFocus;
    }

    private static void OnFirstFocus(World world)
    {
        // Check if the world has already been focused at least once
        if (!firstFocusList[world])
        {
            firstFocusList[world] = true;

            world.RunSynchronously(() =>
            {
                if (Config!.GetValue(Enable))
                {
                    // Runs on every stream for the local user
                    foreach (FrooxEngine.Stream stream in world.LocalUser.Streams)
                    {
                        // Only change the period of streams if they are implicit streams
                        if (stream is ImplicitStream implicitStream)
                        {
                            // Cuts the frequency of updates for value streams in half
                            implicitStream.SetUpdatePeriod(implicitStream.Period * 2, implicitStream.Phase);
                        }
                        // Check for the user's voice audio stream
                        else if (stream.Name == "Voice")
                        {
                            // See if the stream is a mono opus stream, incase a stream is named
                            // voice when it's not the user's voice for whatever reason
                            if (stream is OpusStream<MonoSample> voiceStream)
                            {
                                // Reduces the bitrate, default value is 25000
                                voiceStream.BitRate.Value = (int)Config!.GetValue(voiceQuality);
                            }
                        }
                    }
                }

                // Update any new streams that are added after the first focus
                world.LocalUser.StreamAdded += UpdateNewStreams;
            });
        }
    }

    private static void UpdateNewStreams(FrooxEngine.Stream stream)
    {
        if (Config!.GetValue(Enable))
        {
            stream.World.RunSynchronously(() =>
            {
                // Only change the period of streams if they are implicit streams
                if (stream is ImplicitStream implicitStream)
                {
                    // Cuts the frequency of updates for value streams in half
                    implicitStream.SetUpdatePeriod(implicitStream.Period * 2, implicitStream.Phase);
                }
                // Checks if the stream is a valid stereo opus stream
                else if (stream is OpusStream<StereoSample> audioStream)
                {
                    // Set the minimum volume to prevent packets being sent
                    // when the stream is not playing any audio
                    audioStream.MinimumVolume.Value = 0.005f;
                }
            });
        }
    }

    private static void CleanDictionary(World world)
    {
        firstFocusList.Remove(world);
    }
}
