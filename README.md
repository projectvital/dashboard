# dashboard
[![VITAL Logo](https://github.com/projectvital/dashboard/raw/master/readme-logo-vital.png)](http://www.project-vital.eu)

Learning Analytics is a new research domain that consists in collecting, measuring, analysing and reporting data about online learning in order to better understand and optimise learning and the environment in which it occurs.

The VITAL dashboards consist of a selection of graphs and overviews for on the one hand students and the other hand instructors allowing them to understand how they learn online but also to compare their profile to user patterns of their peers. Educators get dynamic and real-time overviews of how their students are progressing, which students might be at risk of dropping out or of failing for the course and which parts of the courses cause difficulties/require more feedback.
The project also aims to develop a generic and reusable model for implementing learning analytics in language learning and teaching practises.

Components
==========
A SQL Server database can be created using the script in the Database subfolder of the repository.

The following image shows the infrastructure of the project:
[![Infrastructure](https://github.com/projectvital/dashboard/raw/master/readme-infrastructure.png)]( https://github.com/projectvital/dashboard/blob/master/Dashboard%20backend%20infrastructure.pdf)

The framework itself is divided into 3 Visual Studio Solutions, handling the `Data Preparation` and `Data Visualisation` parts.

Data Preparation - Daemon
-------------------------
The Daemon is used to populate the SQL Server database with data that was logged to an LRS. This task can be scheduled to run every now and then (i.e. Windows Task Scheduler). The frequency depends on the desired level of up-to-dateness. 
Note: During the project only complete datasets (from date A to B) were pulled from the LRS. Incremental pulls are not yet possible.

The Daemon can be configured using the parameters in its `App.config`.
The default DaemonMode is set to `pull`, which will download the specified dataset to files.
Those files can then be imported into the database using DaemonMode `import_json`.

Data Preparation - WCF
----------------------
Once the statements are in the database, you can use the restful WCF API to provide/retrieve data to/from consumers.
Consumers need to log in to get a valid SessionToken from the AccountController. That token must be provided in every following communication.
CORS is enabled by default, which can be configured in `App_Start\WebApiConfig.cs`.
The WCF module must be configured by adding the database connection string to the `Web.config` with name `LMGAnalyticsContext`.

Data Visualisation - Dashboards
-------------------------------
The dashboard website uses [D3](https://github.com/d3/d3) v4 to visualize most data.
To configure this project a reference to the WCF `ApiUrl` must be added to `Web.config`.

Notes
=====
Recommended software requirements
---------------------------------
Visual Studio 2012
SQL Server 2014
XAPI version 1.0.2+

Disclaimer
----------
![Erasmus+ Logo](https://github.com/projectvital/dashboard/raw/master/readme-logo-erasmus%2B.jpg)


This project has been funded with support from the European Commission (Project number: 2015-
BE02-KA203- 012317). The information in this research reflects the views only of the authors, and the
Commission cannot be held responsible for any use which may be made of the information
contained therein.

More info &amp; project deliverables:
www.project-vital.eu


