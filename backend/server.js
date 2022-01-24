require('dotenv').config();
 
const jwt = require('jsonwebtoken');
const express = require('express');
const cors = require('cors');
const bodyParser = require('body-parser');
 
const app = express();
const port = process.env.PORT || 4000;

///////////////////////////////////////
<<<<<<< HEAD
const jsreport = require('jsreport')()

if (process.env.JSREPORT_CLI) {
  // export jsreport instance to make it possible to use jsreport-cli
  module.exports = jsreport
} else {
  jsreport.init().then(() => {
    // running
  }).catch((e) => {
    // error during startup
    console.error(e.stack)
    process.exit(1)
  })
}

///////////////////////////////////////
=======
>>>>>>> 443183f307e24a099bf2c14d3d3f283ac72ae89b
const WebSocket = require('ws')
const wss = new WebSocket.Server({port: 8080},()=>{
    console.log('server started')
})

wss.on('connection',(ws)=>{
    ws.on('message',(data)=>{
        console.log('data received: ' + data)
<<<<<<< HEAD
=======
        //const stringMessage = data.toString('utf-8').trim();
        //ws.send(JSON.stringify(stringMessage))
        //ws.send(data);
>>>>>>> 443183f307e24a099bf2c14d3d3f283ac72ae89b
        wss.broadcast(data);
    })
})

wss.broadcast = function broadcast(msg){
    wss.clients.forEach(function each(client){
      client.send(msg);
    });
  };

wss.on('listening',()=>{
    console.log('server is listening on port 8080')
}) 

///////////////////////////////////////

// enable CORS
app.use(cors());

// parse application/json
app.use(bodyParser.json());

// parse application/x-www-form-urlencoded
app.use(bodyParser.urlencoded({ extended: true }));
 
// database conection
const db = require("./models");

// For explotation. Database is not dropped.
db.sequelize.sync(); 

// Development only. Drops and re-sync db everytime the server starts.
// db.sequelize.sync({ force: true }).then(() => {
//   console.log("Drop and re-sync db.");
// });

//middleware that checks if JWT token exists and verifies it if it does exist.
//In all future routes, this helps to know if the request is authenticated or not.
app.use(function (req, res, next) {
  // check header or url parameters or post parameters for token
  var token = req.headers['authorization'];
  if (!token) return next(); //if no token, continue

  if(req.headers.authorization.indexOf('Basic ') === 0){
    // verify auth basic credentials
    const base64Credentials =  req.headers.authorization.split(' ')[1];
    const credentials = Buffer.from(base64Credentials, 'base64').toString('ascii');
    const [username, password] = credentials.split(':');

    req.body.username = username;
    req.body.password = password;

    return next();
  }

  token = token.replace('Bearer ', '');
  // .env should contain a line like JWT_SECRET=V3RY#1MP0RT@NT$3CR3T#
  jwt.verify(token, process.env.JWT_SECRET, function (err, user) {
    if (err) {
      return res.status(401).json({
        error: true,
        message: "Invalid user."
      });
    } else {
      req.user = user; //set the user to req so other routes can use it
      req.token = token;
      next();
    }
  });
});

require("./routes/user.routes")(app);
require("./routes/images.routes")(app);
require("./routes/texts.routes")(app);
require("./routes/reviews.routes")(app);

app.listen(port, () => {
  console.log('Server started on: ' + port);
});