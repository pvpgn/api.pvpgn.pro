using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using WebAPI;
using WebAPI.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Moq;
using Newtonsoft.Json;
using Xunit;

namespace Tests
{
    public class CharacterUnitTest
    {
        private CharacterController _api;

        public CharacterUnitTest()
        {
            _api = new CharacterController();

            // set resource path for charsave editor
            CharacterEditor.Resources.CurrentDirectory = "../../../../CharacterEditor";
        }

        [Fact]
        public void CharInfoTest()
        {
            var dir = "../Tests/charinfo/";
            foreach (var f in Directory.GetFiles(dir))
            {
                var fileName = Path.Combine(CharacterEditor.Resources.CurrentDirectory, f);
                Debug.WriteLine(f);

                var fileMock = new Mock<IFormFile>();
                var fs = new FileStream(fileName, FileMode.Open);
                fileMock.Setup(_ => _.OpenReadStream()).Returns(fs);
                fileMock.Setup(_ => _.FileName).Returns(fileName);
                fileMock.Setup(_ => _.Length).Returns(fs.Length);

                // get json object
                var result = (Response) _api.Post(fileMock.Object).Value;
                Assert.Equal("success", result.Result);
                var postResult = (SuccessResponse) result;

                var obj = (CharInfo) postResult.Data;
                // make sure returned json data can be built to the same bytes sequence
                Assert.Equal(obj.Hash, Helper.MD5(obj.GetBytes()));

                // get bytes from json object
                var putResult = (FileStreamResult) _api.Put(JsonConvert.SerializeObject(obj)).Result;
                byte[] bytes = new byte[putResult.FileStream.Length];
                putResult.FileStream.Read(bytes, 0, (int) putResult.FileStream.Length);

                // compare returned bytes with original file
                Assert.Equal(Helper.MD5(bytes), Helper.MD5(File.ReadAllBytes(fileName)));
            }
        }

        [Fact]
        public void CharSaveTest()
        {
            var dir = "../Tests/charsave/";
            foreach (var f in Directory.GetFiles(dir))
            {
                var fileName = Path.Combine(CharacterEditor.Resources.CurrentDirectory, f);
                Debug.WriteLine(f);

                var fileMock = new Mock<IFormFile>();
                var fs = new FileStream(fileName, FileMode.Open);
                fileMock.Setup(_ => _.OpenReadStream()).Returns(fs);
                fileMock.Setup(_ => _.FileName).Returns(fileName);
                fileMock.Setup(_ => _.Length).Returns(fs.Length);

                // get json object
                var result = (Response)_api.Post(fileMock.Object).Value;
                if (result.Result == "error")
                {
                    // it should correct determine newbie files
                    Assert.Equal("D2GSNEWBIE", ((ErrorResponse)result).ErrorCode);
                    continue;
                }
                Assert.Equal("success", result.Result);

                var postResult = (SuccessResponse)result;
                var obj = (CharSaveResponse)postResult.Data;

                // get bytes from json object
                var putResult = (FileStreamResult)_api.Put(JsonConvert.SerializeObject(obj)).Result;
                byte[] bytes = new byte[putResult.FileStream.Length];
                putResult.FileStream.Read(bytes, 0, (int)putResult.FileStream.Length);

                // compare returned bytes with original file
                // (do not compare bytes here, because it will be incorrect if stat differs - it may differ often even with equal values)
                Assert.Equal(obj.HashWithOriginalStat, Helper.MD5(File.ReadAllBytes(fileName)));
                // here we just check that returned json data was correctly converted into bytes
                Assert.Equal(obj.Hash, Helper.MD5(bytes));
            }
        }
    }
}
