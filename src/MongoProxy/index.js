const express = require('express');
const MongoClient = require('mongodb').MongoClient
const url = 'mongodb://admin:sw8k83ng01bw5@vm-dev-cont2:27017'
const app = express();

app.listen(3005);
app.get('/logs', async (req, res) => {
  try {
    const db = await MongoClient.connect(url);
    const dbo = db.db("local");
    const logs = await dbo.collection('logs').find().sort({createdAt: -1}).limit(10).toArray();
    res.status(200);
    res.type('application/json');
    res.header('Access-Control-Allow-Origin', '*')
    console.log(`got ${logs.length} logs`);
    res.send(JSON.stringify(logs))
  }
  catch(e) {
    console.error(e);
    res.status(500)
  }
})