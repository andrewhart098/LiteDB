using System;
using System.Linq;
using Xunit;
using LiteDB;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;

namespace UnitTest
{
    
    public class ObjectIdTest
    {
        [Fact]
        public void ObjectId_Test()
        {
            var oid0 = ObjectId.Empty;
            var oid1 = ObjectId.NewObjectId();
            var oid2 = ObjectId.NewObjectId();
            var oid3 = ObjectId.NewObjectId();

            var c1 = new ObjectId(oid1);
            var c2 = new ObjectId(oid2.ToString());
            var c3 = new ObjectId(oid3.ToByteArray());

            Assert.Equal(oid0, ObjectId.Empty);
            Assert.Equal(oid1, c1);
            Assert.Equal(oid2, c2);
            Assert.Equal(oid3, c3);

            Assert.Equal(c1.CompareTo(c2), -1); // 1 < 2
            Assert.Equal(c2.CompareTo(c3), -1); // 2 < 3

            // serializations
            var joid = JsonSerializer.Serialize(c1, true);
            var jc1 = JsonSerializer.Deserialize(joid).AsObjectId;

            Assert.Equal(c1, jc1);
        }
    }
}
