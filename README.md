# KartverketRegister

Oppgave 1. � opprette en ASP.NET Core MVC applikasjon med frontend, skjema og kart


Prosjektoppgave UiA Institutt for informasjonssystemer, Kartverket og Norsk Luftambulanse (NLA)

>_<

# Hvordan starte applikasjonen??

	S�rg for at docker desktop kj�rer til enhver tid!

## Docker Compose

	�pne terminal i Mappa til repoen og skriv - Docker compose build
	
	Etter applikasjonen er bygget kan du starte den med - Docker compose up
	
	Applikasjonen skal n� kj�re p� 8080 port!

## Visual Studio

	Kj�r start_mariadb.bat - Den starter opp databasen
	
	�pne KartverketRegister.sln med Visual Studio
	
	Start applikasjonen med http
	
## terminal

	Kj�r start_mariadb.bat - Den starter opp databasen

	�pne terminal i repoen og skriv - Dotnet build
	
	Etter den blir ferdig skriv - Dotnet Run
	
## Debug

	Om docker compose ikke starter p� f�rste fors�k s� pr�v en gang til, for databasen starter av og til pararelt med MVC appen og derfor kr�sjer appen