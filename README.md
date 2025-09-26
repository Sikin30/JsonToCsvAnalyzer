# JSON to CSV File Analyzer

This is a C# console app that reads data from a JSON file, lets you analyze/filter it, and exports results to CSV.

## How to run

1. Build the project:

```bash
dotnet build


Sample 
dotnet run
Enter the path to the JSON file:
sample_data.json
Loaded 3 records.

Menu:
1. Show total records
2. Filter by Country
3. Show average age
4. Export to CSV
5. Exit
Choose an option: 1
Total records: 3

Menu:
1. Show total records
2. Filter by Country
3. Show average age
4. Export to CSV
5. Exit
Choose an option: 2
Enter country to filter by: USA
Found 2 records for country: USA

Menu:
1. Show total records
2. Filter by Country
3. Show average age
4. Export to CSV
5. Exit
Choose an option: 4
Enter file path to save CSV (e.g., output.csv): output20250926.csv
Data exported successfully to output20250926.csv

Menu:
1. Show total records
2. Filter by Country
3. Show average age
4. Export to CSV
5. Exit
Choose an option: 5
Goodbye!