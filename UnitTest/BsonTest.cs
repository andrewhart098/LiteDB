using System;
using System.Linq;
using Xunit;
using LiteDB;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;

namespace UnitTest
{
    
    public class BsonTest
    {
        private BsonDocument CreateDoc()
        {
            // create same object, but using BsonDocument
            var doc = new BsonDocument();
            doc["_id"] = 123;
            doc["FirstString"] = "BEGIN this string \" has \" \t and this \f \n\r END";
            doc["CustomerId"] = Guid.NewGuid();
            doc["Date"] = DateTime.Now;
            doc["MyNull"] = null;
            doc["EmptyObj"] = new BsonDocument();
            doc["EmptyString"] = "";
            doc["maxDate"] = DateTime.MaxValue;
            doc["minDate"] = DateTime.MinValue;
            doc.Set("Customer.Address.Street", "Av. Cacapava");

            doc["Items"] = new BsonArray();

            doc["Items"].AsArray.Add(new BsonDocument());
            doc["Items"].AsArray[0].AsDocument["Qtd"] = 3;
            doc["Items"].AsArray[0].AsDocument["Description"] = "Big beer package";
            doc["Items"].AsArray[0].AsDocument["Unit"] = (double)10 / (double)3;

            doc["Items"].AsArray.Add("string-one");
            doc["Items"].AsArray.Add(null);
            doc["Items"].AsArray.Add(true);
            doc["Items"].AsArray.Add(DateTime.Now);

            return doc;
        }

        [Fact]
        public void Bson_Test()
        {
            var o = CreateDoc();

            var bson = BsonSerializer.Serialize(o);

            var json = JsonSerializer.Serialize(o);

            var d = BsonSerializer.Deserialize(bson);

            Assert.Equal(d["_id"], 123);
            Assert.Equal(d["_id"].AsInt64, o["_id"].AsInt64);

            Assert.Equal(o["FirstString"].AsString, d["FirstString"].AsString);
            Assert.Equal(o["Date"].AsDateTime.ToString(), d["Date"].AsDateTime.ToString());
            Assert.Equal(o["CustomerId"].AsGuid, d["CustomerId"].AsGuid);
            Assert.Equal(o["MyNull"].RawValue, d["MyNull"].RawValue);
            Assert.Equal(o["EmptyString"].AsString, d["EmptyString"].AsString);

            Assert.Equal(d["maxDate"].AsDateTime, DateTime.MaxValue);
            Assert.Equal(d["minDate"].AsDateTime, DateTime.MinValue);

            Assert.Equal(o["Items"].AsArray.Count, d["Items"].AsArray.Count);
            Assert.Equal(o["Items"].AsArray[0].AsDocument["Unit"].AsDouble, d["Items"].AsArray[0].AsDocument["Unit"].AsDouble);
            Assert.Equal(o["Items"].AsArray[4].AsDateTime.ToString(), d["Items"].AsArray[4].AsDateTime.ToString());
        }
    }
}
