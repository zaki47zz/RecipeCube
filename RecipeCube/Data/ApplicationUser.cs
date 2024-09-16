using Microsoft.AspNetCore.Identity;

namespace RecipeCube.Data
{
    public class ApplicationUser : IdentityUser
    {
        public int group_id { get; set; }
        public bool dietary_restrictions { get; set; }
        public bool preferred_checked { get; set; }
        public bool exclusive_checked { get; set; }
        public bool status { get; set; }
    }
}
