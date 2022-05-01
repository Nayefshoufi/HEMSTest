using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Web;

namespace HEMS_Test.Core
{
    public static class EFExtensions
    {



        public static IQueryable<T> ApplyOrderBy<T>(this IQueryable<T> query, params IOrderByExpression<T>[] orderByExpressions) where T : class
        {
            if (orderByExpressions == null)
                return query;

            IOrderedQueryable<T> output = null;

            for (int i = 0; i < orderByExpressions.Length; i++)
            {
                if (output == null)
                    output = orderByExpressions[i].ApplyOrderBy(query);
                else
                    output = orderByExpressions[i].ApplyThenBy(output);
            }

            return output ?? query;
        }

        public static IQueryable<T> ApplyInclude<T>(this IQueryable<T> query, GetMode mode, params string[] CustomInclude)
        {
            var props = typeof(T).GetProperties().Where(p => p.GetCustomAttribute<LinkedPropAttribute>() != null || p.GetCustomAttribute<ListPropAttribute>() != null).ToList();

            switch (mode)
            {
                case GetMode.Full:
                    props.ForEach(p => query = query.Include(p.Name));
                    break;
                case GetMode.WithParent:
                    props.Where(p => p.GetCustomAttribute<LinkedPropAttribute>() != null).ToList().ForEach(p =>
                    {
                        query = query.Include(p.Name);
                    });
                    break;
                case GetMode.WithChildren:
                    props.Where(p => p.GetCustomAttribute<ListPropAttribute>() != null).ToList().ForEach(p =>
                    {
                        query = query.Include(p.Name);
                    });
                    break;
                case GetMode.Native:
                    break;
                case GetMode.Custom:
                    if (CustomInclude != null)
                    {
                        if (CustomInclude.Count() > 0)
                        {
                            props.Where(p => p.GetMethod.IsVirtual).ToList().ForEach(p =>
                            {
                                if (CustomInclude.Contains(p.Name))
                                {
                                    query = query.Include(p.Name);
                                }
                            });
                        }
                    }
                    break;
            }

            return query;
        }


    }
}
