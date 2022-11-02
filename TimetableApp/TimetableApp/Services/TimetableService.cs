using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;
using TimetableApp.Models;

namespace TimetableApp.Services
{
    public class TimetableService
    {
        private const string BaseUrl = "http://192.168.244.82:5000/api/Timetable";

        // private const string BaseUrl = "http://192.168.1.179:5000/api/Timetable";
        // const string BaseUrl = "http://10.154.32.225:5000/api/Timetable";
        private const string HerokuBaseUrl = "https://test-msu-back.herokuapp.com/";

        // настройки для десериализации для нечувствительности к регистру символов
        private JsonSerializerOptions options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
        };

        // настройка клиента
        private HttpClient GetClient()
        {
            var handler = new HttpClientHandler();
            handler.ClientCertificateOptions = ClientCertificateOption.Manual;
            handler.ServerCertificateCustomValidationCallback =
                (httpRequestMessage, cert, cetChain, policyErrors) => { return true; };

            var client = new HttpClient(handler);
            // client.Timeout = TimeSpan.FromSeconds(5);
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            return client;
        }

        public string GetUrlForHurokuBack(string dir, int course)
        {
            var uriBuilder = new UriBuilder(HerokuBaseUrl);
            var query = HttpUtility.ParseQueryString(uriBuilder.Query);
            query["direction"] = dir;
            query["course"] = course.ToString();
            uriBuilder.Query = query.ToString() ?? string.Empty;
            return uriBuilder.ToString();
        }

        public async Task<List<LessonGroup>> Get(string dir, int course)
        {
            var uriBuilder = new UriBuilder(BaseUrl);
            var query = HttpUtility.ParseQueryString(uriBuilder.Query);
            query["dir"] = dir;
            query["course"] = course.ToString();
            uriBuilder.Query = query.ToString() ?? string.Empty;
            var url = uriBuilder.ToString();
            HttpClient client = GetClient();
            string result = await client.GetStringAsync(url);
            var groups = JsonSerializer.Deserialize<IEnumerable<IEnumerable<Lesson>>>(result, options);
            var list = new List<LessonGroup>();
            foreach (var group in groups)
            {
                var lessonGroup = new LessonGroup(group.FirstOrDefault().Date, "-");
                lessonGroup.AddRange(group);
                list.Add(lessonGroup);
            }
            return list;
        }

        public async Task<List<LessonGroup>> GetTeacherTimetable(string teacher)
        {
            var uriBuilder = new UriBuilder(BaseUrl);
            uriBuilder.Path += "/byteacher";
            var query = HttpUtility.ParseQueryString(uriBuilder.Query);
            query["teacher"] = teacher;
            uriBuilder.Query = query.ToString() ?? string.Empty;
            var url = uriBuilder.ToString();
            HttpClient client = GetClient();
            string result = await client.GetStringAsync(url);
            var groups = JsonSerializer.Deserialize<IEnumerable<IEnumerable<Lesson>>>(result, options);
            var list = new List<LessonGroup>();
            foreach (var group in groups)
            {
                if(!group.Any()) continue;
                var lessonGroup = new LessonGroup(group.FirstOrDefault().Date, "-");
                lessonGroup.AddRange(group);
                list.Add(lessonGroup);
            }
            return list;
        }
    }
}