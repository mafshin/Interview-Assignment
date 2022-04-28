# Configuration
Insurance Api behaviour can be configured via [appsettings.json](/src/Insurance.Api/appsettings.json) configuration file.

# Tests
There exists a test project named `Insurance.Tests` which contains tests of Insurance Api.

Some of tests are using xUnit `Theory` attribute to easily add new test scenarios as
business rules about insurance calculation changes over time. Ensure to give a meaningful 
name to `TestSceanrio` `testName` argument as it will represent your test scenario in test reports.

Also during development, you can easily write and send sample requests in [SampleRequests.http](/tests/SampleRequests.http) file.  
This file can be used via VSCode [REST Client](https://marketplace.visualstudio.com/items?itemName=humao.rest-client) extension.

# Refactoring
Several things had to be changed in the Insurance Api code. 
1. Configurations had to be read from a configuration file so it can be easily changed per deployment or environement.
2. Http clients used for accessing other Apis is managed by a central factory with named clients.
This allows optimum reusing of clients already created and also leaves the configuration per Api in a
central place.
3. All operations in Api which involves IO (disk or remote resource) must be done asynchronously.
Async operations allows better usage of system resources while Api is waiting on the response from IO operations.
4. Initially BusinessRules class was all about accessing Product Api actions so a ProductApiClient is 
introduced which simplifies access to Product Api actions. In a later stage, all calculations related to insurance 
which in fact is business rules has been moved from controller to business rules class.

For the above refactoring, method extraction is used. For ProductApiClient a Pull Up approach is used
to extract an interface to decouple HomeController from Product Api implementation.

# Order (Shopping Cart) Insurance
For calculating insurance of a shopping cart, we assumed that it can contain multiple products and
each product may have been added multiple times.

# Feature 2
For Task 4, we assume that new rule only applies to digital cameras which are insurable.
Also this rule is only effective for an order (via /order endpoint).

# Feature 3
For this task, we assume that added surcharge per product type is kept in memory, so it
will reset between Insurance Api restarts.
Also the surcharge rate is applied to /product endpoint in addition to /order endpoint.

# Fault Tolerance
Access to ProductApi resources is configured by two retry and circuit breaker policies.
In case of ProductApi getting down or slowness the policies can be configured to retry
requests or break the circuit to prevent cascade effect of retrying failed requests. 
These policies can be configured via `FaultTolerance` section in [appsettings.json](/src/Insurance.Api/appsettings.json)

# Response Caching
A configurable in memory cache is added to cache responses from product api.