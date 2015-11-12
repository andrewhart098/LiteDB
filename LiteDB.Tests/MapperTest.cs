using System;
using System.Linq;
using Xunit;
using LiteDB;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Collections.Specialized;
using System.Security;

namespace UnitTest
{
    public enum MyEnum { First, Second }

    public class MyClass
    {
        [BsonId(false)]
        public int MyId { get; set; }
        [BsonField("MY-STRING")]
        public string MyString { get; set; }
        public Guid MyGuid { get; set; }
        public DateTime MyDateTime { get; set; }
        public DateTime? MyDateTimeNullable { get; set; }
        public int? MyIntNullable { get; set; }
        public MyEnum MyEnumProp { get; set; }
        public char MyChar { get; set; }
        public byte MyByte { get; set; }

        [BsonIndex(ignoreCase: true)]
        public decimal MyDecimal { get; set; }
        public decimal? MyDecimalNullable { get; set; }

        [BsonIndex(true)]
        public Uri MyUri { get; set; }

        // serialize this properties
        [BsonField]
        internal string MyProperty { get; set; }

        // do not serialize this properties
        [BsonIgnore]
        public string MyIgnore { get; set; }
        public string MyReadOnly { get; private set; }
        public string MyWriteOnly { set; private get; }
        public string MyField = "DoNotSerializeThis";
        internal string MyInternalProperty { get; set; }

        // special types
        public NameValueCollection MyNameValueCollection { get; set; }

        // lists
        public string[] MyStringArray { get; set; }
        public List<string> MyStringList { get; set; }
        public Dictionary<int, string> MyDict { get; set; }

        // interfaces
        public IMyInterface MyInterface { get; set; }
        public List<IMyInterface> MyListInterface { get; set; }
        public IList<IMyInterface> MyIListInterface { get; set; }

        // objects
        public object MyObjectString { get; set; }
        public object MyObjectInt { get; set; }
        public object MyObjectImpl { get; set; }
        public List<object> MyObjectList { get; set; }
    }

    public interface IMyInterface
    {
        string Name { get; set; }
    }

    public class MyImpl : IMyInterface
    {
        public string Name { get; set; }
    }

    public class MyFluentEntity
    {
        public int MyPrimaryPk { get; set; }
        public string CustomName { get; set; }
        public bool DoNotIncludeMe { get; set; }
        public DateTime MyDateIndexed { get; set; }
    }

    public class MapperTest
    {
        private MyClass CreateModel()
        {
            var c = new MyClass
            {
                MyId = 123,
                MyString = "John",
                MyGuid = Guid.NewGuid(),
                MyDateTime = DateTime.Now,
                MyProperty = "SerializeTHIS",
                MyIgnore = "IgnoreTHIS",
                MyIntNullable = 999,
                MyStringList = new List<string>() { "String-1", "String-2" },
                MyWriteOnly = "write-only",
                MyInternalProperty = "internal-field",
                MyNameValueCollection = new NameValueCollection(),
                MyDict = new Dictionary<int, string>() { { 1, "Row1" }, { 2, "Row2" } },
                MyStringArray = new string[] { "One", "Two" },
                MyEnumProp = MyEnum.Second,
                MyChar = 'Y',
                MyUri = new Uri("http://www.numeria.com.br"),
                MyByte = 255,
                MyDecimal = 19.9m,
                MyDecimalNullable = 25.5m,

                MyInterface = new MyImpl { Name = "John" },
                MyListInterface = new List<IMyInterface>() { new MyImpl { Name = "John" } },
                MyIListInterface = new List<IMyInterface>() { new MyImpl { Name = "John" } },

                MyObjectString = "MyString",
                MyObjectInt = 123,
                MyObjectImpl = new MyImpl { Name = "John" },
                MyObjectList = new List<object>() { 1, "ola", new MyImpl { Name = "John" }, new Uri("http://www.cnn.com") }
            };

            c.MyNameValueCollection["key-1"] = "value-1";
            c.MyNameValueCollection["KeyNumber2"] = "value-2";

            return c;
        }


