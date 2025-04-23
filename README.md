# Dokumentacja projektu Logistics Warehouse Managment System (LWMS)
Nazwa aplikacji: Logistics Warehouse Managment System.
Jest to aplikacja do zarządzania magazynem do sklepów.

## Spis treści
1. [Opis projektu](#opis-projektu)
    - [Cel](#cel)
    - [Co robi aplikacja](#co-robi-aplikacja)
    - [Dla kogo jest przeznaczona](#dla-kogo-jest-przeznaczona)
2. [Technologie](#technologie)
3. [Struktura katalogów](#struktura-katalogów)
4. [Instrukcja instalacji i uruchomienia](#instrukcja-instalacji-i-uruchomienia)
    - [Wymagania systemowe](#wymagania-systemowe)
    - [Jak uruchomić aplikację?](#jak-uruchomić-aplikację)
    - [Komendy](#komendy)
5. [Zrzuty ekranu](#zrzuty-ekranu)
6. [Przykłady użycia](#przykłady-użycia)
7. [Struktury danych i klasy](#struktury-danych-i-klasy)
    - [Dane](#dane)
        - [Logi](#logi)
        - [Baza](#baza)
        - [Użytkownicy](#użytkownicy)
    - [Klasy](#klasy)
        - [CLI](#cli)
        - [Database](#database)
        - [Logic](#logic)
        - [LogServices](#logservices)
        - [UsersManagement](#usersmanagement)
8. [Obsługa błędów](#obsługa-błędów)
9. [Testowanie](#testowanie)
10. [Problemy i ograniczenia](#problemy-i-ograniczenia)
     - [Co nie działa idealnie?](#co-nie-działa-idealnie)
     - [Co może zostać poprawione?](#co-może-zostać-poprawione)
11. [Plany rozwoju](#plany-rozwoju)
12. [Autorzy](#autorzy)
     - [Kontakt](#kontakt)

## Opis projektu

### Cel
Celem projektu było stworzenie prostej aplikacji konsolowej do zarządzania magazynem. Założenie jest takie, że aplikacja ma się znaleźć na komputerze gdzieś w magazynie i korzystają z niej pracownicy magazynu.

### Co robi aplikacja
Aplikacja głównie służy do zatwierdzania importów do magazynu i eksportów do sklepów. Można też rozdzielać zadania między pracowników, a pracownicy mogą te zadania wykonywać.

### Dla kogo jest przeznaczona
Aplikacja jest przeznaczona dla pracowników magazynu.
Są uwzględione role:

Administrator - może zarządzać użytkownikami

Manager - może przydzielać zadania

Pracownik (Warehouseman) - może wykonywać zadania

Logistyk - może przyjmować towar do magazynu i planować eksporty

## Technologie
- Język programowania: C#
- Środowisko: .NET 9
- IDE: Visual Studio 
- Inne biblioteki/narzędzia
  - Microsoft EntityFramework Core - do bazy danych
  - Bogus - do generowania danych

## Struktura katalogów
```
📁 Projekt/
│
├── 📝 README.md
│
├── 📂 StorageOffice/
│   ├── 📄 Program.cs                       # Plik uruchamiający aplikację
│   ├── 📄 StorageOffice.sln                
│   ├── 📄 StorageOffice.csproj             
│   │
│   ├── 📂 Data/                            # Folder z danymi
│   │   ├── 📄 StorageOffice.db             # baza danych SQLite 
│   │   ├── 📄 users.txt                    # Dane użytkowników (nazwy, zahashowane hasła, role)
│   │   └── 📄 logs.txt                     # Logi systemowe aplikacji
│   │
│   ├── 📂 Migrations/                      # Migracje EF Core - kod wygenerowany automatycznie przez bibliotekę
│   │   └── 📄 [migration files]
│   │
│   └── 📂 classes/                         # Główna struktura kodu
│       │
│       ├── 📂 CLI/                         # Komponenty konsolowego UI
│       │   ├── 📄 Commons.cs               # Mniejsze, częściej używane komponenty
│       │   ├── 📄 Input.cs                 # Wczytywanie danych od użytkownika
│       │   ├── 📄 Option.cs                # Opcje w menu
│       │   ├── 📄 Select.cs                # Listy wyboru opcji
│       │   └── 📄 Table.cs                 # Renderowanie tabeli
│       │
│       ├── 📂 database/                    # Kod związany z bazą danych
│       │   ├── 📄 Database.cs              # Fasada dostępu do bazy (zapytania jako metody)
│       │   ├── 📄 Model.cs                 # Model (struktura bazy)
│       │   └── 📄 DataSeeder.cs            # Generator danych
│       │
│       ├── 📂 Logic/                       # Logika aplikacji
│       │   ├── 📄 MenuHandler.cs           # przełączanie się między menu (Backend)
│       │   │
│       │   └── 📂 screens/                 # konkretne menu (Frontend)
│       │       └── 📄 [screen files]
│       │
│       ├── 📂 LogServices/                 # Logger
│       │   └── 📄 Logger.cs
│       │
│       └── 📂 UsersManagement/             # Zarządzanie użytkownikami
│           ├── 📂 Modules/                 
│           │   └── 📄 User.cs              # Klasa User: reprezentuje zalogowanego użytkownika w aplikacji
│           │
│           └── 📂 Services/                
│               ├── 📄 PasswordManager.cs   # Hasła użytkowników
│               └── 📄 RBAC.cs              # Role użytkowników
│
├── 📂 StorageOffice.UnitTests/             # Testy jednostkowe
│   ├── 📄 StorageOffice.UnitTests.csproj
│   └── 📂 [test folders and files]
│
└── 📂 StorageOffice.IntegrationsTests/     # Testy integracyjne
    ├── 📄 StorageOffice.IntegrationsTests.csproj
    └── 📂 [test folders and files]
```

## Instrukcja instalacji i uruchomienia

### Wymagania systemowe
- system operacyjny Windows 10
- Visual Studio 2022
- środowisko .NET 9

### Jak uruchomić aplikację?

1. Rozpakować archiwum zip
2. Wejść do podfolderu `StorageOffice`
3. Otworzyć plik `StorageOffice.sln` przy pomocy Visual Studio 2022
4. Kliknąć "Uruchom" lub użyć skrótu klawiszowego `ctrl`+`F5`
5. Gdyby były problemy z uruchomieniem, należy usunąć foldery `bin/` oraz `obj/` (znajdują się w folderze `StorageOffice`)
   1. przy następnym uruchomieniu, powinny one się utworzyć automatycznie
6. Spróbować uruchomić tak samo jak w punkcie `4.`
7. Gdyby jeszcze nastąpiły problemy należy zainstalować następujące paczki (przez manadżer pakietów NuGet, lub przez [PowerShell](#Komendy)):
   1. Microsoft.EntityFrameworkCore
   2. Microsoft.EntityFrameworkCore.Sqlite
   3. Microsoft.EntityFrameworkCore.Design
   4. Bogus
8. W ostateczności, gdyby foldery `bin/` oraz `obj/` się nie utworzyły należy wykonać następujące kroki:
   1. utworzyć nowy projekt za pomocą Visual Studio 2022
   2. przekopiować tam cały kod (czyli plik `Program.cs`, foldery `classes/`, `Data/` i `Migrations/`)
   3. zainstalować te same paczki co w punkcie `7.`

==Gdyby, przy uruchamianiu pojawiał się błąd związany z błędem przy otwarciu pliku, należy otworzyć Visual Studio jako  administrator i spróbować ponownie uruchomić aplikację oraz sprawdzić czy w folderze `Data/` znajdują się pliki  `StorageOffice.db`, `users.txt` oraz `logs.txt` i czy pliki tekstowe nie mają pustych linii==

Do uruchomienia testów należy zrobić te same kroki analogicznie w folderach `StorageOffice.IntegrationsTests` oraz `StorageOffice.UnitTests`

### Komendy

Pobieranie paczek:
```bash
dotnet add package Microsoft.EntityFrameworkCore
dotnet add package Microsoft.EntityFrameworkCore.Sqlite
dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet add package Bogus
dotnet ef database update
```

Dodanie nowej migracji (konieczne po zmianie struktury bazy):
```bash
dotnet ef migrations add <nazwa_migracji>
dotnet ef database update
```

## Zrzuty ekranu

![Login page](./images/screenshot1.png)

![Admin panel](./images/widok_administratora.png)

## Przykłady użycia

Poniższe przykłady pokazują, jak wykonywać typowe zadania w aplikacji, krok po kroku, dla różnych ról użytkowników.

### Logowanie do systemu
1. Uruchom aplikację.
2. Naciśnij dowolny klawisz, aby przejść do ekranu logowania.
3. Wprowadź nazwę użytkownika i hasło.
4. Naciśnij Enter, aby zalogować się.
5. Jeśli dane logowania są poprawne, zostaniesz przekierowany do odpowiedniego panelu użytkownika.
6. Jeśli dane logowania są niepoprawne, zostaniesz poinformowany o błędzie i będziesz mógł spróbować ponownie.

### Administrator: przeglądanie logów
1. Zaloguj się jako administrator.
2. Wybierz opcję "View logs" z menu głównego.
3. Zobaczysz listę logów systemowych, które zawierają informacje o działaniach w systemie.
4. Możesz przewijać listę, aby zobaczyć starsze logi.
5. Naciśnij `Esc`, aby wrócić do menu głównego.

### Administrator: dodawanie użytkownika
1. Zaloguj się jako administrator.
2. Wybierz opcję "Manage users" z menu głównego klawiszem `Enter`.
3. Pojawi się menu akcji z opcjami "Add user", "Edit user", "Delete user" i "View users".
4. Wybierz opcję "Add user" klawiszem `Enter`.
5. Wprowadź nazwę użytkownika, hasło i rolę (Administrator, Manager, Warehouseman, Logistics).
6. Zobaczysz komunikat potwierdzający dodanie użytkownika.
7. Naciśnij `Y`, aby potwierdzić dodanie użytkownika.
8. Naciśnij dowolny klawisz, aby wrócić do menu użytkowników.

### Administrator: usuwanie użytkowników
1. Zaloguj się jako administrator.
2. Wybierz opcję "Manage users" z menu głównego klawiszem `Enter`.
3. Pojawi się menu akcji z opcjami "Add user", "Edit user", "Delete user" i "View users".
4. Wybierz opcję "Delete user" klawiszem `Enter`.
5. Zobaczysz listę użytkowników.
6. Wybierz użytkownika (lub użytkowników), którego chcesz usunąć, używając strałek i klawisza `Enter`.
7. Naciśnij `Del`, aby usunąć użytkownika.
8. Pojawi się ekran z potwierdzeniem usunięcia.
9. Naciśnij `Y`, aby potwierdzić usunięcie.
10. Zobaczysz komunikat potwierdzający usunięcie użytkownika.
11. Naciśnij `Esc`, aby wrócić do menu głównego.

### Logistyk: dodawanie dostaw do magazynu
1. Zaloguj się jako logistyk.
2. Wybierz opcję "Create Inbound Shipment (Import)" z menu głównego klawiszem `Enter`.
3. Wybierz 1, aby wprowadzić ID dostawcy z podanych, lub 2, aby dodać nowego dostawcę ręcznie.
4. Jeśli wybierzesz 1, zobaczysz listę dostawców. Wybierz dostawcę, wpisując jego numer ID i klikając `Enter`.
5. Pojawi się komunikat, że stworzno dostawę, ale to jeszcze nie koniec.
6. Naciśnij dowolny klawisz, aby przejść do następnego kroku.
7. Wybierz 1, aby dodać produkt do dostawy
8. Możesz wybrać 1, aby potem wybrać produkt z listy, lub 2, aby dodać produkt ręcznie.
9. Jeśli wybierzesz 1, zobaczysz listę produktów. Wybierz produkt, wpisując jego numer ID i klikając `Enter`.
10. Podaj ilość produktu, który chcesz dodać do dostawy.
11. Jeśli podałeś liczbę poprawnie, zobaczysz komunikat, że produkt został dodany do dostawy.
12. Naciśnij dowolny klawisz, aby przejść do menu edytowania produktów do dostawy.
13. Teraz możesz wybrać 1, aby dodać kolejny produkt do dostawy, lub 2, aby zakończyć dodawanie produktów do dostawy.

### Pracownik magazynu: wykonywanie zadań

## Struktury danych i klasy

### Dane

Dane są przechowywane w folderze [`Data/`](./StorageOffice/Data/)
Dzielą się na 3 części:
- logi systemowe
- baza
- użytkownicy

#### Logi
Logi są przechowywane w pliku `logs.txt` i zawierają informacje o tym co się działo podczas działania aplikacji

#### Baza
Baza danych (oparta na systemie SQLite3) jest przechowywana w pliku `StorageOffice.db` i zawiera dane o
- produktach
- magazynie
- dostawach
- dostawcach
- odbiorcach
- oraz podstawowe informacje o użytkownikach

#### Użytkownicy
Użytkownicy są przechowywani w pliku `users.txt` i zawierają nazwy użytkowników, ich zaszyfrowane hasła oraz infromacje o ich rolach

### Klasy
Klasy są podzielone na foldery, które odpowiadają ich funkcjonalności
wszystkie klasy znajdują się w podfolderze `StorageOffice/classes/`

#### CLI

Folder [`CLI/`](./StorageOffice/classes/CLI/) zawiera klasy, które odpowiadają za wyświetlanie danych na ekranie konsoli


klasa abstrakcyjna [`Select`](./StorageOffice/CLI/Select.cs) jest klasą bazową dla 2 typów list wyboru:
- `RadioSelect`, która wyświetla pozwala na wybór jednego elementu
- `CheckboxSelect`, która wyświetla pozwala na wybór wielu elementów

Zawiera ona listę obiektów klas implementujących interfejs `ISelectable`
Najważniejszą metodą klasy `Select` jest `InvokeOperation()`, która wywołuje metodę `InvokeOperation()` zaznaczonej opcji


Klasy `RadioOption` oraz `CheckboxOption` mają zdarzenie typu `Action`, które jest wywoływane przez metodę 
`InvokeOperation()`


#### Database

Klasy dotyczące bazy danych znajdują się w folderze [`database/`](./StorageOffice/classes/database/)

Klasa [StorageDatabase](./StorageOffice/classes/database/Database.cs) jest odpowiedzialna za komunikację reszty kodu z bazą danych. Zaimplementowany został tutaj wzorzec [Fasada](https://refactoring.guru/pl/design-patterns/facade)


Klasa [Model](./StorageOffice/classes/database/Model.cs) zawiera definicje tabel w bazie danych oraz ich relacje.
Do tworzenia zapytań oraz struktury bazy danych użyty został Entity Framework Core wraz z bazą SQLite.


Klasa [DataSeeder](./StorageOffice/classes/database/DataSeeder.cs) jest odpowiedzialna za generowanie danych do bazy danych. Generuje ona mniej-więcej realistyczne dane. Do generacji danych użyty jest moduł Bogus.


#### Logic

Folder [`Logic/`](./StorageOffice/classes/Logic/) zawiera klasy, które odpowiadają za logikę aplikacji.

Klasy znajdujące się w tym folderze wyciągają dane z bazy przy pomocy odpowiednich metod oraz wywołują odpowiednie metody w klasach z folderu `CLI/`, tak aby wyświetlić dane na ekranie konsoli w odpowiedni sposób.

Klasa [`MenuHandler`](./StorageOffice/classes/Logic/MenuHandler.cs) jest odpowiedzialna za przełączanie się między poszczególnymi ekranami.

Każdy ekran jest tworzony przez odpowiednią metodę klasy `MenuHandler`. 
Metody te pobierają dane z bazy, wstępnie je przetwarzają i podają dalej do konkretnych klas ekranów.

Klasy poszczególnych ekranów są odpowiedzialne za interakcję z użytkownikiem oraz wyświetlanie danych na ekranie konsoli.


#### LogServices

#### UsersManagement

## Obłsuga błędów

Obłsuga błędów w sekcji `Logic/` jest zrealizowana przez ekran `Error`. 
Użytkownik wtedy widzi czerwony komunikat o błędzie i może wrócić do poprzedniego ekranu

## Testowanie

Aplikację testowano na różne sposoby
- testami jednostkowymi
- testami integracyjnymi
- uruchamiając i ręcznie sprawdzając, czy wszystko działa i czy wyświetla się jak należy

Testy jednostkowe i integracyjne zawarte są w folderach:
- `StorageOffice.UnitTests`
- `StorageOffice.IntegrationsTests`


## Problemy i ograniczenia

### Co nie działa idealnie?

Czyszczenie konsoli nie zawsze działa idealnie

jak treść do wyświetlenia nie mieści się na jednym ekranie

to wtedy console clear "ucina" dolną część, a całą resztę zostawia i jest możliwość zobaczenia jej po podscrollowaniu do góry

---------------------------------------

Drugą rzeczą, która nie działa idealnie jest wyświetlanie produktów wg. kategorii

Działają wszystkie kategorie poza elektorniką, która wyrzuca nieobsłużony wyjątek i zatrzymuje cały program

Nie wiem dlaczego to się dzieje

---------------------------------------

Trzecia rzecz działa poprawnie, ale jest uciążliwa

Jest nią przechodzenie między ekranami.
Niektóre listy wyboru są robione przez switch-case i wymagają od użytkownika wpisania konkretnej liczby
a nie po prostu wybrania opcji z listy.

Ta wada wynikła poprzez pośpiech i brak czasu na implementację bardziej rozbudowanego systemu


### Co może zostać poprawione?

Wszystkie ekrany konsoli mogą zostać zrobione tak, aby cała zawartość zmieściła się bez konieczności przewijania

Może zostać zaimplementowany try-catch dla kategorii elektronika, który wyświtlałby ekran błędu informujący
użytkownika, że takiej kategorii nie można wyświetlić, natomiast nie jest to w pełni satysfakcjonujące rozwiązanie

Wszystkie ekrany mogą zostać zaimplementowane jako klasa oraz wywoływane poprzez odpowiednią metodę w klasie `MenuHandler`

## Plany rozwoju

W przyszłości można dodać ustawienia kolorów, tak aby była lepsza personalizacja pod względem wyglądu aplikacji
- np. inny kolor tła lub czcionki


## Autorzy

Wiktor Kycia 3D
Jan Topolewski 3D

### Kontakt

wiktor.kycia@uczen.zsk.poznan.pl
jan.topolewski@uczen.zsk.poznan.pl