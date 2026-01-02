namespace Articles.Shared.Result;

public sealed class Result<TValue> : ResultBase
{
	private readonly TValue _value;

	public Result(TValue value)
	{
		_value = value;
	}

	public Result(Error error) : base(error)
	{
		_value = default!;
	}

	public TValue Value =>
		IsSuccess ? _value! : throw new InvalidOperationException("Can't get a value of a failure result.");

	public static Result<TValue> Success(TValue value) => new(value);

	public static Result<TValue> Failure(Error error) => new(error);

	public static Result<TValue> EmptyFailure() => new(new Error(ErrorType.Failure));

	public static implicit operator Result<TValue>(TValue value) => new(value);

	public static implicit operator Result<TValue>(Error error) => new(error);
}
