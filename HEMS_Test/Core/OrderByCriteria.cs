using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HEMS_Test.Core
{
    public class OrderByCriteria
    {
        public string Property { get; set; }

        public OrderByFunction Function { get; set; }
    }

    public enum OrderByFunction : int
    {
        ASC = 0,
        DESC = 1
    }
}
