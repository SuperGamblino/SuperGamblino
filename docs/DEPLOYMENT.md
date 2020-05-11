[Back to README](../README.md)
## Deployment
### Using docker-compose to host with MySql server
![Deployment gif](media/deployment.gif)
1. Create `docker-compose.yml` file with following content:
    ```yaml
    version: '3.8'
    services:
      database:
        image: "mysql"
        environment:
          - MYSQL_ROOT_PASSWORD=<database root password>
          - MYSQL_DATABASE=<database name>
          - MYSQL_USER=<database username>
          - MYSQL_PASSWORD=<database password>
        ports:
        - 3306:3306
      bot:
        image: "emil8250/supergamblino"
        environment:
          - Settings__BotSettings__Token=<bot token>
          - Settings__BotSettings__TopGgToken=<top gg token>
          - Settings__BotSettings__Prefix=!
          - Settings__ColorSettings__Info=#439ff0
          - Settings__ColorSettings__Warning=#bf1004
          - Settings__DatabaseSettings__Address=172.17.0.1
          - Settings__DatabaseSettings__Port=3306
          - Settings__DatabaseSettings__Name=<database name>
          - Settings__DatabaseSettings__Username=<database username>
          - Settings__DatabaseSettings__Password=<database password>
    ```
2. Install [`docker-compose`](https://docs.docker.com/compose/install/)
3. Run bot by going into `docker-compose.yml` location and running:
    ```sh
   $ docker-compose up
    ```
### Using docker-compose to host bot from source files with MySql server
1. Download latest source files:
    ```sh
   $ git clone https://github.com/Emil8250/SuperGamblino
   $ cd SuperGamblino
    ```
2. Create `docker-compose.yml` file with following content:
    ```yaml
    version: '3.8'
        services:
          database:
            image: "mysql"
            environment:
              - MYSQL_ROOT_PASSWORD=<database root password>
              - MYSQL_DATABASE=<database name>
              - MYSQL_USER=<database username>
              - MYSQL_PASSWORD=<database password>
            ports:
            - 3306:3306
          bot:
            build: .
            environment:
              - Settings__BotSettings__Token=<bot token>
              - Settings__BotSettings__TopGgToken=<top gg token>
              - Settings__BotSettings__Prefix=!
              - Settings__ColorSettings__Info=#439ff0
              - Settings__ColorSettings__Warning=#bf1004
              - Settings__DatabaseSettings__Address=172.17.0.1
              - Settings__DatabaseSettings__Port=3306
              - Settings__DatabaseSettings__Name=<database name>
              - Settings__DatabaseSettings__Username=<database username>
              - Settings__DatabaseSettings__Password=<database password>
    ```
3. Install [`docker-compose`](https://docs.docker.com/compose/install/)
4. Run bot by going into `docker-compose.yml` location and running:
    ```sh
   $ docker-compose up --build
    ``` 
### Hosting bot with dotnet-sdk
1. Setup MySql server
2. Download latest source files:
    ```sh
   $ git clone https://github.com/Emil8250/SuperGamblino
   $ cd SuperGamblino
    ```
 4. Install [`dotnet-sdk`](https://dotnet.microsoft.com/download)
 5. Run bot for the first time:
    ```sh
    $ dotnet run -c Release --project SuperGamblino
    ```
 6. Wait for program to exit and head into `SuperGamblino/bin/Release/netcoreapp*/`
 7. Create `config.json` file with following content:
     ```json
     {
     	"Settings" : {
     		"BotSettings": {
     			"Token": "<bot token>",
                 "TopGgToken": "<top gg token>",
     			"Prefix": "!"
     		},
     		"ColorSettings": {
     			"Info": "#439ff0",
     			"Success": "#4beb50",
     			"Warning": "#bf1004"
     		},
     		"DatabaseSettings": {
     			"Address": "<database address>",
     			"Port": 3306,
     			"Name": "<database name>",
     			"Username": "<database username>",
     			"Password": "<database password>"
     		}	
     	}
     }
     ```
 8. Run the bot with:
    ```sh
    $ cd ../../..
    $ dotnet run -c Release --project SuperGamblino
     ```
### More tips about hosting
[Docker-Compose docs](https://docs.docker.com/compose/)

[![Made by KanarekLife](https://img.shields.io/badge/MadeBy-KanarekLife-yellow?style=for-the-badge)](https://github.com/KanarekLife)