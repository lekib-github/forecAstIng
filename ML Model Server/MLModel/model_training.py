from sklearn.model_selection import train_test_split
from sklearn.neural_network import MLPRegressor

from DataModel.data_processing import data_preprocess_pipeline

# How many hours we are using for prediction, and how many hours we are predicting
HOURS_IN_INPUT = 24
HOURS_IN_OUTPUT = 120


def train_MLPRegressor(df):
    input_data, output_targets, ct_in, ct_out = data_preprocess_pipeline(df, HOURS_IN_INPUT, HOURS_IN_OUTPUT)

    train_data, test_data, train_target, test_target = train_test_split(input_data, output_targets,
                                                                        test_size=0.2, random_state=42)

    model = MLPRegressor(verbose=True, hidden_layer_sizes=128)

    # model = GridSearchCV(MLPRegressor(verbose=True), {"hidden_layer_sizes":[(512, 256, 128)]},
    #                      cv=KFold(5), n_jobs=-1)

    model.fit(train_data, train_target)

    # for rank, accuracy, params in zip(model.cv_results_["rank_test_score"],
    #                                   model.cv_results_["mean_test_score"],
    #                                   model.cv_results_["params"]):
    #     print("Rank: {:2d} Cross-val: {:.1f}%".format(rank, 100 * accuracy),
    #           *("{}: {:<5}".format(key, str(value)) for key, value in params.items()))

    print(model.score(test_data, test_target))

    # The way to get usable data from results:
    print(ct_out.named_transformers_["std scaler"].inverse_transform(test_target[100].reshape(HOURS_IN_OUTPUT, 5)))
    print("============")
    print(ct_out.named_transformers_["std scaler"].inverse_transform(model.predict(test_data[100].reshape(1, -1)).reshape(HOURS_IN_OUTPUT, 5)))

    return model, ct_in, ct_out

# Some manual experimenting with different number of hours in/out and size of hidden layer(s)
# hours 6 in 1 out
# 16 0.881 0.058
# 32 0.88 0.054 !!
# 128 0.874 0.047 overfitting vvv
# 256 0.865 0.037
# 512 0.850 0.032

# hours 24 in 24 out, harder for model, no using first predicted hours for next one in MLP (possibly implicitly)
# 32 0.576 0.15
# 64 0.58 0.144 !!
# 128 0.554 0.135 overfitting vvv
# 512 0.516 0.114
# (8,8) 0.56 0.16
# (32,32) 0.57 0.14
# (64,64) 0.54 0.12
# (512,512) 0.416, 0.03
# (16,16,16,16) 0.56 0.155
# (8,8,8,8) 0.53 0.163
# (32,32,32,32) 0.54 0.139

# hours 48 in 24 out
# 8 0.55 0.16
# 16 0.55 0.15 !!
# 32 0.54 0.14
# 64 0.499 0.12
# 128 0.45 0.10

# hours 12 in 24 out? Somehow better, seemingly larger context not better at least for MLP
# 16 0.61 0.16
# 32 0.61 0.15
# 64 0.61 0.15
# 128 0.60 0.149
# 256 0.59 0.14

# hours 6 in 24 out? Somehow better, seemingly larger context not better at least for MLP
# 16 0.6 0.164
# 32
# 64 0.61 0.16
# 128
# 256 0.61 0.155

# hours 24 in 48 out
# 128 0.46  0.165

# hours 24 in 72 out
# 128 0.43 0.18

# 3, 24; 24/48, 120??
