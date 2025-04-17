# ProjektSzyper
C# project for school


## Setup
komendy, które zostały użyte do tworzenia projektu
```bash
dotnet add package Microsoft.EntityFrameworkCore
dotnet add package Microsoft.EntityFrameworkCore.Sqlite
dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet ef database update
```

komendy do uruchomienia, po zmianie struktury bazy:
```bash
dotnet ef migrations add <nazwa_migracji>
dotnet ef database update
```
