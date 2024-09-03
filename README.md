# A Mobile Application for Time-series Forecasting

## RQ and Topic Brief

### How effective are LSTM NNs for time-series data forecasting?

<p>Deep neural networks are machine learning models that have historically proven effective and scalable in extracting patterns in data of various complexity. Time-series data forecasting is particularly compelling in weather forecasting, trend assessment in stock market price evolution, processing and analysis of personal medical records, etc. Solving these tasks with a reasonably high degree of accuracy is an ambitious goal in ML research due to practical applicability and the technical challenge posed by the amount and complexity of real-world time-series data. This project aims to train a Long Short-Term Memory network (LSTM) to be used for weather forecasting with an associated Android application. Later, we plan to extend the LSTM model to other applications, particularly stock market price prediction, which will be the focus of the project extension to a Bachelor Thesis.</p>

## Programming Languages and Frameworks

- **C#** with **MAUI** for frontend (i.e the app itself)
- **Python** with scikit-learn and/or Pandas, and PyTorch _OR_ TensorFlow Lite _OR_ EON Compiler (Edge platform)

## Key Steps
 
1. **Familiarity** with the **relevant technologies**
    - Theory behind **LSTM NNs** 
        - Deep Learning by Ian Goodfellow, Yoshua Bengio, and Aaron Courville, The MIT Press, 2016.
        - Deep Learning: A Textbook, 1st edition, Springer (2018).
        - Various online resources such as _articles_ with or without implementation in code.
    - Basics of the **Pandas** and **ML libraries** for Python
        - Various online resources such as _guides_ and _tutorials_.
        - Official [documentation](https://pandas.pydata.org/docs/).
    - Android app development with **MAUI** for C#
        - Various online resources such as _guides_ and _tutorials_.
        - Official [documentation](https://learn.microsoft.com/en-us/dotnet/maui/?view=net-maui-8.0).
2. Sourcing and processing of **training data**, and **training** the model 
    - [WeatherBench](https://github.com/pangeo-data/WeatherBench).
    - [VisualCrossing](https://www.visualcrossing.com/).
    - [Kaggle](https://www.kaggle.com/search?q=Weather+in%3Adatasets).
    - [ECMWF](https://www.ecmwf.int)...
3. Sourcing and processing **inference data**, communication with public station servers
    - [Weatherstack](https://weatherstack.com/), among possibly others.
4. Developing the **Android app** with a _GUI_
    - Choosing **cities** (later, choosing **stocks**) to **track**.
    - Choosing **time intervals** for predictions, and
    - A choice for an approximate **uncertainty threshold**, as in, no display of future predictions below _n_% certainty chosen by user, if possible to implement reasonably.
    - Miscalleneous **settings**
        - dark/light mode.
        - units, etc.
5. C# frontend and Python backend **intercommunication**
    - _API_ development.
    - _Formatting_ output, etc.
6. _Bachelor Thesis Extension._ Steps **2.** and **3.** for **stock price prediction tasks** enhanced by sentiment analysis of major press agencies' news
7. _Bachelor Thesis Extension._ Model **correctness logging**
    - Evaluating the **performance** of the model over time.
    - **Comparison** with different models/methods (mainly for stock market prediction).
8. User and developer **documentation**

## Conclusion

<p>The final product should be a forecasting application based on AI and ML with a versatile LSTM model at its core. This feature will differentiate the developed SW from, for example, other weather apps on the market. In this way, the project will provide a user-friendly proof of concept for the practicality of existing research on LSTM NNs for time-series data forecasting.</p>
