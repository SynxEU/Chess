# ChessAPI

## Table of Contents
1. [Om projektet](#om-projektet)
2. [Funktionalitet](#funktionalitet)
   - [TO:DO](#to-do)
   - [Kendte Problemer](#kendte-problemer)
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
Projektet er lavet i forbindelse med et skoleprojekt i Big Data og har fokus på dataindsamling og visning af historisk data.

## Funktionalitet
- Hent spillerprofiler (brugernavn, land, medlemsdato, osv.)
- Hent statistikker (blitz/rapid/bullet ratings og antal spil)
- Lagring i dokument database (MongoDB)
- Lagring i relationel database med EF Core efter filtrering
- Visning af grafer over historisk data
- Visning af information over spilleren

### TO:DO
- [x] Menu til at finde gammelt data på spiller
- [ ] Grafer til at vise udviklingen af ratings

### Kendte Problemer
- Ikke alt historisk data bliver vist på nuværende tidspunkt
- Dups af stats i SSMS

## Teknologier
Projektet er udviklet med følgende teknologier:
- C# / .NET 9
- Entity Framework Core
- SQL Server Management Studio (SSMS)
- MongoDB
- API ([Chess.com](https://www.chess.com/news/view/published-data-api?ref=public_apis&utm_medium=website))
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
1. HttpClient henter Data fra Chess.com API.
2. Data parses til BsonDocument med MongoDB.
3. Dataen gemmes ufiltret i en MongoDB
4. Dataen hentes igen og filtres
5. Data gemmes i SQL Server via Entity Framework Core.
6. Models mappes til DTO'er og vises i konsol app

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
