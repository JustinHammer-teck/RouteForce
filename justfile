#!/usr/bin/env just --justfile

default:
    just --list
    
run:
    dotnet run --project src/tui
    
init:
    dotnet run --project src/tui -- init
    
help:
    dotnet run --project src/tui -- help