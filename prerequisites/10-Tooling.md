# **~~~~ Prerequisites of tool installation ~~~~ **

<p><em>Authors : Artem SHEYKO + Taras CHIEN
Contributor : Vincent Thavonekham + Igor Leontiev</em></p>
<p><strong><em>A - ON THE  DATASCIENCE SIDE</em> : Développement et déploiement des modules contenant Azure Machine Learning en tant que modules IoT Edge</strong></p>
<ol>
<li>Installer Anaconda : <a href="https://conda.io/docs/user-guide/install/index.html">https://conda.io/docs/user-guide/install/index.html</a> et vérifier que pip est installé (Win + R -&gt; cmd -&gt; pip -V)</li>
(else use your favorite search engine on "install pip windows" (or linux! ). For Windows, it is here : 

<a href="https://github.com/BurntSushi/nfldb/wiki/Python-&-pip-Windows-installation"> 
https://github.com/BurntSushi/nfldb/wiki/Python-&-pip-Windows-installation  </a>

<li>Installer Machine Learning Workbench : <a href="https://aka.ms/azureml-wb-msi">https://aka.ms/azureml-wb-msi</a></li>
</ol>
<p><strong><em>B - ON THE DEVELOPPER SIDE</em> : Développement et déploiement des modules contenant la logique personnalisée et des services Azure</strong>*</p>
<p>Cette partie part du principe que le lecteur utilise un ordinateur exécutant Windows pour simuler un appareil Internet des Objets (IoT).</p>
<p>Pour commencer à travailler avec Azure IoT Edge et passer le processus de création des modules, il faut:</p>
<ol>
<li>
<p>Disposer d’un abonnement Azure. Consultez la rubrique Obtenir une version d’évaluation gratuite d’Azure : <a href="https://azure.microsoft.com/pricing/free-trial/">https://azure.microsoft.com/pricing/free-trial/</a></p>
</li>
<li>
<p>Installer Docker et s’assurer qu’il s’exécute correctement : <a href="https://docs.docker.com/docker-for-windows/install/">https://docs.docker.com/docker-for-windows/install/</a></p>
</li>
<li>
<p>Installer Visual Studio Code <a href="https://code.visualstudio.com/">https://code.visualstudio.com/</a>,</p>
<p>a.	l’extension Azure IoT Edge pour Visual Studio Code <a href="https://marketplace.visualstudio.com/items?itemName=vsciot-vscode.azure-iot-edge">https://marketplace.visualstudio.com/items?itemName=vsciot-vscode.azure-iot-edge</a>,</p>
<p>b. l’extension C# pour Visual Studio Code (développée par OmniSharp) <a href="https://marketplace.visualstudio.com/items?itemName=ms-vscode.csharp">https://marketplace.visualstudio.com/items?itemName=ms-vscode.csharp</a>,</p>
<p>c.	kit de développement logiciel (SDK) .NET Core 2.0 <a href="https://www.microsoft.com/net/core">https://www.microsoft.com/net/core</a></p>
</li>
<li>
<p>Exécuter la commande suivante via Invite de commandes (Win + R -&gt; cmd) pour télécharger le script de contrôle IoT Edge :
<em>pip install -U azure-iot-edge-runtime-ctl</em></p>
</li>
<li>
<p>Installer Device Explorer Twin : <a href="https://github.com/Azure/azure-iot-sdk-csharp/releases">https://github.com/Azure/azure-iot-sdk-csharp/releases</a></p>
</li>
</ol>

<p><strong><em>C - ON THE DEVICE</em> : </strong></p>
<li>Install Python 2.7.x</li>
<li>Install Docker</li>
