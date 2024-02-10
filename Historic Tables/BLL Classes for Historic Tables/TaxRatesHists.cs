using EFDA.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFDA.BLL
{
    public class TaxRatesHists : Base<tblTaxRatesHist, Int32>
    {
        private Errors errorsBll;
        public override int Add(tblTaxRatesHist t)
        {
            int id = 0;
            try
            {
                db.tblTaxRatesHists.Add(t);
                db.SaveChanges();
                id = t.ID;
            }
            catch (Exception ex)
            {
                errorsBll.Add(new tblError() { Date = DateTime.Now, Module = "TaxRatesHists", Message = "Add(tblTaxRatesHist t) failed. Please address the issue in the message and run it again." + " Message: " + (ex.InnerException != null ? ex.InnerException.Message.ToString() : ex.Message.ToString()) });
                throw ex;
            }
            return id;
        }

        public override bool Delete(tblTaxRatesHist t)
        {
            throw new NotImplementedException();
        }

        public override tblTaxRatesHist Get(int id)
        {
            throw new NotImplementedException();
        }

        public override List<tblTaxRatesHist> GetAll()
        {
            throw new NotImplementedException();
        }

        public override List<tblTaxRatesHist> GetAll(int id)
        {
            throw new NotImplementedException();
        }

        public override bool Update(tblTaxRatesHist t)
        {
            throw new NotImplementedException();
        }
    }
}
