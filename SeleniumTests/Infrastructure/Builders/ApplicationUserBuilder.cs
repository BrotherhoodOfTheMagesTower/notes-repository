using NotesRepository.Areas.Identity.Data;

namespace SeleniumTests.Infrastructure.Builders;

public class ApplicationUserBuilder
{
    private ApplicationUser _user = new ApplicationUser();

    public ApplicationUser Build() => _user;

    public ApplicationUserBuilder WithId(string id)
    {
        _user.Id = id;
        return this;
    }

    public ApplicationUserBuilder WithFirstName(string firstName)
    {
        _user.FirstName = firstName;
        return this;
    }

    public ApplicationUserBuilder WithLastName(string lastName)
    {
        _user.LastName = lastName;
        return this;
    }

    public ApplicationUserBuilder WithEmail(string email)
    {
        _user.Email = email;
        _user.UserName = email;
        _user.NormalizedEmail = email.ToUpper();
        _user.NormalizedUserName = email.ToUpper();
        _user.EmailConfirmed = true;
        _user.SecurityStamp = Guid.NewGuid().ToString();
        return this;
    }
}
