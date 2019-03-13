using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using System.Data.Common;
using System.Configuration;

namespace UFO_app
{
    class Program
    {
        static void Main(string[] args)
        {
            //string provider = ConfigurationManager.AppSettings["provider"];
            //string connectionString = ConfigurationManager.AppSettings["connectionString"];
            //DbProviderFactory factory = DbProviderFactories.GetFactory(provider);
            //using (DbConnection connection = factory.CreateConnection())
            //{
            //    if(connection == null)
            //    {
            //        Console.WriteLine("Connection error.");
            //        Console.ReadLine();
            //    }
            //    connection.ConnectionString = connectionString;
            //    connection.Open();
            //    DbCommand command = factory.CreateCommand();
            //    if (command == null)
            //    {
            //        Console.WriteLine("Command error.");
            //        Console.ReadLine();
            //    }
            //}
            string currentDirectory = Directory.GetCurrentDirectory();
            DirectoryInfo directory = new DirectoryInfo(currentDirectory);
            var fileName = Path.Combine(directory.FullName, "scrubbed.csv");
            var fileContents = ReadSightings(fileName);
            //var queryResults = GetQuery(fileContents);
            List<SightingData> newAdd = NewSighting(fileContents);
            //var queryAfterAdd = AddEntry(queryResults, newAdd);
            //int i = 0;
            //foreach (var name in queryAfterAdd)
            //{
            //    Console.WriteLine(i + " " + name);
            //    i++;
            //}
            List<SightingData> queryAfterRemove = RemoveSightings(fileContents);
            //{
            //    Console.WriteLine(j + " " + name);
            //    j++;
            //}
            List<SightingData>resultsAfterUpdate = UpdatedEntries(queryAfterRemove);
            //var k = 0;
            //foreach (var name in resultsAfterUpdate)
            //{
            //    Console.WriteLine(k + " " + name);
            //    k++;
            //}
            //Console.Read();
            var queryResults = GetQuery(resultsAfterUpdate);
            var sourceFile = Path.Combine(directory.FullName, "sightings.json");
            DataWriter(queryResults, sourceFile);
            //if (newAdd != null)
            //{
            CSVWriter(fileContents, fileName);
            //}
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

        public static List<SightingData> GetQuery(List<SightingData> fileContents)
        {
            Console.WriteLine("Welcome to my UFO sightings app designed to analyze UFO sightings data. \n");
            Console.WriteLine("Are you interested in a US domestic search? y/n");
            string domesticSearch = Console.ReadLine().ToLower();
            if (domesticSearch.ToLower() == "y")
            {
                Console.WriteLine("Would you like to search by City? y/n");
                string citySearch = Console.ReadLine().ToLower();
                if (citySearch.ToLower() == "y")
                {
                    Console.WriteLine("Please enter the name of a US city.");
                    string queryCity = Console.ReadLine().ToLower();
                    Console.WriteLine("Please enter the US state which contains your US city.");
                    string queryStateOfCity = Console.ReadLine().ToLower();
                    IEnumerable<SightingData> searchSightingsQuery = from sighting in fileContents where string.Equals(queryCity, sighting.City, StringComparison.OrdinalIgnoreCase) == true && sighting.State == queryStateOfCity select sighting;
                    List<SightingData> finalResult = searchSightingsQuery.ToList();
                    return finalResult;
                }
                else
                {
                    Console.WriteLine("Please enter the name of a US state in the form of it's two letter abbreviation.");
                    string queryState = Console.ReadLine().ToLower();
                    IEnumerable<SightingData> searchSightingsQuery = from sighting in fileContents where sighting.State == queryState select sighting;
                    List<SightingData> finalResult = searchSightingsQuery.ToList();
                    return finalResult;
                }
            }
            else if (domesticSearch.ToLower() == "n")
            {
                Console.WriteLine("Would you like to search by a non-US country? y/n");
                string internationalSearch = Console.ReadLine().ToLower();
                if (internationalSearch == "y")
                {
                    Console.WriteLine("Please enter the name of a non-US country enclosed in brackets by two letter abbreviation for example canada would be ca.");
                    string queryCountry = Console.ReadLine().ToLower();
                    IEnumerable<SightingData> searchSightingsQuery = from sighting in fileContents where sighting.Country == queryCountry select sighting;
                    List<SightingData> finalResult = searchSightingsQuery.ToList();
                    return finalResult;
                }
                else
                {
                    Console.WriteLine("Are you interested in a range of dates? y/n");
                    string dateRangeInterest = Console.ReadLine().ToLower();
                    if (dateRangeInterest.ToLower() == "y")
                    {
                        Console.WriteLine("Please enter a beginning date in the format mm/dd/yyyy hh:mm.");
                        string begDate = Console.ReadLine();
                        DateTime beginningRange;
                        DateTime.TryParse(begDate, out DateTime begRange);
                        beginningRange = begRange;
                        Console.WriteLine("Please enter an ending date in the format mm/dd/yyyy hh:mm.");
                        string endDate = Console.ReadLine();
                        DateTime endingRange;
                        DateTime.TryParse(endDate, out DateTime endRange);
                        endingRange = endRange;
                        IEnumerable<SightingData> searchSightingsQuery = from sighting in fileContents where sighting.SightingDate > beginningRange && sighting.SightingDate < endingRange select sighting;
                        List<SightingData> finalResult = searchSightingsQuery.ToList();
                        return finalResult;
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
                Console.WriteLine("I'm sorry no search could be returned");
                return null;
            }
        }


        //public static SightingData NewSighting(IEnumerable<SightingData> searchSightingsQuery)
        public static List<SightingData> NewSighting(List<SightingData> fileContents)
        {

            Console.WriteLine("Would you like to add a sighting? y/n");
            string addSighting = Console.ReadLine().ToLower();
            //var addedEntry = searchSightingsQuery.ToList();
            var newSighting = new SightingData();
            if (addSighting == "y")
            {
                Console.WriteLine("Please enter a date and time in the format mm/dd/yyyy hh:mm.");
                DateTime sightingDate;
                var newSightingDate = Console.ReadLine();
                if (DateTime.TryParse(newSightingDate, out sightingDate))
                {
                    newSighting.SightingDate = sightingDate;
                }
                Console.WriteLine("Please enter a city.");
                newSighting.City = Console.ReadLine().ToLower();
                Console.WriteLine("Please enter a state by two letter abbreviation or if in a non-US country just leave blank3.");
                newSighting.State = Console.ReadLine().ToLower();
                Console.WriteLine("Please enter a country. If in US type us or if international enter the country in () example (canada).");
                newSighting.Country = Console.ReadLine().ToLower();
                Console.WriteLine("Please enter the UFOs shape.");
                newSighting.Shape = Console.ReadLine().ToLower();
                Console.WriteLine("Please enter the duration of this sighting.");
                newSighting.Duration = Console.ReadLine().ToLower();
                Console.WriteLine("Please enter any comments regarding this sighting.");
                newSighting.Comments = Console.ReadLine();
                Console.WriteLine("Please enter the current date in the format mm/dd/yyyy hh:mm.");
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
                fileContents.Add(newSighting);
                return fileContents;
                //return newSighting;
            }
            else
            {
                return fileContents;
            }
        }

        //public static List<SightingData> AddEntry(IEnumerable<SightingData> searchSightingsQuery, SightingData newSighting)
        //{
        //    var addedEntry = searchSightingsQuery.ToList();
        //    addedEntry.Add(newSighting);
        //    return addedEntry;
        //}

        //public static List<SightingData> RemoveSightings(List<SightingData> fileContents)
        public static List<SightingData> RemoveSightings(List<SightingData> fileContents)
        {
            Console.WriteLine("Would you like to remove a result or results? y/n.");
            string removeResult = Console.ReadLine().ToLower();
            if (removeResult == "y")
            {
                Console.WriteLine("Would you like to remove all results of a city? y/n");
                string removeByCity = Console.ReadLine().ToLower();
                if (removeByCity == "y")
                {
                    Console.WriteLine("please enter the name of a city in your search results to remove.");
                    var cityToRemove = Console.ReadLine().ToLower();
                    IEnumerable<SightingData> toRemoveCity = from sighting in fileContents where sighting.City == cityToRemove select sighting;
                    var toRemoveCityList = toRemoveCity.ToList();
                    foreach (SightingData thing in toRemoveCityList)
                    {
                        fileContents.Remove(thing);
                    }
                    return fileContents;
                }
                else if (removeByCity == "n")
                {
                    Console.WriteLine("Would you like to remove results within a date range? y/n");
                    var rangeToRemove = Console.ReadLine().ToLower();
                    if (rangeToRemove == "y")
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
                        IEnumerable<SightingData> toRemoveRange = from sighting in fileContents where sighting.SightingDate > beginningRange && sighting.SightingDate < endingRange select sighting;
                        var toRemoveRangeList = toRemoveRange.ToList();
                        foreach (SightingData thing in toRemoveRangeList)
                        {
                            fileContents.Remove(thing);
                        }
                        return fileContents;
                    }
                    else if (rangeToRemove == "n")
                    {
                        Console.WriteLine("Ok we will remove by index.");
                        Console.WriteLine("Please enter an index to remove or press x to exit.");
                        var indexToRemove = Console.ReadLine();
                        int intToRemove = int.Parse(indexToRemove);
                        fileContents.Remove(fileContents[intToRemove]);
                        return fileContents;
                    }
                    else
                    {                        
                        return fileContents;
                    }
                }
                else
                {
                    return fileContents;
                }

            }
            else
            {
                return fileContents;
            }
        }

        public static List<SightingData> UpdatedEntries (List<SightingData> fileContents)
        {
            Console.WriteLine("Would you like to update results? y/n");
            string updateResult = Console.ReadLine().ToLower();
            if (updateResult.ToLower() == "y")
            {
                Console.WriteLine("Please enter the new city that you would like to update results to.");
                var updateCity = Console.ReadLine().ToLower();
                foreach (SightingData selection in fileContents)
                {
                    selection.City = updateCity;
                }
                return fileContents;
            }
            else
            {
                return fileContents;
            }
        }

        public static void DataWriter(List<SightingData> fileContents, string sourceFile)
        {
            var serializer = new JsonSerializer();
            using (var writer = new StreamWriter(sourceFile))
            using (var sourceWriter = new JsonTextWriter(writer))
            {
                serializer.Serialize(sourceWriter, fileContents);
            }
        }

        public static void CSVWriter(List<SightingData> fileContents, string fileName)
        {
            using (var writer = new StreamWriter(fileName))
            {
                foreach (SightingData entry in fileContents)
                {
                    writer.WriteLine(entry.SightingDate.ToString() + "," + entry.City + "," + entry.State + "," + entry.Country + "," +
                                    entry.Shape + "," + entry.Duration + "," + entry.Comments + "," + entry.DatePosted + "," + entry.Latitude +
                                    "," + entry.Longitude);
                }
            }
        //    string CSVThing = newSighting.SightingDate.ToString() + "," + newSighting.City + "," + newSighting.State + "," + newSighting.Country + "," +
        //                        newSighting.Shape + "," + newSighting.Duration + "," + newSighting.Comments + "," + newSighting.DatePosted + "," +
        //                        newSighting.Latitude.ToString() + "," + newSighting.Longitude.ToString() + "\n";

        //    File.AppendAllText(fileName, CSVThing);
        }
    }
}
