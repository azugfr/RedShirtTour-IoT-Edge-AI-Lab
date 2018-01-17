## Setup an environment to manage Azure Machine Learning models



[The previous lab](/Lab%206%20-%20Create%20and%20deploy%20a%20process%20data%20module) describes how to create and deploy a data filter module. In this lab we will setup an environment to manage Azure Machine Learning models.

**Objective:** configure the environment to manage Azure ML models.


Azure ML model management enables you to effectively deploy and manage Machine Learning models that are built with different frameworks, including SparkML, Keras, TensorFlow, Microsoft Cognitive Toolkit or Python.


To use Command Line Interfaces (CLIs) from Azure Machine Learning Workbench you must execute commands in the command prompt (Win+ R):

`pip install azure-cli`

`pip install azure-cli-ml`

The environment can be created in one of the available subscriptions. To connect using the site <https://aka.ms/devicelogin> :

`az login`

If you need to disconnect, the following command can be used:

`az logout`

List all available Azure subscriptions:

`az account list -o table`

Select the Azure subscription to use:

`az account set -s [Subscription Id or name]`

Since among the resources groups of the subscription we can already have the group dedicated to the environment, we can see the list of all the groups using the command: 

`az group list`

If you need to delete some resource group:

`az group delete -n [Resource group name]`

To start the configuration process, you must register the environment provider by entering the following command:

`az provider register -n Microsoft.MachineLearningCompute`

To deploy and test your web service on the local computer, configure a locale using the following command. The name of there source group is optional.

`az ml env setup -l [Azure Region, e.g. eastus2or westeurope] -n [Environment name] -g [Resource group name]`

To see more information about the environment:

`az ml env show -g [Resource group name] -n [Environment name]`

The local environment configuration command creates the following resources in the subscription:

· Resource group (if it didn't be specified or the supplied name does not exist)

· Storage account

· Azure Container Registry

· Application Insights Account

Once the configuration is complete, you must define the environment to use with the following command:

`az ml env set -g [Resource group name] -n [Environment name]`

Creating the Model Management account:

`az ml account modelmanagement create -l [Azure Region, e.g. eastus2 or westeurope] -n [Model management account name] -g [Resource group name] --sku-instances [Number of instances, for example 1] --sku-name [Pricing level for example DevTest or S1]`

To use an existing account, use the following command:

`az ml account modelmanagement set -n [Model management account name] -g [Resource group name]`

The model management environment is configured and ready for [creating and deployment of the Machine Learning models](/Lab%208%20-%20Create%20an%20Azure%20Machine%20Learning%20model%20for%20an%20IoT%20Edge%20module).

