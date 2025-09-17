using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CMCS.Data;
using CMCS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace CMCS.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (User.IsInRole("Lecturer"))
            {
                var lecturer = await _context.Lecturer.FirstOrDefaultAsync(l => l.UserId == userId);
                if (lecturer == null)
                {
                    // Handle the case where the user is a Lecturer but a Lecturer record doesn't exist.
                    return View("Error", new ErrorViewModel { ErrorMessage = "Lecturer data not found." });
                }

                var lecturerClaims = await _context.Claims
                    .Where(c => c.LecturerId == lecturer.LecturerId)
                    .OrderByDescending(c => c.CreatedDate)
                    .ToListAsync();

                ViewData["Claims"] = lecturerClaims;
                ViewData["User"] = lecturer; // Make sure to pass the user data to the view
                return View("LecturerDashboard");
            }

           

            if (User.IsInRole("ProgrammeCoordinator"))
            {
                var claimsToApprove = await _context.Claims
                    .Where(c => c.Status == ClaimStatus.Pending)
                    .Include(c => c.Lecturer)
                    .ToListAsync();
                ViewData["ClaimsToApprove"] = claimsToApprove;
                return View("ProgrammeCoordinatorDashboard");
            }

            if (User.IsInRole("AcademicManager"))
            {
                var claimsToApprove = await _context.Claims
                    .Where(c => c.Status == ClaimStatus.ApprovedByCoordinator)
                    .Include(c => c.Lecturer)
                    .ToListAsync();
                ViewData["ClaimsToApprove"] = claimsToApprove;
                return View("AcademicManagerDashboard");
            }

            return View("Unauthorized");
        }


        [HttpPost]
        public async Task<IActionResult> SubmitClaim(Models.Claim claim, List<IFormFile> documents)
        {
            if (ModelState.IsValid)
            {
                // Get the current lecturer's ID to link the claim
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var lecturer = await _context.Lecturer.FirstOrDefaultAsync(l => l.UserId == userId);
                if (lecturer == null)
                {
                    return NotFound();
                }
                claim.LecturerId = lecturer.LecturerId;
                claim.CreatedDate = DateTime.Now;
                claim.Status = ClaimStatus.Pending; // Set initial status

                _context.Claims.Add(claim);
                await _context.SaveChangesAsync(); // Save the claim first to get the ClaimId

                // Handle file uploads
                if (documents != null && documents.Count > 0)
                {
                    foreach (var file in documents)
                    {
                        // For demonstration, we'll save to the server's filesystem.
                        // In a real application, you'd use a more secure storage solution.
                        var filePath = Path.Combine("wwwroot/documents", file.FileName);
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }

                        // Create and save the Document entity
                        var document = new Document
                        {
                            ClaimId = claim.ClaimId,
                            FileName = file.FileName,
                            FilePath = filePath,
                            ContentType = file.ContentType,
                            FileSize = file.Length,
                            UploadedDate = DateTime.Now
                        };
                        _context.Documents.Add(document);
                    }
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction("Index"); // Redirect back to the dashboard
            }

            // If ModelState is not valid, return to the view with errors
            return View("LecturerDashboard", claim);
        }

    }
}
