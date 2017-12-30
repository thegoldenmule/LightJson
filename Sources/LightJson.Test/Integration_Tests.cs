﻿using NUnit.Framework;

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
    }
}