# Deployment and Configuration Guide

## Azure Function App Configuration

### AzureWebJobsStorage Connection String

The `AzureWebJobsStorage` connection string is required for Azure Functions runtime to store function execution state and metadata.

#### Symptoms of Misconfiguration
- Health trace: `azure.functions.webjobs.storage: Unhealthy`
- Error: `Unable to access AzureWebJobsStorage, errorCode=AuthorizationFailure`
- Function execution failures

#### Resolution Steps

1. **Verify Connection String in Azure Portal**
   - Navigate to your Function App in Azure Portal
   - Go to Configuration → Application settings
   - Check the `AzureWebJobsStorage` setting value

2. **Validate Storage Account Access**
   - Ensure the storage account exists and is accessible
   - Verify the connection string format: `DefaultEndpointsProtocol=https;AccountName=<account>;AccountKey=<key>;EndpointSuffix=core.windows.net`

3. **Check RBAC Permissions (if using Managed Identity)**
   - Ensure the Function App's Managed Identity has the following roles on the storage account:
     - Storage Blob Data Contributor
     - Storage Queue Data Contributor
     - Storage Table Data Contributor

4. **Rotate Keys if Necessary**
   - If using storage account keys, regenerate them in Azure Portal
   - Update the connection string in Function App settings
   - Restart the Function App

5. **Verify SAS Token Expiration**
   - If using SAS tokens, ensure they haven't expired
   - Generate new SAS tokens with appropriate permissions and expiration dates

#### Testing the Fix

After updating the configuration:
1. Restart the Function App
2. Monitor Application Insights for health status
3. Verify no `AuthorizationFailure` errors appear
4. Test HTTP triggers to confirm they execute successfully

## Code Changes

### Fixed Issues
- **DivideByZeroException**: Removed intentional divide-by-zero code from HttpTrigger1
- Function now returns a simple success message without risky operations

### Testing Locally
```bash
# Build the solution
dotnet build

# Create local.settings.json with required configuration
# Example local.settings.json:
# {
#   "IsEncrypted": false,
#   "Values": {
#     "AzureWebJobsStorage": "UseDevelopmentStorage=true",
#     "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated"
#   }
# }

# Run the function locally (requires Azure Functions Core Tools)
func start
```

**Note**: For local development, you can use `"AzureWebJobsStorage": "UseDevelopmentStorage=true"` with Azurite storage emulator, or provide an actual Azure Storage connection string.

### Deployment
```bash
# Deploy to Azure Function App
func azure functionapp publish <function-app-name>
```
