# KartverketRegister

Oppgave 1. Å opprette en ASP.NET Core MVC applikasjon med frontend, skjema og kart


Prosjektoppgave UiA Institutt for informasjonssystemer, Kartverket og Norsk Luftambulanse (NLA)

>_<

# Hvordan starte applikasjonen??

	Sørg for at docker desktop kjører til enhver tid!

## Docker Compose

	Åpne terminal i Mappa til repoen og skriv - Docker compose up - Den kommandoen bygger og kjører applikasjonen

	*Første startup pleier å feile fordi MVC appen starter før selve databasen, bare kjør docker compose up igjen!*
	
	Applikasjonen skal nå kjøre på 8080 port!

## Visual Studio

	Kjør start_mariadb.bat - Den starter opp databasen
	
	Åpne KartverketRegister.sln med Visual Studio
	
	Start applikasjonen med http
	
## Terminal

	Kjør start_mariadb.bat - Den starter opp databasen

	Åpne terminal i repoen og skriv - Dotnet run
	
## Debug

	Om docker compose ikke starter på første forsøk så prøv en gang til, for databasen starter av og til pararelt med MVC appen og derfor kræsjer appen
	

# Drift
The app can be run locallly or inside Docker.
To run the system without Docker, make sure you have the .NET 8 SDK installed, then use the following commands:
	
## Run Locally 
	git clone https://github.com/HenellTT/KartverketRegister.git
	cd KartverketRegister
	dotnet restore
	dotnet build
	dotnet run






# Systemarkitektur

## Oversikt
Applikasjonen er en ASP.NET Core MVC applikasjon som kjøres i en Docker Container. 
Den følger Model-View-Controller(MVC) sitt arkitekture rammeverk og leverer responsive nettsider med dynamisk innhold.
Brukerinteraksjon skjer via skjemaer og kart, og data som flyter mellom frontend, backend og databasen.
	
## Hovedkomponenter
- Controller: Håndterer HTTP Forespørseler (GET og POST). Sender forespørseler til riktig logikk og returnerer views med data
- Models: Inneholder data som vises i brukergrensesnittet
- View: Ansvar for visning av user interface. viser data som er lagd i models til brukeren
- Database: Lagrer brukernavn og henter informasjon fra skjema og kart samt kommuniserer med backend. Vi bruker Mariadb som kjører i en egen docker container. Applikasjonen og databasen kommuniserer med et internt docker nettverk, fra docker-compose.yml
- Docker: Pakker applikasjonen i en container og sørger for at applikasjonen kjøres likt. 
- Kart: Integrert kart fra Leaflet i frontend. Lar brukeren velge sted på kart, sender kordinatene til backend

## Dataflyt
- GET: Bruker åpner side → Controller henter data fra database → View viser data via ViewModel.
- POST(skjema): Bruker sender skjema → Controller validerer og lagrer → Redirect til visningsside som henter og viser lagret data.
- Post(Kart): Bruker klikker på kart → Frontend sender koordinater til backend → Backend lagrer → Visningsside henter og viser på kart/tekst.


				

