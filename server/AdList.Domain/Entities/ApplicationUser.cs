using AdList.Domain.Abstract;

namespace AdList.Domain.Entities
{
    public class ApplicationUser : EntityBase
    {
        public string Email { get; set; } = null!;

        public string? Name { get; set; }
    }
}
