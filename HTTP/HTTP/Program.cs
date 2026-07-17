using System.Net.Http.Json;

namespace HTTP
{
    internal class Program
    {               
        static async Task Main(string[] args)
        {
            string urlPlanets = "https://swapi.py4e.com/api/planets";
            string urlVeh = "https://swapi.py4e.com/api/vehicles";
            string urlSpec = "https://swapi.py4e.com/api/species";
            string urlSS = "https://swapi.py4e.com/api/starships";
            string urlFilms = "https://swapi.py4e.com/api/films";
            string urlPeople = "https://swapi.py4e.com/api/people";

            var client = new HttpClient();

            //Planets
            var responseP = await client.GetFromJsonAsync<PlanetResponse>(urlPlanets);
            var planets = responseP.results;

            if (planets != null)
            {
                Console.WriteLine(planets[1].name);
            }

            //Vehicles
            var responseVeh = await client.GetFromJsonAsync<VehiclesResponse>(urlVeh);
            var vehicles = responseVeh.results;

            if (vehicles!=null) {
                Console.WriteLine(vehicles[1].name);
            }

            //Species
            var responseSpec = await client.GetFromJsonAsync<SpeciesResponse>(urlSpec);
            var species = responseSpec.results;
            if (species != null)
            {
                Console.WriteLine(species[0].name);
            }

            //Starships
            var responseSS = await client.GetFromJsonAsync<StarshipResponse>(urlSS);
            var starships = responseSS.results;
            if (starships != null)
            {
                Console.WriteLine(starships[0].name);
            }

            //Films
            var responseF = await client.GetFromJsonAsync<FilmsResponse>(urlFilms);
            var films = responseF.results;
            if (films != null)
            {
                Console.WriteLine(films[0].title);
            }

            //People
            var responsePeople = await client.GetFromJsonAsync<PeopleResponse>(urlPeople);
            var people = responsePeople.results;
            if (people != null)
            {
                Console.WriteLine(people[0].name);
            }
        }
    }
}
