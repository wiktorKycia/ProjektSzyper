# ProjektSzyper
C# project for school


## Setup
komendy, które zostały użyte do tworzenia projektu
```bash
dotnet add package Microsoft.EntityFrameworkCore
dotnet add package Microsoft.EntityFrameworkCore.Sqlite
```

komendy do uruchomienia, po zmianie struktury bazy:
```bash
dotnet ef migrations add <nazwa_migracji>
dotnet ef database update
```
