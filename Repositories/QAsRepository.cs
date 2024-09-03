using EDUHUNT_BE.Data;
using EDUHUNT_BE.DTOs;
using EDUHUNT_BE.Interfaces;
using EDUHUNT_BE.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EDUHUNT_BE.Repositories
{
    public class QAsRepository : IQas
    {
        private readonly AppDbContext _context;
        private UserManager<ApplicationUser> _userManager;

        public QAsRepository(AppDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public async Task<ServiceResponses.GeneralResponse> DeleteQA(Guid id)
        {
            var qA = await _context.QAs.FindAsync(id);
            if (qA == null)
            {
                return new ServiceResponses.GeneralResponse(false,"Not Found");
            }

            _context.QAs.Remove(qA);
            await _context.SaveChangesAsync();

            return new ServiceResponses.GeneralResponse(true,"Delete Successfully");
        }

        public async Task<IEnumerable<QA>> GetConversations(Guid id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());

            if (user == null)
            {
                return null;
            }

            var userRoles = await _userManager.GetRolesAsync(user);

            if (userRoles.Contains("User"))
            {
                var mentorQAs = await _context.QAs
                    .Where(q => q.AskerId == id)
                    .ToListAsync();

                return mentorQAs;
            }
            else if (userRoles.Contains("Mentor"))
            {
                var userQAs = await _context.QAs
                    .Where(q => q.AnswerId == id)
                    .ToListAsync();

                return userQAs;
            }
            else
            {
                return null;
            }
        }


        public async Task<QA> GetQADetail(Guid id)
        {
            var qA = await _context.QAs.FindAsync(id);

            if (qA == null)
            {
                return null; // or handle accordingly
            }

            return qA;
        }

        public async Task<IEnumerable<QA>> GetQAs()
        {
            return await _context.QAs.ToListAsync();
        }

        public async Task<IEnumerable<QA>> GetQAsByUserId(Guid answerId, Guid askedId)
        {
            return await _context.QAs.Where(q => q.AskerId == askedId && q.AnswerId == answerId).ToListAsync();
        }

        public async Task<IEnumerable<ApplicationUser>> GetUsersOrMentors(Guid id)
        {
            try
            {
                // Retrieve the user by ID
                var user = await _userManager.FindByIdAsync(id.ToString());

                if (user == null)
                {
                    return null;
                }

                // Retrieve roles directly without using IsInRoleAsync
                var userRoles = await _userManager.GetRolesAsync(user);

                if (userRoles.Contains("User"))
                {
                    // Retrieve mentors if the user has the "User" role
                    var mentors = await _userManager.GetUsersInRoleAsync("Mentor");
                    return mentors;
                }
                else if (userRoles.Contains("Mentor"))
                {
                    // Retrieve users if the user has the "Mentor" role
                    var users = await _userManager.GetUsersInRoleAsync("User");
                    return users;
                }
                else
                {
                    throw new InvalidOperationException("Invalid role for the specified user");
                }
            }
            catch (Exception ex)
            {
                // Log the exception for debugging
                Console.WriteLine($"Exception: {ex.Message}");
                throw; // Rethrow the exception for the calling code to handle
            }
        }


        public async Task<ServiceResponses.GeneralResponse> PostQA(QA qA)
        {
            qA.Id = Guid.NewGuid(); // Generate a new GUID for the Id
            _context.QAs.Add(qA);
            await _context.SaveChangesAsync();

            return new ServiceResponses.GeneralResponse(true,"Post Successfully");
        }

        public async Task<ServiceResponses.GeneralResponse> PutQA(Guid id, QA qA)
        {
            if (id != qA.Id)
            {
                return new ServiceResponses.GeneralResponse(false,"Id not found");
            }

            _context.Entry(qA).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!QAExists(id))
                {
                    return new ServiceResponses.GeneralResponse(false,"Not Found");
                }
                else
                {
                    throw;
                }
            }

            return new ServiceResponses.GeneralResponse(true,"Put Successfully");
        }

        private bool QAExists(Guid id)
        {
            return _context.QAs.Any(e => e.Id == id);
        }
    }
}
