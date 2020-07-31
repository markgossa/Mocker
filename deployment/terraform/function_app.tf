resource "azurerm_function_app" "example" {
  name                       = "app-mocker-automated-tests-1234"
  location                   = azurerm_resource_group.resource_group_1.location
  resource_group_name        = azurerm_resource_group.resource_group_1.name
  app_service_plan_id        = azurerm_app_service_plan.app_service_plan.id
  storage_account_name       = azurerm_storage_account.storage_account_1.name
  storage_account_access_key = azurerm_storage_account.storage_account_1.primary_access_key
  version                    = "~3"
  site_config {
      ip_restriction {
        ip_address = var.ip_address
      }
  }
}
