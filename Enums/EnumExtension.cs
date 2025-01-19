using System;
using System.ComponentModel;
using System.Reflection;

namespace MyFirstProgram.Enums;

static class EnumExtensions
{
    public static string ToFriendlyString(this Enum value)
    {
        var stringValue = value.ToString();
        for (int i = 1; i < stringValue.Length; i++)
        {
            if (char.IsUpper(stringValue[i]))
            {
                stringValue = stringValue.Insert(i, " ");
                i++;
            }
        }
        return stringValue;
    }

    public static string ToDescriptiveOrFriendlyString(this Enum value)
    {
            var field = value.GetType().GetField(value.ToString());
            if (field == null) return value.ToString();
            
            var attribute = field.GetCustomAttribute(typeof(DescriptionAttribute)) as DescriptionAttribute;
            return attribute == null ? value.ToString() : attribute.Description;

    }
}
