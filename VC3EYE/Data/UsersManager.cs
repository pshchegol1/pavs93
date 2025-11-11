using Microsoft.EntityFrameworkCore;
using VC3EYE.Entities;

namespace VC3EYE.Data
{
    public class UsersManager
    {
        private readonly Vc3eyeContext _db;

        public UsersManager(Vc3eyeContext db)
        {
            _db = db;
        }

        public List<User> GetAllUsers()
        {
            List<User> users = _db.Users.Include(x => x.Role).ToList();

            return users;
        }
    }
}
