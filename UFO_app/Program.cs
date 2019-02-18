using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;

namespace UFO_app
{
    class Program
    {
        static void Main(string[] args)
        { 
            string currentDirectory = Directory.GetCurrentDirectory();
            DirectoryInfo directory = new DirectoryInfo(currentDirectory);
            var fileName = Path.Combine(directory.FullName, "scrubbed.csv");
            var fileContents = ReadSightings(fileName);
            Console.WriteLine(fileContents);
            var sourceFile = Path.Combine(directory.FullName, "sightings.json");
        }

        public static List<string[]> ReadSightings(string fileName)
        {
            var sightings = new List<string[]>();
            using (var reader = new StreamReader(fileName))
            {
                string line = "";
                while ((line = reader.ReadLine()) != null)
                {
                    var sightingData = new SightingData();
                    string[] values = line.Split(',');
                    DateTime sightingDate;
                    if (DateTime.TryParse(values[0], out sightingDate))
                    {
                        sightingData.SightingDate = sightingDate;
                    }
                    sightingData.City = values[1];
                    sightingData.State = values[2];
                    sightingData.Country = values[3];
                    sightingData.Shape = values[4];
                    sightingData.Duration = values[5];
                    sightingData.Comments = values[6];
                    sightingData.DatePosted = values[7];
                    Double latitude;
                    if (double.TryParse(values[8], out latitude))
                    {
                        sightingData.Latitude = latitude;
                    }
                    Double longitude;
                    if (double.TryParse(values[9], out longitude))
                    {
                        sightingData.Longitude = longitude;
                    }
                    sightings.Add(values);
                }
            }
            return sightings;
        }

        public static List<string[]> AddSighting(List<string[]> sightings)
        {

            Console.WriteLine("Would you like to add a sighting? y/n \n");
            string addSighting = Console.ReadLine().ToLower();
            if (addSighting == "y")
            {
                List<string> newSighting = new List<string>();
                Console.WriteLine("Please enter a date and time in the format mm/dd/yyyy hh:mm.");
                newSighting.Add(Console.ReadLine());
                Console.WriteLine("Please enter a city.");
                newSighting.Add(Console.ReadLine());
                Console.WriteLine("Please enter a state by two letter abbreviation or if not in a US state just leave blank.");
                newSighting.Add(Console.ReadLine());
                Console.WriteLine("Please enter a country. If in US type us or if international enter the country in () example (canada).");
                newSighting.Add(Console.ReadLine());
                Console.WriteLine("Please enter the UFOs shape.");
                newSighting.Add(Console.ReadLine());
                Console.WriteLine("Please enter the duration of this sighting.");
                newSighting.Add(Console.ReadLine());
                Console.WriteLine("Please enter any comments regarding this sighting.");
                newSighting.Add(Console.ReadLine());
                Console.WriteLine("Please enter the current date.");
                newSighting.Add(Console.ReadLine());
                Console.WriteLine("If known please enter the latitude of the sighting.");
                newSighting.Add(Console.ReadLine());
                Console.WriteLine("If known please enter the longitude of the sighting.");
                newSighting.Add(Console.ReadLine());
                string[] addNewSighting = newSighting.ToArray();
                sightings.Add(addNewSighting);
            }
            return sightings;
        }

        public static void DataWriter(List<string[]> sightings, string sourceFile)
        {
            var serializer = new JsonSerializer();
            using (var writer = new StreamWriter(sourceFile))
            using (var sourceWriter = new JsonTextWriter(writer))
            {
                int i = 0;
                while (i < 100)
                {
                    serializer.Serialize(sourceWriter, sightings[i]);
                    i++;
                }
            }
        }

        //public static List<string[]> RemoveSightings(List<string[]> sightings)
        //{
        //    List<string[]> toRemove = from sighting in sightings where values[1] == "louisville" select sighting;
        //    sightings.Remove(toRemove);
        //    return sightings;
        //}
    }
}
