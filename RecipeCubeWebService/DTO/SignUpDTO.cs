namespace RecipeCubeWebService.DTO
{
    public class SignUpDTO
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
        public required bool DietaryRestrictions { get; set; }
    }
}
