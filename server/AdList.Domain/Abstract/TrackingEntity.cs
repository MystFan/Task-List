namespace AdList.Domain.Abstract
{
    public abstract class TrackingEntity : EntityBase, ITrackingEntity
    {
        public string Author { get; set; } = null!;

        public DateTime CreatedAt { get; set; }

        public DateTime? ModifiedAt { get; set; }
    }
}
