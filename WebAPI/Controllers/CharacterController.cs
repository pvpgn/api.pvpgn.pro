using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using CharacterEditor;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Newtonsoft.Json;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CharacterController : ControllerBase
    {
        // POST api/character
        /// <summary>
        /// Parse charsave or charinfo file
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult<Response> Post(IFormFile file)
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
            // 1) try charinfo
            try
            {
                var charinfo = new CharInfo();
                charinfo.Read(data);
                data = charinfo.GetBytes();
                return new SuccessResponse(charinfo);
            }
            catch (Exception e1)
            {
                errorMessage += e1.ToString();
                // 2) if charinfo failed then read charsave
                try
                {
                    var charsave = new SaveReader("1.13c");
                    charsave.Read(data);
                    var response = new CharSaveResponse(charsave);
                    return new SuccessResponse(response);
                }
                catch (EndOfStreamException e2)
                {
                    return new ErrorResponse("D2GSNEWBIE", e2.Message);
                }
                catch (Exception e2)
                {
                    errorMessage += "\n\n" + e2.ToString();
                }
            }
            return new ErrorResponse("BAD_FILE", "Unsupported file format. Only charsave or charinfo are allowed.\n\n" + errorMessage);
        }

        // PUT api/character
        /// <summary>
        /// Edit charsave or charinfo
        /// </summary>
        /// <param name="data"></param>
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
                    charsaveResponse.ReplaceData(charsave);

                    var ms = new MemoryStream(charsave.GetBytes(true)); // true is important!
                    return File(ms, "application/octet-stream", charsave.Character.Name);
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
