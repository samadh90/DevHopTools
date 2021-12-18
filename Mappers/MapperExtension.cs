using System;
using System.Reflection;

namespace DevHopTools.Mappers
{
    public static class MapperExtension
    {
        public static TTo Map<TTo>(this object from)
            where TTo : new()
        {
            // créer une instance vide du nouvel objet
            TTo result = new TTo();
            return from.MapToInstance(result);
        }

        public static TTo MapToInstance<TTo>(this object from, TTo result)
            where TTo : new()
        {
            if (from is null) return default(TTo);
            // récupérer toutes les propriétes de cet objet
            PropertyInfo[] toProperties = typeof(TTo).GetProperties();
            foreach (PropertyInfo toProperty in toProperties)
            {
                // recuperer une propriété dans l'objet de départ qui porte le meme nom
                PropertyInfo fromProp = from.GetType().GetProperty(toProperty.Name);
                if (fromProp != null)
                {
                    // récupérer la valeur ds l'objet de départ
                    object value = fromProp.GetValue(from);
                    try
                    {
                        // insérer cette valeur dans le nouvel objet
                        toProperty.SetValue(result, value);
                    }
                    catch (Exception ex) { }
                }
            }
            return result;
        }
    }
}
