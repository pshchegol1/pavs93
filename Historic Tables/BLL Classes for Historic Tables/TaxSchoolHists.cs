using EFDA.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFDA.BLL
{
    public class TaxSchoolHists : Base<tblTaxSchoolHist, Int32>
    {
        private Errors errorsBll;
        public override int Add(tblTaxSchoolHist t)
        {
            int id = 0;
            try
            {
                db.tblTaxSchoolHists.Add(t);
                db.SaveChanges();
                id = t.ID;
            }
            catch (Exception ex)
            {
                errorsBll.Add(new tblError() { Date = DateTime.Now, Module = "TaxSchoolHists", Message = "Add(tblTaxSchoolHist t) failed. Please address the issue in the message and run it again." + " Message: " + (ex.InnerException != null ? ex.InnerException.Message.ToString() : ex.Message.ToString()) });
            }
            return id;
        }

        public override bool Delete(tblTaxSchoolHist t)
        {
            throw new NotImplementedException();
        }

        public override tblTaxSchoolHist Get(int id)
        {
            throw new NotImplementedException();
        }

        public override List<tblTaxSchoolHist> GetAll()
        {
            throw new NotImplementedException();
        }

        public override List<tblTaxSchoolHist> GetAll(int id)
        {
            throw new NotImplementedException();
        }

        public override bool Update(tblTaxSchoolHist t)
        {
            throw new NotImplementedException();
        }
    }
}
