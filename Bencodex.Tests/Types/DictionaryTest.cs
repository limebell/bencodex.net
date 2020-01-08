using System;
using System.Collections.Generic;
using Bencodex.Types;
using Xunit;

namespace Bencodex.Tests.Types
{
    public class DictionaryTest
    {
        [Fact]
        public void Equality()
        {
            var a = new Dictionary(new KeyValuePair<IKey, IValue>[]
            {
                new KeyValuePair<IKey, IValue>(
                    new Binary(new byte[] { 0x61 }),  // 'a'
                    new Integer(1)
                ),
                new KeyValuePair<IKey, IValue>(
                    new Binary(new byte[] { 0x62 }),  // 'b'
                    new Integer(2)
                ),
                new KeyValuePair<IKey, IValue>(
                    new Binary(new byte[] { 0x63 }),  // 'c'
                    new Integer(3)
                ),
            });
            Assert.Equal(a, a);

            var a2 = new Dictionary(new KeyValuePair<IKey, IValue>[]
            {
                new KeyValuePair<IKey, IValue>(
                    new Binary(new byte[] { 0x62 }),  // 'b'
                    new Integer(2)
                ),
                new KeyValuePair<IKey, IValue>(
                    new Binary(new byte[] { 0x63 }),  // 'c'
                    new Integer(3)
                ),
                new KeyValuePair<IKey, IValue>(
                    new Binary(new byte[] { 0x61 }),  // 'a'
                    new Integer(1)
                ),
            });
            Assert.Equal(a, a2);

            var b = new Dictionary(new KeyValuePair<IKey, IValue>[]
            {
                new KeyValuePair<IKey, IValue>(
                    new Binary(new byte[] { 0x61 }),  // 'a'
                    new Integer(2) // diffrent
                ),
                new KeyValuePair<IKey, IValue>(
                    new Binary(new byte[] { 0x62 }),  // 'b'
                    new Integer(2)
                ),
                new KeyValuePair<IKey, IValue>(
                    new Binary(new byte[] { 0x63 }),  // 'c'
                    new Integer(3)
                ),
            });
            Assert.NotEqual(a, b);

            var c = new Dictionary(new KeyValuePair<IKey, IValue>[]
            {
                new KeyValuePair<IKey, IValue>(
                    new Binary(new byte[] { 0x61 }),  // 'a'
                    new Integer(2)
                ),
                new KeyValuePair<IKey, IValue>(
                    new Binary(new byte[] { 0x62 }),  // 'b'
                    new Integer(2)
                ),
                new KeyValuePair<IKey, IValue>(
                    new Binary(new byte[] { 0x64 }),  // 'd' -- different
                    new Integer(3)
                ),
            });
            Assert.NotEqual(a, c);
        }

