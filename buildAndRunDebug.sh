#!/bin/bash
dotnet restore ./src/SmartCracker
dotnet build ./src/SmartCracker
dotnet src/SmartCracker/bin/Debug/netcoreapp1.0/SmartCracker.dll
