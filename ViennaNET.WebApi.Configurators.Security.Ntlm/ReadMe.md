# Build with NTLM-based authorization services

Contains:
* NtlmSecurityConfigurator - registers authentication scheme for NTLM, ISecurityContextFactory and WithPermissionsAttribute in the built-in DI
* NtlmSecurityContextFactory - implementation of ISecurityContextFactory based on retrieving data from headers, a Windows account from a request, or the current service account
* NtlmSecurityContext - implementation of ISecurityContext taking into account the request for user privileges from the security service
* WithPermissionsAttribute - authorization attribute for controllers and actions, checking user privileges