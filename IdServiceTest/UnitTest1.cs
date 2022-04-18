using Microsoft.VisualStudio.TestTools.UnitTesting;
using Opsi.Cloud.Core;
using Opsi.Cloud.Core.Model;
using System;
using System.Collections.Generic;

/*
Comments
Due to unfamiliarity with C# and possibly also an issue with the environment setup on my personal PC, I was unable to get Unit tests to run.
I receive a "'ExternalIdService' is inaccessible due to its protection level"
For some reason my IDE also does not give me auto completion for the Unit test, so I'm unable to verify if my code below is valid.

I have written a unit test below.  It is not an ideal test.

I initially attempted to unit test the private methods but after going down a rabbit-hole on how to do that in C# I discovered I had more basic issues
such as how to even access the ExternalIdService within the UnitTest.

Given the ExternalIdService and more time and experience with C#, I would do the following:
- Unit test the `GetEntity` method.
    - Test against a valid object 2 or 3 keys deep
    - Test against an object that's missing the keys
    - Test against an attribute selector that is invalid/doesn't match the regex.
- (Possibly) Unit test `regExEntityDescriptions`
- Unit test `generateEntityOutput` method
    - Test against each type of output
    - (Possibly) test against a combination of outputs in a single entity
    - Test against invalid types
*/
namespace IdServiceTest
{

    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        async public void ExternalIdForOrder ()
        {
            var externalIdService = new ExternalIdService (null);

            var entityOrder = new Dictionary<string, object> { { "id", 4 } };
            await externalIdService.GenerateAsync (
                new List<Dictionary<string, object>> { entityOrder },
                new TypeMetadata { Name = "Order" });

            string shouldStartWith = DateTime.Now.ToString ("ddMMyyyy");
            Assert.StartsWith(entityOrder["externalId"], "ORD-" + shouldStartWith);
        }
    }
}