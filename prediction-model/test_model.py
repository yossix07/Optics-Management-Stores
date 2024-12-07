import numpy as np
from keras.models import load_model
from sklearn.preprocessing import MinMaxScaler

model = load_model("sales_prediction_model_5.h5")

def predict_sales(monthly_sales):
    try:
        number_of_months = 6
        
        # Normalize the input data
        scaler = MinMaxScaler(feature_range=(0, 1))
        monthly_sales_scaler = scaler.fit_transform(monthly_sales['sales'].values.reshape(-1, 1))

        # Assuming you have a new input sequence for prediction named x_test
        x_test = np.array([monthly_sales_scaler[-number_of_months:].flatten()])

        # Reshape x_test to fit the model input shape
        x_test = np.reshape(x_test, (x_test.shape[0], x_test.shape[1], 1))

        # Make predictions
        predictions = model.predict(x_test)
        predictions = scaler.inverse_transform(predictions)

        return predictions
    except:
        return None
