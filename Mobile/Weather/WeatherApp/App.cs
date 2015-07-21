using System;
using System.Text;
using Bridge;
using Bridge.Html5;
using Bridge.jQuery2;

namespace WeatherApp
{
    public class App
    {
        /*
         * WU DEVELOPER KEY TERMS OF USE 
         * Weather Underground, hereafter "WU", (http://api.wunderground.com/api/) provides weather data via its web API. 
         * A developer key is required as a parameter to API calls. By default, Bridge.NET, Inc., hereafter "The company", 
         * provides you with a hardcoded WU developer key. The company authorizes you to use this key solely for the purposes 
         * of building and running this Demo project as is. 
         * 
         * *Any other use is strictly prohibited*.
         * 
         * If you want to use the Bridge Weather Demo App in any other context or for any other purpose, you need to 
         * obtain your own WU developer key. You can sign up for a WU developer key at the following address, 
         * for free: http://api.wunderground.com/weather/api/d/login.html?MR=1
         */

        const string key = "c53b1d4229bccdc8";
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
                OnClick = App.UpdateButton_Click
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
            App.GetForecast();
        }
    }
}
