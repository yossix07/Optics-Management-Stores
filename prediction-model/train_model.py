import numpy as np
import pandas as pd
from sklearn.model_selection import train_test_split
from keras.models import Sequential
from keras.layers import LSTM, Dense
from keras.optimizers import Adam
from sklearn.preprocessing import MinMaxScaler
from keras.regularizers import l2

number_of_months = 6

# Assuming you have a dataset variable containing the sales data
dataset = pd.read_csv('train.csv')

# Group sales by month (per year) and calculate the sum
dataset['date'] = pd.to_datetime(dataset['date'])
# sum of sales per month per product per store

'''
monthly_sales_old = dataset.groupby(dataset['date'].dt.to_period('M')).sum(numeric_only=True).reset_index()
monthly_sales_store = dataset.groupby(['store', dataset['date'].dt.to_period('M')]).sum(numeric_only=True).reset_index()
monthly_sales_item = dataset.groupby(['item', dataset['date'].dt.to_period('M')]).sum(numeric_only=True).reset_index()
'''

monthly_sales = dataset.groupby(['store','item', dataset['date'].dt.to_period('M')]).sum(numeric_only=True).reset_index()

# Create input sequences (x) and corresponding labels (y)
x = []
y = []

for i in range(number_of_months, len(monthly_sales)):
    # Extract the store and item for the current data point
    current_store = monthly_sales['store'].iloc[i]
    current_item = monthly_sales['item'].iloc[i]
    
    # Check if the previous number_of_months data points have the same store and item
    if all(monthly_sales['store'].iloc[i-number_of_months+1:i+1] == current_store) and all(monthly_sales['item'].iloc[i-number_of_months+1:i+1] == current_item):
        x.append(monthly_sales['sales'].iloc[i-number_of_months:i].values.flatten())
        y.append(monthly_sales['sales'].iloc[i])

# Convert to numpy arrays
x = np.array(x)
y = np.array(y)

# Normalize the data
scaler = MinMaxScaler(feature_range=(0, 1))
x = scaler.fit_transform(x)
y = scaler.fit_transform(y.reshape(-1, 1))

# Split the data into training, validation, and test sets
x_train_val, x_test, y_train_val, y_test = train_test_split(x, y, test_size=0.2, random_state=42)
x_train, x_val, y_train, y_val = train_test_split(x_train_val, y_train_val, test_size=0.2, random_state=42)

# Define the model architecture
model = Sequential()
model.add(LSTM(64, input_shape=(number_of_months, 1), kernel_regularizer=l2(0.5)))
# model.add(Dense(128, activation='linear', kernel_regularizer=l2(0.5)))
model.add(Dense(64, activation='linear', kernel_regularizer=l2(0.5)))
model.add(Dense(32, activation='linear', kernel_regularizer=l2(0.5)))
model.add(Dense(1))

# Compile the model with a custom learning rate
learning_rate = 0.001
optimizer = Adam(learning_rate=learning_rate)
model.compile(loss='mean_squared_error', optimizer=optimizer)

# Reshape the input data to fit the model input shape
x_train = np.reshape(x_train, (x_train.shape[0], x_train.shape[1], 1))
x_val = np.reshape(x_val, (x_val.shape[0], x_val.shape[1], 1))
x_test = np.reshape(x_test, (x_test.shape[0], x_test.shape[1], 1))
y_train = y_train.reshape(-1, 1)
y_val = y_val.reshape(-1, 1)
y_test = y_test.reshape(-1, 1)

# Train the model
model.fit(x_train, y_train, epochs=20, batch_size=1024, validation_data=(x_val, y_val))

# Evaluate the model on the test data
loss = model.evaluate(x_test, y_test)
accuracy = 1 - loss
print("Accuracy:", accuracy)

# Save the model
model.save("sales_prediction_model_5.h5")
