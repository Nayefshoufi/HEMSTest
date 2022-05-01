using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HEMS_Test.Core
{
    public interface IOrderByExpression<T> where T : class
    {
        IOrderedQueryable<T> ApplyOrderBy(IQueryable<T> query);
        IOrderedQueryable<T> ApplyThenBy(IOrderedQueryable<T> query);
    }
}
