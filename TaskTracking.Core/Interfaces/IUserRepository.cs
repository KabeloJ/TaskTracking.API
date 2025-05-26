using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskTracking.Core.Entities;

namespace TaskTracking.Core.Interfaces
{
    public interface IUserRepository
    {
        Task<User> AddUserAsync(User user);
        Task<User?> GetUserByEmailAsync(string email);
    }
}
