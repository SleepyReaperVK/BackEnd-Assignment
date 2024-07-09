# Back End Application

## Overview

This project implements 3 microservises kafkaProducer , kafka consumer and an ETL microservice using lockal dockerized Apache Kafka & zookeper, MongoDB, Redis.

## Technology Stack

1. Apache Kafka
2. MongoDB
3. Redis
4. Docker and Docker Compose
5. Git

The application is primarily developed in C#/.NET.

## Architecture
1.The kafka produser will spit out an Event each second to the kafka broker.
2.Then kafka consumer will poll from the broker , parse the Date and insert to Mongo.
3.Then ETL will poll the lastTimestamp from redis(if exist), will find it in mongo then insert from lastTime till the latest to redis.  

# Kafka Producer Application

### 1. Program.cs
- Entry point of the application.

### 2. ConfigLoader.cs
- Loads configuration settings from `config.yaml`.

### 3. KafkaProducer.cs
- Implements Kafka producer logic.

### 4. Event.cs
- Represents an event object.

# Kafka Consumer Application

### 1. Program.cs
- Entry point of the application.

### 2. ConfigurationHandler.cs
- Loads configuration settings from `config.yaml`.

### 3. KafkaConsumer.cs
- Implements Kafka consumer logic.

### 4. MongoDbHandler.cs
- Manages MongoDB connection and operations.

### 5. EventData.cs
- Represents an event data object.


# ETL Microservice Application

This project implements a microservice for ETL (Extract, Transform, Load) operations between MongoDB and Redis.

### 1. Program.cs

This file serves as the entry point for the microservice, initializing necessary components and starting the ETL process.

### 2. MongoDBService.cs

Handles interactions with MongoDB, including connection setup, data fetching based on timestamps, and index management.

### 3. RedisService.cs

Manages Redis operations such as connection handling, retrieving and setting data (timestamps and ETL results).

### 4. ETLService.cs

Orchestrates the ETL process:
- Retrieves data from MongoDB based on the last processed timestamp stored in Redis.
- Transfers data to Redis.
- Updates the last processed timestamp in Redis.

