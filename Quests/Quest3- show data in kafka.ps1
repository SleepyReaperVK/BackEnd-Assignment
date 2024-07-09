docker exec -it kafka kafka-console-consumer --bootstrap-server kafka:9092 --topic my-topic
docker exec -it kafka kafka-console-consumer --bootstrap-server kafka:9092 --topic my-topic --from-beginning

docker exec -it kafka bash
cd /bin
kafka-console-consumer --bootstrap-server kafka:9092 --topic my-topic
kafka-console-consumer --bootstrap-server kafka:9092 --topic my-topic --from-beginning
