using Microsoft.EntityFrameworkCore;
using LoginDashboardApp.Models;

namespace LoginDashboardApp.Data
{
    public class UsersManager
    {
        private readonly ApplicationDbContext _db;

        public UsersManager(ApplicationDbContext db)
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
