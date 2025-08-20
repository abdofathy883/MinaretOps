namespace Core.Interfaces
{
    public interface IAuditable
    {
        public DateOnly CreatedAt { get; set; }
        public DateOnly? UpdatedAt { get; set; }
    }
}
