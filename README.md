# Back End Application

## Overview

This project implements 3 microservices: Kafka Producer, Kafka Consumer, and an ETL microservice using locally Dockerize Apache Kafka & Zookeeper, MongoDB, and Redis.

## Technology Stack

1. Apache Kafka
2. MongoDB
3. Redis
4. Docker and Docker Compose
5. Git

The application is primarily developed in C#/.NET.

## Architecture

1. The Kafka Producer will produce an Event every second to the Kafka broker.
2. The Kafka Consumer will poll from the broker, parse the Date, and insert it into MongoDB.
3. The ETL microservice will poll the lastTimestamp from Redis (if it exists), find it in MongoDB, and insert data from the last time till the latest into Redis.

## Known Issues    

10/7/2024
1. Relocate the seconds from the producer,ETL to .yaml. 
2. Make a Event factory in Producer, Event should be simple plain obj. Done
3. Make use of the cursor in MongoDB , get sorted events from MongoDB. 
4. test if the producer starts twice (up , stop , up) how will the new index will behave in mongo/redis.Done
5. update lastTimestamp when event inserted to redis.




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
- Entry point for the microservice, initializing necessary components and starting the ETL process.

### 2. MongoDBService.cs
- Handles interactions with MongoDB, including connection setup, data fetching based on timestamps, and index management.

### 3. RedisService.cs
- Manages Redis operations such as connection handling, retrieving and setting data (timestamps and ETL results).

### 4. ETLService.cs
- Orchestrates the ETL process:
  - Retrieves data from MongoDB based on the last processed timestamp stored in Redis.
  - Transfers data to Redis.
  - Updates the last processed timestamp in Redis.
