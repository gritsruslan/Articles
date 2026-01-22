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

	public async Task<IEnumerable<BlogReadModel>> GetReadModels(CancellationToken cancellationToken)
	{
		return await dbContext.Blogs.Select(b => new BlogReadModel
		{
			Id = b.Id,
			Title = b.Title,
			ArticlesCount = 0,
			LastArticleCreatedAt = null
		}).ToListAsync(cancellationToken);
	}
}
