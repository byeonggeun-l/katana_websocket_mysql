const WebSocket = require('ws') 
const wss= new WebSocket.Server({ port: 3000 },()=>{ 
    console.log('서버 시작') 
})

clients = {};
clientsName = [];

const mysql = require('mysql');
var mysqlconnection = mysql.createConnection({
      host     : 'localhost',
      user     : 'root',
      password : '2261bbs',
      database : 'katana'
});
mysqlconnection.connect();

wss.on('connection', function connection(ws) { 
      ws.on('message', (data) => {
            const obj = JSON.parse(data);
            console.log("obj.Type : " + obj.Type);

            if (obj.Type === "SingUp") {
                  mysqlconnection.query(`SELECT * FROM user_data where ID="${obj.ID}"`, function (error, results, fields) {
                        if (error) {
                              console.log(error);
                              return;
                        }
                        if (results == "") {
                              let sql = 'INSERT INTO `user_data`(`ID`, `PW`) VALUES (?,?)';
                              let params = [obj.ID, obj.PW];
                              mysqlconnection.query(sql, params, function (err, rows, fields) {
                                    if (err) {
                                          data = { Type: 'FailedJoin' };
                                          ws.send(JSON.stringify(data));
                                    } else {
                                          data = { Type: 'SuccessJoin' };
                                          ws.send(JSON.stringify(data));
                                    }
                              });
                        }
                        else {
                              console.log(results);
                              data = { Type: 'AlreadyExists' };
                              ws.send(JSON.stringify(data));
                        }
                  });
            }
            else if (obj.Type === "Login") {
                  mysqlconnection.query(`SELECT * FROM user_data where ID="${obj.ID}" AND PW="${obj.PW}"`, function (error, results, fields) {
                        if (error) {
                              console.log(error);
                              return;
                        }
                        if (results == "") {
                              data = { Type: 'FailedLogin' };
                              ws.send(JSON.stringify(data));
                        }
                        else {
                              console.log(JSON.stringify(results));
                              let json = JSON.stringify(results);
                              let parserJson = JSON.parse(json);
                              console.log(parserJson[0].EXP);
                              data = {
                                    Type: 'SuccessLogin',
                                    ID: obj.ID,
                                    Level: parserJson[0].Level,
                                    x: parserJson[0].Position_X,
                                    y: parserJson[0].Position_Y,
                              };
                              client = {
                                    ID: obj.ID,
                                    WS: ws,
                                    Level: parserJson[0].Level,
                                    x: parserJson[0].Position_X,
                                    y: parserJson[0].Position_Y,
                              };
                              ws.send(JSON.stringify(data));

                              let anotherPlayerJoindata = {
                                    Type: 'AnotherPlayerJoin',
                                    ID: obj.ID,
                                    Level: parserJson[0].Level,
                                    x: parserJson[0].Position_X,
                                    y: parserJson[0].Position_Y,
                              };

                              // 새로운 사람이 들어왔으면 기존 플레이어들에게 새로운 플레이어를
                              // 생성할 수 있게 한다.
                              for (let i = 0; i < clientsName.length; ++i) {
                                    clients[clientsName[i]].WS.send(JSON.stringify(anotherPlayerJoindata));
                              }

                              // 새롭게 들어간 플레이어의 클라이언트에도
                              // 기존 플레이어를 넣어준다.
                              for (let i = 0; i < clientsName.length; ++i) {
                                    let anotherAlreadyPlayerData =
                                    {
                                          Type: 'AnotherPlayerJoin',
                                          ID: clients[clientsName[i]].ID,
                                          Level: clients[clientsName[i]].Level,
                                          x: clients[clientsName[i]].x,
                                          y: clients[clientsName[i]].y,
                                    };
                                    ws.send(JSON.stringify(anotherAlreadyPlayerData));
                              }

                              // clients.push(client);
                              clients[obj.ID] = client;
                              clientsName.push(obj.ID);
                              
                        }
                  });
            }
            else if (obj.Type === "Move") {
                  data = {
                        Type: 'PlayerMove',
                        ID: obj.ID,
                        x: obj.x,
                        y: obj.y,
                  };
                  ws.send(JSON.stringify(data));
                  for (let i = 0; i < clientsName.length; ++i) {
                        if (clients[clientsName[i]].ID === obj.ID) {
                              continue;
                        }
                        clients[clientsName[i]].WS.send(JSON.stringify(data));
                  }
            }
            else if (obj.Type === "NewestPosition") {
                  clients[obj.ID].x = obj.x;
                  clients[obj.ID].y = obj.y;
                  mysqlconnection.query(`UPDATE user_data SET Position_X = ${obj.x} WHERE user_data . ID = "${obj.ID}";`, function (error, results, fields) {
                        if (error) {
                              console.log(error);
                              return;
                        }
                  });
            }
            else if (obj.Type === "Destroy") {
                  let i = 0;
                  for (let j = 0; j < clientsName.length; ++j) {
                        if (clientsName[j] === obj.ID) {
                              i = j;
                              break;
                        }
                  }
                  // 나갔으면 삭제시키자.
                  clientsName.splice(i, 1);
                  delete clients[obj.ID];

                  data = {
                        type: 'DeleteQuitPlayer',
                        ID: obj.ID,
                  }
                  for (let i = 0; i < clientsName.length; ++i) {
                        clients[clientsName[i]].WS.send(JSON.stringify(data));
                  }


            }
            else if (obj.Type === "ThrowApple") {
                  data = {
                        Type: 'ThrowApple',
                        ID: obj.ID,
                        x: obj.x,
                        y: obj.y,
                  };
                  ws.send(JSON.stringify(data));
                  for (let i = 0; i < clientsName.length; ++i) {
                        clients[clientsName[i]].WS.send(JSON.stringify(data));
                  }
            }
      })      

      ws.on('close', function() {
      });
})

wss.on('listening',()=>{ 
   console.log('리스닝 ...') 
})