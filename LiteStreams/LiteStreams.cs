using ResoniteModLoader;
using FrooxEngine;

namespace LiteStreams;

public partial class LiteStreams : ResoniteMod
{
    public override string Name => "LiteStreams";
    public override string Author => "Raidriar796";
    public override string Version => "1.3.0";
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
        enable.OnChanged += (value) => UpdateValueStreams();
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
}
