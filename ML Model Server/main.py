import json
import lzma
import pickle

from Services.weather_data_service import get_dataframe_from_coords
from MLModel.model_training import train_MLPRegressor, get_requested_forecast
from flask import Flask, request, Response
from datetime import datetime

import pandas as pd
pd.options.display.max_columns = 10
pd.options.display.max_rows = 10

PULL_DATA_FROM_INTERNET = False
DATA_PATH = "Resources/prague_data"

TRAIN_MODEL_ANEW = False
MODEL_PATH = "Resources/MLPRegressor.model"


server = Flask(__name__)
model = None
ct_in = None
ct_out = None


@server.route("/forecast", methods=['GET'])
def get_forecast():
    lat = request.args.get('lat', type=float)
    lon = request.args.get('lon', type=float)
    start_date = request.args.get('date', type=str)

    if lat is None or lon is None or start_date is None:
        error_response = json.dumps({'error': 'Missing parameters! Please provide latitude, longitude, and date.'})
        return Response(error_response, status=400, mimetype='application/json')

    return Response(get_requested_forecast(model, ct_in, ct_out, lat, lon, datetime.fromisoformat(start_date)), mimetype='application/json')


# Press the green button in the gutter to run the script.
if __name__ == '__main__':
    if PULL_DATA_FROM_INTERNET:
        df = get_dataframe_from_coords(50, 14.4)
        df.to_pickle(DATA_PATH)
    else:
        df = pd.read_pickle(DATA_PATH)

    if TRAIN_MODEL_ANEW:
        model, ct_in, ct_out = train_MLPRegressor(df)

        # Serialize the model.
        with lzma.open(MODEL_PATH, "wb") as model_file:
            pickle.dump((model, ct_in, ct_out), model_file)

    with lzma.open(MODEL_PATH, "rb") as model_file:
        (model, ct_in, ct_out) = pickle.load(model_file)

    server.run(debug=True, host='0.0.0.0', port=80)
