# Thankifi — Being Thankful as a Service
Thankifi is a free (as in speech) API that makes it easy for you to express gratitude. Be grateful!

Made with ❤️ and gratitude by [Lucas Maximiliano Marino](https://lucasmarino.me).

Check the [Open Api Documentation](https://api.thanki.fi). Find the docker image [here](https://hub.docker.com/r/thankifi/thankifi).

The dataset which the service uses te populate it's database is community driven an available at [this repository](https://github.com/thankifi/dataset).

# Running your own instance

## Two ways
You can either download this repo and start the server with `dotnet run -p src/Thankifi.Api/Thankifi.Api.csproj` or you can setup a docker installation.

## Requirements
Both ways have some basic requirements: 
- A PostgreSQL database.
- An env variable:
    + `DB_CONNECTION_STRING`, for the connection string to the Postgresql database

## Docker
The official [docker image](https://hub.docker.com/r/thankifi/thankifi) exposes port 5100.

## License
Thankifi — Being Thankful as a Service

Copyright (C) 2021  Lucas Maximiliano Marino <https://lucasmarino.me>

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <https://www.gnu.org/licenses/>.