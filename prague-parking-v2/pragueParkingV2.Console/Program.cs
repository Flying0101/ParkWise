
using prague_parking_v2.pragueParkingV2.Core.Models;
using prague_parking_v2.pragueParkingV2.Core.Services;
using prague_parking_v2.pragueParkingV2.DataAccess;
using Spectre.Console;

namespace prague_parking_v2.pragueParkingV2.Console
{
    internal class Program
    {
        //files paths are saved as variables to make a cleaner solution

        private static ParkingGarage parkingGarage = null!;
        private static ConfigurationManager configManager = null!;
        private static DisplayManager displayManager = null!;

        // run the "starter" method for all the logic and run main meny loop
        static void Main(string[] args)
        {
            Initialize();
            RunMainLoop();
        }

        static void Initialize()
        {
            var jsonDataAccess = new JsonDataAccess();
            configManager = new ConfigurationManager(jsonDataAccess);

            // Ensure configuration is loaded
            configManager.ReloadConfiguration();

            // Pass the current configuration
            parkingGarage = new ParkingGarage(jsonDataAccess, configManager.CurrentConfig);
            displayManager = new DisplayManager(parkingGarage, configManager);
        }

        static void RunMainLoop()
        {
            while (true)
            {
                // call the UI
                displayManager.ShowMainMenu();
                var choice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .AddChoices(new[] {
                            "Park Vehicle", "Remove Vehicle", "Find Vehicle",
                            "Move Vehicle", "Show Parking Lot Status",
                            "Reload Configuration", "Configuration Settings", "Exit"
                        }));

                switch (choice)
                {
                    case "Park Vehicle":
                        try
                        {
                            ParkVehicle();
                        }
                        catch (Exception ex)
                        {
                            System.Console.WriteLine($"Error parking your vehicle: {ex.Message}");
                        }
                        PauseForUser();
                        break;

                    case "Remove Vehicle":
                        try
                        {
                            RemoveVehicle();

                        }
                        catch (Exception ex)
                        {
                            System.Console.WriteLine($"Error: {ex.Message}");
                        }
                        PauseForUser();
                        break;

                    case "Find Vehicle":
                        try
                        {
                            FindVehicle();

                        }
                        catch (Exception ex)
                        {
                            System.Console.WriteLine($"Error: {ex.Message}");
                        }
                        PauseForUser();
                        break;

                    case "Move Vehicle":
                        try
                        {
                            MoveVehicle();

                        }
                        catch (Exception ex)
                        {
                            System.Console.WriteLine($"Error: {ex.Message}");
                        }
                        PauseForUser();
                        break;

                    case "Show Parking Lot Status":
                        try
                        {
                            displayManager.ShowParkingLotStatus();

                        }
                        catch (Exception ex)
                        {
                            System.Console.WriteLine($"Error: {ex.Message}");
                        }
                        PauseForUser();
                        break;

                    case "Reload Configuration":
                        try
                        {
                            ReloadConfiguration();
                        }
                        catch (Exception ex)
                        {
                            System.Console.WriteLine($"Error: {ex.Message}");
                        }
                        PauseForUser();
                        break;

                    case "Configuration Settings":
                        try
                        {
                            ShowConfigurationMenu();
                        }
                        catch (Exception ex)
                        {
                            System.Console.WriteLine($"Error: {ex.Message}");
                        }
                        PauseForUser();
                        break;

                    case "Exit":
                        System.Console.WriteLine("Shutting down program....");
                        Thread.Sleep(2000);
                        return;
                }
            }
        }
        private static void PauseForUser()
        {
            System.Console.WriteLine("\nPress Enter to return to the main menu...");
            System.Console.ReadLine();
        }
        static void ParkVehicle()
        {
            var vehicleType = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Select vehicle type:")
                    .AddChoices(new[] { "Car", "Motorcycle" }));

            //ask for registration number
            var registrationNumber = AnsiConsole.Ask<string>("Enter registration number:");

            //check vehicle type
            Vehicle vehicle = vehicleType == "Car"
                ? new Car(registrationNumber)
                : new Motorcycle(registrationNumber);

