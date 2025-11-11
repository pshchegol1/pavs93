using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using VC3EYE.Data;
using VC3EYE.Models;
using VC3EYE.Entities;
using Microsoft.EntityFrameworkCore;

namespace VC3EYE.Pages
{
    public class ViewUsersModel : PageModel
    {
        private readonly Vc3eyeContext _db;
        public List<User> user { get; set; }
        public ViewUsersModel(Vc3eyeContext db)
        {
            _db = db;
        }


        //public List<User> GetAllUsers()
        //{
        //    List<User> UserS= _db.Users.Include(x => x.Role).ToList();

        //    return UserS;
        //}
        public void OnGet()
        {
            
            user=_db.Users.Include(x => x.Role).ToList();

        }
    }
}
