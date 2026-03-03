namespace Articles.API.Requests;

internal sealed record CreateArticleRequest(string Title, string Data, string[] AttachedFiles);
