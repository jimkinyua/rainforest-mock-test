using System.Collections.Concurrent;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddProblemDetails();
builder.Services.AddSingleton<ProductStore>();

var app = builder.Build();

app.UseExceptionHandler();

var products = app.MapGroup("/products");

products.MapGet("/", (ProductStore store) => TypedResults.Ok(store.All()));

products.MapGet("/{id:int}", (int id, ProductStore store) =>
    store.Find(id) is { } product
        ? Results.Ok(product)
        : Results.NotFound());

products.MapPost("/", (CreateProductRequest request, ProductStore store) =>
{
    if (string.IsNullOrWhiteSpace(request.Name))
    {
        return Results.ValidationProblem(new Dictionary<string, string[]>
        {
            ["Name"] = ["Name is required."]
        });
    }

    var product = store.Add(request.Name, request.Price);
    return Results.Ok(product);
});

app.Run();

public record CreateProductRequest(string Name, decimal Price);

public record Product(int Id, string Name, decimal Price);

public class ProductStore
{
    private readonly ConcurrentDictionary<int, Product> _products = new();
    private int _nextId;

    public ProductStore()
    {
        Add("Tea, 500 g", 450m);
        Add("Coffee, 250 g", 900m);
    }

    public IReadOnlyList<Product> All() => _products.Values.OrderBy(p => p.Id).ToList();

    public Product? Find(int id) => _products.GetValueOrDefault(id);

    public Product Add(string name, decimal price)
    {
        var id = Interlocked.Increment(ref _nextId);
        var product = new Product(id, name, price);
        _products[id] = product;
        return product;
    }
}

public partial class Program;
