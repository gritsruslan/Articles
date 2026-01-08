namespace Articles.Shared.Monitoring;

public interface ITracedRequest;

public interface ITracedQuery : ITracedRequest;

public interface ITracedCommand : ITracedRequest;

