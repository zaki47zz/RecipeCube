using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RecipeCube.Areas.Identity.Pages.SqlClient;
using RecipeCube.Data;
using static RecipeCube.Areas.Identity.Pages.SqlClient.FoodService;

namespace RecipeCube.Areas.Identity.Pages.Account.Manage
{
    public class ExclusiveFoodModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public List<Food> Foods { get; set; }

        public ExclusiveFoodModel(
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
            public bool exclusive_checked { get; set; }
        }

        private async Task LoadAsync(ApplicationUser user)
        {
            var exclusiveFood = user.exclusive_checked;
            Input = new InputModel
            {
                exclusive_checked = exclusiveFood
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            // 手動實例化 FoodService
            var foodService = new FoodService();
            Foods = await foodService.GetAllIngredientsAsync();
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

            if (Input.exclusive_checked != user.exclusive_checked)
            {
                var fieldData = new Dictionary<string, object>
                {
                    { "exclusive_checked", Input.exclusive_checked }
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

        public async Task<IActionResult> OnPostSaveExclusiveAsync(int ingredientId)
        {
            var userId = _userManager.GetUserId(User);
            var foodService = new FoodService(); // 手動實例化 FoodService
            var rowsAffected = await foodService.SaveIngredientsAsync("Exclusive_Ingredients", userId, ingredientId);

            if (rowsAffected > 0)
            {
                StatusMessage = "食材已成功加入不可食用清單";
            }
            else
            {
                StatusMessage = "加入不可食用清單時發生錯誤";
            }

            return RedirectToPage();
        }
    }
}
