namespace RecipeCubeWebService.DTO;

    public class ProductEvaluatesDTO
    {
        public int EvaluateId { get; set; }

        public string UserId { get; set; }

        public string UserName {  get; set; }

        public int? ProductId { get; set; }

        public string CommentMessage { get; set; }

        public int? CommentStars { get; set; }

        public DateOnly? Date { get; set; }
    }

