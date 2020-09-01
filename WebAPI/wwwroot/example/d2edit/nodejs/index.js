// npm i request fs
const request = require('request')
const fs = require('fs')

let url = 'http://:localhost:5000/'

function charSaveGet(file, output) {
    url +='d2char?type=charsave'
    let req = request.post({url, form}, function (err, resp, body) {
        if (err) {
          console.log('Error!');
        } else {
          console.log('Character Retrieved: ' + body);
          if(output){
            // This might prove useful to verify a character's file exists OR has specific data before committing changes to the file. 
              let mod = JSON.parse(body)
              // Since we're in a JSON object now(mod), a new parameter can be created/used here (from this function) to change character data before passing it into charSavePut();
              mod.data['idx'] = 0
              charSavePut(mod.data, output)
          }
        }
      });
    var form = req.form();
    form.append('file', fs.createReadStream(file));
 
    return req;
}

// This function's parameter, json, should be structured exactly as what charSaveGet() would return (JSON object)
function charSavePut(json, fileName) {
    json.data['idx'] = 0
    // If additional changes need to be made to the JSON object (json), do them here before .stringify() is applied
    json = JSON.stringify(json)
    const config = {
            url: url,
            headers: {
                "Content-Type": "application/json",
                "Accept": '*/*',
                "Accept-Encoding": "gzip, deflate"
            },
            body: json
    }
    url +='d2char'
    let req = request.put(config, function (err, resp, body) {
        if (err) {
            console.log('Error!', err);
        } else {
            console.log('Character Retrieved: ' + body);
        }
      });

    return req.pipe(fs.createWriteStream(fileName))
}
// charSaveGet(inputFile, outputFile)
//// inputFile/outputFile === directory+file of the character save.
//
// Passsing a second parameter into charSaveGet will invoke charSavePut and use its string as the output filename.
// This will not work unless you change the parameters below!!
charSaveGet('./inputFilePath+Name', './outputFilePath+Name')
