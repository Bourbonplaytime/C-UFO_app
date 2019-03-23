using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace UFO_app
{
    class Program
    {
        static void Main()
        {
            string currentDirectory = Directory.GetCurrentDirectory();
            DirectoryInfo directory = new DirectoryInfo(currentDirectory);
            var fileName = Path.Combine(directory.FullName, "scrubbed.csv");
            var fileContents = ReadSightings(fileName);
            List<SightingData> newAdd = NewSighting(fileContents);
            List<SightingData> queryAfterRemove = RemoveSightings(newAdd);
            List<SightingData> queryResults = GetQuery(queryAfterRemove);
            ConsoleWriter(queryResults);
            List<SightingData> resultsAfterUpdate = UpdatedEntries(queryResults);;
            CSVWriter(resultsAfterUpdate, fileName);
            ConsoleWriter(resultsAfterUpdate);
        }

        //Read .csv into a list of custom objects for querying and manipulation then return them back to the program
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

        //Query the list of custom objects based on user picked criteria to create a custom sampling 
        public static List<SightingData> GetQuery(List<SightingData> fileContents)
        {
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
                    Console.WriteLine("Please enter the name of a non-US country by two letter abbreviation.");
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
                        Console.WriteLine("I'm sorry no custom search could be returned.");
                        return fileContents;
                    }
                }

            }
            else
            {
                Console.WriteLine("I'm sorry no custom search could be returned");
                return fileContents;
            }
        }

        //gives the user an option to add a custom sighting and provide information to the program 
        public static List<SightingData> NewSighting(List<SightingData> fileContents)
        {

            Console.WriteLine("Would you like to add a sighting? y/n");
            string addSighting = Console.ReadLine().ToLower();
            if (addSighting == "y")
            {
                bool keepAdding = true;
                while(keepAdding)
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
                    newSighting.City = Console.ReadLine().ToLower();
                    Console.WriteLine("Please enter a state by two letter abbreviation or if in a non-US country just leave blank.");
                    newSighting.State = Console.ReadLine().ToLower();
                    Console.WriteLine("Please enter a country. If in US type us or if international use two letter abbreviation.");
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
                    Console.WriteLine("Do you have another entry to add? y/n");
                    string keepGoing = Console.ReadLine().ToLower();
                    if(keepGoing != "y")
                    {
                        keepAdding = false;
                    }
                    else
                    {
                        continue;
                    }
                }
                return fileContents;
            }
            else
            {
                return fileContents;
            }
        }

        //gives the user an option to remove sightings from the master list so they will be excluded in the final results 
        public static List<SightingData> RemoveSightings(List<SightingData> fileContents)
        {
            Console.WriteLine("Would you like to remove a result or results? y/n.");
            string removeResult = Console.ReadLine().ToLower();
            if (removeResult == "y")
            {
                Console.WriteLine("Would you like to remove all results of a US city? y/n");
                string removeByCity = Console.ReadLine().ToLower();
                if (removeByCity == "y")
                {
                    Console.WriteLine("please enter the name of a city in your search results to remove.");
                    var cityToRemove = Console.ReadLine().ToLower();
                    Console.WriteLine("Please enter the name of the state which contains your city as a 2 letter abbreviation");
                    var stateOfCityToRemove = Console.ReadLine().ToLower();
                    IEnumerable<SightingData> toRemoveCity = from sighting in fileContents where sighting.City == cityToRemove && sighting.State == stateOfCityToRemove select sighting;
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

        //allows the user an option to update current entries
        public static List<SightingData> UpdatedEntries (List<SightingData> fileContents)
        {
            Console.WriteLine("Would you like to update results? y/n");
            string updateResult = Console.ReadLine().ToLower();
            if (updateResult == "y")
            {
                Console.WriteLine("Would you like to update results by US city? y/n");
                string updateUSCity = Console.ReadLine().ToLower();
                if(updateUSCity == "y")
                {
                    Console.WriteLine("Please select a city to update.");
                    var cityToUpdate = Console.ReadLine().ToLower();
                    Console.WriteLine("Please select the state which contains this city in 2 letter format.");
                    var stateOfCityToUpdate = Console.ReadLine().ToLower();
                    IEnumerable<SightingData> cityUpdate = from sighting in fileContents where sighting.City == cityToUpdate && sighting.State == stateOfCityToUpdate select sighting;
                    List<SightingData> toUpdateCityList = cityUpdate.ToList();
                    Console.WriteLine("Please enter a new city that you would like to update results to.");
                    var updateCity = Console.ReadLine().ToLower();
                    foreach (SightingData selection in toUpdateCityList)
                    {
                        selection.City = updateCity;
                    }
                    return fileContents;
                }
                else
                {
                    Console.WriteLine("Would you like to update results by international city? y/n");
                    string updateInternational = Console.ReadLine().ToLower();
                    if(updateInternational == "y")
                    {
                        Console.WriteLine("Please select a city to update.");
                        var internationalUpdate = Console.ReadLine().ToLower();
                        Console.WriteLine("Please enter the name of a non-US country as a two letter abbreviation.");
                        var countryOfCityToUpdate = Console.ReadLine().ToLower();
                        IEnumerable<SightingData> internationalCityUpdate = from sighting in fileContents where sighting.City.Contains(internationalUpdate) && sighting.Country == countryOfCityToUpdate select sighting;
                        List<SightingData> toUpdateCityList = internationalCityUpdate.ToList();
                        Console.WriteLine("Please enter a new city that you would like to update results to.");
                        var updateInCity = Console.ReadLine().ToLower();
                        foreach (SightingData selection in toUpdateCityList)
                        {
                            selection.City = updateInCity;
                        }
                        return fileContents;
                    }
                    else
                    {
                        Console.WriteLine("Please enter a date to update in the format mm/dd/yyyy hh:mm");
                        string updateDate = Console.ReadLine();
                        DateTime dateToBeUpdated;
                        DateTime.TryParse(updateDate, out DateTime parsedDate);
                        dateToBeUpdated = parsedDate;
                        IEnumerable<SightingData> updateQuery = from sighting in fileContents where sighting.SightingDate == dateToBeUpdated select sighting;
                        List<SightingData> listToUpdate = updateQuery.ToList();
                        Console.WriteLine("Please enter a date to update selection to in the format mm/dd/yyyy hh:mm");
                        string dateUpdatedTo = Console.ReadLine();
                        DateTime dateUpdated;
                        DateTime.TryParse(dateUpdatedTo, out DateTime parsedUpdatedDate);
                        dateUpdated = parsedUpdatedDate;
                        foreach(SightingData selection in listToUpdate)
                        {
                            selection.SightingDate = dateUpdated;
                        }
                        return fileContents;
                    }
                }
            }
            else
            {
                return fileContents;
            }
        }

        //write the final results set to the .csv. This overwrites the original data to only return the custom dataset
        public static void CSVWriter(List<SightingData> finalResult, string fileName)
        {
            using (var writer = new StreamWriter(fileName))
            {
                foreach (SightingData entry in finalResult)
                {
                    writer.WriteLine(entry.SightingDate + "," + entry.City + "," + entry.State + "," + entry.Country + "," +
                                entry.Shape + "," + entry.Duration + "," + entry.Comments + "," + entry.DatePosted + "," + entry.Latitude +
                                "," + entry.Longitude);

                }
            }
        }

        //write the final results to the console
        public static void ConsoleWriter(List<SightingData> queryResults)
        {
            foreach(SightingData UFO in queryResults)
            {
                Console.WriteLine(UFO.SightingDate + "," + UFO.City + "," + UFO.State + "," + UFO.Country + "," +
                            UFO.Shape + "," + UFO.Duration + "," + UFO.Comments + "," + UFO.DatePosted + "," + UFO.Latitude +
                            "," + UFO.Longitude);
            }
            Console.WriteLine("The total number of matches for this search is " + queryResults.Count());
            Console.ReadLine();
        }
    }
}
