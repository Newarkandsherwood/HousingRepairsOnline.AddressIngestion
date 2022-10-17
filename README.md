# HousingRepairsOnline.AddressIngestion

This is a function which runs periodically to ingest address data into Cosmosdb from a blob storage container in Azure.

## Development

### Ingesting a new address type

To ingest a new address type e.g leasehold, copy one of the modules already defined in  `main.tf` file and replace with the appropriate values for the new address type, see below.

These are the module parameters that are specific to the address type and need to be set:

| Parameter                  | Value                                                                                      |
| -------------------------- | ------------------------------------------------------------------------------------------ |
| `address-type`             | Name of address type e.g communal, tenant, leasehold, etc as a string.                     |
| `csv-blob-path-production` | Set to terraform variable for the blob path of the address type's **production** data CSV. |
| `csv-blob-path-staging`    | Set to terraform variable for the blob path of the address type's **staging** data CSV.    |

### Terraform Variable Validation

There is validation within the module to make sure that the address type matches specific values.
When adding a new address type, update the validation block of the `address-type` variable in the module's `variable.tf` file to include the new address type as it should be written.

### Repository secrets

Add the values of the blob paths as Github secrets so that they can added as a terraform variables during the infrastructure job. See below.

### Updating the pipeline

#### Infrastructure Job

Update the `Setup tfvars` step in the `infrastructure pipeline job` to use the new Github secrets to set the terraform variables within the `env.tfvars` created during this job.

#### Build and Deploy Jobs

Add a step in the `build-and-deploy-staging` job to build and deploy the code into the staging function app and do the same for the `build-and-deploy-production` to deploy the code to production function app.

After the infrstructure job has successfully deployed the module resources for the new address type, retrieve the publish profiles for the staging and production function apps and set the value as secrets to the repository. For example: `AZURE_FUNCTIONAPP_PUBLISH_PROFILE_<ADDRESS TYPE>_STAGING` & `AZURE_FUNCTIONAPP_PUBLISH_PROFILE_<ADDRESS TYPE>_PRODUCTION`.

Rerun the build and deploy jobs once the publish profile for Staging and Production have been set.

### Trigger/Test the function

Function is triggered by a change in the CSV, make a small change in the CSV e.g add a whitespace, and this should trigger the function to ingest the data and save it into the relevant CosmosDB SQL container for the address type.
