// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using RecipeCube.Areas.Identity.Pages.SqlClient;
using RecipeCube.Data;
using System.ComponentModel.DataAnnotations;

namespace RecipeCube.Areas.Identity.Pages.Account.Manage
{
    public class GroupModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public GroupModel(
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
            [Display(Name = "Group Id")]
            public int group_id { get; set; }
        }

        private async Task LoadAsync(ApplicationUser user)
        {
            var Group_Id = user.group_id;
            Input = new InputModel
            {
                group_id = Group_Id
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
            if (Input.group_id != user.group_id)
            {
                var fieldData = new Dictionary<string, object>
                {
                    { "group_id", Input.group_id }
                };
                var updater = new UpdateSql();
                var rowsAffected = await updater.UpdateTableAsync("User", fieldData, "Id", user.Id);

                if (rowsAffected == 0)
                {
                    ModelState.AddModelError(string.Empty, "Error updating group_id.");
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
