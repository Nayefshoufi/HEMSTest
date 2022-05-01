using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HEMS_Test.Core
{
    public enum GetMode
    {
        /// <summary>
        /// Only Object
        /// </summary>
        Native,
        /// <summary>
        /// Object with its parents
        /// </summary>
        WithParent,
        /// <summary>
        /// Object with its children (Lists)
        /// </summary>
        WithChildren,
        /// <summary>
        /// Object with its Parents and Childers
        /// </summary>
        Full,
        /// <summary>
        /// Custom Method Calling
        /// </summary>
        Custom


    }
}
