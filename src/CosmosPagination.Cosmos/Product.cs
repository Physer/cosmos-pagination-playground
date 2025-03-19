using Newtonsoft.Json;

namespace CosmosPagination.Cosmos;

public record Product([property: JsonProperty("id")] string Id, string Name, decimal Price);
