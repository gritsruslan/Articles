namespace Articles.API.Requests;

public sealed record CreateArticleRequest(int BlogId, string Title, string Data, string[] AttachedFiles);
