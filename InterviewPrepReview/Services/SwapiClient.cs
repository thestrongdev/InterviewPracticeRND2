using InterviewPrepReview.Models.APIModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace InterviewPrepReview.Services
{
    public class SwapiClient
    {
        private readonly HttpClient _client;

        public SwapiClient(HttpClient client)
        {
            _client = client;
        }


        public async Task<IEnumerable<PeopleResponseModel>> GetResidents(List<string> residentIDs)
        {
            var models = new List<PeopleResponseModel>();

            foreach (var id in residentIDs)
            {
                var response = await _client.GetAsync($"api/people/{id}/");


                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStreamAsync();
                    var model = await JsonSerializer.DeserializeAsync<PeopleResponseModel>(content);

                    models.Add(model);

                }
                else
                {
                    throw new HttpRequestException("SWAPI returned a bad response");
                }
            }

            return models;
            
        }


        public async Task<OnePlanetResponseModel> GetOnePlanet(int id)
        {
            var response = await _client.GetAsync($"api/planets/{id}/");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStreamAsync();
                var model = await JsonSerializer.DeserializeAsync<OnePlanetResponseModel>(content);

                return model;
            
            }
            else
            {
                throw new HttpRequestException("SWAPI returned a bad response");
            }
        }

        public async Task<IEnumerable<PlanetsResponseModel>> GetAllPlanets()
        {
            var count = 1;
            var url = $"api/planets/?page={count}";
            var planetModels = new List<PlanetsResponseModel>();


            do
            {
                var response = await _client.GetAsync(url);
                count += 1;
                url = $"api/planets/?page={count}";

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStreamAsync();
                    var model = await JsonSerializer.DeserializeAsync<PlanetsResponseModel>(content);

                    planetModels.Add(model);

                                    }
                else
                {
                    throw new HttpRequestException("SWAPI returned a bad response");
                }


            } while (count < 7);

            return planetModels;
        }
    }
}
