using System;
using System.Collections.Generic;
using System.Linq;
using HEMS_Test.Models;
using System.Web;
using HEMS_Test.Core;
using System.Reflection;
using System.Data.Entity.Core.Objects;

namespace HEMS_Test.DataAccess.Repository
{
    public class Repository<T> where T : class
    {
        public static List<T> GetAll(GetMode mode, bool withDeleted = false, HEMS_testEntities context = null, IOrderByExpression<T>[] orderBy = null)
        {
            bool externalCTX = true;
            List<T> result = new List<T>();
            if (context == null)
            {
                externalCTX = false;
                context = new HEMS_testEntities();
            }
            var query = context.Set<T>().ApplyInclude(mode);
            if (!withDeleted)
            {
                var search = new SearchExpression("Status", SearchFunction.NotEqual, "DELETED");
                var exp = SearchExpression.GetExpression<T>(search);
                query = query.Where(exp);
            }
            if (orderBy == null)
            {
                orderBy = new IOrderByExpression<T>[] { new OrderByExpression<T>("ID") };
                query = orderBy.First().ApplyOrderBy(query);
            }
            else
            {
                orderBy.ToList().ForEach(o =>
                {
                    query = o.ApplyOrderBy(query);
                });
            }
            result = query.ToList();

            if (!externalCTX)
            {
                context.Database.Connection.Close();
                context.Dispose();
            }
            return result;
        }

        public static int Count(bool withDeleted = false, HEMS_testEntities context = null)
        {
            bool externalCTX = true;
            int result = 0;
            if (context == null)
            {
                externalCTX = false;
                context = new HEMS_testEntities();
            }
            if (!withDeleted)
            {
                var search = new SearchExpression("Status", SearchFunction.NotEqual, "DELETED");
                var exp = SearchExpression.GetExpression<T>(search);
                result = context.Set<T>().Count(exp);
            }
            else
            {
                result = context.Set<T>().Count();
            }
            if (!externalCTX)
            {
                context.Database.Connection.Close();
                context.Dispose();
            }
            return result;
        }

        public static T GetById(object Id, GetMode mode = GetMode.Native, HEMS_testEntities context = null)
        {
            var result = GetBySearchCriteria(mode, new SearchExpression("ID", SearchFunction.Equal, Id.ToString()), false, 0, 1, context);
            return result.FirstOrDefault();
        }

        public static List<T> GetBySearchCriteria(GetMode mode, SearchExpression search, bool withDeleted = false, int start = 0, int length = int.MaxValue, HEMS_testEntities context = null, IOrderByExpression<T>[] orderby = null)
        {
            bool externalCTX = true;
            List<T> result;
            if (context == null)
            {
                externalCTX = false;
                context = new HEMS_testEntities();
            }
            var searchNew = search;
            if (!withDeleted)
            {
                searchNew = new SearchExpression(new List<SearchExpression>() { new SearchExpression("Status", SearchFunction.NotEqual, "DELETED")
                , search }, LogicalOperation.And);
            }
            var exp = SearchExpression.GetExpression<T>(searchNew);
            IQueryable<T> query = context.Set<T>();
            if (orderby == null || orderby.Length == 0)
            {
                if (typeof(T).GetProperty("ID") != null)
                    orderby = new IOrderByExpression<T>[] { new OrderByExpression<T>("ID") };
                else
                    orderby = new IOrderByExpression<T>[] { new OrderByExpression<T>("Id") };
            }
            if (orderby != null)
            {
                query = query.ApplyOrderBy<T>(orderby);
            }


            query = query.Where(exp).Skip(start).Take(length);
            query = query.ApplyInclude(GetMode.Full);

            result = query.ToList();
            if (!externalCTX)
            {
                context.Database.Connection.Close();
                context.Dispose();
            }
            return result;
        }

        public static int CountBySearchCriteria(SearchExpression search, bool withDeleted = false, HEMS_testEntities context = null)
        {
            bool externalCTX = true;
            int result;
            if (context == null)
            {
                externalCTX = false;
                context = new HEMS_testEntities();
            }
            var searchNew = search;
            if (!withDeleted)
            {
                searchNew = new SearchExpression(new List<SearchExpression>() { new SearchExpression("Status", SearchFunction.NotEqual, "DELETED")
               , search }, LogicalOperation.And);
            }
            var exp = SearchExpression.GetExpression<T>(searchNew);
            result = context.Set<T>().Where(exp).Count();

            if (!externalCTX)
            {
                context.Database.Connection.Close();
                context.Dispose();
            }
            return result;
        }
        public static T Save(T value, HEMS_testEntities context = null)
        {

            bool externalCTX = true;
            T result;
            if (context == null)
            {
                externalCTX = false;
                context = new HEMS_testEntities();
            }

            result = context.Set<T>().Add(value);
            context.SaveChanges();

            if (!externalCTX)
            {
                context.Database.Connection.Close();
                context.Dispose();
            }
            return result;
        }

        public static List<T> Save(List<T> value, HEMS_testEntities context = null)
        {

            bool externalCTX = true;
            List<T> result;
            if (context == null)
            {
                externalCTX = false;
                context = new HEMS_testEntities();
            }

            result = context.Set<T>().AddRange(value).ToList();
            context.SaveChanges();

            if (!externalCTX)
            {
                context.Database.Connection.Close();
                context.Dispose();
            }
            return result;
        }

        public static void Update(T value, HEMS_testEntities context = null)
        {

            bool externalCTX = true;
            if (context == null)
            {
                externalCTX = false;
                context = new HEMS_testEntities();
            }

            context.Entry(value).State = System.Data.Entity.EntityState.Modified;// .Set<T>().Add(value);
            context.SaveChanges();
            if (!externalCTX)
            {
                context.Database.Connection.Close();
                context.Dispose();
            }
        }

        public static void SoftDelete(string code, HEMS_testEntities context = null)
        {
            bool externalCTX = true;
            if (context == null)
            {
                externalCTX = false;
                context = new HEMS_testEntities();
            }
            var methodName = $"Delete{typeof(T).Name}";
            var errN = new ObjectParameter("ErrN", 0);
            var errM = new ObjectParameter("ErrN", "");
            context.GetType().GetMethod(methodName).Invoke(context, new object[] { code, errN, errM });
            context.SaveChanges();

            if (!externalCTX)
            {
                context.Database.Connection.Close();
                context.Dispose();
            }
        }
        

    }
}
