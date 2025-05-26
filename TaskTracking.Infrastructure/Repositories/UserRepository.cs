using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskTracking.Core.Entities;
using TaskTracking.Core.Interfaces;
using TaskTracking.Infrastructure.Data;

namespace TaskTracking.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly TaskTrackingDbContext _dbContext;

        public UserRepository(TaskTrackingDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<User> AddUserAsync(User user)
        {
            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();
            return user;
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
        }
    }

}
