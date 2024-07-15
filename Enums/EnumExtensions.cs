using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaapiLibrary.Enums;
public static class EnumExtensions {
    public static string GetDescription(this Enum value) {
        // Ensure the field is not null before proceeding
        var field = value.GetType().GetField(value.ToString());
        if (field == null) {
            return value.ToString(); // Fallback to the enum's name if the field is not found
        }

        // Safely attempt to retrieve the DescriptionAttribute
        var attribute = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;

        // Return the description if available; otherwise, fallback to the enum's name
        return attribute?.Description ?? value.ToString();
    }

}
