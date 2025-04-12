# Struktura menu

- [Menu logowania](#Menu-logowania)
- [Menuu główne](#Menu-główne)

## Role
0 - Gość
1 - Magazynier
2 - Kierownik
3 - Administrator

## Menu logowania
- opcja zalogowania bez hasła jako gość (po prostu jak nazwa lub hasło nie zostaną wpisane, to zaloguje jako gość)
- logowanie użytkownika
  - 2 inputy zrobione tak, że prosi o pojedyńcze litery przez getKey, wstawia je do zmiennej, wypisuje tą zmienną w odpowiednim miejscu (w ten sposób można wyświetlić całą ramkę, bez przerywania programu na input użytkownika)
  - Enter by zatwierdzić, Esc, by odrzucić (anulować)
  - w inpucie z hasłem, pojawiają się znaki `*` zamiast liter

## Menu główne
- w ramce jakiś napis powitalny, lub informacja `Logged in as: <username>`
- Opcje:
  - [Magazyn](#Magazyn)
  - [Zadania](#Zadania) 
  - [Import](#Import)
  - [Eksport](#Eksport)
  - [Users](#Użytkownicy)


## Magazyn
opcje:
- ile jest danych towarów wg. kategorii (prawa 0: może zobaczyć gość i wszyscy nad nim)
- dokładny przegląd produktów jakie znajdują się na magazynie (prawa 1: magazynier i wszyscy nad nim)
- szukanie (prawa 1) - przeszukiwanie, input, szuka po nazwie lub kategorii

## Zadania
(prawa 1: jeśli otwiera magazynier, to pokazuje tylko te, które są do niego przypisane, a jak manager, to może zlecać)
- lista importów i eksportów do zatwierdzenia
  - wybranie dowolnego elementu z listy otworzy listę towarów jakie przyjechały/są do rozwiezienia)
  - w widoku szczegółowym można zatwierdzić taką transakcję

## Import 
- informacja ile jest dostaw do magazynu i w jakim zakresie czasu miały one miejsce (prawa 0)
- szczegółowe zestawienie wszystkich importów jakie mają miejsce oraz szczegółowe listy produktów w każdym z nich (prawa 1)
- zasymuluj import (przyjazd ciężarówki) (prawa 2: tylko manager, ewentualnie admin, jakby nie było przez chwile konta managera)

## Eksport
generalnie to samo co w imporcie, tylko od magazynu

## Użytkownicy
(prawa 3: tylko admin może tu wejść)
- dodaj/usuń użytkownika
- zmień nazwę
- zmień hasło
- zmień rolę
- przeglądaj w tabeli











