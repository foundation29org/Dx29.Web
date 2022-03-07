<div style="margin-bottom: 1%; padding-bottom: 2%;">
	<img align="right" width="100px" src="https://dx29.ai/assets/img/logo-Dx29.png">
</div>

Dx29 Web
==============================================================================================================================================
[![Build Status](https://f29.visualstudio.com/Dx29%20v2/_apis/build/status/DEV-WEB/Dx29.Web?branchName=develop)](https://f29.visualstudio.com/Dx29%20v2/_build/latest?definitionId=104&branchName=develop)
[![Build Status](https://f29.visualstudio.com/Dx29%20v2/_apis/build/status/DEV-WEB/Dx29.Web.Management?branchName=develop)](https://f29.visualstudio.com/Dx29%20v2/_build/latest?definitionId=106&branchName=develop)
[![MIT license](https://img.shields.io/badge/license-MIT-brightgreen.svg)](http://opensource.org/licenses/MIT)

<p>&nbsp;</p>

### **Overview**

**What is Dx29?**

Dx29 is a non-profit project to help patients and physicians understand the patient disease. It is a completely free software for analysis and management of your symptoms, creation and sharing of your medical history and help to reach a diagnosis.

If you already have a diagnosis, you will be able to generate your medical history and also keep up to date with clinical trials and patient groups of your disease.

The data in Dx29 can only be shared by its owner. Foundation 29 will never share it with anyone. In the future, we will include functionality so that patients can store their own data in their own private data space.

<p>&nbsp;</p>

**What is Foundation 29?**

We are a non-profit foundation. We work for a medicine in which empowered patients take control of their health through their data and work with doctors and institutions to improve their well-being and that of others.

We create technologies that allow a radical change in your capabilities as users and in the entire medical ecosystem.

The activism of patients, their families and associations are at our core. We put special focus on rare diseases. They are in our origin and in our name: February 29, world day of rare diseases.

Visit our [website](https://foundation29.org/).

<p>&nbsp;</p>

**This project**

The arquitecture of this application is defined in [Dx29 architecture guide](https://dx29-v2.readthedocs.io/en/latest/index.html).

Dx29 uses a **microservices-based** architecture, so it consists of several connected containers. We have divided them according to their functionality and use within the platform.Thus, we will have:

>- Backend containers:
>>- Manage data containers
>>- Algorithms and functionalities containers
>>- Expose algorithms and functionalities container
>- Frontend containers:
>>- Webapp container
>- External containers

This project includes only the code of the **frontend container** of the application. However, in order to use it locally, it is necessary to deploy the backend containers. This will be explained in the Getting Started section of this guide.

In Dx29 application there are two user profiles: patient and physician. The functionalities of these two profiles will be practically the same, the only difference being that a physician can manage several patients with the application while patients will only be able to manage their own case.

Thus, the functionalities of the Dx29 application will be as follows:
>- Find and organize patient symptoms. Automatically find patient symptoms in his/her medical reports in virtually any language and organize patient history.
>- Seize patient little time with physicians. Arrange patient data chronologically to tell his/her medical history fast and center on what is important.
>- Share patient information. Share a case with a physician. If you are a Physician you can manage multiple cases from your patients panel.
>- Analise patient data. Use state-of-the-art algorithms to predict what rare disease fits patient data. Add genetic information, if available, for more accurate results.

We use [Blazor](https://dotnet.microsoft.com/en-us/apps/aspnet/web-apps/blazor) and [C# - ASP.NET](https://docs.microsoft.com/en-GB/visualstudio/get-started/csharp/tutorial-aspnet-core?view=vs-2022) as programming languages. 

>- src folder: This is made up of multiple folders which contains the source code of the projects: Dx29.Web & Dx29.Management
>- .gitignore file
>- README.md file
>- manifests folder: with the YAML configuration files for deploy in Azure Container Registry and Azure Kubernetes Service.
>- pipeline samples YAML file. For automatizate the tasks of build and deploy on Azure both projects that expose this one: Dx29.Web and Dx29.Management.

The Dx29 frontend has been implemented in the **Dx29.Web** project.

This project consists of several subprojects, mainly:
>- Dx29.Web.UI, includes all the elements to build the user interface of the application. It has been programmed using Blazor.
>- Dx29.Web. It can be considered as the server of the application, since it contains all the functions and methods necessary to offer the functionalities of Dx29 to the graphical view or client (Dx29.Web.UI). It has been implemented using C# - ASP.NET.
>- The two previous projects communicate with each other using the Dx29.Web.Client project. This includes the services necessary to expose the server functions to the client using a REST interface, that is, the communication is established according to the HTTP protocol.
>- As this project also communicates with the Dx29 Backend, the server must have access to the functionality exposed by the latter. For this purpose, the Dx29.Services project is used.
>- The Dx29.Data project will define all the models, extensions, resources and data needed by the previous ones.

Finally, the Dx29 and Dx29.Azure projects are also added. These two are generic and are used as a library to add the common or more general functionalities used in Dx29 projects programmed in C#.

The **Dx29.Management** will allow the use of methods in the Dx29 web application or the frontend (Dx29.Web) that require authentication and authorisation. For example, those related to the databases: Medical History. It will be used on demand, in order to be able to perform automated actions.

It shall not be exposed for use by third parties. It is an internal management tool for Fundation 29 programmers.

This project consists of several subprojects, mainly:
>- Dx29.WebManagement, it contains all the functions and methods necessary to offer the functionalities of Dx29. It has been implemented using C# - ASP.NET.
>- Finally, the Dx29.Data and Dx29.Azure projects are also added. These two are generic and are used as a library to add the common or more general functionalities used in Dx29 projects programmed in C#.


<p>&nbsp;</p>

### **Getting Started**

####  1. Configuration: Pre-requisites
Both projects must include a configuration file: appsettings.json. This includes the dependencies with other microservices:

|  Key           | Value     |		                                                         |
|----------------|-----------|---------------------------------------------------------------|
| FileStorage    | Endpoint  |http://dx29-filestorage/api/v1/                                |
| BioEntity      | Endpoint  |http://dx29-bioentity/api/v1/                                  |
| Localization   | Endpoint  |http://dx29-localization/api/v1/                               |
| TermSearch     | Endpoint  |http://dx29-termsearch2:8080/api/v1/                           |
| Diagnosis      | Endpoint  |http://dx29-bionet/api/v1/                                     |
| Management     | Endpoint  |http://dx29-medicalhistory/api/v1/                             |
| MedicalHistory | Endpoint  |http://dx29-medicalhistory/api/v1/                             |
| ResourceGroup  | Endpoint  |http://dx29-medicalhistory/api/v1/                             |
| Annotations    | Endpoint  |http://dx29-annotations/api/v1/                                |
| Exomiser       | Endpoint  |http://dx29-exomiser.northeurope.cloudapp.azure.com/api/v1/    |
| Documents      | Endpoint  |http://dx29-documents/api/v1/                                  |
| Mailing        | Endpoint  |http://dx29-mailing:8080/api/                                  |
| Legacy         | Endpoint  |http://dx29-legacy:8080/api/                                   |
| OpenDx29       | Endpoint  |https://dx29.ai/api                                            |
| PhenSimilarity | Endpoint  |http://dx29-phensimilarity:8080/api/v1/                        |

In addition, these projects access the user database (SQL) and the blob that openDx29 uses to exchange information with Dx29 v2. They also uses SignalR for notification management and AppInsights for logging. Therefore, in order to run it, the file appsettings.secrets.json must be added to the secrets folder with the following information:

|  Key                 | Value               |		                                                                                |
|----------------------|---------------------|--------------------------------------------------------------------------------------|
| ConnectionStrings    | IdentityConnection  |SQL database endpoint and credentials                                                 |
| ConnectionStrings    | OpenDataBlobStorage |OpenDx29 blob endpoint and credentials                                                |
| SignalR              | ConnectionString    |SignalR connection string & credentials                                               |
| SignalR              | HubName             |SignalR Hub HubName                                                                   |
| AppInsights          | Key                 |Secret key for connecting with AppInsights                                            |
| Account              | Key                 |Secret from SQL database (encrypt)                                                    |
| Account              | Inx                 |Secret from SQL database (encrypt)                                                    |
| Records              | Key                 |Secret from SQL database (encrypt)                                                    |
| Records              | Inx                 |Secret from SQL database (encrypt)                                                    |
| IdentityServer       | Clients             |"Dx29.Web.UI": {"Profile": "IdentityServerSPA"}                                       |
| IdentityServer       | Key                 |"Key": {"Type": "File","FilePath": Path certificate,"Password": Password certificate} |


<p>&nbsp;</p>

####  2. Download and installation

Download the repository code with `git clone` or use download button.

We use [Visual Studio 2019](https://docs.microsoft.com/en-GB/visualstudio/ide/quickstart-aspnet-core?view=vs-2022) for working with this project.

Backend microservices must be deployed for working with the application. Therefore, you need to have the Docker images of:
>- [Dx29.Annotations](TODO_LINK)
>- [Dx29.APIGateway](https://github.com/foundation29org/Dx29.APIGateway)
>- [Dx29.Bioentity](https://github.com/foundation29org/Dx29.Bioentity) 
>- [Dx29.BioNET](https://github.com/foundation29org/Dx29.BioNET)
>- [Dx29.Documents](https://github.com/foundation29org/Dx29.Documents)
>- [Dx29.FileStorage](https://github.com/foundation29org/Dx29.FileStorage)
>- [Dx29.Localization](https://github.com/foundation29org/Dx29.Localization) 
>- [Dx29.Mailing](https://github.com/foundation29org/Dx29.Mailing)
>- [Dx29.MedicalHistory](https://github.com/foundation29org/Dx29.MedicalHistory)
>- [Dx29.PhenSimmilarity](https://github.com/foundation29org/Dx29.PhenSimmilarity)
>- [Dx29.Segmentation](https://github.com/foundation29org/Dx29.Segmentation)
>- [Dx29.TermSearch](https://github.com/foundation29org/Dx29.TermSearch2)

You can use the [Dx29.Compose](https://github.com/foundation29org/Dx29.Compose) project to help you perform the tasks of:
>- Get the images of the environment you have previously created in Azure
>- Deploy the microservices Docker images (downloaded or compiled with the projects) needed with the configuration expected by Dx29.Web.

<p>&nbsp;</p>

####  3. Latest releases

The latest release of Dx29.Web project in the [Dx29 application](https://dx29.ai/) is: v0.15.02.

The latest release of Dx29.Management in the [Dx29 application](https://dx29.ai/) project is: v0.15.02.

<p>&nbsp;</p>

#### 4. API references

The methods outlined by the Dx29.Management project can be found at this [link](https://app.dx29.ai/management/index.html).

<p>&nbsp;</p>

### **Build and Test**

#### 1. Build

We could use Docker for build both images: Dx29.Web and Dx29.Management. 

Docker builds images automatically by reading the instructions from a Dockerfile – a text file that contains all commands, in order, needed to build a given image.

>- A Dockerfile adheres to a specific format and set of instructions.
>- A Docker image consists of read-only layers each of which represents a Dockerfile instruction. The layers are stacked and each one is a delta of the changes from the previous layer.

Consult the following links to work with Docker:

>- [Docker Documentation](https://docs.docker.com/reference/)
>- [Docker get-started guide](https://docs.docker.com/get-started/overview/)
>- [Docker Desktop](https://www.docker.com/products/docker-desktop)

The first step is to run docker image build. We pass in . as the only argument to specify that it should build using the current directory. This command looks for a Dockerfile in the current directory and attempts to build a docker image as described in the Dockerfile. 
```docker image build . ```

[Here](https://docs.docker.com/engine/reference/commandline/docker/) you can consult the Docker commands guide.

<p>&nbsp;</p>

#### 2. Deployment

To work locally, it is only necessary to install the project and build it using Visual Studio 2019. 

The deployment of these projects in an environment is described in [Dx29 architecture guide](https://dx29-v2.readthedocs.io/en/latest/index.html), in the deployment section. In particular, it describes the steps to execute to work with this project as a microservice (Docker image) available in a kubernetes cluster:

1. Create an Azure container Registry (ACR). A container registry allows you to store and manage container images across all types of Azure deployments. You deploy Docker images from a registry. Firstly, we need access to a registry that is accessible to the Azure Kubernetes Service (AKS) cluster we are creating. For this purpose, we will create an Azure Container Registry (ACR), where we will push images for deployment.
2. Create an Azure Kubernetes cluster (AKS) and configure it for using the prevouos ACR
3. Import image into Azure Container Registry
4. Publish the application with the YAML files that defines the deployment and the service configurations. 

These projects includes, YAML examples to perform the deployment tasks as a microservice in an AKS. 

Note that Dx29.Management service is configured as "ClusterIP" since it is not exposed externally in the [Dx29 application](https://dx29.ai/), but is internal for the application to use. If it is required to be visible there are two options:
>- The first, as realised in the Dx29 project an API is exposed that communicates to third parties with the microservice functionality.
>- The second option is to directly expose this microservice as a LoadBalancer and configure a public IP address and DNS.

**Interesting link**: [Deploy a Docker container app to Azure Kubernetes Service](https://docs.microsoft.com/en-GB/azure/devops/pipelines/apps/cd/deploy-aks?view=azure-devops&tabs=java)

<p>&nbsp;</p>


### **Contribute**

Please refer to each project's style and contribution guidelines for submitting patches and additions. The project uses [gitflow workflow](https://nvie.com/posts/a-successful-git-branching-model/). 
According to this it has implemented a branch-based system to work with three different environments. Thus, there are two permanent branches in the project:
>- The develop branch to work on the development environment.
>- The master branch to work on the test and production environments.

In general, we follow the "fork-and-pull" Git workflow.

>1. Fork the repo on GitHub
>2. Clone the project to your own machine
>3. Commit changes to your own branch
>4. Push your work back up to your fork
>5. Submit a Pull request so that we can review your changes

The project is licenced under the **(TODO: LICENCE & LINK & Brief explanation)**


### **Acknowledgements**

This repository includes resources from the Human Phenotype Ontology consortium [https://hpo.jax.org/](https://hpo.jax.org/). Refer to the following publication for up-to date information: S. Köhler et al., The Human Phenotype Ontology in 2021, Nucleic Acids Research, Volume 49, Issue D1, 8 January 2021, Pages D1207–D1217, [https://doi.org/10.1093/nar/gkaa1043](https://doi.org/10.1093/nar/gkaa1043)


<p>&nbsp;</p>
<p>&nbsp;</p>

<div style="border-top: 1px solid !important;
	padding-top: 1% !important;
    padding-right: 1% !important;
    padding-bottom: 0.1% !important;">
	<div align="right">
		<img width="150px" src="https://dx29.ai/assets/img/logo-foundation-twentynine-footer.png">
	</div>
	<div align="right" style="padding-top: 0.5% !important">
		<p align="right">	
			Copyright © 2020
			<a style="color:#009DA0" href="https://www.foundation29.org/" target="_blank"> Foundation29</a>
		</p>
	</div>
<div>
	
