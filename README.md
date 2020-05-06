# SuperGamblino [![Chat](https://img.shields.io/badge/chat-on%20discord-7289da.svg)](https://discord.gg/fG3FJDW) [![Invite](https://img.shields.io/badge/invite-bot%20discord-7289da.svg)](https://discordapp.com/oauth2/authorize?client_id=688160933574475800&scope=bot&permissions=8) ![.NET Core](https://github.com/Emil8250/SuperGamblino/workflows/.NET%20Core/badge.svg)
This is a DSharpPlus open source discord gambling bot

## Setup
- Install .NET Core 3.1
- Clone this project
- Fill out MySQL connection properties in the config file (this will generate the first time the application runs) & the bot token.
- Run the project

Alternatively you can get the docker image from [DockerHub]( https://hub.docker.com/repository/docker/emil8250/supergamblino "DockerHub"). 

## Contribute
Contributing to this project is really simple. 
- First you head over to the [Issues page]( https://github.com/Emil8250/SuperGamblino/issues "Issues") and look for an issue you'd like to work on.
  - If you don't find an issue you'd like to work on, you can simply create one.
 - When you've found an issue to work on, you fork this repository, create your feature, fix your bug, or whatever the issue is. 
 - Remember to write on an issue of you're working on it.
 - Then you create a pull request, and if it's accepted it'll end up on the master branch.
 
 ## Discussion and question
 For easier discussion, questions and feedback, please join our [Discord](https://discord.gg/fG3FJDW "SuperGamblino Discord").

## Commands
 - help - The help command displays available commands, and gives detailed information about them. 
 - coinflip - This is a simple coinflip command which takes two arguments Head/Tail and the amount to bet.
 - cooldown - Simply displays the cooldown on your Hourly and Daily rewards.
 - credits - Shows you how many credits you currently have.
 - hourly - Gives you 20 credits, and is available once an hour.
 - daily - Gives you 500 credits, and is available once every day.
 - globaltop - shows you a global top10.
 - roulette - This is a classic roulette game, where you can bet on Black, Red, Green or a specific number.
 - history - Displays the recent games and the results here from.
 - profile - Shows a profile page
 - work - Gives you credits based on your job. Your job is based on your level.

## Configuration
To configure the bot you can either setup create config.json file or deliver correct environment variables to set everything up.

example config.json
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
			"Address": "127.0.0.1",
			"Port": 3306,
			"Name": "supergamblino",
			"Username": "<username to db>",
			"Password": "<password to db>"
		}	
	}
}

```

example bot-config.env
```dotenv
Settings__BotSettings__Token=<bot token>
Settings__BotSettings__TopGgToken=<top gg token>
Settings__BotSettings__Prefix=!
Settings__ColorSettings__Info=#439ff0
Settings__ColorSettings__Success=#4beb50
Settings__ColorSettings__Warning=#bf1004
Settings__DatabaseSettings__Address=172.17.0.1
Settings__DatabaseSettings__Port=3306
Settings__DatabaseSettings__Name=supergamblino
Settings__DatabaseSettings__Username=user
Settings__DatabaseSettings__Password=fish123
```

## When using docker compose use .env file below to set the database up

example db-configuration.env
```dotenv
MYSQL_ROOT_PASSWORD=samplePassword
MYSQL_DATABASE=supergamblino
MYSQL_USER=user
MYSQL_PASSWORD=fish123
```
