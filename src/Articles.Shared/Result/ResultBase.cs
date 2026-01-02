namespace Articles.Shared.Result;

public abstract class ResultBase
{
	private readonly Error? _error;

	protected ResultBase(Error error)
	{
		IsSuccess = false;
		_error = error;
	}

	protected ResultBase()
	{
		IsSuccess = true;
		_error = null;
	}

	public bool IsSuccess { get; }

	public bool IsFailure => !IsSuccess;

	public Error Error => IsFailure
		? _error!
		: throw new InvalidOperationException("Can't get an error from a successful result.");
}
