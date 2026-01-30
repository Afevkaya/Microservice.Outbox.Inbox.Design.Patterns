namespace Order.Outbox.Table.Publisher.Service.Entities;

public class OrderOutbox
{
    public Guid IdempotentToken { get; set; }
    public DateTime OccuredOn { get; set; }
    public DateTime? ProcessDate { get; set; }
    // Event Type
    public string Type { get; set; }
    // Event Data
    public string Payload { get; set; }
}