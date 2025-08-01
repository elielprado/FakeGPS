namespace FakeGPS.Common
{
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using Microsoft.Win32;

    /// <summary>
    /// Static class to help with Registry operations.
    /// </summary>
    public static class RegistryHelper
    {
        private const string BasePath = @"SYSTEM\CurrentControlSet\Enum\ROOT\SENSOR";
        private const string LatitudeProperty = @"SENSOR_PROPERTY_LATITUDE";
        private const string LongitudeProperty = @"SENSOR_PROPERTY_LONGITUDE";

        /// <summary>
        /// Finds the correct registry path for FakeGPS dynamically.
        /// </summary>
        /// <returns>The registry path string.</returns>
        private static string FindFakeGPSRegistryPath()
        {
            using (var rootKey = Registry.LocalMachine.OpenSubKey(BasePath))
            {
                if (rootKey != null)
                {
                    foreach (var subKeyName in rootKey.GetSubKeyNames())
                    {
                        string fullPath = $@"{BasePath}\{subKeyName}\Device Parameters\FakeGPS";
                        using (var key = Registry.LocalMachine.OpenSubKey(fullPath))
                        {
                            if (key != null)
                            {
                                return fullPath;
                            }
                        }
                    }
                }
            }

            throw new InvalidOperationException("FakeGPS registry path not found under ROOT\\SENSOR.");
        }

        /// <summary>
        /// Sets the Latitude and Longitude in the Registry.
        /// </summary>
        /// <param name="latLong">The <see cref="LatLong"/> coordinates.</param>
        public static void SetLatLong(LatLong latLong)
        {
            try
            {
                string path = FindFakeGPSRegistryPath();
                using (var key = Registry.LocalMachine.CreateSubKey(path))
                {
                    string latitudeStr = Math.Round(latLong.Latitude, 5).ToString("F5", CultureInfo.InvariantCulture);
                    string longitudeStr = Math.Round(latLong.Longitude, 5).ToString("F5", CultureInfo.InvariantCulture);

                    key.SetValue(LatitudeProperty, latitudeStr);
                    key.SetValue(LongitudeProperty, longitudeStr);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while setting LatLong: {ex.Message}");
                if (Debugger.IsAttached)
                {
                    Debugger.Break();
                }
                throw;
            }
        }

        /// <summary>
        /// Gets the current Latitude and Longitude from the Registry.
        /// </summary>
        /// <returns>The <see cref="LatLong"/> coordinates.</returns>
        public static LatLong GetLatLong()
        {
            string path = FindFakeGPSRegistryPath();
            using (var key = Registry.LocalMachine.OpenSubKey(path))
            {
                if (key == null)
                {
                    throw new InvalidOperationException("FakeGPS registry path not found.");
                }

                string latitudeStr = key.GetValue(LatitudeProperty)?.ToString();
                string longitudeStr = key.GetValue(LongitudeProperty)?.ToString();

                if (latitudeStr == null || longitudeStr == null)
                {
                    throw new InvalidOperationException("Latitude or Longitude not found in registry.");
                }

                double latitude = double.Parse(latitudeStr, CultureInfo.InvariantCulture);
                double longitude = double.Parse(longitudeStr, CultureInfo.InvariantCulture);

                return new LatLong { Latitude = latitude, Longitude = longitude };
            }
        }
    }
}
