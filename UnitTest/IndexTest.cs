using System;
using System.Linq;
using Xunit;
using LiteDB;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;

namespace UnitTest
{
    
    public class IndexTest
    {
        private const string dbpath = @"C:\Temp\index.db";

        [TestInitialize]
        public void Init()
        {
            File.Delete(dbpath);
        }

        [Fact]
        public void Index_Insert()
        {
            using (var db = new LiteEngine(dbpath))
            {
                var c = db.GetCollection("col1");
                var d = new BsonDocument();

                var id1 = c.NextVal();
                var id2 = c.NextVal();
                var id3 = c.NextVal();

                d["Name"] = "John 1";
                c.Insert(id1, d);

                d["Name"] = "John 2";
                c.Insert(id2, d);

                d["Name"] = "John 3";
                c.Insert(id3, d);

                d["Name"] = "John A";
                c.Insert("A", d);

                var r = c.Find(Query.GTE("_id", 1));

                foreach (var nd in r)
                {
                    Debug.Print(nd["Name"].AsString);
                }



            }
        }

        [Fact]
        public void Index_Search()
        {
        }

        [Fact]
        public void Index_Delete()
        {
        }

    }
}
