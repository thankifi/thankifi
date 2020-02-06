# TaaS — Thanks as a Service
TaaS is a public API that makes it easy for you to express gratitude. Be grateful!

Made with ❤️ and gratitude by [Lucas Maximiliano Marino](https://lucasmarino.me).

Check the [Open Api Documentation](https://api.taas.space).

The dataset which the service uses te populate it's database is community driven an available at [this repository](https://github.com/elementh/taas.data).

# Running your own instance

## Two ways
You can either download this repo and start the server with `dotnet` or you can setup a docker installation.

## Requirements
Both ways have some basic requirements: 
- A required Postgresql database.
- A required env variables:
    + `DB_CONNECTION_STRING`, for the connection string to the Postgresql database
- Some optional env variables:
    + `IMPORTER_SOURCE_URL`, for using your own data source.
        - Defaults to `https://raw.githubusercontent.com/elementh/taas.data/master/src`
    + `IMPORTER_CRON_CONFIGURATION`, for custom update intervals of the database.
        - Defaults to `0 0 1 * *` which means it will check for updates at midnight, on day 1 of every month.
        - For format specification read the [cronos docs](https://github.com/HangfireIO/Cronos#cron-format).

## Docker example
The official docker image exposes port 5100.

A simple, working docker compose example:

```yaml
taas:
    image: elementh/taas:0.1.1
    restart: unless-stopped
    container_name: taas
    environment:
        DB_CONNECTION_STRING: "Server=taas_database;Port=5432;Database=taas;User Id=taas;Password=taas;"
    ports:
        - 5100:5100 # This is only needed if you don't use traefik or something like that.

taas_database:
    image: postgres:12.1-alpine   
    restart: unless-stopped
    container_name: taas_database
    environment:
        POSTGRES_USER: "taas"
        POSTGRES_PASSWORD: "taas"
    volumes:
        - ./db-taas:/var/lib/postgresql/data
```

# License
TaaS — Thanks as a Service

Copyright (C) 2020  Lucas Maximiliano Marino <https://lucasmarino.me>

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