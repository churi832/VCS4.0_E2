using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace Sineva.VHL.IF.WebApi.Models
{
    public static class DeepCopyExtensions
    {
        public static T DeepCopy<T>(this T original) where T : new()
        {
            return DeepCopier<T>.Copy(original);
        }

        private static class DeepCopier<T> where T : new()
        {
            private static readonly Func<T, T> copier;

            static DeepCopier()
            {
                var originalParam = Expression.Parameter(typeof(T), "original");
                var memberBindings = new List<MemberBinding>();
                for (int i = 0; i < typeof(T).GetProperties().Count(); i++)
                {
                    var prop = typeof(T).GetProperties()[i];
                    if (prop.CanRead && prop.CanWrite)
                    {
                        var originalProp = Expression.Property(originalParam, prop);
                        memberBindings.Add(Expression.Bind(prop, originalProp));
                    }
                }

                var body = Expression.MemberInit(Expression.New(typeof(T)), memberBindings);
                var lambda = Expression.Lambda<Func<T, T>>(body, originalParam);
                copier = lambda.Compile();
            }

            public static T Copy(T original) => copier(original);
        }
    }
}