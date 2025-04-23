# ChessAPI

## Table of Contents
1. [Om projektet](#om-projektet)
2. [Funktionalitet](#funktionalitet)
   - [TO:DO](#to-do)
3. [Teknologier](#teknologier)
4. [Database](#database)
5. [Data Flow](#data-flow)
6. [Installation og Kørsel](#installation-og-kørsel)
7. [Bidrag](#bidrag)
8. [Kontakt](#kontakt)
9. [Begrænsninger](#begrænsninger)
10. [Licens](#licens)

## Om projektet
Dette projekt bruger Chess.com’s offentlige API til at hente og lagre data om spillere og deres spilstatistikker. \
Projektet er lavet i forbindelse med et skoleprojekt i Big Data og har fokus på dataindsamling og forberedelse til senere analyse.

## Funktionalitet
- Hent spillerprofiler (brugernavn, land, medlemsdato, osv.)
- Hent statistikker (blitz/rapid/bullet ratings og antal spil)
- Lagring i relationel database med EF Core

### TO:DO
To:Do er ting der kommer til projektet med tiden som web versionen lige så stille kommer frem:
- Automatisk håndtering af dataopdatering (Hvis muligt)
- Menu til at finde gammelt data på spiller
- Selv at kunne indtaste spiller gennem konsolen

## Teknologier
Projektet er udviklet med følgende teknologier:
- C# / .NET 9
- Entity Framework Core
- SQL Server Management Studio (SSMS)
- API (Chess.com)
- Newtonsoft.Json (JSON håndtering)

## Database

### Entities
Databasen inkluderer følgende entiteter: \
\
**Models:**
1. UserData:
   - ChessPlayer: indeholder generel information om brugeren.
   - StreamingPlatform: information om hvorvidt brugeren streamer (f.eks. Twitch, YouTube). 
2. Modes:
   - ChessBlitz, ChessRapid, ChessBullet, ChessDaily: hver mode indeholder f.eks. antal spil, rating, wins osv. 
3. PlayerStats:
   - Tactics: Andet ord for lessons folk har lavet og deres nuværende rating
   - Stats 
4. Records: toppræstationer inddelt i:
   - Best
   - Last
   - Record: Win/Draw/loss
5. Scale:
   - Highest, Lowest: bruges f.eks. til at vise højeste og laveste rating over tid.

Databaseforbindelsen er sat op via `ChessDbContext` og benytter en LocalDB som standard.

## Data Flow
1. HttpClient henter JSON fra Chess.com API.
2. JSON parses til models med Newtonsoft.Json.
3. Data gemmes i SQL Server via Entity Framework Core.
4. Models mappes til DTO'er og vises i konsol app

## Installation og Kørsel
1. Klon projektet:
   ```sh
   git clone https://github.com/SynxEU/Chess
   ```
2.  Navigere til startup filen:
   ```sh
   cd ChessAPI
   ```
3. Kør projektet:
   ```sh
   dotnet run
   ```
4. Applikationen starter i konsollen, hvor du får det data som er presat i applikation ( Kommer så du selv kan skrive i fremtidig update)

## Bidrag
Ændringer og issues er mere end velkomne! Hvis du har forslag til forbedringer, fejlrettelser eller nye funktioner, er du velkommen til at oprette en issue eller lave en pull request.

## Kontakt
Hvis du har spørgsmål eller ønsker at bidrage, kan du kontakte mig via Discord under brugernavnet: **synx_eu**. \
**E-mail support er ikke muligt lige nu.**

## Begrænsninger
- Applikationen kører i øjeblikket **kun lokalt**.

## Licens
Dette projekt er udgivet under **MIT-licensen**. Se [LICENSE](LICENSE) for mere information.
