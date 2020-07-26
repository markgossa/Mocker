Feature: MockerHttp
	
Scenario: Applies rules based on method
Given There are no HTTP rules in the rules database
When I add a rule based on <filterMethod> method into the rule database which returns <responseBody>
And I send a <filterMethod> request to route null with body IgnoreMe
Then I should receive a response with <responseBody>
Examples:
| filterMethod | responseBody     |
| GET          | You sent a GET   |
| POST         | You sent a POST  |
| PUT          | {"Name": "Mark"} |

Scenario: Applies rules based on body
Given There are no HTTP rules in the rules database
When I add a rule based on <filterBody> body into the rule database which returns <responseBody>
And I send a <filterMethod> request to route null with body <filterBody>
Then I should receive a response with <responseBody>
Examples:
| filterMethod | filterBody       | responseBody               |
| POST         | Hello world!     | Hello back!                |
| PUT          | Hey world!       | Hey back!                  |
| PATCH        | {"Name": "Mark"} | {"Response": "Hello Mark"} |

Scenario: Applies rules based on route
Given There are no HTTP rules in the rules database
When I add a rule based on <filterRoute> route into the rule database which returns <responseBody>
And I send a <filterMethod> request to route <filterRoute> with body HelloWorld
Then I should receive a response with <responseBody>
Examples:
| filterMethod | filterRoute | responseBody  |
| POST         | api/route1  | Hello back!   |
| PUT          | api/route66 | Hello back 66!|

Scenario: Applies rules based on header
Given There are no HTTP rules in the rules database
When I add a header-based rule to the rule database which returns <responseBody>
And I send a <filterMethod> request which contains the filter headers
Then I should receive a response with <responseBody>
Examples:
| filterMethod | responseBody  |
| POST         | Hello back!   |
| PUT          | Hello back 66!|

Scenario: Applies rules based on query
Given There are no HTTP rules in the rules database
When I add a query-based rule to the rule database which returns <responseBody>
And I send a request which contains the filter query
Then I should receive a response with <responseBody>
Examples:
| responseBody |
| Hello back!  |

Scenario: Applies rules based on method and body
Given There are no HTTP rules in the rules database
When I add a rule filter on <filterMethod> method and <filterBody> body into the rule database which returns <responseBody>
And I send a <filterMethod> request to route null with body <filterBody>
Then I should receive a response with <responseBody>
Examples:
| filterMethod | filterBody            | responseBody    |
| POST         | Hello, this is a POST | You sent a POST |
| PUT          | Hello, this is a PUT  | You sent a PUT  |

Scenario: Applies rules based on complex filter and ignores non matching requests
Given There are no HTTP rules in the rules database
When I add a complex rule which filters on method, body, headers, route and query with 0 delay
And I send a GET request to route null with body HelloWorld!
Then I should receive the default response

Scenario: Applies rules based on complex filter and handles matching requests with no delay
Given There are no HTTP rules in the rules database
When I add a complex rule which filters on method, body, headers, route and query with 0 delay
And I send a matching complex request
Then I should receive the correct complex response with correct response properties with 0 delay

Scenario: Applies rules based on complex filter and handles matching requests with delay
Given There are no HTTP rules in the rules database
When I add a complex rule which filters on method, body, headers, route and query with 2000 delay
And I send a matching complex request
Then I should receive the correct complex response with correct response properties with 2000 delay