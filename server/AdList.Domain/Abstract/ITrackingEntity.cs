namespace AdList.Domain.Abstract
{
    public interface ITrackingEntity
    {
        public string Author { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? ModifiedAt { get; set; }
    }
}
