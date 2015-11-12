using LiteDB;
using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnitTest
{
    public class DropCollectionTest
    {
        [Fact]
        public void DropCollection_Test()
        {
            using (var db = new LiteDatabase(DB.Path()))
            {
                Assert.False(db.CollectionExists("customerCollection"));
                var collection = db.GetCollection<Customer>("customerCollection");
                
                collection.Insert(new Customer());
                Assert.True(db.CollectionExists("customerCollection"));

                db.DropCollection("customerCollection");
                Assert.False(db.CollectionExists("customerCollection"));
            }
        }
    }
}
