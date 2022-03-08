# RefactorMe

The ExternalIdService is badly written and is in dire need of a refactor.
Its purpose is to generate an external ID for entities that is passed to it based on a string template that is configurable by a user.

Template eg.
 - `ORD-{date:ddMMyyyy}-{increment:order}` => "ORD-01012022-01"
 - `ST-{entity:location.address.postalOrZipCode}-{increment:site}` => "ST-0042-01"

For entity:
```
{
  id: 1,
  location: {
    address: {
      postalOrZipCode: "0042" 
    } 
  }
}
```
 - `PRD-{increment:product}` => "PRD-01"

Only refactor the `RefactorMe` project where you may change code, add comments and add more files. 
The rest is shared code and should not be changed, though you may comment on all design elements of the entire repo.

The solution is runnable as in: no compile errors. Run-time errors may occur.

# Assessment Criteria

Code readability
 - Neat and consistent formatting
 - Naming conventions
  
Code structure
 - Separation and encapsulation of concerns
 - Extensibility and maintainability
 - Error handling

Testing
 - A single unit test will do

Good luck.
