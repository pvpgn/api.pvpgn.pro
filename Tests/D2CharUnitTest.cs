using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using CharacterEditor;
using WebAPI;
using WebAPI.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Moq;
using Newtonsoft.Json;
using WebAPI.D2Char;
using Xunit;

namespace Tests
{
    public class D2CharUnitTest
    {
        private D2CharController _api;

        public D2CharUnitTest()
        {
            _api = new D2CharController();

            // set resource path for charsave editor
            CharacterEditor.Resources.CurrentDirectory = "../../../../CharacterEditor";
            new SaveReader("1.13c"); // load resources
        }

        [Fact]
        public void CharInfoTest()
        {
            var dir = "../Tests/charinfo/";
            foreach (var f in getFilesDirAndSubdirs(dir))
            {
                var fileName = Path.Combine(CharacterEditor.Resources.CurrentDirectory, f);
                var fileMock = getMockFIle(fileName);
                // get json object
                var result = (Response) _api.Post(fileMock.Object, "charinfo").Value;
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
            foreach (var f in getFilesDirAndSubdirs(dir))
            {
                var fileName = Path.Combine(CharacterEditor.Resources.CurrentDirectory, f);
                var fileMock = getMockFIle(fileName);
                // get json object
                var result = (Response)_api.Post(fileMock.Object, "charsave").Value;
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

        [Fact]
        public void CharItemTest()
        {
            var dir = "../Tests/charitem/";
            foreach (var f in getFilesDirAndSubdirs(dir).Where(s => !s.EndsWith(".gif")))
            {
                var fileName = Path.Combine(CharacterEditor.Resources.CurrentDirectory, f);
                var fileMock = getMockFIle(fileName);
                // get json object
                var result = (Response)_api.Post(fileMock.Object, "charitem").Value;
                Assert.Equal("success", result.Result);

                var postResult = (SuccessResponse)result;
                var obj = (CharItemResponse)postResult.Data;

                // get bytes from json object
                var putResult = (FileStreamResult)_api.Put(JsonConvert.SerializeObject(obj)).Result;
                byte[] bytes = new byte[putResult.FileStream.Length];
                putResult.FileStream.Read(bytes, 0, (int)putResult.FileStream.Length);

                // compare returned bytes with original file
                Assert.Equal(Helper.MD5(bytes), Helper.MD5(File.ReadAllBytes(fileName)));

                // save output gif for debug
                //File.Copy("../WebAPI/wwwroot/" + obj.DisplayData.ImagePath, dir + Path.GetFileNameWithoutExtension(fileName) + ".gif", true);
            }
        }

        [Fact]
        public void TblReadTest()
        {
            var tbl = new TblReader("string.tbl");
            Assert.StartsWith("45\nGreetings", tbl.FindString("WarrivAct1IntroGossip1"));
            Assert.Equal("Gothic Staff", tbl.FindString("8bs"));
            Assert.Equal("End of Beta", tbl.FindString("Endthispuppy"));
        }



        private Mock<IFormFile> getMockFIle(string fileName)
        {
            Debug.WriteLine(fileName);

            var fileMock = new Mock<IFormFile>();
            var fs = new FileStream(fileName, FileMode.Open);
            fileMock.Setup(_ => _.OpenReadStream()).Returns(fs);
            fileMock.Setup(_ => _.FileName).Returns(fileName);
            fileMock.Setup(_ => _.Length).Returns(fs.Length);
            return fileMock;
        }

        private List<string> getFilesDirAndSubdirs(string dirPath)
        {
            var files = Directory.GetFiles(dirPath).ToList();
            foreach (var d in Directory.GetDirectories(dirPath))
            {
                files.AddRange(getFilesDirAndSubdirs(d));
            }
            return files;
        }
    }
}
