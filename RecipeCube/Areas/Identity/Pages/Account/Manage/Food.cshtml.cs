using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RecipeCube.Areas.Identity.Pages.SqlClient;
using RecipeCube.Data;
using RecipeCube.Models;
using System.ComponentModel.DataAnnotations;
using System.Linq;

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

            if (user.dietary_restrictions)
            {
                ViewData["Dietaryrestrictions"] = true;
            }
            else
            {
                ViewData["Dietaryrestrictions"] = false;
            }




            // 如果 preferred_checked 為 true
            if (user.preferred_checked)
            {
                var joinSql = new JOINSql();
                var preferedIngredients = await joinSql.SelectJoinAsync("Prefered_Ingredients", user.Id);
                if (preferedIngredients == null || preferedIngredients.All(string.IsNullOrWhiteSpace))

                {
                    ViewData["PreferedIngredients"] = "找不到偏好食材資料，請確認是否有填入資料";
                }
                else
                {
                    // 將結果存入 ViewData 或 Model
                    ViewData["PreferedIngredients"] = string.Join(", ", preferedIngredients);
                }
            }


            if (user.exclusive_checked == true)
            {
                var joinSql = new JOINSql();
                var exclusiveIngredients = await joinSql.SelectJoinAsync("Exclusive_Ingredients", user.Id);
                if (exclusiveIngredients == null || exclusiveIngredients.All(string.IsNullOrWhiteSpace))
                {
                    ViewData["ExclusiveIngredients"] = "找不到不可食材資料，請確認是否有填入資料";
                }
                else
                {
                    // 將結果存入 ViewData 或 Model
                    ViewData["ExclusiveIngredients"] = string.Join(", ", exclusiveIngredients);
                }
            }
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


            if (Input.dietary_restrictions != user.dietary_restrictions || Input.preferred_checked != user.preferred_checked || Input.exclusive_checked != user.exclusive_checked)
            {
                var fieldData = new Dictionary<string, object>
                {
                    { "dietary_restrictions", Input.dietary_restrictions },
                    { "preferred_checked", Input.preferred_checked },
                    { "exclusive_checked", Input.exclusive_checked },
                };
                var updater = new UpdateSql();
                var rowsAffected = await updater.UpdateTableAsync("User", fieldData, "Id", user.Id);

                if (rowsAffected == 0)
                {
                    ModelState.AddModelError(string.Empty, "Error");
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
