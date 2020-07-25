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
And I send a GET request which contains the filter query
Then I should receive a response with <responseBody>
Examples:
| responseBody |
| Hello back!  |
