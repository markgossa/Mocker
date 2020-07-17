Feature: MockerAdmin

Background: Reset the test environment
	Given There is no HTTP history

Scenario: Saves and retrieves HTTP history by HTTP method
	Given I have sent <body> to the HTTP mock using the <httpMethod> HTTP method <count> times
	When I query for those <httpMethod> requests by HTTP method
	Then the result should have <count> requests
	Examples:
	| count | httpMethod | body |
	| 1     | DELETE     |      |
	| 3     | GET        |      |
	| 1     | PATCH      | 4    |
	| 1     | POST       | 5    |
	| 10    | PUT        | 6    |
	| 1     | HEAD       | 7    |
	| 1     | OPTIONS    | 8    |
	| 1     | TRACE      | 9    |

Scenario: Saves and retrieves HTTP history by HTTP method and body
	Given I have sent <body1> to the HTTP mock using the <httpMethod> HTTP method <count> times
	And I have sent <body2> to the HTTP mock using the <httpMethod> HTTP method <count> times
	When I query for those <httpMethod> requests by HTTP method and body <body1>
	Then the result should have <count> requests
	Examples:
	| count | httpMethod | body1 | body2 |
	| 1     | POST       | 4     | 6     |
	| 2     | OPTIONS    | 8     | 9     |

Scenario: Saves and retrieves HTTP history by HTTP method and JSON body
	Given I have sent <body1> to the HTTP mock using the <httpMethod> HTTP method <count> times
	And I have sent <body2> to the HTTP mock using the <httpMethod> HTTP method <count> times
	When I query for those <httpMethod> requests by HTTP method and body <body1>
	Then the result should have <count> requests
	Examples:
	| count | httpMethod | body1                             | body2                                       |
	| 1     | POST       | {"name": "mark"}                  | {"name": "markg"}                           |
	| 2     | OPTIONS    | {"name": "mark","gender": "male"} | {"name": "mark","favouriteTeam": "Chelsea"} |

Scenario: Saves and retrieves HTTP history by header
	Given I have sent a <httpMethod> request to the HTTP mock with header key <headerKey> and value <headerValue1>
	And I have sent a <httpMethod> request to the HTTP mock with header key <headerKey> and value <headerValue2>
	When I query for that request by <httpMethod> method and header key <headerKey> and value <headerValue1>
	Then the result should have one <httpMethod> request with header key <headerKey> and value <headerValue1>
	Examples:
	| httpMethod | headerKey | headerValue1 | headerValue2 |
	| DELETE     | header1   | 1            | 9            |
	| GET        | header2   | 2            | 8            |
	| PATCH      | header3   | 3            | 7            |
	| POST       | header4   | 4            | 6            |
	| PUT        | header5   | 5            | 4            |
