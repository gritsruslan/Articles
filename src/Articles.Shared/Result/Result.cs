namespace Articles.Shared.Result;

public sealed class Result : ResultBase
{
	public Result(Error error) : base(error)
	{
	}

	public Result()
	{
	}

	public static Result Success() => new();

	public static Result Failure(Error error) => new(error);

	public static Result EmptyFailure() => new(new Error(ErrorType.Failure));

	public static implicit operator Result(Error error) => new(error);
}
