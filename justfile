#!/usr/bin/env just --justfile

default:
    just --list
    
run:
    dotnet run --project src/tui
    
init:
    dotnet run --project src/tui -- init
    
help:
    dotnet run --project src/tui -- help
    
migrate-add MIGRATION:
    dotnet ef migrations add "{{ MIGRATION }}" --project src/RouteForce.Infrastructure --startup-project src/RouteForce.Web --output-dir Persistent/Migrations

migrate-update: 
    dotnet ef database update --project src/RouteForce.Infrastructure --startup-project src/RouteForce.Web