# BurstChat
A simple chat application built using ASPNET Core and angular.

![burstchat-api](https://github.com/kostas-vl/burstchat/workflows/burstchat-api/badge.svg) ![burstchat-identity-server](https://github.com/kostas-vl/burstchat/workflows/burstchat-identity-server/badge.svg) ![burstchat-signal](https://github.com/kostas-vl/burstchat/workflows/burstchat-signal/badge.svg) ![burstchat-client](https://github.com/kostas-vl/burstchat/workflows/burstchat-client/badge.svg)

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


## Asterisk 
BurstChat enables real time communication with voice calls, using the Asterisk SIP server and webRTC. For development purposes, there are sample configuration files for asterisk in the config/dev/asterisk directory. 

> For obvious security reasons, do not use any config file in a production Asterisk instance.

## Database
PostgreSQL is relational database of choice, together with Entity Framework Core as an ORM. Now, since the repository contains multiple projects, each project might be dependent to its own database context. A list of these database contexts are

* **BurstChatDb**: Is the main database context used to store BurstChat's data. The project associated with this context is `BurstChat.Api` and all migrations are run from its directory. The `BurstChat.IdentityServer` also has a minor dependency. 
* **ConfigurationDb**: Is the first database context of IdentityServer4 and the associated project is `BurstChat.IdentityServer`. All migrations for this context are run, from the project's directory.
* **PersistedGrantDb**: Is the second database context of IdentityServer4 and the associated project is again `BurstChat.IdentityServer`. All migrations are run from the project's directory.

> As for Asterisk's database (the one used to realtime pjsip), execute config/dev/sql/burstchat_asterisk_schema.sql script, in postgresql.

#### Migrations
All migrations are stored in the Persistence directory of the `BurstChat.Infrastructure` project, but any new migration, needs to executed in the directory of the  for each project a specific set of options need to be passed to the `dotnet-ef` tool.

###### BurstChatDb
```bashscript
# For migrations on the BurstChat Database, from the root directory of the project, go to the BurstChat Api project directory.
cd src/BurstChat.Api/

# Use the dotnet-ef tool to add a new migration.
dotnet ef migrations add CoolNewMigration  --project ../BurstChat.Infrastructure/ --startup-project . --output-dir ../BurstChat.Infrastructure/Persistence/Migrations -c BurstChatContext

# Or use the burst_db migration script, in the scripts directory.
../../scripts/burst_db_add_migration.sh CoolNewMigration
```

###### PersistedGrantDb
```bashscript
# For migrations on the PersistedGrant Database of IdentityServer4, from the root directory of the project, go to the BurstChat IdentityServer project directory.
cd src/BurstChat.BurstChat.IdentityServer/

# Use the dotnet-ef tool to add a new migration.
dotnet ef migrations add CoolNewMigration --project ../BurstChat.Infrastructure/ --startup-project . --output-dir ../BurstChat.Infrastructure/Persistence/Migrations/PersistedGrantDb -c PersistedGrantDbContext

# Or use the burst_db migration script, in the scripts directory.
../../scripts/persisted_grant_db_add_migration.sh CoolNewMigration
```

###### ConfigurationDb
```bashscript
# For migrations on the Configuration Database of IdentityServer4, from the root directory of the project, go to the BurstChat IdentityServer project directory.
cd src/BurstChat.BurstChat.IdentityServer/

# Use the dotnet-ef tool to add a new migration.
dotnet ef migrations add $1 --project ../BurstChat.Infrastructure/ --startup-project . --output-dir ../BurstChat.Infrastructure/Persistence/Migrations/PersistedGrantDb -c PersistedGrantDbContext

# Or use the burst_db migration script, in the scripts directory.
../../scripts/configuration_db_add_migration.sh CoolNewMigration
```

After adding all the migrations you require, simply update the database with
```bashscript
dotnet ef database update
```

## Settings
All projects make use of static settings. So a quick list of them are:

#### BurstChat.Api
* **appsettings.AccessTokenValidation:** Settings for communicating with Identity Server.
* **appsettings.Database.json:** Settings for the database connection.
* **appsettings.Domains.json:** Contains the trusted CORS origins.

#### BurstChat.IdentityServer
* **appsettings.Database.json:** Settings for the database connection.
* **appsettings.Domains.json:** Contains the trusted CORS origins.
* **appsettings.IdentitySecrets.json:** Contains the secrets of all clients and apis. This is for development purposes since all client and api resource are reconfigured with each execution.
* **appsettings.SigningCredentials.json:** The BurstChat.Identity server uses a X509 certificate for signing the generated tokens. This file  For development purposes a self signed certificate is used. Its location and passphrase need to configured are configured in this file.
* **appsettings.Smtp.json:** Configuration for the smtp server that will be used for sending one time passwords to users for changing their passwords.

#### BurstChat.Signal
* **appsettings.AccessTokenValidation:** Settings for communicating with Identity Server.
* **appsettings.Domains.json:** Contains the trusted CORS origins.

#### BurstChat.Client
* **environment.ts** Contains all static settings. 
* **environment.prod.ts** Contains all static *production* settings. 

## Building and running
For all dotnet projects a simple `dotnet run` should do the trick. For the `BurstChat.Api` and `BurstChat.IdentityServer`, you need to make sure all migration updates have been executed, so go to each project's directory and make use of the `dotnet ef database update` command to apply them.

For the client project you can run:
* `npm install` to install all npm dependencies.
* `npm start` to start the web version of the client.
* `npm run start-electron` to start the electron version.

## Disclaimer
All project are under heavy development. *NOTHING IS STABLE*
