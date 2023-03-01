# Prisma task

## Reqirements

* docker, docker-compose installed

## About

It's monolithic implementation of the scheme : 

client -> <shared bus (RabbitMQ)> <- listener -> repo

It will be better to rework it as microservices architecture! ;-)
