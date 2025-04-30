using ChessAPI.Models;
using System.Globalization;
using ChessAPI.DataCollect.API;
using ChessAPI.Services.DTO.PlayerStats;
using ChessAPI.Services.DTO.UserData;
using ChessAPI.Services.Extenstions;
using Spectre.Console;
using Timer = System.Timers.Timer;

class Program
{
    static async Task Main(string[] args)
    {
        var factory = new DbContextFactory();
        var context = factory.CreateDbContext(args);

        string? username = string.Empty;

        // Username will become dynamic next major update
        // These will be seated data to make a select menu

        // username = "magnuscarlsen"; // Premium + no streamer
        username = "hikaru"; // Streamer + Premium
        // username = "gothamchess"; // Streamer
        // username = "synx_eu"; // Basic
        // username = "dewa_kipas"; // Banned
        // username = "erik"; // Staff
        // username = "nox"; // Mod
        // username = "Mastervoliumpl"; // Jakub

        string basePlayerURL = "https://api.chess.com/pub/player/";
        string profileUrl = string.Empty;

        if (!string.IsNullOrEmpty(username)
            || !string.IsNullOrWhiteSpace(username))
        {
            profileUrl = basePlayerURL + username.ToLower();
        }
        else
        {
            Console.WriteLine($"Failed to fetch data. " +
                              $"No username provided. " +
                              $"Please try again.");
            return;
        }
        
        await DataMongo.InsertRawDataIntoMongoDB(profileUrl);
        
        // Use "de-DE" for dot (1.234), or "fr-FR" for space (1 234)
        CultureInfo format = new CultureInfo("de-DE");

        ChessPlayerDTO player = (await ProcessData.ProcessAndSaveFilteredPlayerData(context)).ToDTO();
        PlayerStatsDTO stats = (await ProcessData.ProcessAndSaveFilteredStatsData(context)).ToDTO();

        Display(player, stats, format);

    }

