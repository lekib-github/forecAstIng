# User documentation

## forecAstIng - A Mobile App for Time-Series Data Forecasting

This app allows for tracking multiple locations for weather forecasts, as well as ticker symbols for stock data. 

Locations/ symbols are added by clicking on the "+" icon labeled "Add more..." on the main page, entering the name of the location or stock, and clicking on either "Add location" or "Add stock" respectively. On application startup, it will attempt to locate the device and provide weather data for that location automatically. 

The main page consists of 3 tabs - "History" "Today" "Forecasts" - located at the top of the page. Navigating between them presents the relevant data. On the "Today" page, today's conditions are presented, including current, maximum, and minimum temperature/price for each respective location/stock.  Clicking the "More..." button opens the details page for the selected location/stock with hourly data and more detailed information; the back arrow can be pressed to return to the main page. The "History" and "Forecasts" tabs have similar layouts, with the first showing past values, and the second predicted ones. Each location/stock presents a scrolling view of 2/3/5/7 (default) days of historical/forecast data. Clicking on any of the entries, which are annotated by dates they represent,  opens the aforementioned details page for that date.

From the main page, entries can be deleted by swiping them to the left. Pulling down or clicking on the button of the tab currently opened will refresh the data.

Settings can be found in the top right corner when clicking on the gear icon. There, number of history/forecast days (2/3/5/7), temperature units (Fahrenheit/celsius), and light/dark mode (light, dark, system default) can be selected.
