using System.ComponentModel;
using System.Reflection;

namespace CubeRobot.Util;

public static class EnumExtensions
{
    /// <summary>
    /// Returns text written in <see cref="DescriptionAttribute"/>. Use this attribute to decorate
    /// enum value
    /// </summary>
    public static string? GetDescriptor<T>(this T enumValue) where T : Enum
    {
        FieldInfo? info = enumValue.GetType().GetField(enumValue.ToString());

        if (info is null)
            return null;

        DescriptionAttribute? attr = info.GetCustomAttribute(typeof(DescriptionAttribute), false) as DescriptionAttribute;

        return attr?.Description;
    }
}