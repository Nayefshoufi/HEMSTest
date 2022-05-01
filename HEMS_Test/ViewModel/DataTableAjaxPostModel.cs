using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HEMS_Test.ViewModel
{
    public class DataTableAjaxPostModel
    {
        // properties are not capital due to json mapping
        public int draw { get; set; }
        public int start { get; set; }
        public int length { get; set; }
        public List<Column> columns { get; set; }
        public Search search { get; set; }
        public List<Order> order { get; set; }

        public Nullable<bool> IsDeleted { get; set; }

        public DataTableAjaxPostModel Copy()
        {
            DataTableAjaxPostModel result = new DataTableAjaxPostModel();
            result.draw = this.draw;
            result.start = this.start;
            result.length = this.length;
            result.columns = new List<Column>();
            this.columns.ForEach(c => result.columns.Add(c.Copy()));
            result.search = this.search.Copy();
            result.order = new List<Order>();
            if (this.order != null)
            {
                this.order.ForEach(o => result.order.Add(o.Copy()));
            }
            result.IsDeleted = this.IsDeleted;
            return result;
        }
    }

    public class Column
    {
        public string data { get; set; }
        public string name { get; set; }
        public bool searchable { get; set; }
        public bool orderable { get; set; }
        public Search search { get; set; }

        public Column Copy()
        {
            Column result = new Column();
            result.data = this.data;
            result.name = this.name;
            result.searchable = this.searchable;
            result.search = this.search.Copy();
            return result;
        }
    }

    public class Search
    {
        public string value { get; set; }
        public string regex { get; set; }

        public Search Copy()
        {
            Search result = new Search();
            result.value = this.value;
            result.regex = this.regex;
            return result;
        }
    }

    public class Order
    {
        public int column { get; set; }
        public string dir { get; set; }

        public Order Copy()
        {
            Order result = new Order();
            result.column = this.column;
            result.dir = this.dir;
            return result;
        }
    }
}