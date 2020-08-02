Feature: MockerAdminHttpHistory

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
	| 1     | TRACE      |      |

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

Scenario: Saves and retrieves HTTP history by HTTP method and route
	Given I have made a <httpMethod> HTTP request <count> times to route <route1>
	And I have made a <httpMethod> HTTP request <count> times to route <route2>
	When I query for those <httpMethod> requests by HTTP method and route <route1>
	Then the result should have <count> requests
	Examples:
	| count | httpMethod | route1 | route2 |
	| 1     | POST       | api/4  | api/6  |
	| 2     | OPTIONS    | api/8  | api/9  |

Scenario: Saves and retrieves HTTP history by HTTP method and correct timestamp returned
	Given I have sent <body> to the HTTP mock using the <httpMethod> HTTP method 1 times
	When I query for those <httpMethod> requests by HTTP method
	Then the result should correct timestamp data
	Examples:
	| httpMethod | body |
	| GET        |      |
	| POST       | 5    |

Scenario: Saves and retrieves HTTP history (large request) by HTTP method
	Given I have sent a large request body to the HTTP mock using the POST HTTP method 2 times
	When I query for those POST requests by HTTP method
	Then the result should have 2 requests
	And the large request body should be correct
