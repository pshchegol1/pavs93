using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

//The annotations used within the .Data project will require the System.CommponentModel.DataAnnotation assembly

//This assembly is added via your References
#region Additional Namespaces
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
#endregion

namespace NorthwindSystem.Data
{
    //Use an annotation to link this class to the appropriate SQL table
    [Table("Products")]
    public class Product
    {
        //Mapping of the SQL Table attributes will be to class properties
        private string _QuantityPerUnit;


        //Use an annotation to identify the primary key
        //1) Identity pkey on your SQL table - [Key] pkey name must end in ID or Id

        //2)a compound pkey on your SQL table - [Key, Column(Order=n)] where n is the natural number indicating the physical order of the attribute in the PKEY

        //3) a User supplied pkey on your SQL table - [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]

        [Key]
        public int ProductID { get; set; }
        public string ProductName{ get; set; }
        public int? SupplierID { get; set; }
        public int? CategoryID { get; set; }
        public string QuantityPerUnit
        {
            get
            {
                return _QuantityPerUnit;
            }
            set
            {
                _QuantityPerUnit = string.IsNullOrEmpty
                    (value.Trim()) ? null : value;
            }
        }
        public decimal? UnitPrice { get; set; }
        public Int16? UnitsInStock { get; set; }
        public Int16? UnitsOnOrder { get; set; }
        public Int16? ReorderLevel { get; set; }
        public bool Discontinued { get; set; }

        // Sample of the computed field on your SQL
        //To annotate this property to be taken as a SQL computed field use [DatabaseGenerated(DatabaseGeneratedOption.Computed]
        //public decimal Total { get; set; }

        //Sample creating a read only property that is not an actual field on your SQL table

        //Example FirstName LastName attributes are often  combined into a single field for display purposes:FullName
        //Use the NotMapped annotationto handle this

        //[NotMapped]
        //public string FullName
        //{
        //    get FirstName + "" + LastName;
        //}
    }
}