    private static void Display(ChessPlayerDTO? player, PlayerStatsDTO stats, CultureInfo? format)
    {
        AnsiConsole.Write(new Panel(
                new Markup(
                    $"[bold yellow]Player Profile[/]\n\n" +
                    $"[bold]ID:[/] {player?.ChessId.ToString("N0", format) ?? "N/A"}\n" +
                    $"[bold]Chess.com ID:[/] {player?.playerId.ToString("N0", format) ?? "N/A"}\n" +
                    $"[bold]Name:[/] {player?.Name ?? "No name provided"}\n" +
                    $"[bold]Username:[/] {player?.Username ?? "N/A"}\n" +
                    $"[bold]League:[/] {player?.League ?? "N/A"}\n" +
                    $"[bold]FIDE Rating:[/] {stats?.Fide.ToString("N0", format) ?? "N/A"}\n" +
                    $"[bold]Status:[/] " +
                    $"{(
                        player?.Status == "closed:fair_play_violations" ? "[red bold]BANNED[/]"
                        : player?.Status == "closed" ? "[red bold]CLOSED[/]"
                        : player?.Status == "mod" ? "[#005faf bold]MODERATOR[/]"
                        : player?.Status == "staff" ? "[green bold]STAFF[/]"
                        : player?.Status == "premium" ? "[#00ffff bold]PREMIUM[/]"
                        : player?.Status == "basic" ? "[#4e7837 bold]BASIC[/]"
                        : player?.Status ?? "N/A"
                    )}\n" +
                    $"[bold]Followers:[/] {player?.Followers.ToString("N0", format) ?? "N/A"}\n" +
                    $"[bold]Location:[/] {player?.Location ?? "N/A"}\n" +
                    $"[bold]Last Online:[/] {(player?.LastOnline != null ? UnixToDate(player.LastOnline) : "N/A")}\n" +
                    $"[bold]Joined:[/] {(player?.Joined != null ? UnixToDate(player.Joined) : "N/A")}\n" +
                    $"[bold]Verified:[/] {(player?.Verified == true ? "[green]Yes[/]" : "[red]No[/]")}\n" +
                    $"[bold]Is Streamer:[/] {(player?.IsStreamer == true ? "[green]Yes[/]" : "[red]No[/]")}"
                ))
            .Border(BoxBorder.Rounded)
            .Padding(new Padding(1)));




        if (player is
            { IsStreamer: true })
        {
            Table streamTable = new Table()
                .Border(TableBorder.Rounded)
                .Title("[bold blue]Streaming Platforms[/]")
                .AddColumn("[bold]Platform[/]")
                .AddColumn("[bold]Channel URL[/]");

            foreach (StreamingPlatformDTO platform in player.StreamingPlatforms)
            {
                streamTable.AddRow(platform.Type, platform.ChannelUrl);
            }

            AnsiConsole.Write(streamTable);
        }

        void WriteCombinedStats(string title, PlayerStatsDTO modeStats)
        {
            Table table = new Table()
                .Border(TableBorder.Rounded)
                .Title($"[bold blue]{title}[/]");

            table.AddColumn("[bold]Mode[/]");
            table.AddColumn("[bold]Last Rating[/]");
            table.AddColumn("[bold]Best Rating[/]");
            table.AddColumn("[bold green]Wins[/]");
            table.AddColumn("[bold yellow]Draws[/]");
            table.AddColumn("[bold red]Losses[/]");

            table.AddRow(
                "[bold]Daily[/]",
                modeStats.ChessDaily?
                    .Last?.Rating
                    .ToString("N0", format) ?? "N/A",
                modeStats.ChessDaily?
                    .Best?.Rating
                    .ToString("N0", format) ?? "N/A",
                modeStats.ChessDaily?
                    .Record?.Win
                    .ToString("N0", format) ?? "N/A",
                modeStats.ChessDaily?
                    .Record?.Draw
                    .ToString("N0", format) ?? "N/A",
                modeStats.ChessDaily?
                    .Record?.Loss
                    .ToString("N0", format) ?? "N/A"
            );

            table.AddRow(
                "[bold]Blitz[/]",
                modeStats.ChessBlitz?
                    .Last?.Rating
                    .ToString("N0", format) ?? "N/A",
                modeStats.ChessBlitz?
                    .Best?.Rating
                    .ToString("N0", format) ?? "N/A",
                modeStats.ChessBlitz?
                    .Record?.Win
                    .ToString("N0", format) ?? "N/A",
                modeStats.ChessBlitz?
                    .Record?.Draw
                    .ToString("N0", format) ?? "N/A",
                modeStats.ChessBlitz?
                    .Record?.Loss
                    .ToString("N0", format) ?? "N/A"
            );

            table.AddRow(
                "[bold]Bullet[/]",
                modeStats.ChessBullet?
                    .Last?.Rating
                    .ToString("N0", format) ?? "N/A",
                modeStats.ChessBullet?
                    .Best?.Rating
                    .ToString("N0", format) ?? "N/A",
                modeStats.ChessBullet?
                    .Record?.Win
                    .ToString("N0", format) ?? "N/A",
                modeStats.ChessBullet?
                    .Record?.Draw
                    .ToString("N0", format) ?? "N/A",
                modeStats.ChessBullet?
                    .Record?.Loss
                    .ToString("N0", format) ?? "N/A"
            );

            table.AddRow(
                "[bold]Rapid[/]",
                modeStats.ChessRapid?
                    .Last?.Rating
                    .ToString("N0", format) ?? "N/A",
                modeStats.ChessRapid?
                    .Best?.Rating
                    .ToString("N0", format) ?? "N/A",
                modeStats.ChessRapid?
                    .Record?.Win
                    .ToString("N0", format) ?? "N/A",
                modeStats.ChessRapid?
                    .Record?.Draw
                    .ToString("N0", format) ?? "N/A",
                modeStats.ChessRapid?
                    .Record?.Loss
                    .ToString("N0", format) ?? "N/A"
            );

            AnsiConsole.Write(table);
        }

        if (stats != null)
            WriteCombinedStats("Stats Overview", stats);

        if (stats.Tactics.Highest.Rating > 400)
        {
            Table tacticsTable = new Table()
                .Border(TableBorder.Rounded)
                .Title("[bold blue]Lessons[/]")
                .AddColumn("[bold]Highest Rating[/]")
                .AddColumn("[bold]Lowest Rating[/]")
                .AddColumn("[bold]Highest Date[/]")
                .AddColumn("[bold]Lowest Date[/]");

            tacticsTable.AddRow(
                stats.Tactics?
                    .Highest?.Rating
                    .ToString("N0", format) ?? "N/A",
                stats.Tactics?
                    .Lowest?.Rating
                    .ToString("N0", format) ?? "N/A",
                stats.Tactics?
                    .Highest?.Date != null
                    ? UnixToDate(stats.Tactics.Highest.Date)
                    : "N/A",
                stats.Tactics?
                    .Lowest?.Date != null
                    ? UnixToDate(stats.Tactics.Lowest.Date)
                    : "N/A"
            );

            AnsiConsole.Write(tacticsTable);
        }
    }

    static string UnixToDate(long unixTime)
        => DateTimeOffset.FromUnixTimeSeconds(unixTime)
            .ToLocalTime()
            .Date.ToShortDateString();
}