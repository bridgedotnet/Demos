Bridge.Class.define('WeathermanBuilder.Weatherman', {
    statics: {
        $config:  {
            fields: {
                key: "",
                weatherUrl: "http://api.wunderground.com/api/{0}/forecast/geolookup/conditions/q/{1},{2}.json"
            },
            init: function () {
                Bridge.ready(this.main);
            }
        },
        main: function () {
            WeathermanBuilder.Weatherman.getForecast();
            // refresh button
            $(Bridge.merge(document.createElement('button'), {
                type: "button", 
                className: "btn btn-primary", 
                innerHTML: "<span class=\"glyphicon glyphicon-refresh\"></span>", 
                onclick: WeathermanBuilder.Weatherman.updateButton_Click
            } )).appendTo("#btnDiv");
        },
        getForecast: function () {
            Bridge.global.navigator.geolocation.getCurrentPosition(function (position) {
                WeathermanBuilder.Weatherman.getWeather(position.coords.latitude, position.coords.longitude);
            });
        },
        getWeather: function (lat, lng) {
            $.getJSON(Bridge.String.format(WeathermanBuilder.Weatherman.weatherUrl, WeathermanBuilder.Weatherman.key, lat, lng), null, function (data, s, jqXHR) {
                // get all the information
                var selectedMetric = $("input[type='radio'][name='metric']:checked");
                var metric = (selectedMetric.length > 0) ? selectedMetric.val() : "c";
                var location = data.location.city;
                var temp = data.current_observation["temp_" + metric];
                var img = data.current_observation.icon_url;
                var desc = data.current_observation.weather;
                var wind = data.current_observation.wind_string;
                $("#location").html(location.toString());
                $("#temp").html(temp.toString());
                $("#desc").html(desc.toString());
                $("#wind").html(wind.toString());
                $("#img").attr("src", img.toString());
                // select the user prefered metric                    
                $("#" + metric).attr("checked", "checked").parent().addClass("active");
            }).fail(function () {
                Bridge.global.alert("Key required to call the Weather Underground API not found.");
            });
        },
        updateButton_Click: function (e) {
            WeathermanBuilder.Weatherman.getForecast();
        }
    }
});