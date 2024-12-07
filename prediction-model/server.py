from flask import Flask, request, jsonify
import json
from datetime import datetime
from test_model import predict_sales
import pandas as pd

app = Flask(__name__)

@app.route('/predict', methods=['POST'])
def execute_model():
    try:
        # Get the request data as JSON
        data = request.get_json()

        # Extract the dates and sales from the request
        dates = list(data.keys())
        sales = list(data.values())

        # Create a DataFrame from the dates and sales
        df = pd.DataFrame({'date': dates, 'sales': sales})
        df['date'] = pd.to_datetime(df['date'])

        # Group sales by month and calculate the sum
        monthly_sales = df.groupby(df['date'].dt.to_period('M')).sum(numeric_only=True).reset_index()

        # Call the predict_sales function from test_model.py
        predictions = predict_sales(monthly_sales)

        if predictions is None:
            raise Exception('Error executing model')

        # Return the predictions as a JSON response
        return jsonify({'prediction': predictions.tolist()[0][0]})

    except Exception as e:
        # Return an error response if an exception occurred
        return jsonify({'error': str(e)}), 500

if __name__ == '__main__':
    #app.run(host='192.168.1.108', debug=True, threaded=True)
    app.run(host='192.168.1.108', threaded=True)
