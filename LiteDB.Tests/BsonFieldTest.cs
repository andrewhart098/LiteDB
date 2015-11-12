using System.Collections.Generic;
using LiteDB;
using Xunit;

namespace UnitTest
{
    public class MyBsonFieldTestClass
    {
        [BsonField("MY-STRING")]
        public string MyString { get; set; }

        [BsonField]
        internal string MyInternalPropertySerializable { get; set; }

        [BsonField]
        private string MyPrivatePropertySerializable { get; set; }

        [BsonField]
        protected  string MyProtectedPropertySerializable { get; set; }

        [BsonField("INTERNAL-PROPERTY")]
        internal string MyInternalPropertyNamed { get; set; }

        [BsonField("PRIVATE-PROPERTY")]
        private string MyPrivatePropertyNamed { get; set; }

        [BsonField("PROTECTED-PROPERTY")]
        protected string MyProtectedPropertyNamed { get; set; }

        internal string MyInternalPropertyNotSerializable { get; set; }
        private string MyPrivatePropertyNotSerializable { get; set; }
        protected string MyProtectedPropertyNotSerializable { get; set; }

        public void SetPrivateProperties(string str)
        {
            MyPrivatePropertyNamed = str+"Named";
            MyPrivatePropertySerializable = str+"Serializable";
            MyPrivatePropertyNotSerializable = str+"NotSerialisable";
        }

        public void SetProtectedProperties(string str)
        {
            MyProtectedPropertyNamed = str + "Named";
            MyProtectedPropertySerializable = str + "Serializable";
            MyProtectedPropertyNotSerializable = str + "NotSerialisable";
        }

        public string GetMyPrivatePropertySerializable()
        {
            return MyPrivatePropertySerializable;
        }
        public string GetMyProtectedPropertySerializable()
        {
            return MyProtectedPropertySerializable;
        }
        public string GetMyPrivatePropertyNamed()
        {
            return MyPrivatePropertyNamed;
        }
        public string GetMyProtectedPropertyNamed()
        {
            return MyProtectedPropertyNamed;
        }
        public string GetMyPrivatePropertyNotSerializable()
        {
            return MyPrivatePropertyNotSerializable;
        }
        public string GetMyProtectedPropertyNotSerializable()
        {
            return MyProtectedPropertyNotSerializable;
        }


    }

    public class BsonFieldTest
    {
        private MyBsonFieldTestClass CreateModel()
        {
            var c = new MyBsonFieldTestClass
            {
                MyString = "MyString",
                MyInternalPropertyNamed = "InternalPropertyNamed",
                MyInternalPropertyNotSerializable = "InternalPropertyNotSerializable",
                MyInternalPropertySerializable = "InternalPropertySerializable",
 
            };

            c.SetProtectedProperties("ProtectedProperties");
            c.SetPrivateProperties("PrivateProperty");

            return c;
        }

        [Fact]
        public void BsonField_Test()
        {
            var mapper = new BsonMapper();
            mapper.UseLowerCaseDelimiter('_');

            var obj = CreateModel();
            var doc = mapper.ToDocument(obj);

            var json = JsonSerializer.Serialize(doc, true);
            var nobj = mapper.ToObject<MyBsonFieldTestClass>(doc);

            Assert.Equal(doc["MY-STRING"].AsString, obj.MyString);
            Assert.Equal(doc["INTERNAL-PROPERTY"].AsString, obj.MyInternalPropertyNamed);
            Assert.Equal(doc["PRIVATE-PROPERTY"].AsString, obj.GetMyPrivatePropertyNamed());
            Assert.Equal(doc["PROTECTED-PROPERTY"].AsString, obj.GetMyProtectedPropertyNamed());
            Assert.Equal(obj.MyString, nobj.MyString);
            //Internal
            Assert.Equal(obj.MyInternalPropertyNamed, nobj.MyInternalPropertyNamed);
            Assert.Equal(obj.MyInternalPropertySerializable, nobj.MyInternalPropertySerializable);
            Assert.Equal(nobj.MyInternalPropertyNotSerializable,null);
            //Private
            Assert.Equal(obj.GetMyPrivatePropertyNamed(), nobj.GetMyPrivatePropertyNamed());
            Assert.Equal(obj.GetMyPrivatePropertySerializable(), nobj.GetMyPrivatePropertySerializable());
            Assert.Equal(nobj.GetMyPrivatePropertyNotSerializable(), null);
            //protected
            Assert.Equal(obj.GetMyProtectedPropertyNamed(), nobj.GetMyProtectedPropertyNamed());
            Assert.Equal(obj.GetMyProtectedPropertySerializable(), nobj.GetMyProtectedPropertySerializable());
            Assert.Equal(nobj.GetMyProtectedPropertyNotSerializable(), null);

            


        }
    }
}