namespace Articles.API.Requests;

public sealed record CreateArticleRequest(string Title, string Data, string[] AttachedFiles);
