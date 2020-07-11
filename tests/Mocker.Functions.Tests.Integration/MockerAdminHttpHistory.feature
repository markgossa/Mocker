Feature: MockerAdmin

Background: Reset the test environment
	Given There is no HTTP history

Scenario: Adds and retrieves HTTP history based on HTTP method
	Given I have sent <data> to the HTTP mock using the <httpMethod> HTTP method <count> times
	When I query for those <httpMethod> requests by HTTP method
	Then the result should have <count> <httpMethod> requests with correct <data>
	Examples:
	| count | httpMethod | data |
	| 1     | DELETE     |      |
	| 3     | GET        |      |
	| 1     | PATCH      | 4    |
	| 1     | POST       | 5    |
	| 10    | PUT        | 6    |
