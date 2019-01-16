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
    }
}
