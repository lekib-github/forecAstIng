import lzma
import pickle

from Services.weather_data_service import get_dataframe_from_coords
from MLModel.model_training import train_MLPRegressor

import pandas as pd
pd.options.display.max_columns = 10
pd.options.display.max_rows = 10

PULL_DATA_FROM_INTERNET = True
DATA_PATH = "Resources/prague_data"

TRAIN_MODEL_ANEW = True
MODEL_PATH = "Resources/MLPRegressor.model"


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
    else:
        with lzma.open(MODEL_PATH, "rb") as model_file:
            (model, ct_in, ct_out) = pickle.load(model_file)
