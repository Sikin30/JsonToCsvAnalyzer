using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Linq;

class Program
{
    static List<Person> people = new();

    static void Main(string[] args)
    {
        Console.WriteLine("Enter the path to the JSON file:");
        string jsonFilePath = Console.ReadLine();

        if (!File.Exists(jsonFilePath))
        {
            Console.WriteLine("File not found!");
            return;
        }

        try
        {
            string jsonString = File.ReadAllText(jsonFilePath);
            people = JsonSerializer.Deserialize<List<Person>>(jsonString);
            Console.WriteLine($"Loaded {people.Count} records.");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error reading or parsing JSON file:");
            Console.WriteLine(ex.Message);
            return;
        }

        RunMenu();
    }
    
    // Placeholder for menu function - will add next
    static void RunMenu()
{
    while (true)
    {
        Console.WriteLine("\nMenu:");
        Console.WriteLine("1. Show total records");
        Console.WriteLine("2. Filter by Country");
        Console.WriteLine("3. Show average age");
        Console.WriteLine("4. Export to CSV");
        Console.WriteLine("5. Exit");
        Console.Write("Choose an option: ");

        string choice = Console.ReadLine();
        switch (choice)
        {
            case "1":
                ShowTotalRecords();
                break;
            case "2":
                FilterByCountry();
                break;
            case "3":
                ShowAverageAge();
                break;
            case "4":
                ExportToCsv();
                break;
            case "5":
                Console.WriteLine("Goodbye!");
                return;
            default:
                Console.WriteLine("Invalid option, try again.");
                break;
        }
    }
}

static void ShowTotalRecords()
{
    Console.WriteLine($"Total records: {people.Count}");
}

static void ShowAverageAge()
{
    if (people.Count == 0)
    {
        Console.WriteLine("No data to calculate.");
        return;
    }
    double average = people.Average(p => p.Age);
    Console.WriteLine($"Average Age: {average:F2}");
}

static void FilterByCountry()
{
    Console.Write("Enter country to filter by: ");
    string country = Console.ReadLine();

    var filtered = people.Where(p => string.Equals(p.Country, country, StringComparison.OrdinalIgnoreCase)).ToList();

    Console.WriteLine($"Found {filtered.Count} records for country: {country}");

    // Replace people with filtered for other operations
    people = filtered;
}

static void ExportToCsv()
{
    Console.Write("Enter file path to save CSV (e.g., output.csv): ");
    string csvFilePath = Console.ReadLine();

    try
    {
        using var writer = new StreamWriter(csvFilePath);
        writer.WriteLine("Id,Name,Age,Country");

        foreach (var person in people)
        {
            string line = $"{person.Id},{EscapeCsv(person.Name)},{person.Age},{EscapeCsv(person.Country)}";
            writer.WriteLine(line);
        }

        Console.WriteLine($"Data exported successfully to {csvFilePath}");
    }
    catch (Exception ex)
    {
        Console.WriteLine("Error exporting to CSV:");
        Console.WriteLine(ex.Message);
    }
}

// Helper to escape commas and quotes in CSV
static string EscapeCsv(string s)
{
    if (s.Contains(",") || s.Contains("\""))
    {
        s = s.Replace("\"", "\"\"");
        s = $"\"{s}\"";
    }
    return s;
}

}
