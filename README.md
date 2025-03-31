# Descrição
Este projeto é uma integração entre duas aplicações, uma em node e outra em dotnet que se comunicam através de filas rabbitMQ e base de dados PostgreSQL e com toda a infra conteinerizada através das tecnologias de Docker e Docker-compose.

# Como rodar:
* Para rodar basta ter o Docker Desktop instalado em sua máquina. [Docker Desktop](https://www.docker.com/products/docker-desktop/)
* Buildar os containers (rabbitMQ, postgres, app-node, app-dotnet através do seguinte comando: `docker-compose up --build -d`

# Para parar:
* Rodar o comando `docker-compose down` ou `docker-compose down -v` para limpar as memórias do RabbitMQ e PostgreSQL.
