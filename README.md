# KartverketRegister

Oppgave 1. Å opprette en ASP.NET Core MVC applikasjon med frontend, skjema og kart

1. Controller, view modell og view
2. Responsive nettsider med dynamisk innhold hentet fra web server.
3. At applikasjonen håndterer GET og POST forespørsler.
4. Skjema som tar data fra brukeren og at data vises i annen nettside.
5. Kart i tillegg til skjemaet (punktet over) og at data hentes fra kartet og vises i en annen nettside.

6. Dokumentasjon i Github om drift, system arkitektur, testing scenarier og resultater.
7. Dokumentasjonen i selve koden.

_______________________________________________________________________________________________
Prosjektoppgave UiA Institutt for informasjonssystemer, Kartverket og Norsk Luftambulanse (NLA)
_______________________________________________________________________________________________


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
	= Den skal være good nå.

	Om lagring av markere feiler, kan det hende du har gammel database oppsett fra tidligere versjon av applikasjonen. Bruk RESET DATABASE knappen, det burde løse problemmet!
	- Ved final levering blir det fiksa

# DATABASE MIGRASJON
	Automatisk - Eller:
	Start applikasjonen, Gå til /Migrate . Der kommer opp et passord input. Passordet er secrethash. Om du skriver riktig passord så skal database migrasjonen kjøre.

# Default Brukere

- 1234@user.test
- 1234@admin.test
- 1234@employee.test

Passord til alle brukere er !Testink00!

# Drift
The app can be run locallly or inside Docker.
For å kjøre systemet uten Docker må du ha .NET 8 SDK installert.
Bruk deretter følgende kommandoer
	

# Requirements

- .NET 9 SDK
- Docker
- MariaDB (startes automatisk i Docker, eller via start_mariadb.bat)
- Tilgang til internet for kartet

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

## Security
- XSS
- Anti Forgery Tokens
- SQL Injections
- Password Hashing - Raw passwords not saved
- HTML Injections

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

## User Tests


### Administrating
**Mål:** Evaluere hvor enkelt det er for en administrator å delegere en oppgave ved å tildele en markør til en ubehandlet innsending.

Main landing page - Admin
><img width="50%" height="50%" alt="Main landing page - Admin" src="https://github.com/user-attachments/assets/855e425a-8ad1-455d-a5f1-5ef527b53149" />

Assign submissions
><img width="50%" height="50%" alt="Assign submissions" src="https://github.com/user-attachments/assets/3cdaa3ab-014a-4dbc-9fe8-4511a8e88968"  />
| Bruker | Observasjoner og Problemer | Resultat | Forslag til forbedring |
| :---: | :--- | :---: | :--- |
| **U1**  | Gikk rett til "Assign submissions" via toppmenyen og fullførte oppgaven raskt. | ✅ | Ingen. |
| **U2**  | Kikket først på "Manage Users", før de fant "Assign submissions". | 🟡 | Gi "Assign submissions" en mer tydelig merking, f.eks. med et tydelig ikon. |
| **U3**  | Fant "Assign submissions" umiddelbart, men brukte tid på å forstå hvilken kolonne/knapp som utløste tildelingsdialogen. | 🟡 | Gjør knappen for **"Assign"** mer synlig og brukervennlig, f.eks. med et tydelig ikon. |
| **U4**  | Klikket på "Submissions" i stedet for "Assign Submissions" først. Måtte gå tilbake. | 🟡 | Tydeliggjør forskjellene mellom **"Submission"** i toppmenyen og **"Assign"** (visuell adskillelse eller endret plassering). |
| **U5**  | Fullførte oppgaven feilfritt og raskt. | ✅ | Ingen. |



### Employee
**Mål:** Evaluere brukervennligheten ved vurdering av innsendt rapport

Main landing page - Employee
> <img width="50%" height="50%" alt="Main landing page - Employee" src="https://github.com/user-attachments/assets/239f1d20-083a-4321-8562-2781e083a88d" />

Review report
> <img width="50%" height="50%" alt="Review report" src="https://github.com/user-attachments/assets/b607b4e2-f665-42c0-a75e-bf0041eae7c4" />
| Bruker | Observasjoner og Problemer | Resultat | Forslag til forbedring |
| :---: | :--- | :---: | :--- |
| **U1**  | Klikket på den første rapporten i listen merket 'Review'. Fullførte vurderingen raskt. | ✅ | Ingen. |
| **U2**  | Var usikker på om de skulle bruke knappen i raden eller klikke på selve rapportnavnet for å starte vurdering. | 🟡 | Standardiser interaksjonen: Gjør hele raden eller et eksplisitt **"Review"-ikon** klikkbart. |
| **U3**  | Klikket på 'Review', men slet med å finne gokjent/ikke godkjent-knapp. | 🟡 | Gjør **'Approve/Reject'** knappen mer fremtredende, f.eks. ved å ha knappene farget med grønt og rødt. |
| **U4**  | Feilfritt og raskt. | ✅ | Ingen. |
| **U5**  | Klarte å navigere til vurderingen, men klaget over at tekstfeltet for kommentarer var for lite. | 🟡 | Øk størrelsen på tekstfeltet for kommentarer i vurderingsskjemaet. |

### User - Pilot
Spørte brukeren om å logge seg inn som bruker - pilot, og sende inn en rapport:

Main landing page - User
> <img width="50%" height="50%" alt="Main landing page - User" src="https://github.com/user-attachments/assets/43770c58-6388-4cf1-b699-e21fd627ef3f" />

FlightMode
> <img width="50%" height="50%" alt="FlightMode w/ marker" src="https://github.com/user-attachments/assets/acf41db8-5b9a-43d7-b76e-84fd7ea21d48" />

Register Marker
> <img width="50%" height="50%" alt="Register Marker" src="https://github.com/user-attachments/assets/f53ad8ce-0ad4-4725-b875-4fbebbace789" />
| Bruker | Observasjoner og Problemer | Resultat | Forslag til forbedring |
| :---: | :--- | :---: | :--- |
| **U1**  | Trykket på 'FlightMode' | ❌ | Gjør overgangen fra 'Start Report' på hovedsiden til **'FlightMode'** mer lineær/tydelig. |
| **U2**  | Fant knappen 'FlightMode' umiddelbart, markerte et punkt og sendte inn. Fullførte registrering feilfritt. | ✅ | Ingen. |
| **U3**  | Var usikker på om de måtte trykke på kartet først eller **'Register Marker'**-knappen. Prøvde kartet uten resultat. | 🟡 | Legg til veiledningstekst: "Trykk 'Register Marker' for å aktivere markørplassering." |
| **U4**  | Utmerket navigasjon. Likte flyten i å velge flyvning før markøren ble registrert. | ✅ | Ingen. |
| **U5**  | Ble overveldet med mange knapper i FlightMode, slet med å finne fram | ❌ | Fjerne unødvendige knapper. |




