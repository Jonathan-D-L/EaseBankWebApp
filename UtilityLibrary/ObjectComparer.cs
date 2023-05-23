using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtilityLibrary
{
    public class ObjectComparer
    {
        public static bool AreObjectsEqual<T>(T object1, T object2)
        {
            var properties = typeof(T).GetProperties();
            return !(from property in properties 
                let value1 = property.GetValue(object1).ToString().ToLower() 
                let value2 = property.GetValue(object2).ToString().ToLower() 
                where !Equals(value1, value2) 
                select value1).Any();
        }
    }
}
