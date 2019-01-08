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
        public string Get(string password)
        {
            return PvpgnHash.GetHash(password);
        }

        /// <summary>
        /// Return hash of many passwords
        /// </summary>
        /// <param name="passwords"></param>
        /// <returns></returns>
        // POST /bnethash
        [HttpPost]
        public string[] Post([FromBody]string[] passwords)
        {
            var hashes = new string[passwords.Length];
            for (var i = 0; i < passwords.Length; i++)
                hashes[i] = PvpgnHash.GetHash(passwords[i]);
            return hashes;
        }
    }
}
