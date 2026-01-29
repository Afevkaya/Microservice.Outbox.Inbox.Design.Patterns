namespace Order.Api.Models.Entities;

public class Order
{
    public Guid Id { get; set; }
    public Guid BuyerId { get; set; }
    public decimal TotalPrice { get; set; }
    public ICollection<OrderItem> OrderItems { get; set; }
    public DateTime CreatedDate { get; set; }
}