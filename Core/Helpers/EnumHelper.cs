using System.ComponentModel;
using System.Reflection;

namespace Core.Helpers
{
    public static class EnumHelper
    {
        public static string GetDescription(Enum value)
        {
            var field = value.GetType().GetField(value.ToString());
            var attr = field?.GetCustomAttribute<DescriptionAttribute>();
            return attr?.Description ?? value.ToString();
        }

        public static List<string> GetAllDescriptions<T>() where T : struct, Enum
        {
            return Enum.GetValues<T>()
                .Select(v => GetDescription(v))
                .ToList();
        }

        public static T ParseFromDescription<T>(string description) where T : struct, Enum
        {
            if (string.IsNullOrWhiteSpace(description))
                return default;

            var trimmed = description.Trim();

            foreach (var value in Enum.GetValues<T>())
            {
                if (string.Equals(GetDescription(value), trimmed, StringComparison.OrdinalIgnoreCase))
                    return value;
            }

            if (Enum.TryParse<T>(trimmed, true, out var parsed))
                return parsed;

            return default;
        }

        public static T? ParseNullableFromDescription<T>(string description) where T : struct, Enum
        {
            if (string.IsNullOrWhiteSpace(description))
                return null;

            var trimmed = description.Trim();

            foreach (var value in Enum.GetValues<T>())
            {
                if (string.Equals(GetDescription(value), trimmed, StringComparison.OrdinalIgnoreCase))
                    return value;
            }

            if (Enum.TryParse<T>(trimmed, true, out var parsed))
                return parsed;

            return null;
        }
    }
}
