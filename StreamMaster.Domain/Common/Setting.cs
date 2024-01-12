﻿using StreamMaster.Domain.Attributes;
using StreamMaster.SchedulesDirect.Domain.Models;

namespace StreamMaster.Domain.Common;

public class M3USettings
{
    public bool M3UFieldChannelId { get; set; } = true;
    public bool M3UFieldChannelNumber { get; set; } = true;
    public bool M3UFieldCUID { get; set; } = true;
    public bool M3UFieldGroupTitle { get; set; } = true;
    public bool M3UFieldTvgChno { get; set; } = true;
    public bool M3UFieldTvgId { get; set; } = true;
    public bool M3UFieldTvgLogo { get; set; } = true;
    public bool M3UFieldTvgName { get; set; } = true;
    public bool M3UIgnoreEmptyEPGID { get; set; } = true;
    public bool M3UUseChnoForId { get; set; } = true;
    public bool M3UStationId { get; set; } = false;
}

public class SDSettings
{
    public bool SeriesPosterArt { get; set; }
    public bool SeriesWsArt { get; set; }
    public string SeriesPosterAspect { get; set; } = "4x3";
    public string ArtworkSize { get; set; } = "Md";
    public bool ExcludeCastAndCrew { get; set; } = false;
    public bool AlternateSEFormat { get; set; }
    public bool PrefixEpisodeDescription { get; set; } = true;
    public bool PrefixEpisodeTitle { get; set; } = true;
    public bool AppendEpisodeDesc { get; set; } = true;
    public int SDEPGDays { get; set; } = 7;
    public bool SDEnabled { get; set; }
    public string SDUserName { get; set; } = string.Empty;
    public string SDCountry { get; set; } = "USA";
    public string SDPassword { get; set; } = string.Empty;
    public string SDPostalCode { get; set; } = string.Empty;

    public string PreferredLogoStyle { get; set; } = "DARK";
    public string AlternateLogoStyle { get; set; } = "WHITE";


    public List<StationIdLineup> SDStationIds { get; set; } = [];
    public bool SeasonEventImages { get; set; } = true;
    public bool XmltvAddFillerData { get; set; } = true;
    //public string XmltvFillerProgramDescription { get; set; } = "This program was generated by Stream Master to provide filler data for stations that did not receive any guide listings from the upstream source.";
    public int XmltvFillerProgramLength { get; set; } = 4;
    public bool XmltvIncludeChannelNumbers { get; set; } = false;
    public bool XmltvExtendedInfoInTitleDescriptions { get; set; }
    public bool XmltvSingleImage { get; set; }
}

public class SDSettingsRequest
{
    public string? PreferredLogoStyle { get; set; }
    public string? AlternateLogoStyle { get; set; }
    public bool? SeriesPosterArt { get; set; }
    public bool? SeriesWsArt { get; set; }
    public string? SeriesPosterAspect { get; set; }
    public string? ArtworkSize { get; set; }
    public bool? ExcludeCastAndCrew { get; set; }
    public bool? AlternateSEFormat { get; set; }
    public bool? PrefixEpisodeDescription { get; set; }
    public bool? PrefixEpisodeTitle { get; set; }
    public bool? AppendEpisodeDesc { get; set; }
    public int? SDEPGDays { get; set; }
    public bool? SDEnabled { get; set; }
    public string? SDUserName { get; set; }
    public string? SDCountry { get; set; }
    public string? SDPassword { get; set; }
    public string? SDPostalCode { get; set; }

    public List<StationIdLineup>? SDStationIds { get; set; }
    public bool? SeasonEventImages { get; set; }
    public bool? XmltvAddFillerData { get; set; }
    //public string? XmltvFillerProgramDescription { get; set; }
    public int? XmltvFillerProgramLength { get; set; }
    public bool? XmltvIncludeChannelNumbers { get; set; }
    public bool? XmltvExtendedInfoInTitleDescriptions { get; set; }
    public bool? XmltvSingleImage { get; set; }
}

public class TestSettings
{
    public int DropInputSeconds { get; set; } = 0;
    public int DropClientSeconds { get; set; } = 0;
}

public class BaseSettings : M3USettings
{
    public int MaxStreamReStart { get; set; } = 3;
    public TestSettings TestSettings { get; set; } = new();
    public int MaxConcurrentDownloads { get; set; } = 8;
    public SDSettings SDSettings { get; set; } = new();
    public int ExpectedServiceCount { get; set; } = 20;
    public string AdminPassword { get; set; } = string.Empty;
    public string AdminUserName { get; set; } = string.Empty;
    public string DefaultIcon { get; set; } = "images/default.png";
    public string UiFolder { get; set; } = "wwwroot";
    public string UrlBase { get; set; } = string.Empty;
    public List<string> LogPerformance { get; set; } = ["*.Queries"];
    public string ApiKey { get; set; } = Guid.NewGuid().ToString().Replace("-", "");
    public AuthenticationType AuthenticationMethod { get; set; } = AuthenticationType.None;
    public bool CacheIcons { get; set; } = true;
    public bool CleanURLs { get; set; } = true;
    public string ClientUserAgent { get; set; } = "VLC/3.0.20-git LibVLC/3.0.20-git";
    public string DeviceID { get; set; } = "device1";
    public string DummyRegex { get; set; } = "(no tvg-id)";
    public string FFMpegOptions { get; set; } = "-hide_banner -loglevel error -i {streamUrl} -c copy -f mpegts pipe:1";
    public bool EnableSSL { get; set; }

    public string FFMPegExecutable { get; set; } = "ffmpeg";
    public string FFProbeExecutable { get; set; } = "ffprobe";
    public int GlobalStreamLimit { get; set; } = 1;
    public int MaxConnectRetry { get; set; } = 20;
    public int MaxConnectRetryTimeMS { get; set; } = 200;
    public int PreloadPercentage { get; set; } = 25;
    public int RingBufferSizeMB { get; set; } = 4;

    public List<string> NameRegex { get; set; } = [];

    public string SSLCertPassword { get; set; } = string.Empty;
    public string SSLCertPath { get; set; } = string.Empty;
    public string StreamingClientUserAgent { get; set; } = "VLC/3.0.20-git LibVLC/3.0.20-git";
    public StreamingProxyTypes StreamingProxyType { get; set; } = StreamingProxyTypes.StreamMaster;
    public bool VideoStreamAlwaysUseEPGLogo { get; set; } = true;

    public bool ShowClientHostNames { get; set; }
}

public class ProtectedSettings : BaseSettings
{
    [NoMap]
    public string ServerKey { get; set; } = Guid.NewGuid().ToString().Replace("-", "");
}

public class Setting : ProtectedSettings
{

}