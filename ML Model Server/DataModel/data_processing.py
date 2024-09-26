from sklearn.compose import ColumnTransformer
from sklearn.preprocessing import StandardScaler, Normalizer

from DataModel.weather_prediction_data import HourlyWeatherPredictionData, weather_data_encoder
import MLModel

import pandas as pd
import numpy as np
import json

pd.options.display.max_columns = 10
pd.options.display.max_rows = 10

np.set_printoptions(precision=5)
np.set_printoptions(suppress=True)


# For encoding seasonal and time-of-day differences in temperature and weather in general.
# In principle, we don't care what year it is, and in large part what day in a month it is.
# Function assumes data has date column with pandas timestamps in it.
def extract_cyclical_time_features(data):
    def sin_tf(to_transform, max_val): return np.sin(to_transform * (2. * np.pi / max_val))
    def cos_tf(to_transform, max_val): return np.cos(to_transform * (2. * np.pi / max_val))

    dates = data['date']
    # subtracting one so months in range from 0(-11)
    data.insert(0, "month_sin", sin_tf(dates.transform(lambda x: x.month - 1), 12))
    data.insert(1, "month_cos", cos_tf(dates.transform(lambda x: x.month - 1), 12))
    data.insert(2, "hour_sin", sin_tf(dates.transform(lambda x: x.hour), 24))
    data.insert(3, "hour_cos", cos_tf(dates.transform(lambda x: x.hour), 24))
    data.drop('date', axis=1, inplace=True)

    return data


def data_preprocess_pipeline(data, hours_in_input, hours_in_output):
    data_in = extract_cyclical_time_features(data)
    # We do not want to predict months and hours
    data_out = data_in.drop(columns=['month_sin', 'month_cos', 'hour_sin', 'hour_cos'], axis=1)

    scaler_in = ColumnTransformer([
        ('std scaler', StandardScaler(), ['temperature_2m', 'apparent_temperature', 'precipitation', 'wind_speed_10m', "relative_humidity_2m"]),
    ], remainder='passthrough')
    scaler_out = ColumnTransformer([
        ('std scaler', StandardScaler(), slice(0, data_out.shape[1]))])  # all cols

    # Pipe with normalizer might be better
    # pipe = sklearn.pipeline.make_pipeline(scaler_ct, Normalizer())

    data_in = scaler_in.fit_transform(data_in)
    inputs = np.array([np.concatenate(data_in[i:i + hours_in_input]) for i in range(0, len(data) - hours_in_input - hours_in_output, hours_in_input)])

    data_out = scaler_out.fit_transform(data_out)
    targets = np.array([np.concatenate(data_out[i:i + hours_in_output]) for i in range(hours_in_input, len(data) - hours_in_output, hours_in_input)])

    return inputs, targets, scaler_in, scaler_out


def prepare_json_from_prediction_aray(prediction_arr, ct_out, start_date):
    prediction_arr = ct_out.named_transformers_["std scaler"].inverse_transform(prediction_arr.reshape(MLModel.model_training.HOURS_IN_OUTPUT, 5))

    df = pd.DataFrame({'temperature_2m': prediction_arr[:, 0], 'relative_humidity_2m': prediction_arr[:, 1],
                       'apparent_temperature': prediction_arr[:, 2], 'precipitation': prediction_arr[:, 3],
                       'wind_speed_10m': prediction_arr[:, 4]})

    prediction_object = HourlyWeatherPredictionData(df, start_date)

    return json.dumps(prediction_object, default=weather_data_encoder, indent=4)
