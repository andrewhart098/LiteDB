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
                Assert.IsFalse(db.CollectionExists("customerCollection"));
                var collection = db.GetCollection<Customer>("customerCollection");
                
                collection.Insert(new Customer());
                Assert.IsTrue(db.CollectionExists("customerCollection"));

                db.DropCollection("customerCollection");
                Assert.IsFalse(db.CollectionExists("customerCollection"));
            }
        }
    }
}
