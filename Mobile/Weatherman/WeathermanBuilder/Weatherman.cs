using System;
using System.Text;
using Bridge;
using Bridge.Html5;
using Bridge.jQuery2;

namespace WeathermanBuilder
{
    [FileName("WeatherMan")]
    public class Weatherman
    {
        // IMPORTANT: If you have downloaded this from the Bridge.NET KB article link make sure you replace String.Empty with your wunderground.com developer key as described in the KB article.
        const string key = "YOUR KEY HERE";
        const string weatherUrl = "http://api.wunderground.com/api/{0}/forecast/geolookup/conditions/q/{1},{2}.json";

        [Ready]
        public static void Main()
        {           
            GetForecast();

            // refresh button
            new jQuery(new ButtonElement
            {
                Type = ButtonType.Button,
                ClassName = "btn btn-primary",
                InnerHTML = "<span class=\"glyphicon glyphicon-refresh\"></span>",
                OnClick = Weatherman.UpdateButton_Click
            }).AppendTo("#btnDiv");
        }

        public static void GetForecast()
        {
            Global.Navigator.Geolocation.GetCurrentPosition(delegate(GeolocationPosition position)
            {
                GetWeather(position.Coords.Latitude, position.Coords.Longitude);
            });
        }

        public static void GetWeather(double lat, double lng)
        {
            jQuery.GetJSON(
                string.Format(weatherUrl, key, lat, lng),
                null,
                delegate(object data, string s, jqXHR jqXHR)
                {
                    // get all the information
                    var selectedMetric = jQuery.Select("input[type='radio'][name='metric']:checked");
                    var metric = (selectedMetric.Length > 0) ? selectedMetric.Val() : "c";
                    var location = data["location"]["city"];
                    var temp = data["current_observation"]["temp_" + metric];
                    var img = data["current_observation"]["icon_url"];
                    var desc = data["current_observation"]["weather"];
                    var wind = data["current_observation"]["wind_string"];

                    jQuery.Select("#location").Html(location.ToString());
                    jQuery.Select("#temp").Html(temp.ToString());
                    jQuery.Select("#desc").Html(desc.ToString());
                    jQuery.Select("#wind").Html(wind.ToString());

                    jQuery.Select("#img").Attr("src", img.ToString());

                    // select the user prefered metric                    
                    jQuery.Select("#" + metric)
                        .Attr("checked", "checked")
                        .Parent().AddClass("active");
                }
            )
            .Fail(delegate()
            {
                Global.Alert("Key required to call the Weather Underground API not found.");
            });
        }

        public static void UpdateButton_Click(Event e)
        {
            Weatherman.GetForecast();
        }
    }
}
