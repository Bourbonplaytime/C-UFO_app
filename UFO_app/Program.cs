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
            var queryResults = GetQuery(fileContents);
            foreach (var name in queryResults)
            {
                Console.WriteLine(name);
            }
            Console.Read();

            //var sourceFile = Path.Combine(directory.FullName, "sightings.json");
            //var sourceContents = DataWriter(sourceFile);
        }

        public static List<SightingData> ReadSightings(string fileName)
        {
            var sightings = new List<SightingData>();
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
                    sightings.Add(sightingData);
                }
            }
            return sightings;
        }

        public static IEnumerable<SightingData> GetQuery(List<SightingData> sightings)
        {
            Console.WriteLine("Welcome to my UFO sightings app designed to analyze UFO sightings data. \n");
            Console.WriteLine("Are you interested in a US domestic search? y/n");
            string domesticSearch = Console.ReadLine();
            if(domesticSearch.ToLower() == "y")
            {
                Console.WriteLine("Would you like to search by City? y/n");
                string citySearch = Console.ReadLine();
                if(citySearch.ToLower() == "y")
                {
                    Console.WriteLine("Please enter the name of a US city.");
                    string queryCity = Console.ReadLine().ToLower();
                    Console.WriteLine("Please enter the US state which contains your US city.");
                    string queryStateOfCity = Console.ReadLine().ToLower();
                    IEnumerable<SightingData> searchSightingsUSCity = from sighting in sightings where sighting.City == queryCity && sighting.State == queryStateOfCity select sighting;
                    return searchSightingsUSCity;
                }
                else
                {
                    Console.WriteLine("Please enter the name of a US state.");
                    string queryState = Console.ReadLine().ToLower();
                    IEnumerable<SightingData> searchSightingsUSState = from sighting in sightings where sighting.State == queryState select sighting;
                    return searchSightingsUSState;

                }
            }
            else if (domesticSearch.ToLower() == "n")
            {
                Console.WriteLine("Would you like to search by a non-US country? y/n");
                string internationalSearch = Console.ReadLine();
                if(internationalSearch == "y")
                {
                    Console.WriteLine("Please enter the name of a non-US country enclosed in brackets for example (canada)");
                    string queryCountry = Console.ReadLine().ToLower();
                    IEnumerable<SightingData> searchSightingsCountry = from sighting in sightings where sighting.Country == queryCountry select sighting;
                    return searchSightingsCountry;
                }
                else
                {
                    Console.WriteLine("Are you interested in a range of dates? y/n");
                    string dateRangeInterest = Console.ReadLine();
                    if (dateRangeInterest.ToLower() == "y")
                    {
                        Console.WriteLine("Please enter a beginning date in the format mm/dd/yyyy");
                        string begDate = Console.ReadLine();
                        DateTime beginningRange;
                        DateTime.TryParse(begDate, out DateTime begRange);
                        beginningRange = begRange;
                        Console.WriteLine("Please enter an ending date in the format mm/dd/yyyy");
                        string endDate = Console.ReadLine();
                        DateTime endingRange;
                        DateTime.TryParse(endDate, out DateTime endRange);
                        endingRange = endRange;
                        IEnumerable<SightingData> searchSightingsDateRange = from sighting in sightings where sighting.SightingDate > beginningRange && sighting.SightingDate < endingRange select sighting;
                        return searchSightingsDateRange;
                    }
                    else
                    {
                        Console.WriteLine("I'm sorry no search could be returned.");
                        return null;
                    }
                }

            }
            else
            {
                Console.WriteLine("I'm sorry no search could be returned.");
                return null;
            }
        }


        public static List<SightingData> AddSighting(List<SightingData> sightings)
        {

            Console.WriteLine("Would you like to add a sighting? y/n");
            string addSighting = Console.ReadLine().ToLower();
            if (addSighting == "y")
            {
                var newSighting = new SightingData();
                Console.WriteLine("Please enter a date and time in the format mm/dd/yyyy hh:mm.");
                DateTime sightingDate;
                var newSightingDate = Console.ReadLine();
                if (DateTime.TryParse(newSightingDate, out sightingDate))
                {
                    newSighting.SightingDate = sightingDate;
                }
                Console.WriteLine("Please enter a city.");
                newSighting.City = Console.ReadLine();
                Console.WriteLine("Please enter a state by two letter abbreviation or if not in a US state just leave blank.");
                newSighting.State = Console.ReadLine();
                Console.WriteLine("Please enter a country. If in US type us or if international enter the country in () example (canada).");
                newSighting.Country = Console.ReadLine();
                Console.WriteLine("Please enter the UFOs shape.");
                newSighting.Shape = Console.ReadLine();
                Console.WriteLine("Please enter the duration of this sighting.");
                newSighting.Duration = Console.ReadLine();
                Console.WriteLine("Please enter any comments regarding this sighting.");
                newSighting.Comments = Console.ReadLine();
                Console.WriteLine("Please enter the current date.");
                newSighting.DatePosted = Console.ReadLine();
                Console.WriteLine("If known please enter the latitude of the sighting.");
                var newLatitude = Console.ReadLine();
                Double latitude;
                if (double.TryParse(newLatitude, out latitude))
                {
                    newSighting.Latitude = latitude;
                }
                Console.WriteLine("If known please enter the longitude of the sighting.");
                var newLongitude = Console.ReadLine();
                Double longitude;
                if (double.TryParse(newLongitude, out longitude))
                {
                    newSighting.Longitude = longitude;
                }
                sightings.Add(newSighting);
            }
            return sightings;
        }

        public static List<SightingData> RemoveSightings(List<SightingData> sightings)
        {
            IEnumerable<SightingData> toRemove = from sighting in sightings where sighting.City == "louisville" select sighting;
            foreach(SightingData thing in toRemove)
            {
                sightings.Remove(thing);
            }
            return sightings;
        }

        //public static void DataWriter(List<string[]> sightings, string sourceFile)
        //{
        //    var serializer = new JsonSerializer();
        //    using (var writer = new StreamWriter(sourceFile))
        //    using (var sourceWriter = new JsonTextWriter(writer))
        //    {
        //        int i = 0;
        //        while (i < 100)
        //        {
        //            serializer.Serialize(sourceWriter, sightings[i]);
        //            i++;
        //        }
        //    }
        //}
    }
}
