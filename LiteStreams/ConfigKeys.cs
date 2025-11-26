using ResoniteModLoader;

namespace LiteStreams;

public partial class LiteStreams : ResoniteMod
{
    [AutoRegisterConfigKey]
    public static readonly ModConfigurationKey<bool> Enable =
        new("Enable", "Enable LiteStreams", () => true);
    
    [AutoRegisterConfigKey]
    public static readonly ModConfigurationKey<VoiceQuality> voiceQuality =
        new("voiceQuality", "Quality of your voice stream", () => VoiceQuality.Reduced);
}
