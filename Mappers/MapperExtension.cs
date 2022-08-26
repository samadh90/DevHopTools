using System;
using System.Reflection;

namespace DevHopTools.Mappers
{
    public static class MapperExtension
    {
        public static TTo Map<TTo>(this object from)
            where TTo : new()
        {
            // create an empty instance of the new object
            TTo result = new TTo();
            return from.MapToInstance(result);
        }

        public static TTo MapToInstance<TTo>(this object from, TTo result)
            where TTo : new()
        {
            if (from is null) return default(TTo);
            // get all the properties of this object
            PropertyInfo[] toProperties = typeof(TTo).GetProperties();
            foreach (PropertyInfo toProperty in toProperties)
            {
                // retrieve a property in the starting object that has the same name
                PropertyInfo fromProp = from.GetType().GetProperty(toProperty.Name);
                if (fromProp != null)
                {
                    // retrieve the value in the starting object
                    object value = fromProp.GetValue(from);
                    try
                    {
                        // insert this value into the new object
                        toProperty.SetValue(result, value);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message);
                    }
                }
            }
            return result;
        }
    }
}
