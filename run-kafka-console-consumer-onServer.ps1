docker exec -it kafka kafka-console-consumer --bootstrap-server kafka:9092 --topic my-topic
docker exec -it kafka bash
cd /bin
kafka-console-consumer --bootstrap-server localhost:9092 --topic my-topic