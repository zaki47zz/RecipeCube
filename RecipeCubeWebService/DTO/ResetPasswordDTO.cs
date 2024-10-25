namespace RecipeCubeWebService.DTO
{
    public class ResetPasswordDTO
    {
        public required string Password { get; set; }
        public required string Token { get; set; }
    }
}
