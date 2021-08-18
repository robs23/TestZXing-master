using Newtonsoft.Json;
using SQLite;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestZXing.Static
{
    public static class Extensions
    {
        static public int InsertOrReplaceAll(this SQLiteConnection connection, IEnumerable objects, bool runInTransaction = true)
        {
            var c = 0;
            if (objects == null)
                return c;

            if (runInTransaction)
            {
                connection.RunInTransaction(() =>
                {
                    foreach (var r in objects)
                    {
                        c += connection.Insert(r, "OR REPLACE", Orm.GetType(r));
                    }
                });
            }
            else
            {
                foreach (var r in objects)
                {
                    c += connection.Insert(r, "OR REPLACE", Orm.GetType(r));
                }
            }

            return c;
        }
        public static T CloneJson<T>(this T source)
        {
            // Don't serialize a null object, simply return the default for that object
            if (Object.ReferenceEquals(source, null))
            {
                return default(T);
            }

            // initialize inner objects individually
            // for example in default constructor some list property initialized with some values,
            // but in 'source' these items are cleaned -
            // without ObjectCreationHandling.Replace default constructor values will be added to result
            var deserializeSettings = new JsonSerializerSettings { ObjectCreationHandling = ObjectCreationHandling.Replace };

            return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(source), deserializeSettings);
        }

        public static bool ContainsNullSafe(this string str, string search)
        {
            bool res = false;
            if (str != null)
            {
                return str.Contains(search);
            }
            return res;
        }
    }
}
