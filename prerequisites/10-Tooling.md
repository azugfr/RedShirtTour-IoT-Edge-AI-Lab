## **~~~~ Prerequisites of tool installation ~~~~ **



Authors : Artem SHEIKO + Taras CHIEN
Contributors : Vincent Thavonekham + Igor Leontiev



**A - ON THE DEVELOPPER SIDE** : Développement et déploiement des modules contenant la logique personnalisée et des services Azure

Cette partie part du principe que le lecteur utilise un ordinateur exécutant Windows pour simuler un appareil Internet des Objets (IoT).

Pour commencer à travailler avec Azure IoT Edge et passer le processus de création des modules, il faut:

1. Disposer d’un abonnement Azure. Consultez la rubrique Obtenir [une version d’évaluation gratuite d’Azure](https://azure.microsoft.com/pricing/free-trial/).

2. Installer [Docker](https://docs.docker.com/docker-for-windows/install/) et s’assurer qu’il s’exécute correctement.

3. Installer [Visual Studio Code](https://code.visualstudio.com/)

   - Extension [Azure IoT Edge pour Visual Studio Code](https://marketplace.visualstudio.com/items?itemName=vsciot-vscode.azure-iot-edge) ;

    - Extension [C# pour Visual Studio Code (développée par OmniSharp)](https://marketplace.visualstudio.com/items?itemName=ms-vscode.csharp) ;

    - [Kit de développement logiciel (SDK) .NET Core 2.0](https://www.microsoft.com/net/core).

4. Installer [Python](https://www.python.org/ftp/python/2.7.14/python-2.7.14.msi) et vérifier que pip est installé (Win + R -> cmd -> pip -V)
  (else use your favorite search engine on "install pip windows". For Windows, detailed description can be found [here](https://github.com/BurntSushi/nfldb/wiki/Python-&-pip-Windows-installation)).

5. Exécuter la commande suivante via Invite de commandes (Win + R -> cmd) pour télécharger le script de contrôle IoT Edge : pip install -U azure-iot-edge-runtime-ctl.

6. Installer Device Explorer Twin : https://github.com/Azure/azure-iot-sdk-csharp/releases.

**B - ON THE DATASCIENCE SIDE** : Développement et déploiement des modules contenant Azure Machine Learning en tant que modules IoT Edge

1. Installer [Machine Learning Workbench](https://aka.ms/azureml-wb-msi).

**C - ON THE DEVICE** :

- Install Python 2.7.x
- Install Docker