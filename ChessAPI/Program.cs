using Newtonsoft.Json;
using ChessAPI.Models;
using System.Globalization;
using Spectre.Console;

class Program
{
    static async Task Main()
    {
        string? username = string.Empty;
        
        // Username will become dynamic next major update
        
        // username = "magnuscarlsen";
        // username = "hikaru";
        username = "gothamchess";
        
        
        string basePlayerURL = "https://api.chess.com/pub/player/";
        string profileUrl = string.Empty;
        
        if (!string.IsNullOrEmpty(username) 
            || !string.IsNullOrWhiteSpace(username))
        {
            profileUrl = basePlayerURL + username.ToLower();
        }
        
        // Use "de-DE" for dot (1.234), or "fr-FR" for space (1 234)
        CultureInfo numberFormat = new CultureInfo("de-DE"); 

        using HttpClient client = new HttpClient();
        client.DefaultRequestHeaders
            .UserAgent
            .ParseAdd("CSharpApp/1.0");

        HttpResponseMessage profileResponse = await client.GetAsync(profileUrl);
        HttpResponseMessage statsResponse = await client.GetAsync(string.Concat(profileUrl, "/stats"));

        if (!statsResponse.IsSuccessStatusCode 
            || !profileResponse.IsSuccessStatusCode)
        {
            Console.WriteLine($"Failed to fetch data. " +
                              $"Status codes:\n" +
                              $" Stats = {statsResponse.StatusCode}\n" +
                              $" Profile = {profileResponse.StatusCode}");
            return;
        }

        string statsJson = await statsResponse.Content.ReadAsStringAsync();
        string profileJson = await profileResponse.Content.ReadAsStringAsync();

        Stats? stats = JsonConvert.DeserializeObject<Stats>(statsJson);
        ChessPlayer? player = JsonConvert.DeserializeObject<ChessPlayer>(profileJson);
        
        AnsiConsole.Write(new Panel(
                new Markup(
                    $"[bold yellow]Player Profile[/]\n\n" +
                    $"[bold]Name:[/] {player?.name ?? "N/A"}\n" +
                    $"[bold]ID:[/] {player?.player_id.ToString("N0", numberFormat) ?? "N/A"}\n" +
                    $"[bold]League:[/] {player?.league ?? "N/A"}\n" +
                    $"[bold]FIDE Rating:[/] {stats?.fide.ToString("N0", numberFormat) ?? "N/A"}\n" +
                    $"[bold]Status:[/] {player?.status ?? "N/A"}\n" +
                    $"[bold]Followers:[/] {player?.followers.ToString("N0", numberFormat) ?? "N/A"}\n" +
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
                    .ToString("N0", numberFormat) ?? "N/A",
                modeStats.chess_daily?
                    .best?.rating
                    .ToString("N0", numberFormat) ?? "N/A",
                modeStats.chess_daily?
                    .record?.win
                    .ToString("N0", numberFormat) ?? "N/A",
                modeStats.chess_daily?
                    .record?.draw
                    .ToString("N0", numberFormat) ?? "N/A",
                modeStats.chess_daily?
                    .record?.loss
                    .ToString("N0", numberFormat) ?? "N/A"
            );
            
            table.AddRow(
                "[bold]Blitz[/]",
                modeStats.chess_blitz?
                    .last?.rating
                    .ToString("N0", numberFormat) ?? "N/A",
                modeStats.chess_blitz?
                    .best?.rating
                    .ToString("N0", numberFormat) ?? "N/A",
                modeStats.chess_blitz?
                    .record?.win
                    .ToString("N0", numberFormat) ?? "N/A",
                modeStats.chess_blitz?
                    .record?.draw
                    .ToString("N0", numberFormat) ?? "N/A",
                modeStats.chess_blitz?
                    .record?.loss
                    .ToString("N0", numberFormat) ?? "N/A"
            );

            table.AddRow(
                "[bold]Bullet[/]",
                modeStats.chess_bullet?
                    .last?.rating
                    .ToString("N0", numberFormat) ?? "N/A",
                modeStats.chess_bullet?
                    .best?.rating
                    .ToString("N0", numberFormat) ?? "N/A",
                modeStats.chess_bullet?
                    .record?.win
                    .ToString("N0", numberFormat) ?? "N/A",
                modeStats.chess_bullet?
                    .record?.draw
                    .ToString("N0", numberFormat) ?? "N/A",
                modeStats.chess_bullet?
                    .record?.loss
                    .ToString("N0", numberFormat) ?? "N/A"
            );

            table.AddRow(
                "[bold]Rapid[/]",
                modeStats.chess_rapid?
                    .last?.rating
                    .ToString("N0", numberFormat) ?? "N/A",
                modeStats.chess_rapid?
                    .best?.rating
                    .ToString("N0", numberFormat) ?? "N/A",
                modeStats.chess_rapid?
                    .record?.win
                    .ToString("N0", numberFormat) ?? "N/A",
                modeStats.chess_rapid?
                    .record?.draw
                    .ToString("N0", numberFormat) ?? "N/A",
                modeStats.chess_rapid?
                    .record?.loss
                    .ToString("N0", numberFormat) ?? "N/A"
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
                    .ToString("N0", numberFormat) ?? "N/A",
                stats.tactics?
                    .lowest?.rating
                    .ToString("N0", numberFormat) ?? "N/A",
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
