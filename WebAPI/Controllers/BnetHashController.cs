using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebAPI.BnetHash;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebAPI.Controllers
{
    [Route("api/bnethash")]
    public class BnetHashController : Controller
    {
        /// <summary>
        /// Return hash of a single password
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        // GET api/bnethash/hello
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
        // POST api/bnethash
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
