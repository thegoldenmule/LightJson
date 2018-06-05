using System;
using NUnit.Framework;

namespace LightJson.Test
{
    [TestFixture]
    public class Integration_Tests
    {
        [Test]
        public void Primitives()
        {
            var before = PrimitiveObject.Instance();

            var json = new JsonObject(before).ToString(true);

            var after = (PrimitiveObject) JsonValue.Parse(json).As(typeof(PrimitiveObject));

            Assert.IsTrue(PrimitiveObject.AreEqual(before, after));
        }

        [Test]
        public void PrimitiveDict()
        {
            var before = PrimitiveDictionaryObject.Instance();

            var json = new JsonObject(before).ToString();
            
            var after = (PrimitiveDictionaryObject) JsonValue.Parse(json).As(typeof(PrimitiveDictionaryObject));

            Assert.IsTrue(PrimitiveDictionaryObject.AreEqual(before, after));
        }

        [Test]
        public void PrimitiveArray()
        {
            var before = PrimitiveArrayObject.Instance();

            var json = new JsonObject(before).ToString();

            var after = (PrimitiveArrayObject) JsonValue.Parse(json).As(typeof(PrimitiveArrayObject));

            Assert.IsTrue(PrimitiveArrayObject.AreEqual(before, after));
        }

        [Test]
        public void Composite()
        {
            var before = CompositeObject.Instance();

            var json = new JsonObject(before).ToString();

            var after = (CompositeObject) JsonValue.Parse(json).As(typeof(CompositeObject));

            Assert.IsTrue(CompositeObject.AreEqual(before, after));
        }

        [Test]
        public void CompositeArray()
        {
            var before = CompositeArrayObject.Instance();

            var json = new JsonObject(before).ToString();

            var after = (CompositeArrayObject) JsonValue.Parse(json).As(typeof(CompositeArrayObject));

            Assert.IsTrue(CompositeArrayObject.AreEqual(before, after));
        }

        [Test]
        public void CompositeDict()
        {
            var before = CompositeDictionaryObject.Instance();

            var json = new JsonObject(before).ToString();
            
            var after = (CompositeDictionaryObject) JsonValue.Parse(json).As(typeof(CompositeDictionaryObject));

            Assert.IsTrue(CompositeDictionaryObject.AreEqual(before, after));
        }

        [Test]
        public void NamedField()
        {
            var before = new PrimitiveNamed();

            var json = new JsonObject(before).ToString();

            var after = (PrimitiveNamed) JsonValue.Parse(json).As(typeof(PrimitiveNamed));

            Assert.AreEqual(before.Foo, after.Foo);
            Assert.IsTrue(json.Contains("bar"));
            Assert.IsFalse(json.Contains("Foo"));
        }

        [Test]
        public void IgnoreField()
        {
            var before = new PrimitiveIgnored();

            var json = new JsonObject(before).ToString();

            Assert.IsTrue(json.Contains("Foo"));
            Assert.IsFalse(json.Contains("Bar"));
        }

        [Test]
        public void Dynamic()
        {
            var before = PrimitiveDynamic.Instance();

            var json = new JsonObject(before).ToString();
            
            var after = (PrimitiveDynamic) JsonValue.Parse(json).As(typeof(PrimitiveDynamic));

            Assert.AreEqual(before.DynamicByte, after.DynamicByte);
            Assert.AreEqual(before.DynamicShort, after.DynamicShort);
            Assert.AreEqual(before.DynamicInt, after.DynamicInt);
            Assert.AreEqual(before.DynamicLong, after.DynamicLong);
            Assert.IsTrue(Math.Abs((float) before.DynamicFloat - (float) after.DynamicFloat) < float.Epsilon);
            Assert.AreEqual(before.DynamicBool, after.DynamicBool);
            Assert.AreEqual(before.DynamicString, after.DynamicString);
        }

        [Test]
        public void DynamicNull()
        {
            var json = new JsonObject(new PrimitiveDynamic()).ToString();

            var after = (PrimitiveDynamic) JsonValue.Parse(json).As(typeof(PrimitiveDynamic));

            Assert.IsNull(after.DynamicByte);
        }
    }
}