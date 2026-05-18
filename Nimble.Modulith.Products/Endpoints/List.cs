using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using Nimble.Modulith.Products.Data;

namespace Nimble.Modulith.Products.Endpoints;

public class ListResponse
{
    public List<ProductSummary> Products { get; set; } = [];
}

public class ProductSummary
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime DateCreated { get; set; }
    public string CreatedByUser { get; set; } = string.Empty;
}

public class List(ProductsDbContext dbContext) : EndpointWithoutRequest<ListResponse>
{
    private readonly ProductsDbContext _dbContext = dbContext;

    public override void Configure()
    {
        Get("/products");
        Tags("products");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "List all products";
            s.Description = "Retrieves a list of all products";
        });
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var products = await _dbContext.Products
            .Select(p => new ProductSummary
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                DateCreated = p.DateCreated,
                CreatedByUser = p.CreatedByUser
            })
            .ToListAsync(ct);

        Response = new ListResponse
        {
            Products = products
        };
    }
}