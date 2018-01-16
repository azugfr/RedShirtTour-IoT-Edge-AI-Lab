# This script generates the scoring and schema files;
# creates the schema, and holds the init and run functions needed to 
# operationalize the model

# Import data collection library. Only supported for docker mode.
# Functionality will be ignored when package isn't found
try:
    from azureml.datacollector import ModelDataCollector
except ImportError:
    print("Data collection is currently only supported in docker mode. May be disabled for local mode.")
    # Mocking out model data collector functionality
    class ModelDataCollector(object):
        def nop(*args, **kw): pass
        def __getattr__(self, _): return self.nop
        def __init__(self, *args, **kw): return None
    pass

import os

# Format time method 
def ts_to_date(ts):
    import datetime
    return datetime.datetime.fromtimestamp(ts).strftime('%Y-%m-%d %H:%M:%S.%f')

# Prepare the web service definition by authoring init() and run() functions.
# Test the functions before deploying the web service.
def init():
    global inputs_dc, prediction_dc
    from sklearn.externals import joblib

    global model
    model = joblib.load('model.pkl')

    inputs_dc = ModelDataCollector("model.pkl", identifier="inputs")
    prediction_dc = ModelDataCollector("model.pkl", identifier="prediction")

def run(input_str):
    import json
    import time
    import numpy as np
    import pandas as pd
    import random

    input_json = json.loads(input_str)

    input_df = pd.DataFrame([[input_json['drone']['batteryVoltage'], \
        input_json['drone']['responseTime'], \
        input_json['ambient']['temperature'], \
        input_json['ambient']['humidity'], \
        ]])

    # Append 17 random features just like the training script does it.
    n = 17
    random_state = np.random.RandomState(0)
    n_samples, n_features = input_df.shape
    input_df = np.c_[input_df, random_state.randn(n_samples, n)]
    inputs_dc.collect(input_df)

    pred = model.predict(input_df)
    prediction_dc.collect(pred)

    data = dict(
        prediction = str(pred[0]),
        timestamp = ts_to_date(time.time()),
        randomInt = str(random.randint(0,9))
    )

    #return data # service run
    return json.dumps(data) # debug

def main():
  from azureml.api.schema.dataTypes import DataTypes
  from azureml.api.schema.sampleDefinition import SampleDefinition
  from azureml.api.realtime.services import generate_schema
  import pandas as pd
  import json
  
  df = pd.DataFrame(data=[[31.6, 4.7, 20.8, 77.7]], columns=['drone_batteryVoltage', \
   'drone_responseTime', 'ambient_temperature', 'ambient_humidity'])

  # Turn on data collection debug mode to view output in stdout
  os.environ["AML_MODEL_DC_DEBUG"] = 'true'

  # Test the output of the functions
  init()
  
  input1 = '{ "drone": { "batteryVoltage": 31.6, "responseTime": 4.7 }, \
        "ambient": { "temperature": 20.8, "humidity": 77.7 },\
        "timeCreated": "2017-12-27T16:02:09.1933728Z" }'

  print("Result: " + run(input1))
  
  inputs = {"input_str": SampleDefinition(DataTypes.PANDAS, df)}
  
  # Genereate the schema
  generate_schema(run_func=run, inputs=inputs, filepath='./outputs/service_schema.json')
  print("Schema generated")

if __name__ == "__main__":
    main()
