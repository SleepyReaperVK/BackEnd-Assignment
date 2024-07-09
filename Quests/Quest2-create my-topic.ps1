docker exec -it kafka kafka-topics --create --topic my-topic --bootstrap-server kafka:9092

docker exec -it kafka bash
cd /bin
kafka-console-producer --bootstrap-server kafka:9092 --topic my-topic