            //give user feedback
            if (parkingGarage.ParkVehicle(vehicle))
            {
                AnsiConsole.MarkupLine("[green]Vehicle parked successfully.[/]");
            }
            else
            {
                AnsiConsole.MarkupLine("[red]No available parking spots.[/]");
            }
        }

        static void RemoveVehicle()
        {
            //get vehicle registration number from user input, find, calc and then remove.
            decimal totalFee = 0;
            var registrationNumber = AnsiConsole.Ask<string>("Enter registration number of the vehicle to remove:");
            var vehicle = parkingGarage.FindVehicle(registrationNumber);
            if (vehicle != null)
            {
                totalFee = displayManager.CalculatePrice(vehicle);
            }
            var removedVehicle = parkingGarage.RemoveVehicle(registrationNumber);

            //feedback to user.
            if (removedVehicle != null)
            {
                AnsiConsole.MarkupLine($"[green]Vehicle {registrationNumber} removed successfully. Parking fee:{totalFee}SEK[/]");
            }
            else
            {
                AnsiConsole.MarkupLine($"[red]Vehicle {registrationNumber} not found.[/]");
            }
        }

        static void FindVehicle()
        {
            //get user input
            var registrationNumber = AnsiConsole.Ask<string>("Enter registration number of the vehicle to find:");
            var vehicle = parkingGarage.FindVehicle(registrationNumber);

            //give user feedback
            if (vehicle != null)
            {
                AnsiConsole.MarkupLine($"[green]Vehicle {registrationNumber} found.[/]");
                displayManager.ShowVehicleDetails(vehicle);
            }
            else
            {
                AnsiConsole.MarkupLine($"[red]Vehicle {registrationNumber} not found.[/]");
            }
        }

        static void MoveVehicle()
        {
            //get user input
            var registrationNumber = AnsiConsole.Ask<string>("Enter registration number of the vehicle to move:");
            var newSpotNumber = AnsiConsole.Ask<int>("Enter the new spot number:");

            //give user feedback
            try
            {
                parkingGarage.MoveVehicle(registrationNumber, newSpotNumber);
                AnsiConsole.MarkupLine($"[green]Vehicle {registrationNumber} moved to spot {newSpotNumber} successfully.[/]");
            }
            catch (InvalidOperationException ex)
            {
                AnsiConsole.MarkupLine($"[red]{ex.Message}[/]");
            }
        }

        //might delete later, no need atm.
        static void ReloadConfiguration()
        {
            try
            {
                // reload
                configManager.ReloadConfiguration();

                // Create a new instance with the updated configuration
                var jsonDataAccess = new JsonDataAccess();

                // Create new parking garage with the updated configuration
                parkingGarage = new ParkingGarage(jsonDataAccess, configManager.CurrentConfig);

                //user feedback
                AnsiConsole.MarkupLine($"[green]Configuration reloaded successfully. New parking spot count: {configManager.CurrentConfig.ParkingSpotCount}[/]");
                //update UI
                displayManager = new DisplayManager(parkingGarage, configManager);
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]Error reloading configuration: {ex.Message}[/]");
            }
        }




        static void ShowConfigurationMenu()
        {
            //configuration meny UI
            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Configuration Options:")
                    .AddChoices(new[] {
                "Update Parking lot capacity",
                "Update Prices",
                "Show Current Configuration",
                "Back to Main Menu"
                    }));

            switch (choice)
            {
                case "Update Parking lot capacity":
                    UpdateParkingSpotCount();
                    break;
                case "Update Prices":
                    UpdatePrices();
                    break;
                case "Show Current Configuration":
                    ShowCurrentConfiguration();
                    break;
                case "Back to Main Menu":
                    return;
            }
        }

        static void UpdateParkingSpotCount()
        {
            //get and display current spots
            var currentCount = configManager.CurrentConfig.ParkingSpotCount;
            AnsiConsole.MarkupLine($"Current parking spot count: [blue]{currentCount}[/]");

            //user input
            var newCount = AnsiConsole.Ask<int>("Enter new parking spot count:");

            //validate
            if (newCount <= 0)
            {
                AnsiConsole.MarkupLine("[red]Parking spot count must be greater than 0.[/]");
                return;
            }

            configManager.UpdateParkingSpotCount(newCount);
            var jsonDataAccess = new JsonDataAccess();
            // Reinitialize parking garage with new configuration
            parkingGarage = new ParkingGarage(jsonDataAccess, configManager.CurrentConfig);
            displayManager = new DisplayManager(parkingGarage, configManager);

            //user feedback
            AnsiConsole.MarkupLine($"[green]Parking spot count updated to {newCount}[/]");
        }

        static void UpdatePrices()
        {
            //get current values
            var currentCarPrice = configManager.CurrentConfig.CarPricePerHour;
            var currentMotorcyclePrice = configManager.CurrentConfig.MotorcyclePricePerHour;

            //display current prices
            AnsiConsole.MarkupLine($"Current car price per hour: [blue]{currentCarPrice}[/] SEK");
            AnsiConsole.MarkupLine($"Current motorcycle price per hour: [blue]{currentMotorcyclePrice}[/] SEK");


            var newCarPrice = AnsiConsole.Ask<decimal>("Enter new car price per hour (SEK):");
            var newMotorcyclePrice = AnsiConsole.Ask<decimal>("Enter new motorcycle price per hour (SEK):");

            //validate
            if (newCarPrice < 0 || newMotorcyclePrice < 0)
            {
                AnsiConsole.MarkupLine("[red]Prices cannot be negative.[/]");
                return;
            }

            configManager.UpdatePrices(newCarPrice, newMotorcyclePrice);
            AnsiConsole.MarkupLine("[green]Prices updated successfully.[/]");
        }

        static void ShowCurrentConfiguration()
        {
            //get current config
            var config = configManager.CurrentConfig;

            //display config
            AnsiConsole.Write(new Rule("[yellow]Current Configuration[/]"));
            AnsiConsole.MarkupLine($"Parking Spot Count: [blue]{config.ParkingSpotCount}[/]");
            AnsiConsole.MarkupLine($"Car Price per Hour: [blue]{config.CarPricePerHour}[/] SEK");
            AnsiConsole.MarkupLine($"Motorcycle Price per Hour: [blue]{config.MotorcyclePricePerHour}[/] SEK");
            AnsiConsole.Write(new Rule());
        }
    }
}
