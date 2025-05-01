using System.Diagnostics;
using ChessAPI.Services.DTO.PlayerStats;
using ScottPlot;
using ScottPlot.Plottables;

namespace ChessAPI.Services.Helper;

public class Graph
{
    public static List<PlayerStatsDTO> OrderStatsByDateAndTime(List<PlayerStatsDTO> stats)
        => stats.OrderBy(s => s.FetchedAtDate)
            .ThenBy(s => s.FetchedAtTime)
            .ToList();
    

    public static DateTime[] GetXDates(List<PlayerStatsDTO> orderedStats)
        =>  orderedStats.Select(s => 
                s.FetchedAtDate.ToDateTime(TimeOnly.FromTimeSpan(s.FetchedAtTime)))
            .ToArray();

    public static double[] GetOADates(DateTime[] xDates)
        =>  xDates.Select(d => d.ToOADate()).ToArray();

    public static double[] GetRatings(List<PlayerStatsDTO> orderedStats, Func<PlayerStatsDTO, double?> ratingSelector)
        => orderedStats.Select(s => (double?)ratingSelector(s) ?? double.NaN).ToArray();
    

    public static void SaveIndividualPlot(double[] date, double[] ratings, string legendText, string hexCode)
    {
        Plot plt = new Plot();
        Scatter sc = plt.Add.ScatterLine(date, ratings, Color.FromHex(hexCode));
        sc.LegendText = legendText;

        plt.ShowLegend();
        plt.Title($"{legendText} Historical Rating");
        plt.XLabel("Date");
        plt.YLabel("Rating");
        plt.Axes.DateTimeTicksBottom();
        
        Process.Start(new ProcessStartInfo(SavePlotToFile(plt, legendText)) { UseShellExecute = true });
    }

    public static string SavePlotToFile(Plot plt, string legendText)
    {
        string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
        string folderPath = Path.Combine(desktopPath, "HistoricalData");

        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        string dateString = DateTime.UtcNow.ToString("yyyy-MM-dd");
        string timeString = DateTime.UtcNow.ToString("HH-mm-ss");
        string fileName = $"rating-history-{legendText}-{dateString}-{timeString}.png";
        string filePath = Path.Combine(folderPath, fileName);

        plt.SavePng(filePath, 800, 800);
        return filePath;
    }
}