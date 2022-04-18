using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Opsi.Cloud.Core;
using Opsi.Cloud.Core.Model;

namespace RefactorMe
{
  class Program
  {
    // Only for debugging purposes.
    static async Task Main (string[] args)
    {
      var gen = new ExternalIdService (null);

      var site1 = new Dictionary<string, object>
        { { "id", 1 },
          {
          "location",
          new Dictionary<string, object>
          { { "address", new Dictionary<string, object> { { "postalOrZipCode", "0042" } } }
          }
          }
        };

      var site2 = new Dictionary<string, object> { { "id", 2 } };

      var entityProduct = new Dictionary<string, object> { { "id", 3 } };
      var entityOrder = new Dictionary<string, object> { { "id", 4 } };

      await gen.GenerateAsync (
        new List<Dictionary<string, object>> { site1, site2 },
        new TypeMetadata { Name = "Site" });

      await gen.GenerateAsync (
        new List<Dictionary<string, object>> { entityProduct },
        new TypeMetadata { Name = "Product" });

      await gen.GenerateAsync (
        new List<Dictionary<string, object>> { entityOrder },
        new TypeMetadata { Name = "Order" });

      Console.WriteLine (site1["externalId"]);
      Console.WriteLine (site2["externalId"]);
      Console.WriteLine (entityProduct["externalId"]);
      Console.WriteLine (entityOrder["externalId"]);
    }
  }
}