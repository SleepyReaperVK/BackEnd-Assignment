docker exec -it kafka kafka-console-producer --bootstrap-server localhost:9092 --topic my-topic
docker exec -it kafka bash
cd /bin
kafka-console-producer --bootstrap-server localhost:9092 --topic my-topic