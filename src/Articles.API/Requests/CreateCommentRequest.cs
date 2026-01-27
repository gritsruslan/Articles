namespace Articles.API.Requests;

internal sealed record CreateCommentRequest(Guid ArticleId, string Content);
