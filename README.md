# WlodCar — aplikacja webowa

## Opis

WlodCar to aplikacja webowa zbudowana w ramach projektu studenckiego dla **SWPW w Płocku**.  
Umożliwia rejestrację **użytkowników** oraz **administracji**.

> **Uwaga dla administracji:** rejestracja konta administracyjnego wymaga adresu e-mail w domenie `@wlodcar.pl`, np. `admin@wlodcar.pl`.

## Funkcje

### Dla użytkownika
- rezerwacja samochodów,
- podgląd historii rezerwacji,
- korzystanie z **punktów lojalnościowych** (zniżki),
- edycja profilu (zdjęcie profilowe, numer telefonu, dane osobowe),
- notatki w panelu **„Moje konto”**,
- dobieranie dodatków do rezerwacji,
- wyszukiwanie **dostępnych** aut (niedostępne są odfiltrowywane).

### Dla administratora
- dodawanie aut i zmiana ich statusów,
- zarządzanie flotą,
- wysyłanie e-maili do klientów bezpośrednio z poziomu aplikacji,
- zarządzanie użytkownikami.

---

## Uruchomienie (Visual Studio 2022)

1. W Visual Studio wybierz **Clone Repository** i wklej adres repozytorium:  
   `https://github.com/alSpar50/WlodCar`  
   *(opcjonalnie zmień lokalną ścieżkę docelową).*

2. Otwórz konsolę NuGet:  
   **Narzędzia → Menedżer pakietów NuGet → Konsola Menedżera pakietów**.

3. W konsoli wpisz kolejno (każde polecenie osobno):

   ```powershell
   Drop-Database
   Remove-Migration
   Add-Migration InitialCreate
   Update-Database
