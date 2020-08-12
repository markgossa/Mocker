resource "azurerm_storage_account" "storage_account_1" {
  name                     = "samockertests1234"
  resource_group_name      = azurerm_resource_group.resource_group_1.name
  location                 = azurerm_resource_group.resource_group_1.location
  account_tier             = "Standard"
  account_replication_type = "LRS"
}
