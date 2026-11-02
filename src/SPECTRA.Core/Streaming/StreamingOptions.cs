using SPECTRA.Core.Settings;

namespace SPECTRA.Core.Streaming;

public sealed record StreamingOptions(
    int MaxSize,
    int FramesPerSecond,
    int BitrateMbps,
    bool IsAutomatic,
    string VideoEye = "Left",
    int RotationDegrees = 0)
{
    public static StreamingOptions FromSettings(AppSettings settings)
    {
        ArgumentNullException.ThrowIfNull(settings);
        var safe = new AppSettings
        {
            AutomaticQuality = settings.AutomaticQuality,
            Resolution = settings.Resolution,
            FramesPerSecond = settings.FramesPerSecond,
            BitrateMbps = settings.BitrateMbps,
            VideoEye = settings.VideoEye,
            RotationDegrees = settings.RotationDegrees,
            LeftEyeRotationDegrees = settings.LeftEyeRotationDegrees,
            RightEyeRotationDegrees = settings.RightEyeRotationDegrees,
            AutoConnectWifi = settings.AutoConnectWifi
        }.Normalize();

        var rotation = safe.VideoEye == "Right" ? safe.RightEyeRotationDegrees : safe.LeftEyeRotationDegrees;
        return safe.AutomaticQuality
            ? new StreamingOptions(1920, 60, 8, IsAutomatic: true, VideoEye: safe.VideoEye, RotationDegrees: rotation)
            : new StreamingOptions(safe.Resolution, safe.FramesPerSecond, safe.BitrateMbps,
                IsAutomatic: false, VideoEye: safe.VideoEye, RotationDegrees: rotation);
    }
}
