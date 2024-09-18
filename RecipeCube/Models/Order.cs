namespace RecipeCube.Models;

public partial class Order
{
    public long OrderId { get; set; }

    public int? UserId { get; set; }

    public DateTime? OrderTime { get; set; }

    public int? TotalAmount { get; set; }

    public bool? Status { get; set; }
}
