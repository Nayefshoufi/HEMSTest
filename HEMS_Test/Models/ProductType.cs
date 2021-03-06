//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace HEMS_Test.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class ProductType
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ProductType()
        {
            this.Products = new HashSet<Product>();
        }
    
        public string Product_Type_Code { get; set; }
        public int Seq_Id { get; set; }
        public string Product_Category_Code { get; set; }
        public int Product_Category_Seq_Id { get; set; }
        public string Type_Name { get; set; }
        public string Type_Description { get; set; }
        public string Status { get; set; }
        public System.DateTime Create_Date { get; set; }
        public Nullable<System.DateTime> Update_Date { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Product> Products { get; set; }
        public virtual ProductCategory ProductCategory { get; set; }
    }
}
