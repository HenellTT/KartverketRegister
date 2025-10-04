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
For å kjøre systemet uten Docker må du ha .NET 8 SDK installert.
Bruk deretter følgende kommandoer
	
## Run Locally 
	git clone https://github.com/HenellTT/KartverketRegister.git
	cd KartverketRegister
	dotnet restore
	dotnet build
	dotnet run
Etter at applikasjonen er startet, vil den være tilgjengelig på http://localhost:5000

# Kjør med Docker

Prosjektet inneholder både Dockerfile og docker-compose.yml.
For å bygge og starte systemet med Docker Compose:
	docker-compose build
	docker-compose up
Dette vil starte både applikasjons-containeren og MariaDB-databasen som er definert i compose-filen.

# Requirements

- .NET 8 SDK
- Docker og Docker Compose
- MariaDB (startes automatisk i Docker, eller via start_mariadb.bat)
- Tilgang til Kartverket API for kartdata

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

## Testing Scenario

| **Scenario** | **Input / Handling** | **Forventet resultat** | **Faktisk resultat** |
|---------------|----------------------|--------------------------|----------------------|
| **Registrer hindring** | Fyll ut skjema med type, breddegrad, lengdegrad og alvorlighetsgrad. | Hindringen vises i listen og på kartet. | Fungerte som forventet |
| **Kartvisning** | Åpne kartvisning med eksisterende hindringer i databasen. | Kartet viser alle hindringer med riktige markører. | Fungerte som forventet |
| **Kjøring i Docker** | Kjør `docker-compose up` i prosjektmappen. | Applikasjon og MariaDB startes automatisk. | Fungerte som forventet |
| **GET-forespørsel** | Gå til `/Obstacle` i nettleseren. | Viser alle registrerte hindringer hentet fra databasen. | Fungerte som forventet |
| **POST-forespørsel (skjema)** | Send inn nytt data via skjema. | Data blir validert og lagret, og visningssiden oppdateres automatisk. | Fungerte som forventet |
| **Datavedvarende lagring** | Start applikasjonen på nytt etter at hindringer er lagret. | Tidligere registrerte hindringer forblir synlige i listen og på kartet. | Fungerte som forventet |


## Results

- Systemet kjører korrekt både lokalt og i Docker.
- Brukerinput via hindringsskjemaet fungerer som forventet, med validering og lagring av data i databasen.
- GET- og POST-forespørsler fungerer som de skal innenfor MVC-strukturen.
- Kartintegrasjonen med Kartverket-API viser markører og oppdateres dynamisk basert på lagrede data.
- MariaDB-databasen lagrer hindringer permanent mellom økter.
- Brukergrensesnittet er responsivt og fungerer på både PC og mobil.
- Totalt sett oppfyller applikasjonen alle krav i oppgaven, inkludert:

- - MVC-struktur
- - Responsivt webgrensesnitt
- - GET/POST-funksjonalitet
- - Skjema og visning av data
- - Kartintegrasjon
- - Dokumentasjon av drift, arkitektur, testing og resultater

