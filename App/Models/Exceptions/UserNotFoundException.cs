namespace MyWishly.App.Models.Exceptions
{
    public class UserNotFoundException : Exception
    {
        public UserNotFoundException(string name) : base($"User '{name}' was not found.")
        {
        }

        public UserNotFoundException(string name, Exception innerException) : base($"User '{name}' was not found.", innerException)
        {
        }
    }
}
