# Configuration
Insurance Api behaviour can be configured via [appsettings.json](/src/Insurance.Api/appsettings.json) configuration file.

# Tests
There exists a test project named `Insurance.Tests` which contains tests of Insurance Api.

Some of tests are using xUnit `Theory` attribute to easily add new test scenarios as
business rules about insurance calculation changes over time. Ensure to give a meaningful 
name to `TestSceanrio` `testName` argument as it will represent your test scenario in test reports.

Also during development, you can easily write and send sample requests in [SampleRequests.http](/tests/SampleRequests.http) file.  
This file can be used via VSCode [REST Client](https://marketplace.visualstudio.com/items?itemName=humao.rest-client) extension.