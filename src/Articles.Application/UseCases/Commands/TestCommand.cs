using Articles.Shared.Monitoring;

namespace Articles.Application.UseCases.Commands;

public sealed record TestCommand() : ICommand, ITracedCommand;
