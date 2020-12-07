#### Task
Create service that will ask several Weather APIs asynchronously and return the first answer. Log weather info and service which responded first. Run application and go to localhost:5000/swagger to test API.

##### What would you like to know before trying it out
1. Services have different latency, so usually you get responses only from one service, especially if you request the same city/coordinates, because weather services cache their responses
2. Very unlikely you ever get a response from AccuWeather, because  1. it takes 2 requests to retrieve weather(you have to get locationId first and after that you can request weather info at this location) 2. it allows only 50 request per day, with 2 request per 1 weather request it makes 25, and I'm tired to create new accounts, so AccuWeather sucks
3. There might be strange decisions on my code, but usually they have some basis why I did so, I know better approaches but they just don't suit this task