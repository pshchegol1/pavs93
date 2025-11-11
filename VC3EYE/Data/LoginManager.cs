using Microsoft.EntityFrameworkCore;
using VC3EYE.Entities;
using VC3EYE.Models;

namespace VC3EYE.Data
{
    public class LoginManager
    {
        private readonly Vc3eyeContext _db;

        public LoginManager(Vc3eyeContext db)
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
