# Descrição
Este projeto é uma integração entre duas aplicações, uma em node e outra em dotnet que se comunicam através de filas rabbitMQ e base de dados PostgreSQL com toda a infra conteinerizada com Docker.

# Como rodar:
* Para rodar basta ter o Docker Desktop instalado em sua máquina. [Docker Desktop](https://www.docker.com/products/docker-desktop/)
* Buildar os containers (rabbitMQ, postgres, app-node, app-dotnet, app-python através do seguinte comando: `docker-compose up --build -d`
* Utilizar a chamada GET http://localhost:8000/run-tests para chamar o endpoint da aplicação "app-python" que roda um teste integrado para validar o fluxo das aplicações -> Realiza uma postagem rabbit com um objeto que será inserido e atualizado na base e após isso valida a inserção/atualização.

# Para parar:
* Rodar o comando `docker-compose down` ou `docker-compose down -v` para limpar as memórias do RabbitMQ e PostgreSQL.
