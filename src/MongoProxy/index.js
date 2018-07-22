const express = require('express')
const MongoClient = require('mongodb').MongoClient
const url = 'mongodb://admin:sw8k83ng01bw5@vm-dev-cont2:27017'
const app = express()

const port = 3005;
app.listen(port)
console.log(`listening on ${port}`);
let bds = ['logs', 'models']
for (let bd of bds) {
  app.get(`/${bd}`, async (req, res) => {
    try {
      const db = await MongoClient.connect(url)
      const dbo = db.db('local')
      let objs;
      if (bd === 'logs') {
        objs = await dbo.collection(bd).find(null, {log: false}).sort({ createdAt: -1 }).limit(10).toArray()
      } else {
        objs = await dbo.collection(bd).find(null, {log: false}).toArray()
      }
      res.status(200)
      res.type('application/json')
      res.header('Access-Control-Allow-Origin', '*')
      console.log(`got ${objs.length} objs for ${bd}`)
      res.send(JSON.stringify(objs))
    }
    catch (e) {
      console.error(e)
      res.status(500)
    }
  })
  app.get(`/${bd}/:id`, async (req, res) => {
    try {
      const db = await MongoClient.connect(url)
      const dbo = db.db('local')
      let objs = await dbo.collection(bd).find({_id: req.params.id},).toArray()
      res.status(200)
      res.type('application/json')
      res.header('Access-Control-Allow-Origin', '*')
      res.send(JSON.stringify(objs))
    }
    catch (e) {
      console.error(e);
      res.status(500);
    }
  })
}
