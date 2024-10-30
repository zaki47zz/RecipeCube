using System;
using System.Collections.Generic;

namespace RecipeCubeWebService.Models;

public partial class ProductEvaluate
{
    public int EvaluateId { get; set; }

    public string? UserId { get; set; }

    public int? ProductId { get; set; }

    public string? CommentMessage { get; set; }

    public int? CommentStars { get; set; }

    public DateOnly? Date { get; set; }
}
