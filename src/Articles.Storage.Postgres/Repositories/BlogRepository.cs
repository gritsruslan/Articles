using Articles.Domain.ReadModels;
using Articles.Domain.ValueObjects;
using Articles.Storage.Postgres.Entities;

namespace Articles.Storage.Postgres.Repositories;

internal sealed class BlogRepository(ArticlesDbContext dbContext) : IBlogRepository
{
	public async Task<BlogId> CreateBlog(BlogTitle title, CancellationToken cancellationToken)
	{
		var blog = new BlogEntity
		{
			Title = title.Value,
		};

		var created = await dbContext.Blogs.AddAsync(blog, cancellationToken);
		await dbContext.SaveChangesAsync(cancellationToken);

		return BlogId.Create(created.Entity.Id);
	}

	public Task<Blog?> GetById(BlogId id, CancellationToken cancellationToken)
	{
		return dbContext.Blogs
			.Where(b => b.Id == id.Value)
			.Select(b => new Blog
		{
			Id = BlogId.Create(b.Id),
			Title = BlogTitle.CreateVerified(b.Title)
		}).FirstOrDefaultAsync(cancellationToken);
	}

	public async Task<IEnumerable<BlogReadModel>> GetReadModels(
		int skip, int take, CancellationToken cancellationToken)
	{
		FormattableString query =
			$"""
			SELECT
				b."Id",
				b."Title",
				ArticlesCount,
				LastArticleCreatedAt
			FROM (
				SELECT
					b."Id",
					b."Title",
					COUNT(a."Id") OVER (PARTITION BY a."BlogId")      AS ArticlesCount,
					MAX(a."CreatedAt") OVER (PARTITION BY a."BlogId") AS LastArticleCreatedAt
				FROM "Blogs" b
				LEFT JOIN "Articles" a ON a."BlogId" = b."Id"
			) b
			GROUP BY b."Id", b."Title", ArticlesCount, LastArticleCreatedAt
			ORDER BY ArticlesCount DESC, b."Title"
			LIMIT {take}
			OFFSET {skip};
			""";

		return await dbContext.Database
			.SqlQuery<BlogReadModel>(query)
			.ToListAsync(cancellationToken);
	}
}
