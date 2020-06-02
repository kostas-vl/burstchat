# BurstChat
A simple chat application built using ASPNET Core and angular.

## Projects
This repository tries to follow the Clean Architecture design.

| Projects | Description |
| --- | --- |
| BurstChat.Domain   | A library that contains all domain entities shared between the projects |
| BurstChat.Application | A library that provides functionality to projects that want to interact with the domain layer |
| BurstChat.Infrastructure | A library that provides configuration functionality to all projects and contains the migrations for all databases |
| BurstChat.IdentityServer | The authentication middleware that is built using IdentityServer 4 |
| BurstChat.Api | The RESTful web api that interacts with the database |
| BurstChat.Signal | The web socket backend that enables realtime communication using SignalR |
| BurstChat.Client | The angular frontend. Supports web and an *experimental* electron  version. |

## Database
PostgreSQL is used as the database, together with Entity Framework Core. All migrations are stored in the BurstChat.Infrastructure project in the Persistence directory.

## Settings
All projects make use of static settings. So a quick list of them are:

##### BurstChat.Api
* **appsettings.AccessTokenValidation:** Settings for communicating with Identity Server.
* **appsettings.Database.json:** Settings for the database connection.
* **appsettings.Domains.json:** Contains the trusted CORS origins.

##### BurstChat.IdentityServer
* **appsettings.Database.json:** Settings for the database connection.
* **appsettings.Domains.json:** Contains the trusted CORS origins.
* **appsettings.IdentitySecrets.json:** Contains the secrets of all clients and apis. This is for development purposes since all client and api resource are reconfigured with each execution.
* **appsettings.SigningCredentials.json:** The BurstChat.Identity server uses a X509 certificate for signing the generated tokens. This file  For development purposes a self signed certificate is used. Its location and passphrase need to configured are configured in this file.
* **appsettings.Smtp.json:** Configuration for the smtp server that will be used for sending one time passwords to users for changing their passwords.

##### BurstChat.Signal
* **appsettings.AccessTokenValidation:** Settings for communicating with Identity Server.
* **appsettings.Domains.json:** Contains the trusted CORS origins.

##### BurstChat.Client
* **environment.ts** Contains all static settings. 
* **environment.prod.ts** Contains all static *production* settings. 

## Building and running
For all dotnet projects a simple `dotnet run` should do the trick. Any project 
that contains a Migrations directory, you need to install the `dotnet-ef` cli tool 
using the the dotnet cli itself. After installing it you need to run `dotnet-ef database update` 
to create / update the database.

For the client project you can run:
* `npm install` to install all npm dependencies.
* `npm start` to start the web version of the client.
* `npm run start-electron` to start the electron version.

## Disclaimer
All project are under heavy development. *NOTHING IS STABLE*
