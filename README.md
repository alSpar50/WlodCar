Aby uruchomić aplikację webową WlodCar należy:

W Visual Studio 2022 wybrać pierwszą opcję Clone Repository i wkleić link: https://github.com/alSpar50/WlodCar (opcjonalnie zmodyfikować ścieżkę, gdzie repo ma być umieszczone lokalnie),
po załadowaniu, na górnym pasku VS2022 wejść w Narzędznia -> Menedżer Pakietów NuGet -> Konsola Menadżera Pakietów.
W dolnej sekcji VS2022 odpali się konsola, gdzie należy wpisać kolejno, jedna po drugiej, następujące komendy:
a) Drop-Database
b) Remove-Migration
c) Add-Migration InitialCreate
d) Update-Database

Przy opcji Drop-Database wybrać opcję All (litera A), przy zapytaniu

Po tych krokach u każdego z Was stworzy się lokalnie baza danych, która będzie już bez problemu działać z obecnym kodem.
