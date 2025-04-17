# Struktura menu

- [Menu logowania](#Menu-logowania)
- [Menuu główne](#Menu-główne)

## Role
0 - Magazynier

1 - Logistyk

2 - Kierownik

3 - Administrator

## Menu logowania
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
  - [Logi](#Logi)
  - [Users](#Użytkownicy)
  - Wyloguj


## Magazyn
opcje:
- dokładny przegląd produktów jakie znajdują się na magazynie (magazynier, manager)
- szukanie (magazynier, manager) - przeszukiwanie, input, szuka po nazwie lub kategorii

## Zadania
(jeśli otwiera magazynier, to pokazuje tylko te, które są do niego przypisane, a jak manager, to może zlecać)
- lista importów i eksportów do zatwierdzenia
  - wybranie dowolnego elementu z listy otworzy listę towarów jakie przyjechały/są do rozwiezienia)
  - w widoku szczegółowym można zatwierdzić taką transakcję

## Import 
- szczegółowe zestawienie wszystkich importów jakie mają miejsce oraz szczegółowe listy produktów w każdym z nich (manager, magazynier)
- zasymuluj import (przyjazd ciężarówki) (logistyk)

## Eksport
generalnie to samo co w imporcie, tylko od magazynu

## Logi
- wyświetlenie logów (admin)

## Użytkownicy
(tylko admin może tu wejść)
- dodaj/usuń użytkownika
- zmień nazwę
- zmień hasło
- zmień rolę
- przeglądaj w tabeli











