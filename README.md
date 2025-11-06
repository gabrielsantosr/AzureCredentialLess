# AzureCredentialLess
Explorative project to demonstrate the possibility to query data stored in Microsoft Dynamics CRM in a tenant from an Function App in another tenant without using passwords, nor secrets, nor certificates.
Same thing can be applied to ERPs.

## Scenario.
In the tenant of a company (let's call it "PROVIDER"), there is a Function App that needs to interact with CRMs of its customers.

## What is necessary in PROVIDER's tenant:
- Function app
- Managed Identity
- Multi-tenant App registration (clientId)
- Add the managed identity as the identity of the App Function. As far as I tried, it only works with user-assigned identitiies, not with system-assigned ones.
- Add a federated credential in the app registration that points to the Managed Identity.

## What is necessary in each customer's tenant:
- Service principal with the id of PROVIDER's App registration ( This can be done using azure cli `az ad sp create --id <client-id>` , azure power shell or http request)
- Include the service principal as an app user of the Dataverse (It won't show up in the list when you try to add it; type the clientId to see it.)
- Grant necessary roles to the app user within Dataverse

By including a local Service Principal of PROVIDER's App Registration, customers can grant PROVIDER whichever roles they need.

PROVIDER, from it's side, manages however they wan't to authenticate.

You could still use certificates or secrets, if you are used to it. The multi-tenancy is already solving the problem of having to handle customers' secrets, because the credentials are managed from your App registration, from your tenant.

Adding the federated credential eliminates the need of credentials altogether.

## References

https://dreamingincrm.com/2025/02/06/secretless-cross-tenant-access-logic-apps-dataverse/

https://devblogs.microsoft.com/identity/access-cloud-resources-across-tenants-without-secrets/

https://learn.microsoft.com/en-us/azure/devops/integrate/get-started/authentication/service-principal-managed-identity?view=azure-devops
