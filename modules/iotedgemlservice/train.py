# Please make sure scikit-learn is included the aml_config/conda_dependencies.yml file.

# Load necessary libraries
import pickle
import sys
import os

import numpy as np
from sklearn.metrics import confusion_matrix

from sklearn.linear_model import LogisticRegression
from sklearn.model_selection import train_test_split
from sklearn.metrics import precision_recall_curve

from azureml.logging import get_azureml_logger
from azureml.dataprep.package import run

# Initialize the logger
run_logger = get_azureml_logger() 

# Create the outputs folder where the training results will be stored
os.makedirs('./outputs', exist_ok=True)

print('Python version: {}\n'.format(sys.version))

# Load dataset from a DataPrep package as a pandas DataFrame
df = run('df.dprep', dataflow_idx=0, spark=False)
print ('Dataset shape: {}'.format(df.shape))

# Define the features columns and the label column
X, Y = df[['batteryVoltage', 'responseTime', 'ambientHumidity', 'ambientTemperature']].values, df['severity_status'].values

# Add n more random features to make the problem harder to solve
# Randomness is required because the df.csv dataset is an example dataset
# and it is easily ranked with almost 100% accuracy
# n - number of new random features to add
n = 17
random_state = np.random.RandomState(0)
n_samples, n_features = X.shape
X = np.c_[X, random_state.randn(n_samples, n)]

# Split data 70%-30% into training set and test set
X_train, X_test, Y_train, Y_test = train_test_split(X, Y, test_size=0.3, random_state=0)

# Obtain the value of the parameter of the regularization, which makes it possible to avoid overfitting
reg = 0.5
# Load regularization rate from argument if present
if len(sys.argv) > 1:
    reg = float(sys.argv[1])

print("Regularization rate is {}".format(reg))

# Log the regularization rate
run_logger.log("Regularization Rate", reg)

# Train a logistic regression model on the training set
clf1 = LogisticRegression(C=1/reg).fit(X_train, Y_train)
print (clf1)

# Evaluate the test set
accuracy = clf1.score(X_test, Y_test)
print ("Accuracy is {}".format(accuracy))

# Log accuracy
run_logger.log("Accuracy", accuracy)

# Calculate and log precesion, recall, and thresholds, which are a list of numerical values
y_scores = clf1.predict_proba(X_test)
precision, recall, thresholds = precision_recall_curve(Y_test, y_scores[:,1],pos_label='moderate')
run_logger.log("Precision", precision)
run_logger.log("Recall", recall)
run_logger.log("Thresholds", thresholds)

print('''
==========================================
Serialize and deserialize using the outputs folder.
''')

# Serialize the model on disk in the special 'outputs' folder
print ("Export the model to model.pkl")
f = open('./outputs/model.pkl', 'wb')
pickle.dump(clf1, f)
f.close()

# Load the model back from the 'outputs' folder into memory
print("Import the model from model.pkl")
f2 = open('./outputs/model.pkl', 'rb')
clf2 = pickle.load(f2)

# Predict on a new sample
X_new = [[31.6, 4.7, 20.8, 77.7]]
print ('New sample: {}'.format(X_new))

# Add random features to match the training data
X_new_with_random_features = np.c_[X_new, random_state.randn(1, n)]

# Score on the new sample
pred = clf2.predict(X_new_with_random_features)
print('Predicted class is {}'.format(pred))
