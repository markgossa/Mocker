Feature: MockerAdmin
	In order to avoid silly mistakes
	As a math idiot
	I want to be told the sum of two numbers

Background: Reset the test environment
	Given There is no HTTP history

@mytag
Scenario: Adds and retrieves HTTP history based on HTTP method
	Given I have called the HTTP mock using a GET HTTP method
	When I query for that request by HTTP method
	Then the result should be returned with the correct request count


	#| count1 | httpMethod1 | count2 | httpMethod2 |
	#| 1      | get         | 0      | null        |
	#| 2      | post        | 1      | get         |