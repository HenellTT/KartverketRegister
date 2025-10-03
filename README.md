# KartverketRegister

Oppgave 1. Å opprette en ASP.NET Core MVC applikasjon med frontend, skjema og kart


Prosjektoppgave UiA Institutt for informasjonssystemer, Kartverket og Norsk Luftambulanse (NLA)

>_<

# Hvordan starte applikasjonen??

	Sørg for at docker desktop kjører til enhver tid!

## Docker Compose

	Åpne terminal i Mappa til repoen og skriv - Docker compose build
	
	Etter applikasjonen er bygget kan du starte den med - Docker compose up
	
	Applikasjonen skal nå kjøre på 8080 port!

## Visual Studio

	Kjør start_mariadb.bat - Den starter opp databasen
	
	Åpne KartverketRegister.sln med Visual Studio
	
	Start applikasjonen med http
	
## terminal

	Kjør start_mariadb.bat - Den starter opp databasen

	Åpne terminal i repoen og skriv - Dotnet build
	
	Etter den blir ferdig skriv - Dotnet Run
	
## Debug

	Om docker compose ikke starter på første forsøk så prøv en gang til, for databasen starter av og til pararelt med MVC appen og derfor kræsjer appen
	

# Systemarkitektur

## Oversikt
	Applikasjonen er en ASP.NET Core MVC applikasjon som kjøres i en Docker Container. 
	Den følger Model-View-Controller(MVC) sitt arkitekture rammeverk og leverer responsive nettsider med dynamisk innhold.
	Brukerinteraksjon skjer via skjemaer og kart, og data som flyter mellom frontend, backend og databasen.
	
## Hovedkomponenter
- Controller
    - Håndterer HTTP Forespørseler (GET og POST)
    - Sender forespørseler til riktig logikk og retunerer views med data
- Models
    - Inneholder data som vises i brukergrensesnittet
- View
    - Ansvar for visning av user interface
    - viser data som er lagd i models til brukeren
- Database
    - Lagrer brukernavn og henter informasjon fra skjema og kart
    - Kommuniserer med backend
- Docker
    - Pakker applikasjonen i en container og sørger for at applikasjonen kjøres likt
- kart
    - integrert kart fra Leaflet i frontend
    - lar brukeren velge sted på kart, som sendes videre til backend


