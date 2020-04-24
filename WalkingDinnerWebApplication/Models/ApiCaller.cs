using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;

namespace WalkingDinnerWebApplication.Models
{
    public class GeoData
    {
        public class StandardData
        {
            public List<string> addresst { get; set; }
            public string city { get; set; }
        }
        public class ErrorData
        {
            public string code { get; set; }
            public string message { get; set; }
            public string description { get; set; }
        }
        public float longt { get; set; }
        public float latt { get; set; }
        public StandardData standard { get; set; }
        public ErrorData error { get; set; }
    }

    public class ApiCaller
    {
        public GeoData PostcodeToGeoLocation(string postcode)
        {
            string API_SERVICE_URL = "https://geocode.xyz/";
            string API_KEY = "757077098542105640300x5657";
            
            using (var _http = new HttpClient() )
            {
                //_http.DefaultRequestHeaders.Accept.Add(
                //        new MediaTypeWithQualityHeaderValue("application/json")
                //    );
                
                _http.BaseAddress = new Uri(API_SERVICE_URL);
                var response = _http.GetAsync($"?region=NL&locate={postcode}&json=1&auth={API_KEY}").Result;
                if (response.IsSuccessStatusCode)
                {
                    var json_str = response.Content.ReadAsStringAsync().Result;
                    
                    GeoData result = null;
                    try
                    {
                        result= JsonConvert.DeserializeObject<GeoData>(json_str);
                        //if server ignores us, stop
                        if (result?.error?.code == "002")
                            throw new HttpUnhandledException(result.error.description);
                        //if server remembers us about throttle, just recursive request again
                        if (result?.error?.code == "006")
                            return PostcodeToGeoLocation(postcode);
                        return result;
                    }
                    catch (HttpUnhandledException e)
                    {
                        throw e;
                    }
                    catch
                    {
                        Console.WriteLine("ERROR: " + json_str);
                    }
                    return null;
                }
                else
                {
                    Console.WriteLine(response.StatusCode + ": " + response.ReasonPhrase);
                    if (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden)
                        throw new HttpUnhandledException(response.ReasonPhrase);
                }
            }
            return null;
        }
    }
}






