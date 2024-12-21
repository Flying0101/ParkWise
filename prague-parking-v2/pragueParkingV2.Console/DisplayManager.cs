using prague_parking_v2.pragueParkingV2.Core.Models;
using prague_parking_v2.pragueParkingV2.Core.Services;
using Spectre.Console;

namespace prague_parking_v2.pragueParkingV2.Console
{
    public class DisplayManager
    {
        private readonly ParkingGarage _parkingGarage;
        private readonly ConfigurationManager _configManager;

        //set values
        public DisplayManager(ParkingGarage parkingGarage, ConfigurationManager configManager)
        {
            _parkingGarage = parkingGarage;
            _configManager = configManager;
        }

        public void ShowMainMenu()
        {
            AnsiConsole.Clear();
            AnsiConsole.Write(
                new FigletText("ParkWise App")
                    .Centered()
                    .Color(Color.Green));

            AnsiConsole.MarkupLine("Welcome to ParkWise System");
            AnsiConsole.MarkupLine($"Total Parking Spots: {_configManager.CurrentConfig.ParkingSpotCount}");
            AnsiConsole.MarkupLine($"Car Price per Hour: {_configManager.CurrentConfig.CarPricePerHour:C}");
            AnsiConsole.MarkupLine($"Motorcycle Price per Hour: {_configManager.CurrentConfig.MotorcyclePricePerHour:C}");
            AnsiConsole.WriteLine();
        }

        //rebuild this maybe?
        public void ShowParkingLotStatus()
        {

            var spots = _parkingGarage.GetParkingSpots().ToList();
            int spotsPerRow = 5;
            int totalRows = (int)Math.Ceiling(spots.Count / (double)spotsPerRow);

            var grid = new Table();

            // Add columns dynamically (5 columns)
            for (int i = 0; i < spotsPerRow; i++)
            {
                grid.AddColumn(new TableColumn(String.Empty));
            }

            // Create rows
            for (int row = 0; row < totalRows; row++)
            {
                var rowSpots = spots
                    .Skip(row * spotsPerRow)
                    .Take(spotsPerRow)
                    .ToList();

                // Create cells for this row
                var cells = new List<string>();
                for (int col = 0; col < spotsPerRow; col++)
                {
                    if (col < rowSpots.Count)
                    {
                        var spot = rowSpots[col];
                        string status = Markup.Escape(spot.ToString());
                        // Create a more compact display for each spot
                        cells.Add($"#{spot.Number}\n{status}");
                    }
                    else
                    {
                        cells.Add(string.Empty); // Empty cell for padding
                    }
                }

                grid.AddRow(cells.ToArray());
            }

            // Add border styling
            grid.BorderColor(Color.Grey);
            grid.Border(TableBorder.Square);

            // Optional: Add a title
            AnsiConsole.WriteLine();
            var rule = new Rule("[yellow]Parking Lot Status[/]");
            rule.Style = Style.Parse("grey");
            rule.Justification = Justify.Center;
            AnsiConsole.Write(rule);
            AnsiConsole.WriteLine();

            AnsiConsole.Write(grid);

            // Add summary section
            var totalSpots = spots.Count;
            // Modify this line based on how your ParkingSpot determines if it's occupied
            var occupiedSpots = spots.Count(s => s.ToString().Contains("EMPTY") == false);
            var freeSpots = totalSpots - occupiedSpots;

            AnsiConsole.WriteLine();
            var summaryTable = new Table();
            summaryTable.AddColumn(new TableColumn("Summary").Centered());
            summaryTable.Border(TableBorder.Rounded);
            summaryTable.BorderColor(Color.Grey);

            summaryTable.AddRow(new Markup($"[blue]Total Spots:[/] {totalSpots}"));
            summaryTable.AddRow(new Markup($"[green]Available Spots:[/] {freeSpots}"));
            summaryTable.AddRow(new Markup($"[red]Occupied Spots:[/] {occupiedSpots}"));

            AnsiConsole.Write(summaryTable);
            AnsiConsole.WriteLine();
        }

        //create panel with details.
        public void ShowVehicleDetails(Vehicle vehicle)
        {
            var panel = new Panel($@"Registration Number: {vehicle.RegistrationNumber}
Type: {vehicle.GetType().Name}
Parked Time: {vehicle.ParkedTime}
Price: {CalculatePrice(vehicle):C}")
            {
                Header = new PanelHeader("Vehicle Details"),
                Expand = true,
                Border = BoxBorder.Rounded
            };

            AnsiConsole.Write(panel);
        }

        public decimal CalculatePrice(Vehicle vehicle)
        {
            var duration = DateTime.Now - vehicle.ParkedTime;

            // If less than 10 minutes, parking is free
            if (duration.TotalMinutes < 10)
            {
                return 0;
            }

            // Calculate hours, rounding up (e.g., 1.1 hours becomes 2 hours)
            var hours = Math.Ceiling(duration.TotalHours);

            if (vehicle is Car)
            {
                return (decimal)hours * _configManager.CurrentConfig.CarPricePerHour;
            }
            else if (vehicle is Motorcycle)
            {
                return (decimal)hours * _configManager.CurrentConfig.MotorcyclePricePerHour;
            }

            return 0;
        }
    }
}
