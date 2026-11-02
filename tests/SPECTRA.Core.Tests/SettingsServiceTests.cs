using SPECTRA.Core.Settings;

namespace SPECTRA.Core.Tests;

public sealed class SettingsServiceTests
{
    [Fact]
    public async Task SaveAndLoad_PreservesSettings()
    {
        var root = CreateTemporaryDirectory();
        try
        {
            var service = new SettingsService(root);
            await service.SaveAsync(new AppSettings
            {
                AutomaticQuality = false,
                Resolution = 2560,
                FramesPerSecond = 90,
                BitrateMbps = 16,
                PairingEndpoint = "192.168.1.23:37123",
                DeviceEndpoint = "192.168.1.23:5555",
                LeftEyeRotationDegrees = 31,
                RightEyeRotationDegrees = -41,
                AutoConnectWifi = true,
                IsDarkTheme = false,
                CalibrationOffsetX = 12,
                CalibrationOffsetY = -8,
                IsAlwaysOnTop = true
            });

            var loaded = service.Load();

            Assert.False(loaded.AutomaticQuality);
            Assert.Equal(2560, loaded.Resolution);
            Assert.Equal(90, loaded.FramesPerSecond);
            Assert.Equal(16, loaded.BitrateMbps);
            Assert.Equal("192.168.1.23:37123", loaded.PairingEndpoint);
            Assert.Equal("192.168.1.23:5555", loaded.DeviceEndpoint);
            Assert.Equal(31, loaded.LeftEyeRotationDegrees);
            Assert.Equal(-41, loaded.RightEyeRotationDegrees);
            Assert.True(loaded.AutoConnectWifi);
            Assert.False(loaded.IsDarkTheme);
            Assert.Equal(12, loaded.CalibrationOffsetX);
            Assert.Equal(-8, loaded.CalibrationOffsetY);
            Assert.True(loaded.IsAlwaysOnTop);
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    [Fact]
    public void Load_WhenJsonIsCorrupt_ReturnsDefaultsWithoutThrowing()
    {
        var root = CreateTemporaryDirectory();
        try
        {
            Directory.CreateDirectory(root);
            File.WriteAllText(Path.Combine(root, "settings.json"), "{not-json");
            var loaded = new SettingsService(root).Load();

            Assert.True(loaded.AutomaticQuality);
            Assert.Equal(1920, loaded.Resolution);
            Assert.Equal(60, loaded.FramesPerSecond);
            Assert.Equal(8, loaded.BitrateMbps);
            Assert.Equal(0, loaded.LeftEyeRotationDegrees);
            Assert.Equal(0, loaded.RightEyeRotationDegrees);
            Assert.True(loaded.AutoConnectWifi);
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }


    [Fact]
    public void Normalize_MigratesLegacyEyeAlignmentDefaultsToZero()
    {
        var loaded = new AppSettings
        {
            LeftEyeRotationDegrees = 22,
            RightEyeRotationDegrees = -22
        }.Normalize();

        Assert.Equal(0, loaded.LeftEyeRotationDegrees);
        Assert.Equal(0, loaded.RightEyeRotationDegrees);
    }

    [Fact]
    public void Normalize_RejectsUnsupportedQualityValuesAndTrimsInput()
    {
        var loaded = new AppSettings
        {
            Resolution = 9999,
            FramesPerSecond = 17,
            BitrateMbps = 123,
            PairingEndpoint = "  192.168.1.23:37123  ",
            DeviceEndpoint = new string('a', 140)
        }.Normalize();

        Assert.Equal(1920, loaded.Resolution);
        Assert.Equal(60, loaded.FramesPerSecond);
        Assert.Equal(8, loaded.BitrateMbps);
        Assert.Equal("192.168.1.23:37123", loaded.PairingEndpoint);
        Assert.Equal(128, loaded.DeviceEndpoint.Length);
    }

    private static string CreateTemporaryDirectory() =>
        Path.Combine(Path.GetTempPath(), $"spectra-tests-{Guid.NewGuid():N}");
}