        [Fact]
        public void SetItem()
        {
            var dictionary = Dictionary.Empty
                .SetItem("text", "foo")
                .SetItem("integer", 1337)
                .SetItem("binary", new byte[] { 0x01, 0x02, 0x03, 0x04 })
                .SetItem("boolean", true)
                .SetItem("list", new IValue[] { (Text)"bar", (Integer)1337 });

            Assert.Equal("foo", (Text)dictionary["text"]);
            Assert.Equal("foo", dictionary.GetValue<Text>("text"));
            Assert.Throws<InvalidCastException>(
                () => dictionary.GetValue<Integer>("text"));
            Assert.Throws<InvalidCastException>(
                () => dictionary.GetValue<Binary>("text"));
            Assert.Throws<InvalidCastException>(
                () => dictionary.GetValue<Bencodex.Types.Boolean>("text"));

            Assert.Equal((Integer)1337, (Integer)dictionary["integer"]);
            Assert.Equal(
                (Integer)1337,
                dictionary.GetValue<Integer>("integer"));
            Assert.Throws<InvalidCastException>(
                () => dictionary.GetValue<Binary>("integer"));
            Assert.Throws<InvalidCastException>(
                () => dictionary.GetValue<Text>("integer"));
            Assert.Throws<InvalidCastException>(
                () => dictionary.GetValue<Bencodex.Types.Boolean>("integer"));

            Assert.Equal(
                new byte[] { 0x01, 0x02, 0x03, 0x04 },
                (Binary)dictionary["binary"]);
            Assert.Equal(
                new byte[] { 0x01, 0x02, 0x03, 0x04 },
                dictionary.GetValue<Binary>("binary"));
            Assert.Throws<InvalidCastException>(
                () => dictionary.GetValue<Integer>("binary"));
            Assert.Throws<InvalidCastException>(
                () => dictionary.GetValue<Text>("binary"));
            Assert.Throws<InvalidCastException>(
                () => dictionary.GetValue<Bencodex.Types.Boolean>("binary"));

            Assert.Equal(
                (Bencodex.Types.Boolean)true,
                (Bencodex.Types.Boolean)dictionary["boolean"]);
            Assert.Equal(
                (Bencodex.Types.Boolean)true,
                dictionary.GetValue<Bencodex.Types.Boolean>("boolean"));
            Assert.Throws<InvalidCastException>(
                () => dictionary.GetValue<Integer>("boolean"));
            Assert.Throws<InvalidCastException>(
                () => dictionary.GetValue<Binary>("boolean"));
            Assert.Throws<InvalidCastException>(
                () => dictionary.GetValue<Text>("boolean"));

            Assert.Equal(
                new IValue[] { (Text)"bar", (Integer)1337 },
                (Bencodex.Types.List)dictionary["list"]);
            Assert.Equal(
                new IValue[] { (Text)"bar", (Integer)1337 },
                dictionary.GetValue<Bencodex.Types.List>("list"));
            Assert.Throws<InvalidCastException>(
                () => dictionary.GetValue<Integer>("list"));
            Assert.Throws<InvalidCastException>(
                () => dictionary.GetValue<Binary>("list"));
            Assert.Throws<InvalidCastException>(
                () => dictionary.GetValue<Text>("list"));
        }

