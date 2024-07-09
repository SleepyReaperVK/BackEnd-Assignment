docker exec -it mongodb mongosh --port 27017

#use myDatabase
#db.Events.find().pretty()
#db.Events.find().sort({ Timestamp: -1 }).limit(10).pretty()
#db.Events.find().sort({ _id: -1 }).limit(10).pretty()