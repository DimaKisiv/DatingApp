using System;
using System.Threading.Tasks;
using DatingApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Data
{
    public class AuthRepository : IAuthRepository
    {
        private readonly DataContext _context;
        public AuthRepository(DataContext context)
        {
            _context = context;
        }
        public async Task<User> Login(string username, string password)
        {
            User user = await _context.Users.Include(p => p.Photos).FirstOrDefaultAsync(x => x.Username == username);
            if (user == null)
                return null;
            if (!ValidatePasswordHash(password, user.PasswordSalt, user.PasswordHash))
                return null;
            return user;
        }

        private bool ValidatePasswordHash(string password, byte[] passwordSalt, byte[] passwordHash)
        {
            bool isValid = true;
            using (var encoder = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                byte[] hashToVerify = encoder.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < hashToVerify.Length; i++)
                {
                    if (hashToVerify[i] != passwordHash[i])
                        isValid = false;
                }
            }
            return isValid;
        }

        public async Task<User> Register(User user, string password)
        {
            byte[] passwordHash, passwordSalt;
            CreatePasswordHash(password, out passwordHash, out passwordSalt);
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return user;
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var encoder = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = encoder.Key;
                passwordHash = encoder.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        public async Task<bool> UserExists(string username)
        {
            if (await _context.Users.AnyAsync(x => x.Username == username))
                return true;
            return false;
        }
    }
}