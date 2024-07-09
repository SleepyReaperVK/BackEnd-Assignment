docker exec -it mongodb mongosh --port 27017
use myDatabase
db.createCollection("Events")
#db.myCollection.find().pretty()