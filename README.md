Parse
=

Project 1: Parse
==

An application to create .json files using data from .csv files or from keyboard.

Task
=====

Create an application to parse .csv files into .json files.  
* Firstly, the application will be running from command line, make sure to set shortcuts and aliases;  
* The app should be able to handle files as arguments;  
* Make it user-friendly with shortcuts;  
* Make it possible to pass data from keyboard;

Usage:
=====
The application can receive up to two arguments:
* No arguments - It reads data from keyboard, saves the results to the current working directory as "changelog.json".  

* One .json file (absolute path) - It reads and deserializes data from the file, then asks for data from keyboard; saves the results to the path specified as argument.  

* One .csv file (absolute path) - It looks for a .json file in the CURRENT WORKING DIRECTORY (changelog.json), if it exists, deserializes it (if not, creates a new one in the directory) and processes the data from the .csv file specified as argument; saves the results to the current working directory as "changelog.json".  

* One .json file and one .csv file (order sensitive, absolute paths) - It deserializes data from the .json file, processes the .csv file, combines the results and saves them to the .json file specified as argument.

Object structure:
* Id;  
* Description;  
* URL;  
* RequestId;  
* RequestUrl;  

Note: The objects are separated into two arrays: Defects(bugs) and UserStories.

Example for the .json file:
=====

```
{
    "Versions": [
        {
            "VersionId": "1.0.0",
            "Changelog": {
                "Defects": [
                    {
                        "Id": "",
                        "Description": "",
                        "URL": "",
                        "RequestId": "",
                        "RequestUrl": ""
                    }
                ],
                "UserStories": []
            }
        }
    ]
}
```

Project 2: DevReport
==

A script used for creating .xlsx files (reports) from .scv files.

Task
=====
Create a script to make reports out of given .csv files:
* The script will be run from command line, make sure to set shortcuts and aliases;
* The file will be passed as argument;

Description:
=====
* The script extracts the .csv file to tokens(fields);
* Certain fields are used (ex. Effort completed, Entity type etc.)
* Data is filtered by developer names (for statistic purposes)
