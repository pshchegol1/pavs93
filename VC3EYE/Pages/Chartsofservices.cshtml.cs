using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using VC3EYE.Data;
using VC3EYE.Entities;

namespace VC3EYE.Pages
{
    public class ChartsofservicesModel : PageModel
    {
        private Vc3eyeContext _db;
        public ChartsofservicesModel(Vc3eyeContext db)
        {
            _db = db;
        }
        [BindProperty]
       public List<ServiceDownHistory> serviceDownHistory { get; set; } = default!;

        public List<string> Months { get; set; }
        public List<int> Down { get; set; }
        public async Task OnGetAsync()
        {
            serviceDownHistory = await _db.ServiceDownHistories.ToListAsync();
            var downByMonth = serviceDownHistory.GroupBy(s => new { s.DateAdded.Year, s.DateAdded.Month, s.DateAdded.Day })
                                 .Select(g => new { Month = new DateTime(g.Key.Year, g.Key.Month, g.Key.Day),
                                  MGType = g.Count(a => a.ServiceId == 24) })
                                 .ToArray();
            // Create the X-axis labels 
            Months = downByMonth.Select(s => s.Month.ToString("yyyy-MMM-dd")).ToList();
            //Create Y-Axis Labels
            Down = downByMonth.Select(s => s.MGType).ToList();
        }
    }
    }





