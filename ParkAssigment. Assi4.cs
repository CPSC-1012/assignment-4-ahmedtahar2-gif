/*
 * Thrilladelphia Ride Tracker
 * Author: Ahmed Tahar
 * Date: dec-12-2025
 * 
 * This program stores info about rides in an amusement park.
 * **************************************************
 * ***********************************************
 * ***********************************************
 */

using System;
using System.Collections.Generic;
using System.IO;

namespace RideTracker
{
    public class Ride
    {
        // fields
        private string name;
        private int frightFactor;
        private double costToEnter;
        private int visitorsToday;

        // properties with validation
        public string Name
        {
            get { return name; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new Exception("Name cannot be empty.");
                name = value.Trim();
            }
        }

        public int FrightFactor
        {
            get { return frightFactor; }
            set
            {
                if (value < 0 || value > 100)
                    throw new Exception("Fright Factor must be 0–100.");
                frightFactor = value;
            }
        }

        public double CostToEnter
        {
            get { return costToEnter; }
            set
            {
                if (value < 1)
                    throw new Exception("Cost must be at least 1.00.");
                costToEnter = value;
            }
        }

        public int VisitorsToday
        {
            get { return visitorsToday; }
            set
            {
                if (value < 0)
                    throw new Exception("Visitors cannot be negative.");
                visitorsToday = value;
            }
        }

        // constructors
        public Ride() { }

        public Ride(string name, int fright, double cost, int visitors)
        {
            Name = name;
            FrightFactor = fright;
            CostToEnter = cost;
            VisitorsToday = visitors;
        }

        // readonly properties
        public double PopularityScore
        {
            get { return (FrightFactor / 10.0) * VisitorsToday; }
        }

        public string ThrillLevel
        {
            get
            {
                if (FrightFactor <= 20) return "Mild";
                else if (FrightFactor <= 60) return "Exciting";
                else if (FrightFactor <= 90) return "Thrilling";
                else return "Extreme";
            }
        }

        public string RideDetails()
        {
            return $"Name: {Name}\n" +
                   $"Fright Factor: {FrightFactor}\n" +
                   $"Cost: {CostToEnter:C}\n" +
                   $"Visitors Today: {VisitorsToday}\n" +
                   $"Thrill Level: {ThrillLevel}\n" +
                   $"Popularity Score: {PopularityScore:F2}";
        }

        public string ToCSVString()
        {
            return $"{Name},{FrightFactor},{CostToEnter},{VisitorsToday}";
        }

        public override string ToString()
        {
            return $"{Name,-15} {FrightFactor,-5} {CostToEnter,-8:F2} {VisitorsToday}";
        }
    }

    class Program
    {
        const string FILE_NAME = "rides.csv";

        static List<Ride> rides = new List<Ride>();

        static void Main(string[] args)
        {
            LoadFile();

            bool keepGoing = true;

            while (keepGoing)
            {
                Console.WriteLine("\n--- Thrilladelphia Ride Menu ---");
                Console.WriteLine("1. Display All Rides");
                Console.WriteLine("2. Search Ride");
                Console.WriteLine("3. Add Ride");
                Console.WriteLine("4. Edit Ride");
                Console.WriteLine("5. Remove Ride");
                Console.WriteLine("6. Quit");
                Console.Write("Choice: ");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1": DisplayAll(); break;
                    case "2": Search(); break;
                    case "3": Add(); break;
                    case "4": Edit(); break;
                    case "5": Remove(); break;
                    case "6": keepGoing = false; break;
                    default:
                        Console.WriteLine("Invalid option.");
                        break;
                }
            }

            SaveFile();
            Console.WriteLine("Saved. Goodbye!");
        }

        // ============================================================
        // FILE FUNCTIONS
        // ============================================================

        static void LoadFile()
        {
            if (!File.Exists(FILE_NAME))
                return;

            string[] lines = File.ReadAllLines(FILE_NAME);

            foreach (string line in lines)
            {
                if (line.Trim().Length == 0) continue;

                string[] parts = line.Split(',');

                try
                {
                    Ride r = new Ride(
                        parts[0],
                        int.Parse(parts[1]),
                        double.Parse(parts[2]),
                        int.Parse(parts[3])
                    );
                    rides.Add(r);
                }
                catch
                {
                    Console.WriteLine("Skipping bad line in file.");
                }
            }
        }

        static void SaveFile()
        {
            using (StreamWriter sw = new StreamWriter(FILE_NAME))
            {
                foreach (Ride r in rides)
                {
                    sw.WriteLine(r.ToCSVString());
                }
            }
        }

        // ============================================================
        // MENU FUNCTIONS
        // ============================================================

        static void DisplayAll()
        {
            if (rides.Count == 0)
            {
                Console.WriteLine("No rides yet.");
                return;
            }

            Console.WriteLine("\nName            FF   Cost     Visitors");
            Console.WriteLine("---------------------------------------");

            foreach (Ride r in rides)
            {
                Console.WriteLine(r.ToString());
            }
        }

        static void Search()
        {
            Console.Write("Enter ride name: ");
            string name = Console.ReadLine();

            Ride match = rides.Find(r => r.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

            if (match == null)
            {
                Console.WriteLine("Ride not found.");
            }
            else
            {
                Console.WriteLine("\n" + match.RideDetails());
            }
        }

        static void Add()
        {
            Console.Write("Ride name: ");
            string name = Console.ReadLine();

            // check duplicate
            if (rides.Exists(r => r.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
            {
                Console.WriteLine("A ride with that name already exists.");
                return;
            }

            int fright = ReadInt("Fright Factor (0-100): ", 0, 100);
            double cost = ReadDouble("Cost (>=1): ", 1);
            int visitors = ReadInt("Visitors Today: ", 0, int.MaxValue);

            try
            {
                Ride r = new Ride(name, fright, cost, visitors);
                rides.Add(r);
                Console.WriteLine("Ride added.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error adding ride: " + ex.Message);
            }
        }

        static void Edit()
        {
            Console.Write("Ride name to edit: ");
            string name = Console.ReadLine();

            Ride r = rides.Find(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

            if (r == null)
            {
                Console.WriteLine("Ride not found.");
                return;
            }

            int fright = ReadInt($"New Fright Factor ({r.FrightFactor}): ", 0, 100);
            double cost = ReadDouble($"New Cost ({r.CostToEnter}): ", 1);
            int visitors = ReadInt($"New Visitors ({r.VisitorsToday}): ", 0, int.MaxValue);

            r.FrightFactor = fright;
            r.CostToEnter = cost;
            r.VisitorsToday = visitors;

            Console.WriteLine("Ride updated.");
        }

        static void Remove()
        {
            Console.Write("Ride name to remove: ");
            string name = Console.ReadLine();

            Ride r = rides.Find(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

            if (r == null)
            {
                Console.WriteLine("Ride not found.");
                return;
            }

            rides.Remove(r);
            Console.WriteLine("Ride removed.");
        }

        // ============================================================
        // INPUT HELPERS
        // ============================================================

        static int ReadInt(string msg, int min, int max)
        {
            int x;
            while (true)
            {
                Console.Write(msg);
                if (int.TryParse(Console.ReadLine(), out x))
                {
                    if (x >= min && x <= max) return x;
                }
                Console.WriteLine("Invalid number.");
            }
        }

        static double ReadDouble(string msg, double min)
        {
            double x;
            while (true)
            {
                Console.Write(msg);
                if (double.TryParse(Console.ReadLine(), out x))
                {
                    if (x >= min) return x;
                }
                Console.WriteLine("Invalid number.");
            }
        }
    }
}
