using System.Net.Http.Json;
using System.Text;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace SkyExtensions;




public static class HttpClientExtensions
{
    public class AuthorizedStatus
    {
        public System.Net.HttpStatusCode StatusCode;
        public DateTime ValGuitarItemIdUntil;
    }

    public class HttpClientErrorEventArgs : EventArgs
    {
        public System.Net.HttpStatusCode StatusCode { get; set; }
        public string Message { get; set; }
    }
    public static event EventHandler<HttpClientErrorEventArgs> HttpClientErrorEvent = delegate { };

    public static async Task<AuthorizedStatus> GetAuthorizedStatus(this HttpClient client, string url)
    {

        using (var response = await client.GetAsync(url))
        {
            return new AuthorizedStatus() { StatusCode = response.StatusCode, ValGuitarItemIdUntil = DateTime.Now };
        }
    }



    public async static Task<T> GetModelFromJsonAsync<T>(this HttpClient client, string url)
    {
        try
        {
            using (var response = await client.GetAsync(url))
            {
                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    HttpClientErrorEvent(null, new HttpClientErrorEventArgs()
                    {
                        StatusCode = response.StatusCode,
                        Message = $"{response.StatusCode} {(int)response.StatusCode}  "
                    });
                    return default(T);
                }

                response.EnsureSuccessStatusCode();

                using Stream stream = await response.Content.ReadAsStreamAsync();


                var tt = await System.Text.Json.JsonSerializer.DeserializeAsync<T>(stream,
                    new System.Text.Json.JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                        NumberHandling = JsonNumberHandling.AllowReadingFromString
                    }
                        );

                return tt;
            }

        }
        catch (Exception)
        {
            return default(T);

        }
    }

    public async static Task<(T ReturnValue, System.Net.HttpStatusCode StatusCode)> GetModelWithStatusFromJsonAsync<T>(this HttpClient client, string url)
    {
        try
        {

            using (var response = await client.GetAsync(url))
            {
                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    HttpClientErrorEvent(null, new HttpClientErrorEventArgs()
                    {
                        StatusCode = response.StatusCode,
                        Message = $"{response.StatusCode} {(int)response.StatusCode}  "
                    });
                    return (default(T), response.StatusCode);
                }

                response.EnsureSuccessStatusCode();

                using Stream stream = await response.Content.ReadAsStreamAsync();


                var tt = await System.Text.Json.JsonSerializer.DeserializeAsync<T>(stream,
                    new System.Text.Json.JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                        NumberHandling = JsonNumberHandling.AllowReadingFromString
                    }
                        );

                return (tt, response.StatusCode);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return (default(T), System.Net.HttpStatusCode.ExpectationFailed);
        }

    }
    public async static Task<(T ReturnValue, System.Net.HttpStatusCode StatusCode, Exception Exception)> PostModelWithStatusFromJsonAsync<T>(this HttpClient client, string url, object value)
    {
        try
        {
            using (var response = await client.PostAsJsonAsync(url, value))
            {
                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    HttpClientErrorEvent(null, new HttpClientErrorEventArgs()
                    {
                        StatusCode = response.StatusCode,
                        Message = $"{response.StatusCode} {(int)response.StatusCode}  "
                    });
                    return (default(T), response.StatusCode, null);
                }

                response.EnsureSuccessStatusCode();

                using Stream stream = await response.Content.ReadAsStreamAsync();
                //StreamReader reader = new StreamReader(stream);
                //string text = reader.ReadToEnd();

                var tt = await System.Text.Json.JsonSerializer.DeserializeAsync<T>(stream,
                    new System.Text.Json.JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                        NumberHandling = JsonNumberHandling.AllowReadingFromString
                    }
                        );

                return (tt, response.StatusCode, null);
            }
        }
        catch (Exception ex)
        {
            return (default(T), System.Net.HttpStatusCode.ExpectationFailed, ex);
        }

    }

    public static T GetModelFromJson<T>(this HttpClient client, string url)
    {

        HttpResponseMessage response = client.GetAsync(url).Result;

        response.EnsureSuccessStatusCode();



        using Stream stream = response.Content.ReadAsStreamAsync().Result;
        using (var reader = new StreamReader(stream, Encoding.UTF8))
        {
            string json = reader.ReadToEnd().Replace("\"errorMessages\":[],", "").Replace("\"orderList\":[],", "").Replace("\"orderListAll\":[],", "");

            return DeserializeJsonString<T>(json);
        }
    }
    public static T DeserializeJsonString<T>(string json)
    {
        return JsonConvert.DeserializeObject<T>(json, new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Objects,
            NullValueHandling = NullValueHandling.Ignore
        });
    }

}
