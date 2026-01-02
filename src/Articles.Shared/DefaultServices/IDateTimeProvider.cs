namespace Articles.Shared.DefaultServices;

public interface IDateTimeProvider
{
	DateTime UtcNow { get; }
}
