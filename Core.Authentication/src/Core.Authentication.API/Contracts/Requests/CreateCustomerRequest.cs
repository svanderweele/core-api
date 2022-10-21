namespace Core.Authentication.API.Contracts.Requests;

public class CreateCustomerRequest
{
    public string Username { get; }

    public string FullName { get; }

    public string Email { get; }

    public DateTime DateOfBirth { get; }

    public CreateCustomerRequest(string username, string fullName, string email, DateTime dateOfBirth)
    {
        Username = username;
        FullName = fullName;
        Email = email;
        DateOfBirth = dateOfBirth;
    }
}