﻿using System;
using System.Linq;
using Xunit;
using LiteDB;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;

namespace UnitTest
{
    
    public class BigFileTest
    {
        //[Fact]
        public void BigFile_Test()
        {
            var fileSize = 8L * 1024L * 1024L * 1024L; // 5Gb
            var filename = "C:/Github/LiteDB/TestResults/test-4gb.db"; // DB.Path();

            //File.Delete(filename);

            while (GetFileSize(filename) < fileSize)
            {
                using (var db = new LiteDatabase("journal=false;filename="+filename))
                {
                    var col = db.GetCollection("col1");

                    col.InsertBulk(GetDocs());
                }
            }
        }

        private IEnumerable<BsonDocument> GetDocs()
        {
            for (var i = 0; i < 200000; i++)
            {
                var doc = new BsonDocument()
                    .Add("_id", Guid.NewGuid())
                    .Add("content", DB.LoremIpsum(50, 150, 3, 5, 2));

                yield return doc;
            }
        }

        private long GetFileSize(string filename)
        {
            return File.Exists(filename) ? new FileInfo(filename).Length : 0L;
        }
    }
}
