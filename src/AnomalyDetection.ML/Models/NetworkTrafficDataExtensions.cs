namespace AnomalyDetection.ML.Models;

public static class NetworkTrafficDataExtensions
{
    public static int GetFeatureCount(this NetworkTrafficData data)
    {
        if (data == null) return 0;

        // Count all public properties except any that should be excluded
        return typeof(NetworkTrafficData)
            .GetProperties()
            .Count(p => p.PropertyType == typeof(float)); // Only count float features
    }
}
