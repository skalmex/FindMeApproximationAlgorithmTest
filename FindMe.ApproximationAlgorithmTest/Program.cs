using FindMe.ApproximationAlgorithmTest.Data.Reposaitories;
using FindMe.ApproximationAlgorithmTest.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FindMe.ApproximationAlgorithmTest
{
    class Program
    {
        static void Main(string[] args)
        {
            bool quitApp = false;
            bool measurementsNumberValid = false;
            int measurementsOption = 4;
            List<int> measurementsOptionsList = new List<int>() { 1, 2, 4};
            bool approximationEnabledValid = false;
            bool approximationEnable = false;


            Console.WriteLine("Test algorytmu aproksymacji Skalmex.FindMe. Wciśnij [q] aby zakończyć");

            do {
                Console.WriteLine($"Ilość pomiarów[{Conversions.ConvertNumberListToString(measurementsOptionsList)}]: ");
                string readData = Console.ReadLine();

                measurementsNumberValid = int.TryParse(readData, out measurementsOption);

                if (!measurementsNumberValid || (measurementsNumberValid && !measurementsOptionsList.Contains(measurementsOption)))
                {
                    Console.WriteLine("Nieprawidłowa wartość!");
                    continue;
                }
            } while (!measurementsNumberValid);


            do
            {
                Console.WriteLine("Czy włączyć aproksymację[T|N]: ");
                string readData = Console.ReadLine();

                switch (readData)
                {
                    case "t":
                        {
                            approximationEnable = true;
                            approximationEnabledValid = true;
                        }
                        break;

                    case "n":
                        {
                            approximationEnable = false;
                            approximationEnabledValid = true;
                        }
                        break;

                    case "q":
                        {
                            quitApp = true;
                        }
                        break;

                    default:
                        {
                            approximationEnabledValid = false;
                        }
                        break;
                }

                if (!approximationEnabledValid)
                {
                    Console.WriteLine("Nieprawidłowa wartość!");
                    continue;
                }
            } while (!quitApp && !measurementsNumberValid);




            if (measurementsNumberValid && approximationEnabledValid && !quitApp)
            {

                DataService dataService = new DataService(new RawDataRepository(), measurementsOption, approximationEnable);
                var locations = dataService.RecalculateLocalizationData();

                if (locations != null && locations.Count > 0)
                {
                    bool firstLine = true;

                    foreach (var location in locations)
                    {
                        if (!firstLine) Console.WriteLine($"------------------------------------------------------------");

                        Console.WriteLine($"ID Taga: {location.TagId}");
                        Console.WriteLine($"Pozycja_X: {location.Position_X}");
                        Console.WriteLine($"Pozycja_Y: {location.Position_Y}");
                        Console.WriteLine($"Pozycja_Z: {location.Position_Z}");
                        Console.WriteLine($"Czas: {location.Time}");
                        firstLine = false;
                    }
                }
                else
                {
                    Console.WriteLine("BRAK DANYCH");
                }

                Console.WriteLine("\nWciśnij [Enter] aby zakończyć");
                Console.ReadLine();
            }
        }
    }
}
