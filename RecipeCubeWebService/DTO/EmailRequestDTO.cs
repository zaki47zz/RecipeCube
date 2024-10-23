namespace RecipeCubeWebService.DTO
{
    public class EmailRequestDTO
    {
        public string Email { get; set; }  // 收件人的電子郵件地址
        public string Message { get; set; }  // 信件的內容
        // 後續可套入模板傳遞有HTMl樣式的信件
    }
}
