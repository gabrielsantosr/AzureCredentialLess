# AzureCredentialLess
Explorative project to demonstrate the possibility to access resources in a different tenant without using passwords, nor secrets, nor certificates.

In this project, from an Function App in my tenant, I am querying data on other tenants.

Same can be done from a Logic App, a VM (any kind of resource to which a managed identity can be assigned to), to any other resource in another tenant.

There are two concepts that are being put together here:
1) **Multi-tenancy:** Using a multi-tenant app registration allows the creation of service principals in other tenants. The authentication method (secret, certificate and/or federated credentials), are managed from your tenant. Other tenants admins can determine which resources their local service principal has access to. You manage your credentials, they manage your access within their tenant.
2) **Federated credentials:** You can add a federated credential to your app registration to trust a managed identity within your tenant. And you can use that managed identity as an identity of a Function App or a Web App. That way, your app does not need to hold credentials to authenticate with your App Registration.

**App :arrow_right: Managed Identity :arrow_right: App registration :arrow_right: Service principal** 



## Scenario.
In the tenant of a company (let's call it "PROVIDER"), there is a Function App that needs to interact with resources of its customers.

## What is necessary in PROVIDER's tenant:
- Function app
- Managed Identity (MI)
- Multi-tenant App registration (clientId)
- Add necessary API permissions to the App registration. [Cheat sheat](#api-permissions-cheat-sheet)
- Add a federated credential in the app registration for the MI.
 
 #### _Comments:_
 
 _Regarding the managed Identity, it can be either the Function app's system-assigned or a user-assigned one, assigned to the Function App._

 _Specific to this project: It needs added the environment variable (EV) **client_id**, and, if using a user-assigned MI, the EV **identity_client_id**._

 #### _API Permissions Cheat Sheet_

| Resource | API permissions |
| - | - |
| BC | API.ReadWrite.All, Automation.ReadWrite.All
| CRM | _None_
| Storage Account | _None_ |
| Key Vault | _None_ |

## What is necessary in each customer's tenant:
- Create Service principal with the id of PROVIDER's App registration ( This can be done using azure cli `az ad sp create --id <client-id>` , azure power shell or http request). :exclamation::exclamation:_This step is **not necessary for BC**: If not yet existent, the service principal is created and its API permissions granted when consent is granted from the BC environment._ 
- Include the service principal as an app user of the Dataverse (It won't show up in the list when you try to add it; type the clientId to see it.) or Microsoft Entra Application in the BC environment.
- For CRM and BC, grant necessary roles to the app user within Dataverse or BC. For Azure subscription resources, grant necessary access to the service principal from Access Control (IAM). 

By including a local Service Principal of PROVIDER's App Registration, customers can grant PROVIDER whichever roles they need in their CRM or ERP environments, or their Azure resources.

PROVIDER, from their side, can manage authentication however they want to.

You could still use certificates or secrets, if you are used to it. The multi-tenancy is already solving the problem of having to handle customers' secrets, because the credentials are managed from your App registration, from your tenant, and there is only one credential
needed.
Adding the federated credential eliminates credential management altogether.

## EndPoints
_Comment: Add the function key in the `x-functions-key` header or the query `code` parameter_.  

##### QueryBC
Method: POST

Body sample:
```
{
	"TenantId":"00000000-0000-0000-0000-000000000000",
	"EnvironmentName":"production",
	"ODataQuery":"companies?$select=id,name"
}
```

##### QueryDataverse
_(Using HTTPClient)_

Method: POST

Body sample:
```
{
	"TenantId":"00000000-0000-0000-0000-000000000000",
	"EnvironmentUrl":"https://my-crm.crm11.dynamics.com/",
	"ODataQuery":"accounts?$top=1&$select=name,_defaultpricelevelid_value"
}
```

##### FetchDataverse
_(Using ServiceClient)_
Method: POST

Body sample:
```
{
	"TenantId":"00000000-0000-0000-0000-000000000000",
	"EnvironmentUrl":"https://my-crm.crm11.dynamics.com/",
	"FetchXML":"<fetch><entity name='account'><attribute name='name' /></entity></fetch>"
}
```

##### GetBlobsDetails
Method: POST

Body sample:
```
{
    "TenantId":"00000000-0000-0000-0000-000000000000",
    "Account":"mystorageaccountname",
    "Container":"my-container-name",
    "BlobsPrefix": "myfolder/" // can be null
}
```

##### GetKeyVaultSecret
Method: POST

Body sample:
```
{
    "TenantId":"00000000-0000-0000-0000-000000000000",
    "KeyVaultName":"my-kv-name",
    "KeyVaultSecret":"mySecretName"
}
```


## References

https://dreamingincrm.com/2025/02/06/secretless-cross-tenant-access-logic-apps-dataverse/

https://devblogs.microsoft.com/identity/access-cloud-resources-across-tenants-without-secrets/

https://learn.microsoft.com/en-us/azure/devops/integrate/get-started/authentication/service-principal-managed-identity?view=azure-devops

## To Do
The is still the question regarding unit tests, since the managed identity won't be accessible from local execution.
Probably replacing ManagedIdentity with DefaultAzureCredentials. See: https://learn.microsoft.com/en-us/dotnet/api/azure.identity.defaultazurecredential?view=azure-dotnet, which will still pick up the managed identity online.

I also found this article, which is not exactly the same scenario, but could help: 
https://dreamingincrm.com/2021/11/16/connecting-to-dataverse-from-function-app-using-managed-identity/
Something interesting about this last mentioned linked, is that it is using the managed identity as the service user of the CRM, instead of a service principal. 
