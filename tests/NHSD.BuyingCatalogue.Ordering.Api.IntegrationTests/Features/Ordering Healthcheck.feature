Feature: Buying Catalogue Ordering healthchecks
    As BuyingCatalogueOrderingService
    I want to be be able to check the health of my dependencies
    So that I can behave accordingly

@6101
Scenario: Database Server is up
  When the dependency health-check endpoint is hit for API
  Then the response will be Healthy
  
@5648
Scenario: Database Server is down
    Given The Database Server is down
  When the dependency health-check endpoint is hit for API
    Then the response will be Unhealthy
