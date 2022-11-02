namespace Core.Authentication.API.Contracts.Requests;

public class CreateCustomerRequest
{
    public string Name { get; }

    public string Password { get; }

    public string Email { get; }


    public CreateCustomerRequest(string name, string password, string email)
    {
        Name = name;
        Password = password;
        Email = email;
    }
}