namespace Logistics.Domain.Entities
{
    public class Transporter
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;
        public string Document { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string Phone { get; set; } = default!;
    }
}
