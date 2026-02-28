using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TaskFlow.Interfaces;
using TaskFlow.Models;

namespace TaskFlow.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly IDashboardService _dashboardService;
        private readonly UserManager<ApplicationUser> _userManager;

        public DashboardController(IDashboardService dashboardService,
            UserManager<ApplicationUser> userManager)
        {
            _dashboardService = dashboardService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User)!;
            var vm = await _dashboardService.GetDashboardDataAsync(userId);
            return View(vm);
        }
    }
}
