using Newtonsoft.Json;
using ChessAPI.Models;
using System.Globalization;
using ChessAPI.DataCollect.API;
using Spectre.Console;

class Program
{
    static async Task Main()
    {
        string? username = string.Empty;
        
        // Username will become dynamic next major update
        // These will be seated data to make a select menu
        
        // username = "magnuscarlsen"; // Premium + no streamer
        // username = "hikaru"; // Streamer + Premium
        // username = "gothamchess"; // Streamer
        // username = "synx_eu"; // Basic
        // username = "dewa_kipas"; // Banned
        // username = "erik"; // Staff
        username = "nox"; // Mod
        
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
        
        // Use "de-DE" for dot (1.234), or "fr-FR" for space (1 234)
        CultureInfo format = new CultureInfo("de-DE"); 

        Stats? stats = await GetData.GetStats(string.Concat(profileUrl, "/stats"));
        ChessPlayer? player = await GetData.GetPlayer(profileUrl);
        
        AnsiConsole.Write(new Panel(
                new Markup(
                    $"[bold yellow]Player Profile[/]\n\n" +
                    $"[bold]ID:[/] {player?.player_id.ToString("N0", format) ?? "N/A"}\n" +
                    $"[bold]Name:[/] {player?.name ?? "No name provided"}\n" +
                    $"[bold]Username:[/] {player?.username ?? "N/A"}\n" +
                    $"[bold]League:[/] {player?.league ?? "N/A"}\n" +
                    $"[bold]FIDE Rating:[/] {stats?.fide.ToString("N0", format) ?? "N/A"}\n" +
                    $"[bold]Status:[/] " +
                    $"{(
                        player?.status == "closed:fair_play_violations" ? "[red bold]BANNED[/]" 
                        : player?.status == "closed" ? "[red bold]CLOSED[/]" 
                        : player?.status == "mod" ? "[#005faf bold]MODERATOR[/]" 
                        : player?.status == "staff" ? "[green bold]STAFF[/]" 
                        : player?.status == "premium" ? "[#00ffff bold]PREMIUM[/]" 
                        : player?.status == "basic" ? "[#4e7837 bold]BASIC[/]" 
                        : player?.status ?? "N/A"
                        )}\n" +
                    $"[bold]Followers:[/] {player?.followers.ToString("N0", format) ?? "N/A"}\n" +
                    $"[bold]Location:[/] {player?.location ?? "N/A"}\n" +
                    $"[bold]Last Online:[/] {(player?.last_online != null ? UnixToDate(player.last_online) : "N/A")}\n" +
                    $"[bold]Joined:[/] {(player?.joined != null ? UnixToDate(player.joined) : "N/A")}\n" +
                    $"[bold]Verified:[/] {(player?.verified == true ? "[green]Yes[/]" : "[red]No[/]")}\n" +
                    $"[bold]Is Streamer:[/] {(player?.is_streamer == true ? "[green]Yes[/]" : "[red]No[/]")}"
                ))
            .Border(BoxBorder.Rounded)
            .Padding(new Padding(1)));



        
        if (player is 
            { is_streamer: true })
        {
            Table streamTable = new Table()
                .Border(TableBorder.Rounded)
                .Title("[bold blue]Streaming Platforms[/]")
                .AddColumn("[bold]Platform[/]")
                .AddColumn("[bold]Channel URL[/]");

            foreach (StreamingPlatform platform in player.streaming_platforms)
            {
                streamTable.AddRow(platform.type, platform.channel_url);
            }

            AnsiConsole.Write(streamTable);
        }
        
        void WriteCombinedStats(string title, Stats modeStats)
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
                modeStats.chess_daily?
                    .last?.rating
                    .ToString("N0", format) ?? "N/A",
                modeStats.chess_daily?
                    .best?.rating
                    .ToString("N0", format) ?? "N/A",
                modeStats.chess_daily?
                    .record?.win
                    .ToString("N0", format) ?? "N/A",
                modeStats.chess_daily?
                    .record?.draw
                    .ToString("N0", format) ?? "N/A",
                modeStats.chess_daily?
                    .record?.loss
                    .ToString("N0", format) ?? "N/A"
            );
            
            table.AddRow(
                "[bold]Blitz[/]",
                modeStats.chess_blitz?
                    .last?.rating
                    .ToString("N0", format) ?? "N/A",
                modeStats.chess_blitz?
                    .best?.rating
                    .ToString("N0", format) ?? "N/A",
                modeStats.chess_blitz?
                    .record?.win
                    .ToString("N0", format) ?? "N/A",
                modeStats.chess_blitz?
                    .record?.draw
                    .ToString("N0", format) ?? "N/A",
                modeStats.chess_blitz?
                    .record?.loss
                    .ToString("N0", format) ?? "N/A"
            );

            table.AddRow(
                "[bold]Bullet[/]",
                modeStats.chess_bullet?
                    .last?.rating
                    .ToString("N0", format) ?? "N/A",
                modeStats.chess_bullet?
                    .best?.rating
                    .ToString("N0", format) ?? "N/A",
                modeStats.chess_bullet?
                    .record?.win
                    .ToString("N0", format) ?? "N/A",
                modeStats.chess_bullet?
                    .record?.draw
                    .ToString("N0", format) ?? "N/A",
                modeStats.chess_bullet?
                    .record?.loss
                    .ToString("N0", format) ?? "N/A"
            );

            table.AddRow(
                "[bold]Rapid[/]",
                modeStats.chess_rapid?
                    .last?.rating
                    .ToString("N0", format) ?? "N/A",
                modeStats.chess_rapid?
                    .best?.rating
                    .ToString("N0", format) ?? "N/A",
                modeStats.chess_rapid?
                    .record?.win
                    .ToString("N0", format) ?? "N/A",
                modeStats.chess_rapid?
                    .record?.draw
                    .ToString("N0", format) ?? "N/A",
                modeStats.chess_rapid?
                    .record?.loss
                    .ToString("N0", format) ?? "N/A"
            );
            
            AnsiConsole.Write(table);
        }
        
        if (stats != null)
            WriteCombinedStats("Stats Overview", stats);

        if (stats.tactics.highest.rating > 400)
        {
            Table tacticsTable = new Table()
                .Border(TableBorder.Rounded)
                .Title("[bold blue]Lessons[/]")
                .AddColumn("[bold]Highest Rating[/]")
                .AddColumn("[bold]Lowest Rating[/]")
                .AddColumn("[bold]Highest Date[/]")
                .AddColumn("[bold]Lowest Date[/]");

            tacticsTable.AddRow(
                stats.tactics?
                    .highest?.rating
                    .ToString("N0", format) ?? "N/A",
                stats.tactics?
                    .lowest?.rating
                    .ToString("N0", format) ?? "N/A",
                stats.tactics?
                    .highest?.date != null ? UnixToDate(stats.tactics.highest.date) : "N/A",
                stats.tactics?
                    .lowest?.date != null ? UnixToDate(stats.tactics.lowest.date) : "N/A"
            );
            
            AnsiConsole.Write(tacticsTable);
        }

    }

    static string UnixToDate(long unixTime) 
        => DateTimeOffset.FromUnixTimeSeconds(unixTime)
            .ToLocalTime()
            .Date.ToShortDateString();
}
