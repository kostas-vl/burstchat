# BurstChat
A simple chat application built using ASPNET Core and angular.

## Projects
This repository includes multiple projects that target the backend and the frontend side.

| Projects | Description |
| --- | --- |
| BurstChat.Share   | A shared library with functionality for all dotnet core projects |
| BurstChat.IdentityServer | The authentication middleware that is built using IdentityServer 4 |
| BurstChat.Api | The RESTful web api that interacts with the database |
| BurstChat.Signal | The RPC backend that enables realtime communication using SignalR and web sockets |
| BurstChat.Client | The angular frontend. Supports web and an *experimental* electron  version. |



## Database
PostgreSQL is used as the database, together with Entity Framework Core. In both the BurstChat.Api and BurstChat.IdentityServer projects there is a Migrations directory with all changes that need to be applied.

## Certificates
The BurstChat.Identity server uses a certificate for signing the generated tokens. For development purposes a self signed certificate is used. Its location and passphrase need to configured in the appsetting.SigningCredentials.json file.

## Disclaimer
All project are under heavy development. *NOTHING IS STABLE*
