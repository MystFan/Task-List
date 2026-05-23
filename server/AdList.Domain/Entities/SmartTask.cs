using AdList.Domain.Abstract;

namespace AdList.Domain.Entities
{
    public class SmartTask : TrackingEntity
    {
        public string Title { get; set; } = null!;

        public string? Description { get; set; }

        public DateTime? DueDate { get; set; }

        public CompletionStatus CompletionStatus { get; set; }
    }
}
