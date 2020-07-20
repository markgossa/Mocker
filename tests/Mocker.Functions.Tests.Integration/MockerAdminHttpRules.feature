Feature: MockerAdminHttpRules
	
Scenario: Adds and retrieves a single HTTP rule from the database
Given There are no HTTP rules in the rules database
When I add 1 rules into the rule database
Then 1 rule should exist in the rule database

Scenario: Adds and retrieves a multiple HTTP rule from the database
Given There are no HTTP rules in the rules database
When I add 3 rules into the rule database
Then 3 rule should exist in the rule database