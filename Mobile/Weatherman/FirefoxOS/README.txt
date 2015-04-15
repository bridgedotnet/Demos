ABOUT THE WEATHER UNDERGROUND DEVELOPER KEY 
-------------------------------------------

Weather Underground (http://api.wunderground.com/api/) provides weather data via its web API. 

A developer key is required as a parameter to API calls. By default, no key is provided in this
source code package and the Firefox OS App will report a related error when run. 


HOW TO GET YOUR KEY
-------------------

In order to make this App work you need to request a key for free by signing up for a Weather Underground
developer account (http://www.wunderground.com/weather/api/d/login.html).

Once you have got your key make sure you set it as the value of the 'key' field 
in 'resources\js\weatherMan.js' by replacing "" with your key in the following line of code:

key: ""




