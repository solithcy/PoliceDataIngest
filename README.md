# Police Data Ingest

This project is the police and population data ingest system for [StreetSafe](https://streetsafe.space).

## Requirements
- .NET SDK 9.0
- A Postgres database

## Database Initialisation
To initialise your database, get the connection string to your database in .NET format, then run the following command:
```bash
$ dotnet ef database update --project PoliceDataIngest\PoliceDataIngest.csproj --startup-project PoliceDataIngest\PoliceDataIngest.csproj --context PoliceDataIngest.Context.PoliceDbContext --configuration Debug --no-build "20250820140926_population areas" --connection "[connection string]"
```

## Building
To build this project, run the following commands:
```bash
$ cd PoliceDataIngest
$ dotnet build
```
The build will now be available at `./PoliceDataIngest/bin/Debug/net9.0`.

## Ingesting data
To ingest police and population data you must first create an `appsettings.json` file in the CWD that you will execute the build in. It must follow this format:
```json
{
  "ConnectionStrings": {
    "Database": "[.NET connection URL]"
  }
}
```
Then, run the following:
```bash
$ ./PoliceDataIngest.exe --police-data --pop-data
```

This will download and parse the police dataset, add it to the database, then parsed the included population dataset and add it to the database.