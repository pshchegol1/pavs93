using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EFDA.Entity
{
    [Table("tblTaxSchoolRollHist")]
    public partial class tblTaxSchoolRollHist
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public int? Year { get; set; }
        public decimal? RollNumber { get; set; }
        public int? SchoolID { get; set; }
        public decimal? SupportPercentage { get; set; }
    }
}
