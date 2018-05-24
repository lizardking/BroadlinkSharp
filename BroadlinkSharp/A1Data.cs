namespace BroadlinkSharp
{
    public class A1Data
    {
        public double Temperature { get; set; }
        public double Humidity { get; set; }
        public A1LightEnum Light { get; set; } = A1LightEnum.Unknown;
        public A1AirQualityEnum AirQuality { get; set; } = A1AirQualityEnum.Unknown;
        public A1NoiseLevelEnum NoiseLevel { get; set; } = A1NoiseLevelEnum.Unknown;
    }

    public enum A1LightEnum
    {
        Dark = 0,
        Dim = 1,
        Normal = 2,
        Bright = 3,
        Unknown = -1
    }

    public enum A1AirQualityEnum
    {
        Excellent = 0,
        Good = 1,
        Normal = 2,
        Bad = 3,
        Unknown = -1
    }

    public enum A1NoiseLevelEnum
    {
        Quiet = 0,
        Normal = 1,
        Noisy = 2,
        Unknown = -1
    }
}