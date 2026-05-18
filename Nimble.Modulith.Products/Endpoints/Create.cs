using FastEndpoints;
using Nimble.Modulith.Products.Data;
using Nimble.Modulith.Products.Models;
using System.ComponentModel.DataAnnotations;

namespace Nimble.Modulith.Products.Endpoints;

public class CreateProductRequest
{
    [Required]
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

public class CreateProductResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime DateCreated { get; set; }
    public string CreatedByUser { get; set; } = string.Empty;
}

public class Create(ProductsDbContext dbContext) : Endpoint<CreateProductRequest, CreateProductResponse>
{
    private readonly ProductsDbContext _dbContext = dbContext;

    public override void Configure()
    {
        Post("/products");
        Tags("products");
        Summary(s =>
        {
            s.Summary = "Create a new product";
            s.Description = "Creates a new product in the system";
        });
    }

    public override async Task HandleAsync(CreateProductRequest req, CancellationToken ct)
    {
        var product = new Product
        {
            Name = req.Name,
            Description = req.Description,
            DateCreated = DateTime.UtcNow,
            CreatedByUser = User.Identity?.Name ?? "anonymous"
        };

        _dbContext.Products.Add(product);
        await _dbContext.SaveChangesAsync(ct);

        Response = new CreateProductResponse
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            DateCreated = product.DateCreated,
            CreatedByUser = product.CreatedByUser
        };

        await Send.CreatedAtAsync<GetById>(new { Id = product.Id }, Response, cancellation: ct);
    }
}