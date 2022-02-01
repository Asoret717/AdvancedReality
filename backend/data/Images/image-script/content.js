var mysql = require('mysql');

var con = mysql.createConnection({
    host: "localhost",
    user: "root",
    password: "",
    database: "db_galdar_dev",
    port: "3306",
});

function beforeRender(request, response, done) {
    con.connect(function(err) {
        if (err) throw err;
        con.query('select * from images', function(err, result, fields) {
            if (err) throw err;
            request.data = {
                image: result
            };
            done();
        });
    });
}