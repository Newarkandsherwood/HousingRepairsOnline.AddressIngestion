namespace HousingRepairsOnline.AddressIngestion.Helpers
{
    using System;

    public static class EnvironmentVariableHelper
    {
        public static string GetEnvironmentVariable(string name) => Environment.GetEnvironmentVariable(name) ??
                   throw new InvalidOperationException($"Incorrect configuration: '{name}' environment variable must be set");
    }
}
