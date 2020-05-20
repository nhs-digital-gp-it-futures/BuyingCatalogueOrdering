Feature: Buying Catalogue Ordering healthchecks
    As BuyingCatalogueOrderingService
    I want to be be able to check the health of my dependencies
    So that I can behave accordingly

# TODO Database has not yet been implemented, the heath check is hard coded to return healthy untill the dataabase is implmented
@6101
Scenario: 1. Database Server is up
  When the dependency health-check endpoint is hit for API
  Then the response will be Healthy
