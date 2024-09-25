from datetime import datetime, timedelta


class HourlyWeatherPredictionData:
    def __init__(self, df, start_date):
        self.time = []
        self.temperature_2m = []
        self.relative_humidity_2m = []
        self.apparent_temperature = []
        self.precipitation = []
        self.wind_speed_10m = []

        # Expects real data i.e. rescaled back by column transformer, and as pandas data frame with correct colum names
        for i in range(df.shape[0]):
            self.time.append(f'{start_date.isoformat()}')
            self.temperature_2m.append(df.iloc[i]['temperature_2m'])
            self.relative_humidity_2m.append(df.iloc[i]['relative_humidity_2m'])
            self.apparent_temperature.append(df.iloc[i]['apparent_temperature'])
            self.precipitation.append(df.iloc[i]['precipitation'])
            self.wind_speed_10m.append(df.iloc[i]['wind_speed_10m'])

            start_date += timedelta(hours=1)


def weather_data_encoder(weather_data):
    if isinstance(weather_data, HourlyWeatherPredictionData):
        return {'time': weather_data.time,
                'temperature_2m': weather_data.temperature_2m,
                'relative_humidity_2m': weather_data.relative_humidity_2m,
                'apparent_temperature': weather_data.apparent_temperature,
                'precipitation': weather_data.precipitation,
                'wind_speed_10m': weather_data.wind_speed_10m}

    else:
        raise TypeError('weather_data must be of type HourlyWeatherPredictionData')