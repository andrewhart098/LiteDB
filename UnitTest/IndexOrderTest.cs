using System;
using System.Linq;
using Xunit;
using LiteDB;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;

namespace UnitTest
{
    
    public class IndexOrderTest
    {
        [Fact]
        public void Index_Order()
        {
            using (var db = new LiteDatabase(DB.Path()))
            {
                var col = db.GetCollection<BsonDocument>("order");

                col.Insert(new BsonDocument().Add("text", "D"));
                col.Insert(new BsonDocument().Add("text", "A"));
                col.Insert(new BsonDocument().Add("text", "E"));
                col.Insert(new BsonDocument().Add("text", "C"));
                col.Insert(new BsonDocument().Add("text", "B"));

                col.EnsureIndex("text");

                var asc = string.Join("",
                    col.Find(Query.All("text", Query.Ascending))
                    .Select(x => x["text"].AsString)
                    .ToArray());

                var desc = string.Join("",
                    col.Find(Query.All("text", Query.Descending))
                    .Select(x => x["text"].AsString)
                    .ToArray());

                Assert.Equal(asc, "ABCDE");
                Assert.Equal(desc, "EDCBA");
            }
        }
    }
}
