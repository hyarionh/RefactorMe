using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Opsi.Architecture;
using Opsi.Cloud.Core.Interface;
using Opsi.Cloud.Core.Model;

namespace Opsi.Cloud.Core
{
  internal class ExternalIdService : IExternalIdGeneratorService
  {
    private readonly ILogger<ExternalIdService> _logger;

    /*
    Comments:
    I assume this is the same in C# as other languages, keeping the state here for NamingPattern while setting/using it in an Async method
    will result in unintended behaviour if 2 requests come in simultaneously.
    i.e. second request will change NamingPattern while first request is still running

    In Java it's considered very bad practice to store state in a Service for this same reason.
    */
    // private string NamingPattern = @"";

    private readonly string regExEntityDescriptions = @"({[a-z]*:[^:]*})";

    public ExternalIdService (
      ILogger<ExternalIdService> logger)
    {
      _logger = logger;
    }

    /*
    Comments
    There is an IDE warning about this method: "This async method lacks 'await' operators".
    Unfortunately my C# knowledge is not quite sufficient to know how best to resolve this.  
    It appears from reading up that potentially it could be resolved by ending the method with `return Task.fromResult()`, 
    however there appears to be a difference in how Exception are then treated, making me wary of implementing this unless I fully
    understand the implications of the change.
    */
    public async Task<ServiceActionResult> GenerateAsync (List<Dictionary<string, object>> entities, TypeMetadata typeMetadata)
    {
      var result = new ServiceActionResult ();

      var NamingPattern = getNamingPattern (typeMetadata.Name);

      if (NamingPattern == string.Empty)
      {
        return result;
      }

      /*
      Comments
      Based on the spec, which says "based on a string template that is configurable by a user", this function below is obviously just for the purposes of testing
      and would be replaced with something fetching the user-defined template.
      Depending on the frequency of use, the number of users, etc. it may be best moved into a class or service where the naming patterns can be pre-prepared
      rather than running regex on every request for an external id.
      */
      var entityDescriptions = Regex.Split (NamingPattern, regExEntityDescriptions).Where (s => s != string.Empty).ToArray ();

      foreach (var entity in entities)
      {
        entity["externalId"] = generateEntityOutput (entity, entityDescriptions);
      }

      return result;
    }

    private string generateEntityOutput (Dictionary<string, object> entity, string[] entityDescriptions)
    {
      var sb = new StringBuilder ();
      foreach (var section in entityDescriptions)
      {
        if (section.StartsWith ('{'))
        {
          var args = Regex.Split (section, @":|{|}").Where (s => s != string.Empty).ToArray ();
          switch (args[0])
          {
            case "date":
              sb.Append (GetDate (args[1]));
              break;

            case "increment":
              sb.Append (GetIncrement (args[1]));
              break;

            case "entity":
              sb.Append (GetEntity (args[1], entity));
              break;

            case "reference":
              sb.Append (GetReference (args[1], entity));
              break;

            default:
              break;
          }
        }
        else
        {
          sb.Append (section);
        }
      }
      return sb.ToString ();
    }

    private string getNamingPattern (string name)
    {
      // Example Templates
      switch (name)
      {
        case "Order":
          return @"ORD-{date:ddMMyyyy}-{increment:order}"; // ORD-12122022-01

        case "Site":
          return @"ST-{entity:location.address.postalOrZipCode}-{increment:site}"; // ST-0042-01

        case "Product":
          return @"PRD-{increment:product}"; // PRD-01

        default:
          return "";
      }
    }

    private string GetDate (string format)
    {
      return DateTime.Now.ToString (format);
    }

    private string GetIncrement (string type)
    {
      // Need to get this increment from Redis
      return new Random ().Next (100)
        .ToString ();

    }

    /*
    Comments:
    Depending on the rest of the architecture, I would usually expect an exception to be thrown here. 
    If it's not possible to generate a correct externalId, in most apps this would be a breaking issue.
    In rare cases, e.g. if it's a visual indicator for the UI only, some product owners might prefer it to not break and rather display invalid text.
    */
    private string GetEntity (string attribute, object entity)
    {
      var splitPath = Regex.Split (attribute, @"\.")
        .Where (s => s != String.Empty).ToArray ();
      var resultObject = entity;

      foreach (var attr in splitPath)
      {
        try
        {
          /*
          Comments:
          This is not likely the best way to be doing this, my inexperience with C# showing. 
          As GetType().GetProperty(attr) was giving me a lot of issues (and felt potentially inefficient),
          I opted for type casting here.  Without better knowledge of the rest of the system I cannot tell if this is a poor choice or not.
          In a production system I would not do this without approval/advice from the team lead.
          I have opted for returning an empty string when no relevant attribute is found.  This is something I would normally escalate to the 
          team lead or product owner to find out how they wish it to be handled.  e.g. throw an exception or use a different string such as "invalid".
          */
          var dict = ((Dictionary<string, object>) resultObject);
          if (dict.ContainsKey (attr))
          {
            dict.TryGetValue (attr, out resultObject);
          }
          else
          {
            resultObject = "";
            break;
          }
        }
        catch
        {
          // Would catch a casting exception. Try/Catch would depend on decision from Team Lead/Product Owner.
          resultObject = "";
          break;
        }
      }

      return resultObject.ToString ();
    }

    private string GetReference (string attribute, object entity)
    {
      return entity.GetType ().GetProperty (attribute).GetValue (entity, null).ToString ();
    }
  }

}