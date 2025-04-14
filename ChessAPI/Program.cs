using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ChessAPI.Models;
using System.Globalization;
using Spectre.Console;

class Program
{
    static async Task Main()
    {
        // var username = "magnuscarlsen";
        // var username = "hikaru";
        var username = "gothamchess";
        var statsUrl = $"https://api.chess.com/pub/player/{username.ToLower()}/stats";
        var profileUrl = $"https://api.chess.com/pub/player/{username.ToLower()}";
        
        // Use "de-DE" for dot (1.234), or "fr-FR" for space (1 234)
        var numberFormat = new CultureInfo("de-DE"); 

        using var client = new HttpClient();
        client.DefaultRequestHeaders
            .UserAgent
            .ParseAdd("CSharpApp/1.0");

        var statsResponse = await client.GetAsync(statsUrl);
        var profileResponse = await client.GetAsync(profileUrl);

        if (!statsResponse.IsSuccessStatusCode 
            || !profileResponse.IsSuccessStatusCode)
        {
            Console.WriteLine($"Failed to fetch data. Status codes: Stats = {statsResponse.StatusCode}, Profile = {profileResponse.StatusCode}");
            return;
        }

        var statsJson = await statsResponse.Content.ReadAsStringAsync();
        var profileJson = await profileResponse.Content.ReadAsStringAsync();

        var stats = JsonConvert.DeserializeObject<Stats>(statsJson);
        var player = JsonConvert.DeserializeObject<ChessPlayer>(profileJson);
        
        AnsiConsole.Write(new Panel(new Markup(
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



        
        if (player.is_streamer && player.streaming_platforms != null)
        {
            var streamTable = new Table()
                .Border(TableBorder.Rounded)
                .Title("[bold blue]Streaming Platforms[/]")
                .AddColumn("[bold]Platform[/]")
                .AddColumn("[bold]Channel URL[/]");

            foreach (var platform in player.streaming_platforms)
            {
                streamTable.AddRow(platform.type, platform.channel_url);
            }

            AnsiConsole.Write(streamTable);
        }
        
        void WriteCombinedStats(string title, Stats modeStats)
        {
            var table = new Table().Border(TableBorder.Rounded).Title($"[bold blue]{title}[/]");

            table.AddColumn("[bold]Mode[/]");
            table.AddColumn("[bold]Last Rating[/]");
            table.AddColumn("[bold]Best Rating[/]");
            table.AddColumn("[bold green]Wins[/]");
            table.AddColumn("[bold red]Losses[/]");

            table.AddRow(
                "[bold]Daily[/]",
                modeStats.chess_daily?.last?.rating.ToString("N0", numberFormat) ?? "N/A",
                modeStats.chess_daily?.best?.rating.ToString("N0", numberFormat) ?? "N/A",
                modeStats.chess_daily?.record?.win.ToString("N0", numberFormat) ?? "N/A",
                modeStats.chess_daily?.record?.loss.ToString("N0", numberFormat) ?? "N/A"
            );
            
            table.AddRow(
                "[bold]Blitz[/]",
                modeStats.chess_blitz?.last?.rating.ToString("N0", numberFormat) ?? "N/A",
                modeStats.chess_blitz?.best?.rating.ToString("N0", numberFormat) ?? "N/A",
                modeStats.chess_blitz?.record?.win.ToString("N0", numberFormat) ?? "N/A",
                modeStats.chess_blitz?.record?.loss.ToString("N0", numberFormat) ?? "N/A"
            );

            table.AddRow(
                "[bold]Bullet[/]",
                modeStats.chess_bullet?.last?.rating.ToString("N0", numberFormat) ?? "N/A",
                modeStats.chess_bullet?.best?.rating.ToString("N0", numberFormat) ?? "N/A",
                modeStats.chess_bullet?.record?.win.ToString("N0", numberFormat) ?? "N/A",
                modeStats.chess_bullet?.record?.loss.ToString("N0", numberFormat) ?? "N/A"
            );

            table.AddRow(
                "[bold]Rapid[/]",
                modeStats.chess_rapid?.last?.rating.ToString("N0", numberFormat) ?? "N/A",
                modeStats.chess_rapid?.best?.rating.ToString("N0", numberFormat) ?? "N/A",
                modeStats.chess_rapid?.record?.win.ToString("N0", numberFormat) ?? "N/A",
                modeStats.chess_rapid?.record?.loss.ToString("N0", numberFormat) ?? "N/A"
            );
            
            AnsiConsole.Write(table);
        }
        
        if (stats != null)
            WriteCombinedStats("Stats Overview", stats);

        if (stats.tactics.highest.rating > 400)
        {
            var tacticsTable = new Table()
                .Border(TableBorder.Rounded)
                .Title("[bold blue]Tactics[/]")
                .AddColumn("[bold]Highest Rating[/]")
                .AddColumn("[bold]Lowest Rating[/]")
                .AddColumn("[bold]Highest Date[/]")
                .AddColumn("[bold]Lowest Date[/]");

            tacticsTable.AddRow(
                stats.tactics?.highest?.rating.ToString("N0", numberFormat) ?? "N/A",
                stats.tactics?.lowest?.rating.ToString("N0", numberFormat) ?? "N/A",
                stats.tactics?.highest?.date != null ? UnixToDate(stats.tactics.highest.date) : "N/A",
                stats.tactics?.lowest?.date != null ? UnixToDate(stats.tactics.lowest.date) : "N/A"
            );
            
            AnsiConsole.Write(tacticsTable);
        }

    }

    static string UnixToDate(long unixTime) =>
        DateTimeOffset.FromUnixTimeSeconds(unixTime).ToLocalTime().Date.ToShortDateString();
}
