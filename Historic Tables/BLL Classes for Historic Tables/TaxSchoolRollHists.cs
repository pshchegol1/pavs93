using EFDA.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFDA.BLL
{
    public class TaxSchoolRollHists : Base<tblTaxSchoolRollHist, Int32>
    {
        private Errors errorsBll;
        public override int Add(tblTaxSchoolRollHist t)
        {
            int id = 0;
            try
            {
                db.tblTaxSchoolRollHists.Add(t);
                db.SaveChanges();
                id = t.ID;
            }
            catch(Exception ex)
            {
                errorsBll.Add(new tblError() { Date = DateTime.Now, Module = "TaxSchoolRollHists", Message = "Add(tblTaxSchoolRollHist t) failed. Please address the issue in the message and run it again." + " Message: " + (ex.InnerException != null ? ex.InnerException.Message.ToString() : ex.Message.ToString()) });
                throw ex;
            }
            return id;
        }

        public override bool Delete(tblTaxSchoolRollHist t)
        {
            throw new NotImplementedException();
        }

        public override tblTaxSchoolRollHist Get(int id)
        {
            throw new NotImplementedException();
        }

        public override List<tblTaxSchoolRollHist> GetAll()
        {
            throw new NotImplementedException();
        }

        public override List<tblTaxSchoolRollHist> GetAll(int id)
        {
            throw new NotImplementedException();
        }

        public override bool Update(tblTaxSchoolRollHist t)
        {
            throw new NotImplementedException();
        }
    }
}
