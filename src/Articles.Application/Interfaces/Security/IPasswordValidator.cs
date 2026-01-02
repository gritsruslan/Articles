namespace Articles.Application.Interfaces.Security;

public interface IPasswordValidator
{
	Result Validate(string password);
}
