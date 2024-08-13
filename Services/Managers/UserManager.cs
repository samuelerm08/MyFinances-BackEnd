using Microsoft.EntityFrameworkCore;
using MyFinances.WebApi.Data;
using MyFinances.WebApi.Models;
using MyFinances.WebApi.Models.Security;
using MyFinances.WebApi.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyFinances.WebApi.Repository.Managers
{
    public class UserManager : IUser
    {
        private readonly DbFinancesContext _context;
        private readonly TokenService _tokenService;
        public UserManager(DbFinancesContext context, TokenService tokenService)
        {
            _context = context;
            _tokenService = tokenService;
        }

        public async Task<User> CreateAsync(User user)
        {
            try
            {
                bool exists = await _context.Users.FirstOrDefaultAsync(u =>
                    u.Email == user.Email) != null;

                if (!exists)
                {
                    user.Password = Encrypt.GetEncryptedPassword(user.Password);
                    await _context.Users.AddAsync(user);
                    await _context.SaveChangesAsync();

                    return user;
                }
                else
                {
                    return null;
                }
            }
            catch
            {
                throw new InvalidOperationException(new Exception().Message);
            }
        }

        public async Task<UserToken> Login(UserAuth auth)
        {
            UserToken userToken = null;
            try
            {
                User user = await AuthUser(auth);

                if (user == null)
                {
                    return userToken;
                }

                string token = _tokenService.Generate(user);

                userToken = new UserToken()
                {
                    Email = user.Email,
                    Token = token
                };

                return userToken;
            }
            catch (Exception e)
            {
                throw new InvalidOperationException(e.Message);
            }
        }

        private async Task<User> AuthUser(UserAuth user)
        {
            User validUser = null;

            user.Password = Encrypt.GetEncryptedPassword(user.Password);
            validUser = await _context.Users.FirstOrDefaultAsync
                                    (u =>
                                    u.Email == user.Email &&
                                    u.Password == user.Password);
            return validUser;
        }

        public async Task<User> DeleteAsync(int id)
        {
            User user = null;
            try
            {
                user = await _context.Users
                    .Include(u => u.Goals)
                    .Where(u => u.Id == id)
                    .SingleOrDefaultAsync();

                if (user != null)
                {
                    _context.Users.Remove(user);
                    await _context.SaveChangesAsync();
                    return user;
                }

                return user;
            }
            catch
            {
                throw new InvalidOperationException(new Exception().Message);
            }
        }

        public async Task<User> ModifyAsync(int? id, User user)
        {
            try
            {
                User userToModify = await GetByIdAsync(user.Id);
                userToModify.FirstName = user.FirstName;
                userToModify.LastName = user.LastName;
                _context.Entry(userToModify).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return user;
            }
            catch
            {
                throw new InvalidOperationException(new Exception().Message);
            }
        }

        public async Task<User> GetByIdAsync(int? id)
        {
            try
            {
                User user = await _context.Users
                    .Include(u => u.Goals)
                    .SingleOrDefaultAsync(t => t.Id == id);
                return user;
            }
            catch
            {
                throw new InvalidOperationException(new Exception().Message);
            }
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            try
            {
                return await _context.Users
                    .Include(p => p.Goals)
                    .ToListAsync();
            }
            catch
            {
                throw new InvalidOperationException(new Exception().Message);
            }
        }
    }
}
