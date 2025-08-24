namespace Infrastructure.Exceptions
{
    public class AlreadyExistObjectException: Exception
    {
        public AlreadyExistObjectException(string message): base(message) { }
    }
}
