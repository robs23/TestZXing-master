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

    }
}
