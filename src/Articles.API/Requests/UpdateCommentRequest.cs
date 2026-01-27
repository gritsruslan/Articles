namespace Articles.API.Requests;

internal sealed record UpdateCommentRequest(Guid CommentId, string Content);
