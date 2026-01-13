using Articles.Application.AuthUseCases.Commands.ConfirmEmail;

namespace Articles.Application.UseCases.Commands;

internal sealed class TestCommandHandler : ICommandHandler<TestCommand>
{
	public async Task<Result> Handle(TestCommand request, CancellationToken cancellationToken)
	{
		await Task.Delay(100, cancellationToken);
		throw new NotImplementedException("This useCase is not implemented");
	}
}
