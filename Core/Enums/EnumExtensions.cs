using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaapiLibrary.Core.Enums;
public static class EnumExtensions {

    /// <summary>
    /// Retrieves the DescriptionAttribute value from the provided enum value.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
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
    }//end GetDescription()

}// class
