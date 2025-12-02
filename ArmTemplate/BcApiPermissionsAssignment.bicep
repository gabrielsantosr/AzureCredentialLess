extension 'br:mcr.microsoft.com/bicep/extensions/microsoftgraph/v1.0:0.1.8-preview'
@minLength(36)
@maxLength(36)
@description('The client id of the enterprise application you need to assign BC permissions to. Can be a service principal\'s or a system or user assigned managed identity\'s')
param clientId string

var ResourceAppIds = {
  BC: '996def3d-b36c-4153-8607-a6fd3c01b89f'
  Graph: '00000003-0000-0000-c000-000000000000' //Possibly not necessary
  Target: clientId
}
resource TARGET_SP 'Microsoft.Graph/servicePrincipals@v1.0' existing ={
  appId:ResourceAppIds.Target
}
resource BC_SP 'Microsoft.Graph/servicePrincipals@v1.0' existing = {
  appId: ResourceAppIds.BC
}
resource GRAPH_SP 'Microsoft.Graph/servicePrincipals@v1.0' existing = {
  appId: ResourceAppIds.Graph
}
var bcRoles = [
  'a42b0b75-311e-488d-b67e-8fe84f924341' // API.ReadWrite.All
  'd365bc00-a990-0000-00bc-160000000001' // Automation.ReadWrite.All
]

resource BC_API_PERMISSIONS 'Microsoft.Graph/appRoleAssignedTo@v1.0' = [
  for role in bcRoles: {
    appRoleId: role
    principalId: TARGET_SP.id
    resourceId: BC_SP.id
  }
]

//This is possibly not necessary.
resource permissionGrant 'Microsoft.Graph/oauth2PermissionGrants@v1.0' = {
  clientId: TARGET_SP.id
  consentType: 'AllPrincipals'
  resourceId: GRAPH_SP.id
  scope: 'User.Read'
}
