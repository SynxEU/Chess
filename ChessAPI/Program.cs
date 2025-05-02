using System.Diagnostics;
using System.Drawing;
using ChessAPI.Models;
using System.Globalization;
using System.Text.RegularExpressions;
using ChessAPI.DataCollect.API;
using ChessAPI.Services.DTO.PlayerStats;
using ChessAPI.Services.DTO.UserData;
using ChessAPI.Services.Extenstions;
using ChessAPI.Services.Helper;
using ChessAPI.Services.Services;
using ScottPlot;
using Spectre.Console;
using Color = ScottPlot.Color;
using Timer = System.Timers.Timer;

class Program
{
    static void Main(string[] args)
    {
        var factory = new DbContextFactory();
        var context = factory.CreateDbContext(args);
        
        bool showMenu;
        do
        {
            showMenu = Menu(context).Result;
        } while (showMenu);
    }

    private static async Task<bool> Menu(ChessDbContext context)
    {
        AnsiConsole.Clear();

        var choice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("[bold green]=== Chess API Menu ===[/]")
                .AddChoices(new[]
                {
                    "Player Search",
                    "Historical stats",
                    "[red]Exit[/]"
                }));

        switch (choice)
        {
            case "Player Search":
                return await PlayerSearch(context);
            case "Historical stats":
                return await ChoosePlayer(context);
            case "[red]Exit[/]":
                AnsiConsole.MarkupLine("[yellow]Exiting...[/]");
                return false;
            default:
                return true;
        }
    }
    
    private static async Task<bool> PlayerSearch(ChessDbContext context)
    {
        AnsiConsole.Clear();

        var username = AnsiConsole.Ask<string>(
            "[bold green]Enter a Chess.com username[/] ([grey]or type [red]exit[/] to quit[/]):");

        if (string.Equals(username, "exit", StringComparison.OrdinalIgnoreCase))
        {
            AnsiConsole.MarkupLine("[yellow]Exiting...[/]");
            return false;
        }

        if (string.IsNullOrWhiteSpace(username))
        {
            AnsiConsole.MarkupLine("[red]No username provided. Please try again.[/]");
            Console.ReadKey();
            return true;
        }
        
        AnsiConsole.Clear();

        string profileUrl = $"https://api.chess.com/pub/player/{username.ToLower()}";

        await DataMongo.InsertRawDataIntoMongoDB(profileUrl, username);

        CultureInfo format = new CultureInfo("de-DE");

        try
        {
            ChessPlayerDTO player = (await LoadData.GetPlayerFromDB(context, username)).ToDTO();
            PlayerStatsDTO stats = (await LoadData.GetStatsFromDB(context, player.ToModel())).ToDTO();

            Display(player, stats, format);
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Error fetching data for '{username}': {ex.Message}[/]");
        }

        AnsiConsole.MarkupLine("\n[grey]Press any key to return to the menu...[/]");
        Console.ReadKey();

        return true;
    }

    private static async Task<bool> ChoosePlayer(ChessDbContext context)
    {
        AnsiConsole.Clear();

        List<ChessPlayerDTO> players = await GetHistoricalData.GetLatestPlayersAsync(context);
        int pageSize = 5;
        int totalPages = (int)Math.Ceiling((double)players.Count / pageSize);
        int currentPage = 0;

        while (true)
        {
            List<ChessPlayerDTO> paginatedPlayers = players.Skip(currentPage * pageSize).Take(pageSize).ToList();
        
            List<string> playerChoices = paginatedPlayers.Select(p => FormatPlayerChoice(p)).ToList();
        
            playerChoices.Add("[green]Next Page[/]");
            playerChoices.Add("[yellow]Previous Page[/]");
            playerChoices.Add("[red]Exit[/]");
        
            string choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title($"Select a player or select [bold][red]Exit[/][/] to exit. Page [green]{currentPage + 1}[/] of [yellow]{totalPages}[/].")
                    .AddChoices(playerChoices)
            );

            if (choice.ToLower() == "[red]exit[/]") return false;
            if (choice.ToLower() == "[green]next page[/]" && currentPage < totalPages - 1) { currentPage++; continue; }
            if (choice.ToLower() == "[yellow]previous page[/]" && currentPage > 0) { currentPage--; continue; }

            string rawChoice = StripFormatting(choice);

            ChessPlayerDTO selectedPlayer = players.FirstOrDefault(p => p.Username == rawChoice);
            if (selectedPlayer != null)
            {
                await ShowHistoricalStats(context, selectedPlayer);
            }
            else
            {
                AnsiConsole.MarkupLine($"No such player found: [red bold]{rawChoice}[/]");
            }

            AnsiConsole.MarkupLine("\n[grey]Press any key to return to the menu...[/]");
            Console.ReadKey();
            return true;
        }
    }
    
    private static async Task ShowHistoricalStats(ChessDbContext context, ChessPlayerDTO player)
    {
        List<PlayerStatsDTO> stats = await GetHistoricalData.GetHistoryStatsFromDB(context, player.ToModel());
        var orderedStats = Graph.OrderStatsByDateAndTime(stats); 

        DateTime[] xDates = Graph.GetXDates(orderedStats);
        double[] dates = Graph.GetOADates(xDates);

        double[] bullet = Graph.GetRatings(orderedStats, s => s.ChessBullet?.Last.Rating); 
        double[] blitz = Graph.GetRatings(orderedStats, s => s.ChessBlitz?.Last.Rating); 
        double[] rapid = Graph.GetRatings(orderedStats, s => s.ChessRapid?.Last.Rating); 
        double[] daily = Graph.GetRatings(orderedStats, s => s.ChessDaily?.Last.Rating);
        double[] fide = orderedStats.Select(s => (double)s.Fide).ToArray(); 

        Graph.SaveIndividualPlot(dates, bullet, "Bullet", "#964B00");
        Graph.SaveIndividualPlot(dates, blitz, "Blitz", "#F9B234");
        Graph.SaveIndividualPlot(dates, rapid, "Rapid",  "#50C878");
        Graph.SaveIndividualPlot(dates, daily, "Daily", "#FFDF22");
        Graph.SaveIndividualPlot(dates, fide, "FIDE", "#000000");

        AnsiConsole.MarkupLine("[green]Graphs have been saved[/]");
        AnsiConsole.MarkupLine("\n[grey]Press any key to return to the menu...[/]");
        Console.ReadKey();

        await Menu(context);
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
    
    private static string FormatPlayerChoice(ChessPlayerDTO player)
        => $"[#90d14b] {player.Username[0].ToString().ToUpper()}{player.Username.Substring(1)} [/]";
    
    private static string StripFormatting(string input)
        => Regex.Replace(input, @"\[[^\]]*\]", "").Trim().ToLower();
}