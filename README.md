# Spatial Connect
The Spatial Connect system is comprised of three libraries:
* <a href="https://www.github.com/juwilliams/spatial-connect">Spatial Connect</a>
* <a href="https://www.github.com/juwilliams/spatial-connect-cli">Spatial Connect CLI</a>
* <a href="https://www.github.com/juwilliams/geoservices">GeoServices</a>

The system is a tool designed to help migrate data between systems, specifically <a href="">ArcGIS</a> and <a href="">WebEOC</a>.


## Requirements
* ArcObjects SDK v10+, available with an ArcGIS Engine license or EDN Subscription
* Desktop running Microsoft Windows 7 or greater
* The following ESRI ArcObjects SDK Binaries:
	- ESRI.ArcGIS.DataSourcesFile.dll
	- ESRI.ArcGIS.DataSourcesGDB.dall
	- ESRI.ArcGIS.Display.dll
	- ESRI.ArcGIS.GeoDatabase.dll
	- ESRI.ArcGIS.Geometry.dll
	- ESRI.ArcGIS.Server.dll
	- ESRI.ArcGIS.System.dll
	- ESRI.ArcGIS.SystemUI.dll
	- ESRI.ArcGIS.Version.dll

## Capabilities
* Data Archiving in flat file format (JSON)
* Bi-directional communication between systems via relationship system
* Easy setup
* ETL between systems with input/output fields and optional xsl transformation
* Support for broad range of data sources (XML, JSON, ArcGIS, WebEOC, Shapefiles)


## Getting Started
1. Clone the repositories mentioned above (Spatial Connect, Spatial Connect CLI, GeoServices)
2. Build all of the libraries from source
3. Open Spatial Connect and ensure that all library references are ok, include/update anything that is missing or broken 
	* During this step you will need to update the references for several ESRI.* libraries. These are installed with the ArcObjects SDK and can typically be found at this path on Windows 
	`C:\Program Files (x86)\ArcGIS\DeveloperKit10.4\DotNet\`
4. Use <a href="https://www.github.com/spatial-connect-cli">Spatial Connect CLI</a> to initialize a connect space and create a container
5. Rename the Example-App.config in the Spatial.Connect.Task project to App.config and populate the following:
	* app.settings > app-path : this should be the direct path to the directory created in step 4.
	* system.servicemodel > client > endpoint : replace the address attribute with the address of your instance of WebEOC API
6. Create two Windows Scheduled Tasks, one for push and one for pull.
	* Push and Pull tasks should reference the executable built from the Spatial.Connect.Task project. Additionally they should supply either 'push' or 'pull' as an input argument to indicate their purpose to the executable.
7. Run the schedule tasks. Notifications of service actions will be logged to log.txt inside the /log folder found inside the directory created in step 4.

## Alternative 'Getting Started'
Alternatively, you can simply clone <a href="https://github.com/juwilliams/spatial-connect/tree/master/SpatialConnect.Windows.Task/bin/Release">SpatialConnect Task Release</a> for the latest and drop your ESRI binaries into the folder (files mentioned in step 3 above). The required binaries are listed above under 'Requirements'.






