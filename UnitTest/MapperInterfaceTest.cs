﻿using System;
using System.Linq;
using Xunit;
using LiteDB;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Collections.Specialized;

namespace UnitTest
{

    
    public class MapperInterfaceTest
    {
        public interface IMyInterface
        {
            string Name { get; set; }
        }

        public class MyClassImpl : IMyInterface
        {
            public string Name { get; set; }
        }

        // using property as Interface
        public class MyClassWithInterface
        {
            public int Id { get; set; }
            public IMyInterface Impl { get; set; }
        }

        // using property as base class (object)
        public class MyClassWithObject
        {
            public int Id { get; set; }
            public object Impl { get; set; }
        }

        // using property as is
        public class MyClassWithClassName
        {
            public int Id { get; set; }
            public MyClassImpl Impl { get; set; }
        }

        [Fact]
        public void MapInterfaces_Test()
        {
            var mapper = new BsonMapper();

            var c1 = new MyClassWithInterface { Id = 1, Impl = new MyClassImpl { Name = "John Doe" } };
            var c2 = new MyClassWithObject { Id = 1, Impl = new MyClassImpl { Name = "John Doe" } };
            var c3 = new MyClassWithClassName { Id = 1, Impl = new MyClassImpl { Name = "John Doe" } };

            var bson1 = mapper.ToDocument(c1); // add _type in Impl property
            var bson2 = mapper.ToDocument(c2); // add _type in Impl property
            var bson3 = mapper.ToDocument(c3); // do not add _type in Impl property

            Assert.Equal("UnitTest.MapperInterfaceTest+MyClassImpl, UnitTest", bson1["Impl"].AsDocument["_type"].AsString);
            Assert.Equal("UnitTest.MapperInterfaceTest+MyClassImpl, UnitTest", bson2["Impl"].AsDocument["_type"].AsString);
            Assert.Equal(false, bson3["Impl"].AsDocument.ContainsKey("_type"));

            var k1 = mapper.ToObject<MyClassWithInterface>(bson1);
            var k2 = mapper.ToObject<MyClassWithObject>(bson2);
            var k3 = mapper.ToObject<MyClassWithClassName>(bson3);

            Assert.Equal(c1.Impl.Name, k1.Impl.Name);
            Assert.Equal((c2.Impl as MyClassImpl).Name, (k2.Impl as MyClassImpl).Name);
            Assert.Equal(c3.Impl.Name, k3.Impl.Name);
        }
    }
}
