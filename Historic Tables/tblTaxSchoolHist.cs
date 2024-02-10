using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EFDA.Entity
{
    [Table("tblTaxSchoolHist")]
    public partial class tblTaxSchoolHist
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public int? IDOld { get; set; }

        [StringLength(255)]
        public string Description { get; set; }
    }
}
