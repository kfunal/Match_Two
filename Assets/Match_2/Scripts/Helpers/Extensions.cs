using System;

namespace Helpers
{
    public static class Extensions
    {
        public static T IntToEnum<T>(this int _value)
        {
            return (T)Enum.ToObject(typeof(T), _value);
        }
    }
}
