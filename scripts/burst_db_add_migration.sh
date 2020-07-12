#!/bin/sh

dotnet ef migrations add $1 --project ../BurstChat.Infrastructure/ --startup-project . --output-dir ../BurstChat.Infrastructure/Persistence/Migrations -c BurstChatContext
