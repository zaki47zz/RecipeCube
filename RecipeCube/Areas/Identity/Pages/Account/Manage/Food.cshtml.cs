using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RecipeCube.Data;
using System.ComponentModel.DataAnnotations;

namespace RecipeCube.Areas.Identity.Pages.Account.Manage
{
    public class FoodModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public FoodModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }
        [TempData]
        public string StatusMessage { get; set; }
        [BindProperty]
        public InputModel Input { get; set; }
        public class InputModel
        {
            
            public bool dietary_restrictions { get; set; }
            public bool preferred_checked { get; set; }
            public bool exclusive_checked { get; set; }
        }

        private async Task LoadAsync(ApplicationUser user)
        {
            var Dietary_Restrictions = user.dietary_restrictions;
            var PreferedFood = user.preferred_checked;
            var ExclusiveFood = user.exclusive_checked;
            Input = new InputModel
            {
                dietary_restrictions = Dietary_Restrictions,
                preferred_checked = PreferedFood,
                exclusive_checked = ExclusiveFood
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }


            if (Input.dietary_restrictions != user.dietary_restrictions)
            {
                user.dietary_restrictions = Input.dietary_restrictions;
                var result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    StatusMessage = "Error updating user.";
                    return RedirectToPage();
                }
            }

            if (Input.preferred_checked != user.preferred_checked)
            {
                user.preferred_checked = Input.preferred_checked;  
                var result = await _userManager.UpdateAsync(user);  
                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    StatusMessage = "Error updating user.";
                    return RedirectToPage();
                }
            }

            if (Input.exclusive_checked != user.exclusive_checked)
            {
                user.exclusive_checked = Input.exclusive_checked;
                var result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    StatusMessage = "Error updating user.";
                    return RedirectToPage();
                }
            }

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }
    }
}
