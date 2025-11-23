using Microsoft.EntityFrameworkCore;
using LoginDashboardApp.Models;
using LoginDashboardApp.Data;

namespace LoginDashboardApp.BLL
{
    public class LoginManager
    {
        private readonly  ApplicationDbContext _db;
        public LoginManager(ApplicationDbContext db)
        {
            _db = db;
        }

        public User? GetUserLoggedIn(LoginInfoModel lm)
        {
            var user = _db.Users.Include(x => x.Role).Where(x => x.Email == lm.Email && x.Password == lm.Password).FirstOrDefault();

            return user;
        }

    }
}
