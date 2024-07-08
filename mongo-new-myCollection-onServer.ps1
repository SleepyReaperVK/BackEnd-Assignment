docker exec -it mongodb mongosh --port 27017
use myDatabase
db.createCollection("myCollection")
#db.myCollection.find().pretty()