﻿import openmeteo_requests
import requests_cache
from retry_requests import retry

import pandas as pd


# Set up the Open-Meteo API client with cache and retry on error
cache_session = requests_cache.CachedSession('.cache', expire_after=-1)
retry_session = retry(cache_session, retries=5, backoff_factor=0.2)
open_meteo_client = openmeteo_requests.Client(session=retry_session)


# Can be refactored to support provided variables instead of hardcoded - other data might need
# to be experimented with
def pull_data_via_client(lat, lon, start_date, end_date, history_or_forecast, client):
    # Make sure all required weather variables are listed here
    # The order of variables in hourly or daily is important to assign them correctly below

    # For predictions, "forecast" api has to be used since archive lags behind 5 days..
    url = "https://archive-api.open-meteo.com/v1/archive" if history_or_forecast == "history" else "https://api.open-meteo.com/v1/forecast"
    params = {
        "latitude": lat,
        "longitude": lon,
        "start_date": start_date,
        "end_date": end_date,
        "hourly": ["temperature_2m", "relative_humidity_2m", "apparent_temperature",
                   "precipitation", "wind_speed_10m"]
    }
    responses = client.weather_api(url, params=params)

    # Process first location. Add a for-loop for multiple locations or weather models
    response = responses[0]
    print(f"Coordinates {response.Latitude()}°N {response.Longitude()}°E")
    print(f"Elevation {response.Elevation()} m asl")
    print(f"Timezone {response.Timezone()} {response.TimezoneAbbreviation()}")
    print(f"Timezone difference to GMT+0 {response.UtcOffsetSeconds()} s")

    # Process hourly data. The order of variables needs to be the same as requested.
    hourly = response.Hourly()
    hourly_temperature_2m = hourly.Variables(0).ValuesAsNumpy()
    hourly_relative_humidity_2m = hourly.Variables(1).ValuesAsNumpy()
    hourly_apparent_temperature = hourly.Variables(2).ValuesAsNumpy()
    hourly_precipitation = hourly.Variables(3).ValuesAsNumpy()
    hourly_wind_speed_10m = hourly.Variables(4).ValuesAsNumpy()

    hourly_data = {"date": pd.date_range(
        start=pd.to_datetime(hourly.Time(), unit="s", utc=True),
        end=pd.to_datetime(hourly.TimeEnd(), unit="s", utc=True),
        freq=pd.Timedelta(seconds=hourly.Interval()),
        inclusive="left"
    ), "temperature_2m": hourly_temperature_2m, "relative_humidity_2m": hourly_relative_humidity_2m,
        "apparent_temperature": hourly_apparent_temperature, "precipitation": hourly_precipitation,
        "wind_speed_10m": hourly_wind_speed_10m}

    hourly_dataframe = pd.DataFrame(data=hourly_data)
    return hourly_dataframe


def get_dataframe_from_coords(lat, lon):
    return pull_data_via_client(lat, lon, "2000-01-01", "2023-12-31", "history", open_meteo_client)
