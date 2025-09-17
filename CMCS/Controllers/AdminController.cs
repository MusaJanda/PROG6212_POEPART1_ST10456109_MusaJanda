using System.Linq;
using System.Threading.Tasks;
using CMCS.Models;
using CMCS.Models.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CMCS.Controllers
{
    // Restrict this controller to only users with the "Admin" role for security
    [Authorize]
    public class AdminController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // Action to display all registered users
        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users.ToListAsync();
            return View(users);
        }

        // Action to display a user's current roles and allow for management
        public async Task<IActionResult> ManageRoles(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            var model = new ManageUserRolesViewModel
            {
                UserId = user.Id,
                Email = user.Email,
                Roles = await _userManager.GetRolesAsync(user)
            };

            ViewBag.AllRoles = _roleManager.Roles.Select(r => r.Name).ToList();
            return View(model);
        }

        // Action to handle role assignment via a form submission
        [HttpPost]
        public async Task<IActionResult> UpdateRoles(string userId, string[] roles)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            // Get current roles and remove them
            var existingRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, existingRoles);

            // Add the newly selected roles
            if (roles != null && roles.Length > 0)
            {
                await _userManager.AddToRolesAsync(user, roles);
            }

            return RedirectToAction("Index");
        }
    }
}
