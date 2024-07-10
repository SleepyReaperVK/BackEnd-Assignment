docker build -t kafka-consumer:latest .
#docker run -d --name kafkaC --network eyal_kafka-network --network eyal_mongo-network  kafka-consumer:latest .