        [Fact]
        public void Mapper_Test()
        {
            var mapper = new BsonMapper();
            mapper.UseLowerCaseDelimiter('_');

            var obj = CreateModel();
            var doc = mapper.ToDocument(obj);

            var json = JsonSerializer.Serialize(doc, true);

            var nobj = mapper.ToObject<MyClass>(doc);

            // compare object to document
            Assert.Equal(doc["_id"].AsInt32, obj.MyId);
            Assert.Equal(doc["MY-STRING"].AsString, obj.MyString);
            Assert.Equal(doc["my_guid"].AsGuid, obj.MyGuid);

            // compare 2 objects
            Assert.Equal(obj.MyId, nobj.MyId);
            Assert.Equal(obj.MyString, nobj.MyString);
            Assert.Equal(obj.MyProperty, nobj.MyProperty);
            Assert.Equal(obj.MyGuid, nobj.MyGuid);
            Assert.Equal(obj.MyDateTime, nobj.MyDateTime);
            Assert.Equal(obj.MyDateTimeNullable, nobj.MyDateTimeNullable);
            Assert.Equal(obj.MyIntNullable, nobj.MyIntNullable);
            Assert.Equal(obj.MyEnumProp, nobj.MyEnumProp);
            Assert.Equal(obj.MyChar, nobj.MyChar);
            Assert.Equal(obj.MyByte, nobj.MyByte);
            Assert.Equal(obj.MyDecimal, nobj.MyDecimal);
            Assert.Equal(obj.MyUri, nobj.MyUri);
            Assert.Equal(obj.MyNameValueCollection["key-1"], nobj.MyNameValueCollection["key-1"]);
            Assert.Equal(obj.MyNameValueCollection["KeyNumber2"], nobj.MyNameValueCollection["KeyNumber2"]);


            // list
            Assert.Equal(obj.MyStringArray[0], nobj.MyStringArray[0]);
            Assert.Equal(obj.MyStringArray[1], nobj.MyStringArray[1]);
            Assert.Equal(obj.MyDict[2], nobj.MyDict[2]);

            // interfaces
            Assert.Equal(obj.MyInterface.Name, nobj.MyInterface.Name);
            Assert.Equal(obj.MyListInterface[0].Name, nobj.MyListInterface[0].Name);
            Assert.Equal(obj.MyIListInterface[0].Name, nobj.MyIListInterface[0].Name);

            // objects
            Assert.Equal(obj.MyObjectString, nobj.MyObjectString);
            Assert.Equal(obj.MyObjectInt, nobj.MyObjectInt);
            Assert.Equal((obj.MyObjectImpl as MyImpl).Name, (nobj.MyObjectImpl as MyImpl).Name);
            Assert.Equal(obj.MyObjectList[0], obj.MyObjectList[0]);
            Assert.Equal(obj.MyObjectList[1], obj.MyObjectList[1]);
            Assert.Equal(obj.MyObjectList[3], obj.MyObjectList[3]);

            Assert.Equal(nobj.MyInternalProperty, null);
        }


        [Fact]
        public void MapperEntityBuilder_Test()
        {
            var mapper = new BsonMapper();

            var obj = new MyFluentEntity { MyPrimaryPk = 1, CustomName = "John", DoNotIncludeMe = true, MyDateIndexed = DateTime.Now };

            mapper.Entity<MyFluentEntity>()
                .Key(x => x.MyPrimaryPk)
                .Map(x => x.CustomName, "custom_field_name")
                .Ignore(x => x.DoNotIncludeMe)
                .Index(x => x.MyDateIndexed, true);

            var doc = mapper.ToDocument(obj);

            var json = JsonSerializer.Serialize(doc, true);

            var nobj = mapper.ToObject<MyFluentEntity>(doc);

            // compare object to document
            Assert.Equal(doc["_id"].AsInt32, obj.MyPrimaryPk);
            Assert.Equal(doc["custom_field_name"].AsString, obj.CustomName);
            Assert.NotEqual(doc["DoNotIncludeMe"].AsBoolean, obj.DoNotIncludeMe);
        }
    }
}
