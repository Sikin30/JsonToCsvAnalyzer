
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace JsonToCsvAnalyzer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Enter the path to the JSON file:");
            string jsonFilePath = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(jsonFilePath) || !File.Exists(jsonFilePath))
            {
                Console.WriteLine("File not found or invalid path!");
                return;
            }

            List<Person> people;
            try
            {
                string jsonString = File.ReadAllText(jsonFilePath);
                people = JsonSerializer.Deserialize<List<Person>>(jsonString);
                if (people == null)
                {
                    Console.WriteLine("No data found in JSON file.");
                    return;
                }
                Console.WriteLine($"Loaded {people.Count} records.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error reading or parsing JSON file:");
                Console.WriteLine(ex.Message);
                return;
            }

            var analyzer = new PersonAnalyzer(people);
            var menu = new ConsoleMenu(analyzer);
            menu.Run();
        }
    }

    public class PersonAnalyzer
    {
        private readonly List<Person> _originalPeople;
        private List<Person> _currentPeople;

        public PersonAnalyzer(List<Person> people)
        {
            _originalPeople = new List<Person>(people);
            _currentPeople = new List<Person>(people);
        }

        public IReadOnlyList<Person> People => _currentPeople;

        public void ResetFilter()
        {
            _currentPeople = new List<Person>(_originalPeople);
        }

        public void FilterByCountry(string country)
        {
            if (string.IsNullOrWhiteSpace(country)) return;
            _currentPeople = _currentPeople.Where(p => string.Equals(p.Country, country, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        public double? GetAverageAge()
        {
            if (_currentPeople.Count == 0) return null;
            return _currentPeople.Average(p => p.Age);
        }

        public List<string> GetCountries()
        {
            return _originalPeople.Select(p => p.Country).Distinct().OrderBy(c => c).ToList();
        }

        public void ExportToCsv(string filePath)
        {
            using var writer = new StreamWriter(filePath);
            writer.WriteLine("Id,Name,Age,Country");
            foreach (var person in _currentPeople)
            {
                string line = $"{person.Id},{EscapeCsv(person.Name)},{person.Age},{EscapeCsv(person.Country)}";
                writer.WriteLine(line);
            }
        }

        private string EscapeCsv(string s)
        {
            if (s == null) return "";
            if (s.Contains(",") || s.Contains("\""))
            {
                s = s.Replace("\"", "\"\"");
                s = $"\"{s}\"";
            }
            return s;
        }
    }

    public class ConsoleMenu
    {
        private readonly PersonAnalyzer _analyzer;
        public ConsoleMenu(PersonAnalyzer analyzer)
        {
            _analyzer = analyzer;
        }

        public void Run()
        {
            while (true)
            {
                Console.WriteLine("\nMenu:");
                Console.WriteLine("1. Show all records");
                Console.WriteLine("2. Show total records");
                Console.WriteLine("3. Filter by Country");
                Console.WriteLine("4. Show average age");
                Console.WriteLine("5. Export to CSV");
                Console.WriteLine("6. Reset filter");
                Console.WriteLine("7. Exit");
                Console.Write("Choose an option: ");

                string choice = Console.ReadLine();
                switch (choice)
                {
                    case "1":
                        ShowAllRecords();
                        break;
                    case "2":
                        ShowTotalRecords();
                        break;
                    case "3":
                        FilterByCountry();
                        break;
                    case "4":
                        ShowAverageAge();
                        break;
                    case "5":
                        ExportToCsv();
                        break;
                    case "6":
                        _analyzer.ResetFilter();
                        Console.WriteLine("Filter reset. Showing all records.");
                        break;
                    case "7":
                        Console.WriteLine("Goodbye!");
                        return;
                    default:
                        Console.WriteLine("Invalid option, try again.");
                        break;
                }
            }
        }

        private void ShowAllRecords()
        {
            var people = _analyzer.People;
            if (people.Count == 0)
            {
                Console.WriteLine("No records to display.");
                return;
            }
            foreach (var person in people)
            {
                Console.WriteLine($"{person.Id}, {person.Name}, {person.Age}, {person.Country}");
            }
        }

        private void ShowTotalRecords()
        {
            Console.WriteLine($"Total records: {_analyzer.People.Count}");
        }

        private void ShowAverageAge()
        {
            var avg = _analyzer.GetAverageAge();
            if (avg == null)
            {
                Console.WriteLine("No data to calculate average age.");
            }
            else
            {
                Console.WriteLine($"Average Age: {avg:F2}");
            }
        }

        private void FilterByCountry()
        {
            var countries = _analyzer.GetCountries();
            if (countries.Count == 0)
            {
                Console.WriteLine("No country data available.");
                return;
            }
            Console.WriteLine("Available countries:");
            Console.WriteLine(string.Join(", ", countries));
            Console.Write("Enter country to filter by: ");
            string country = Console.ReadLine();
            if (!countries.Any(c => string.Equals(c, country, StringComparison.OrdinalIgnoreCase)))
            {
                Console.WriteLine("Country not found in data.");
                return;
            }
            _analyzer.FilterByCountry(country);
            Console.WriteLine($"Filter applied. Showing records for country: {country}");
        }

        private void ExportToCsv()
        {
            Console.Write("Enter file path to save CSV (e.g., output.csv): ");
            string csvFilePath = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(csvFilePath))
            {
                Console.WriteLine("Invalid file path.");
                return;
            }
            try
            {
                _analyzer.ExportToCsv(csvFilePath);
                Console.WriteLine($"Data exported successfully to {csvFilePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error exporting to CSV:");
                Console.WriteLine(ex.Message);
            }
        }
    }
}
