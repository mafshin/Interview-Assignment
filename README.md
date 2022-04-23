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

For the above refactoring, method extraction is used.