Feature: MockerAdminHttpRules
	
Scenario: Adds and retrieves a single HTTP rule from the database
	Given There are no HTTP rules in the rules database
	When I add 1 rules into the rule database
	Then 1 rule should exist in the rule database with correct settings

Scenario: Adds and retrieves a multiple HTTP rule from the database
	Given There are no HTTP rules in the rules database
	When I add 3 rules into the rule database
	Then 3 rule should exist in the rule database with correct settings

Scenario: Adds and retrieves a large HTTP rule from the database
	Given There are no HTTP rules in the rules database
	When I add 2 rules into the rule database with large action body
	Then 2 rules with large action body should exist in the rule database
