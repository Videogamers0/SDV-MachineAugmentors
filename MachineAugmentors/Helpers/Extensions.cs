using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MachineAugmentors.Helpers
{
    public static class Extensions
    {
        //Taken from: https://stackoverflow.com/questions/45426266/get-description-attributes-from-a-flagged-enum
        /// <summary>Returns the description attribute of the Enum value</summary>
        public static string GetDescription(this Enum value)
        {
            Type type = value.GetType();
            string name = Enum.GetName(type, value);
            if (name != null)
            {
                System.Reflection.FieldInfo field = type.GetField(name);
                if (field != null)
                {
                    DescriptionAttribute attr = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;
                    if (attr != null)
                    {
                        return attr.Description;
                    }
                }
            }
            return null;
        }

        public static Rectangle GetOffseted(this Rectangle value, Point Offset)
        {
            return new Rectangle(value.X + Offset.X, value.Y + Offset.Y, value.Width, value.Height);
        }

        public static Point AsPoint(this Vector2 value)
        {
            return new Point((int)value.X, (int)value.Y);
        }

        /// <summary>Intended to be used with <see cref="ICursorPosition.ScreenPixels"/></summary>
        /// <param name="value">The cursor position as a Point, that will be multiplied by the Game's zoomLevel if on Android.</param>
        public static Point AsAndroidCompatibleCursorPoint(this Vector2 value)
        {
            if (Constants.TargetPlatform == GamePlatform.Android)
            {
                return new Point((int)(Game1.options.zoomLevel * value.X), (int)(Game1.options.zoomLevel * value.Y));
            }
            else
            {
                return new Point((int)value.X, (int)value.Y);
            }
        }
    }
}
