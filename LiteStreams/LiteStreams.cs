using ResoniteModLoader;
using FrooxEngine;

namespace LiteStreams;

public class LiteStreams : ResoniteMod
{
    public override string Name => "LiteStreams";
    public override string Author => "Raidriar796";
    public override string Version => "1.0.0";
    public override string Link => "https://github.com/Raidriar796/LiteStreams";
    public static ModConfiguration? Config;

    public override void OnEngineInit()
    {
        Config = GetConfiguration();
        Config?.Save(true);

        // Subscribe setup and cleanup methods
        Engine.Current.WorldManager.WorldAdded += RegisterWorlds;
        Engine.Current.WorldManager.WorldRemoved += CleanDictionary;
    }

    // For tracking each world and once you've focused in for the first time
    private static Dictionary<World, bool> firstFocusList = new();

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
                // Runs on every stream for the local user
                foreach (FrooxEngine.Stream stream in world.LocalUser.Streams)
                {
                    // Only change streams that are implicit so the period can be updated
                    if (stream is ImplicitStream implicitStream)
                    {
                        // Cuts the frequency of updates for value streams in half
                        implicitStream.SetUpdatePeriod(implicitStream.Period * 2, implicitStream.Phase);
                    }
                }

                // Update any new streams that are added after the first focus
                world.LocalUser.StreamAdded += UpdateNewStreams;
            });
        }
    }

    private static void UpdateNewStreams(FrooxEngine.Stream stream)
    {
        stream.World.RunSynchronously(() =>
        {
            if (stream is ImplicitStream implicitStream)
            {
                implicitStream.SetUpdatePeriod(implicitStream.Period * 2, implicitStream.Phase);
            }
        });
    } 

    private static void CleanDictionary(World world)
    {
        firstFocusList.Remove(world);
    }
}
