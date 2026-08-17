using LibraryManagementSystem.Domain.Constants;
using LibraryManagementSystem.Infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Web.Controllers;

[Authorize(Roles = "SuperAdmin,Admin")]
public class UsersController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

     public UsersController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager; // Fixed typo here
    }

    // GET: Users
    public IActionResult Index()
    {
        var users = _userManager.Users.ToList();
        return View(users);
    }

    // GET: Users/AssignRole/5
    public async Task<IActionResult> AssignRole(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return NotFound();

        var userRoles = await _userManager.GetRolesAsync(user);
        var allRoles = await _roleManager.Roles.ToListAsync();

        ViewBag.UserId = id;
        ViewBag.UserEmail = user.Email;
        ViewBag.UserRoles = userRoles;

        return View(allRoles);
    }

    // POST: Users/AssignRole
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AssignRole(string userId, string role)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return NotFound();

        // Remove user from all existing roles
        var currentRoles = await _userManager.GetRolesAsync(user);
        await _userManager.RemoveFromRolesAsync(user, currentRoles);

        // Assign the new selected role (unless it's "None")
        if (!string.IsNullOrEmpty(role) && role != "None")
        {
            await _userManager.AddToRoleAsync(user, role);
        }

        TempData["SuccessMessage"] = "Role assigned successfully.";
        return RedirectToAction(nameof(Index));
    }
}