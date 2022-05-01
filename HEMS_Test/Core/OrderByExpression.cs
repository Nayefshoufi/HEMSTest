using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HEMS_Test.Core
{
    public class OrderByExpression<T> : IOrderByExpression<T> where T : class
    {
        private LambdaExpression _expression;
        private bool _descending;
        private string propName;
        public static LambdaExpression CreateExpression<T>(string propertyName) where T : class
        {
            var x = "";
            var param = Expression.Parameter(typeof(T), "x");

            Expression body = param;
            foreach (var member in propertyName.Split('.'))
            {
                body = Expression.PropertyOrField(body, member);
            }
            LambdaExpression result = Expression.Lambda(body, param);
            return result;
        }
        public OrderByExpression(string _propName, bool descending = false)
        {
            propName = _propName;
            _expression = CreateExpression<T>(propName);
            _descending = descending;
        }
        //public IOrderedQueryable<T> ApplyOrderBy(IQueryable<T> query)
        //{
            
        //    if (_descending)
        //        return query.OrderByDescending((Expression<Func<T,dynamic>>)_expression);
        //    else
        //        return query.OrderBy((Expression<Func<T, dynamic>>)_expression);
        //}

        public IOrderedQueryable<T> ApplyOrderBy(IQueryable<T> query)
        {
            // Dynamically creates a call like this: query.OrderBy(p =&gt; p.SortColumn)
            var parameter = Expression.Parameter(typeof(T), "x");
            
            string command = "OrderBy";

            if (_descending)
            {
                command = "OrderByDescending";
            }

            Expression resultExpression = null;
            PropertyInfo property=null;
            var Type = typeof(T);
            foreach (var p in propName.Split('.'))
            {
                property = Type.GetProperty(p);
                Type = property.PropertyType;
            }
            
            // this is the part p.SortColumn
            

            // finally, call the "OrderBy" / "OrderByDescending" method with the order by lamba expression
            resultExpression = Expression.Call(typeof(Queryable), command, new Type[] { typeof(T), property.PropertyType },
               query.Expression, Expression.Quote(_expression));

            return (IOrderedQueryable<T>)query.Provider.CreateQuery<T>(resultExpression);
        }

        public IOrderedQueryable<T> ApplyThenBy(IOrderedQueryable<T> query)
        {
            var parameter = Expression.Parameter(typeof(T), "x");

            string command = "ThenBy";

            if (_descending)
            {
                command = "ThenByDescending";
            }

            Expression resultExpression = null;

            var property = typeof(T).GetProperty(propName);
            // this is the part p.SortColumn


            // finally, call the "OrderBy" / "OrderByDescending" method with the order by lamba expression
            resultExpression = Expression.Call(typeof(Queryable), command, new Type[] { typeof(T), property.PropertyType },
               query.Expression, Expression.Quote(_expression));

            return (IOrderedQueryable<T>)query.Provider.CreateQuery<T>(resultExpression);
        }



    }
}