        [Fact]
        public void Add()
        {
            byte[] textKey = new byte[] { 0x00 };
            byte[] integerKey = new byte[] { 0x01 };
            byte[] binaryKey = new byte[] { 0x02 };
            byte[] booleanKey = new byte[] { 0x03 };
            byte[] listKey = new byte[] { 0x04 };
            var dictionary = Dictionary.Empty
                .Add("text", "foo")
                .Add("integer", 1337)
                .Add("binary", new byte[] { 0x01, 0x02, 0x03, 0x04 })
                .Add("boolean", true)
                .Add("list", new IValue[] { (Text)"bar", (Integer)1337 })
                .Add(textKey, "baz")
                .Add(integerKey, 2020)
                .Add(binaryKey, new byte[] { 0x05, 0x06, 0x07, 0x08 })
                .Add(booleanKey, false)
                .Add(listKey, new IValue[] { (Text)"qux", (Integer)2020 });

            // String keys
            Assert.Equal("foo", (Text)dictionary["text"]);
            Assert.Equal("foo", dictionary.GetValue<Text>("text"));
            Assert.Throws<InvalidCastException>(
                () => dictionary.GetValue<Integer>("text"));
            Assert.Throws<InvalidCastException>(
                () => dictionary.GetValue<Binary>("text"));
            Assert.Throws<InvalidCastException>(
                () => dictionary.GetValue<Bencodex.Types.Boolean>("text"));

            Assert.Equal((Integer)1337, (Integer)dictionary["integer"]);
            Assert.Equal(
                (Integer)1337,
                dictionary.GetValue<Integer>("integer"));
            Assert.Throws<InvalidCastException>(
                () => dictionary.GetValue<Binary>("integer"));
            Assert.Throws<InvalidCastException>(
                () => dictionary.GetValue<Text>("integer"));
            Assert.Throws<InvalidCastException>(
                () => dictionary.GetValue<Bencodex.Types.Boolean>("integer"));

            Assert.Equal(
                new byte[] { 0x01, 0x02, 0x03, 0x04 },
                (Binary)dictionary["binary"]);
            Assert.Equal(
                new byte[] { 0x01, 0x02, 0x03, 0x04 },
                dictionary.GetValue<Binary>("binary"));
            Assert.Throws<InvalidCastException>(
                () => dictionary.GetValue<Integer>("binary"));
            Assert.Throws<InvalidCastException>(
                () => dictionary.GetValue<Text>("binary"));
            Assert.Throws<InvalidCastException>(
                () => dictionary.GetValue<Bencodex.Types.Boolean>("binary"));

            Assert.Equal(
                (Bencodex.Types.Boolean)true,
                (Bencodex.Types.Boolean)dictionary["boolean"]);
            Assert.Equal(
                (Bencodex.Types.Boolean)true,
                dictionary.GetValue<Bencodex.Types.Boolean>("boolean"));
            Assert.Throws<InvalidCastException>(
                () => dictionary.GetValue<Integer>("boolean"));
            Assert.Throws<InvalidCastException>(
                () => dictionary.GetValue<Binary>("boolean"));
            Assert.Throws<InvalidCastException>(
                () => dictionary.GetValue<Text>("boolean"));

            Assert.Equal(
                new IValue[] { (Text)"bar", (Integer)1337 },
                (Bencodex.Types.List)dictionary["list"]);
            Assert.Equal(
                new IValue[] { (Text)"bar", (Integer)1337 },
                dictionary.GetValue<Bencodex.Types.List>("list"));
            Assert.Throws<InvalidCastException>(
                () => dictionary.GetValue<Integer>("list"));
            Assert.Throws<InvalidCastException>(
                () => dictionary.GetValue<Binary>("list"));
            Assert.Throws<InvalidCastException>(
                () => dictionary.GetValue<Text>("list"));

            // Byte array keys
            Assert.Equal("baz", (Text)dictionary[textKey]);
            Assert.Equal("baz", dictionary.GetValue<Text>(textKey));
            Assert.Throws<InvalidCastException>(
                () => dictionary.GetValue<Integer>(textKey));
            Assert.Throws<InvalidCastException>(
                () => dictionary.GetValue<Binary>(textKey));
            Assert.Throws<InvalidCastException>(
                () => dictionary.GetValue<Bencodex.Types.Boolean>(textKey));

            Assert.Equal((Integer)2020, (Integer)dictionary[integerKey]);
            Assert.Equal(
                (Integer)2020,
                dictionary.GetValue<Integer>(integerKey));
            Assert.Throws<InvalidCastException>(
                () => dictionary.GetValue<Binary>(integerKey));
            Assert.Throws<InvalidCastException>(
                () => dictionary.GetValue<Text>(integerKey));
            Assert.Throws<InvalidCastException>(
                () => dictionary.GetValue<Bencodex.Types.Boolean>(integerKey));

            Assert.Equal(
                new byte[] { 0x05, 0x06, 0x07, 0x08 },
                (Binary)dictionary[binaryKey]);
            Assert.Equal(
                new byte[] { 0x05, 0x06, 0x07, 0x08 },
                dictionary.GetValue<Binary>(binaryKey));
            Assert.Throws<InvalidCastException>(
                () => dictionary.GetValue<Integer>(binaryKey));
            Assert.Throws<InvalidCastException>(
                () => dictionary.GetValue<Text>(binaryKey));
            Assert.Throws<InvalidCastException>(
                () => dictionary.GetValue<Bencodex.Types.Boolean>(binaryKey));

            Assert.Equal(
                (Bencodex.Types.Boolean)false,
                (Bencodex.Types.Boolean)dictionary[booleanKey]);
            Assert.Equal(
                (Bencodex.Types.Boolean)false,
                dictionary.GetValue<Bencodex.Types.Boolean>(booleanKey));
            Assert.Throws<InvalidCastException>(
                () => dictionary.GetValue<Integer>(booleanKey));
            Assert.Throws<InvalidCastException>(
                () => dictionary.GetValue<Binary>(booleanKey));
            Assert.Throws<InvalidCastException>(
                () => dictionary.GetValue<Text>(booleanKey));

            Assert.Equal(
                new IValue[] { (Text)"qux", (Integer)2020 },
                (Bencodex.Types.List)dictionary[listKey]);
            Assert.Equal(
                new IValue[] { (Text)"qux", (Integer)2020 },
                dictionary.GetValue<Bencodex.Types.List>(listKey));
            Assert.Throws<InvalidCastException>(
                () => dictionary.GetValue<Integer>(listKey));
            Assert.Throws<InvalidCastException>(
                () => dictionary.GetValue<Binary>(listKey));
            Assert.Throws<InvalidCastException>(
                () => dictionary.GetValue<Text>(listKey));
        }
    }
}
