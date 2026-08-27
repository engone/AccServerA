# AccServerA

Réécriture en C# du service ACCSERVER issu d'une décompilation Visual Basic .NET.

## Cible technique

- C# 7.3 ;
- .NET Framework 4.7.2 ;
- projet de bibliothèque au format MSBuild classique.

Les assemblys métier référencés par `SageAccService.csproj` (AccountingObjects,
AccServicesLib, AuthObjects, etc.) doivent être fournis par l'environnement
Infosciences/Sage pour compiler et exécuter le service.
