using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebAPI.BnetHash;

namespace WebAPI.Controllers
{
    [Route("bnethash")]
    public class BnetHashController : Controller
    {
        /// <summary>
        /// Return hash of a single password
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        // GET /bnethash/hello
        [HttpGet("{password}")]
        public Response Get(string password)
        {
            var hash = PvpgnHash.GetHash(password);
            return new SuccessResponse(hash);
        }

        /// <summary>
        /// Return hash of many passwords
        /// </summary>
        /// <param name="passwords"></param>
        /// <returns></returns>
        // POST /bnethash
        [HttpPost]
        public Response Post([FromBody]string[] passwords)
        {
            if (passwords.Length > 1000)
                return new ErrorResponse(ErrorCode.LIMIT_REACHED, "Max 1000 passwords are allowed per a request");
            var hashes = new string[passwords.Length];
            for (var i = 0; i < passwords.Length; i++)
                hashes[i] = PvpgnHash.GetHash(passwords[i]);
            return new SuccessResponse(hashes);
        }

        /// <summary>
        /// Return cracked password from xsha1 hash
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        // GET /bnethash/crack/7b74e0970b2e55e863a9a5f17ce74eaa7d6933e8
        [HttpGet("crack/{hash}")]
        public Response Crack(string hash)
        {
            if (hash.Length != 40)
                return new ErrorResponse(ErrorCode.BAD_DATA, "Invalid xsha1 hash");
            var xsha1 = new xsha1rev(); 
            var pass = xsha1.Crack16(hash);
            if (pass.Length == 0)
                return new ErrorResponse(ErrorCode.INTERNAL_ERROR, "Password is too strong for me");

            // FIXME: here we can crack up to 20 characters if 16 was not found, but it takes much cpu time
            //var pass = xsha1.Crack20(hash);

            return new SuccessResponse(pass);
        }
    }
}
