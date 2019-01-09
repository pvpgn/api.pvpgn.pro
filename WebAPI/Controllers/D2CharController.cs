using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using CharacterEditor;
using Newtonsoft.Json;
using WebAPI.D2Char;

namespace WebAPI.Controllers
{
    [Route("d2char")]
    [ApiController]
    public class D2CharController : ControllerBase
    {
        // POST /d2char
        /// <summary>
        /// Parse charinfo, charsave or charitem file and return object
        /// </summary>
        /// <param name="file"></param>
        /// <param name="type">charinfo | charsave | charitem</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult<Response> Post(IFormFile file, string type)
        {
            byte[] data;
            try
            {
                using (var fs = file.OpenReadStream())
                {
                    data = new byte[fs.Length];
                    fs.Read(data, 0, (int)fs.Length);
                }
            }
            catch (Exception e)
            {
                return new ErrorResponse("READ_ERROR", e.Message);
            }

            string errorMessage = "";
            try
            {
                switch (type)
                {
                    case null:
                        goto default;

                    case "charinfo":
                        var charinfo = new CharInfo();
                        charinfo.Read(data);
                        data = charinfo.GetBytes();
                        return new SuccessResponse(charinfo);
                        break;

                    case "charsave":
                        var charsave = new SaveReader("1.13c");
                        charsave.Read(data);
                        var charsaveResponse = new CharSaveResponse(charsave);
                        return new SuccessResponse(charsaveResponse);
                        break;

                    case "charitem":
                        var charitem = Item.NewItem(data);
                        var charitemResponse = new CharItemResponse(charitem);
                        return new SuccessResponse(charitemResponse);
                        break;

                    default:
                        return new ErrorResponse("PARAM_MISS", "Type parameter is empty (allowed: charinfo, charsave, charitem)");
                }
            }
            catch (EndOfStreamException e3)
            {
                return new ErrorResponse("D2GSNEWBIE", e3.Message);
            }
            catch (Exception e3)
            {
                errorMessage += "\n\n" + e3.ToString();
            }
            return new ErrorResponse("BAD_FILE", "Unsupported file format. Only charinfo, charsave and charitem are allowed.\n\n" + errorMessage);
        }

        // PUT /d2char
        /// <summary>
        /// Return file from object charinfo, charsave or charitem
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        [HttpPut()]
        public ActionResult<Response> Put([FromBody] object json)
        {
            var data = json.ToString();
            var response = JsonConvert.DeserializeObject<CharacterResponse>(data);
            try
            {
                if (response.FileType == "charsave")
                {
                    // get json into object
                    var charsaveResponse = JsonConvert.DeserializeObject<CharSaveResponse>(data);

                    // load character data
                    var charsave = new SaveReader("1.13c");
                    charsave.Read(charsaveResponse.Data);

                    // replace stats and items from response
                    charsave = charsaveResponse.GetChar(charsave);
                    var ms = new MemoryStream(charsave.GetBytes(true)); // true is important!
                    return File(ms, "application/octet-stream", charsave.Character.Name);
                }
                else if (response.FileType == "charitem")
                {
                    var charitemResponse = JsonConvert.DeserializeObject<CharItemResponse>(data);

                    // load item data
                    var charitem = Item.NewItem(charitemResponse.Data);

                    // replace item data from response
                    charitem = charitemResponse.GetItem(charitem);
                    var ms = new MemoryStream(charitem.GetItemBytes());
                    return File(ms, "application/octet-stream", charitemResponse.DisplayData.Title);
                }
                else if (response.FileType == "charinfo")
                {
                    var charinfo = JsonConvert.DeserializeObject<CharInfo>(data);
                    var ms = new MemoryStream(charinfo.GetBytes());
                    return File(ms, "application/octet-stream", charinfo.charName);
                }
            }
            catch (Exception e)
            {
                return new ErrorResponse("PARSE_ERROR", e.StackTrace);
            }
            return new ErrorResponse("BAD_DATA", "Unsupported data format. Use POST method to retrieve proper data structure.");
        }
    }
}
