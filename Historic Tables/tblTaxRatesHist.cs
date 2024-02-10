using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EFDA.Entity
{
    [Table("tblTaxRatesHist")]
    public partial class tblTaxRatesHist
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public int? TaxRatesPKOld { get; set; }
        public int? Year { get; set; }

        [StringLength(8)]
        public string DistrictCode { get; set; }
        public short? DistrictNumber { get; set; }

        [StringLength(5)]
        public string RateClass { get; set; }
        public float? MillRate { get; set; }
        public float? AllowanceRate { get; set; }
        public float? MinTax { get; set; }
        public bool FlatTax { get; set; }

        [StringLength(50)]
        public string RateDescription { get; set; }
        public float? GeneralLedger { get; set; }
        public decimal? FlatMin { get; set; }
        public decimal? FlatMax { get; set; }
        public int? SchoolID { get; set; }
    }
}
