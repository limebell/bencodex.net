using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using Bencodex.Misc;

namespace Bencodex.Types
{
    public struct Binary :
        IKey,
        IEquatable<byte[]>,
        IEquatable<Binary>,
        IComparable<byte[]>,
        IComparable<Binary>,
        IComparable,
        IEnumerable<byte>
    {
        private static readonly ByteArrayComparer ByteArrayComparer =
            default(ByteArrayComparer);

        private byte[] _value;

        public Binary(byte[] value)
        {
            _value = value ?? throw new ArgumentNullException(nameof(value));
        }

        [Pure]
        byte? IKey.KeyPrefix => null;

        public byte[] Value => _value ?? (_value = new byte[0]);

        public static implicit operator Binary(byte[] bytes)
        {
            return new Binary(bytes);
        }

        public static implicit operator byte[](Binary binary)
        {
            return binary.Value;
        }

        public static bool operator ==(Binary left, Binary right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Binary left, Binary right)
        {
            return !left.Equals(right);
        }

        bool IEquatable<byte[]>.Equals(byte[] otherBytes)
        {
            return Value.SequenceEqual(otherBytes);
        }

        bool IEquatable<Binary>.Equals(Binary other)
        {
            return ((IEquatable<byte[]>)this).Equals(other.Value);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (obj is byte[] otherBytes)
            {
                return ((IEquatable<byte[]>)this).Equals(otherBytes);
            }

            return obj is Binary other &&
                ((IEquatable<Binary>)this).Equals(other);
        }

        public override int GetHashCode()
        {
            int length = Value.Length;
            return Value.Aggregate(
                0,
                (current, t) => unchecked(current * (length + 1) + t)
            );
        }

        int IComparable<byte[]>.CompareTo(byte[] other)
        {
            return ByteArrayComparer.Compare(Value, other);
        }

        int IComparable<Binary>.CompareTo(Binary other)
        {
            return ((IComparable<byte[]>)this).CompareTo(other.Value);
        }

        int IComparable.CompareTo(object obj)
        {
            switch (obj)
            {
                case null:
                    return 1;
                case Binary binary:
                    return ((IComparable<Binary>)this).CompareTo(binary);
                case byte[] bytes:
                    return ((IComparable<byte[]>)this).CompareTo(bytes);
                default:
                    throw new ArgumentException(
                        "the argument is neither Binary nor Byte[]",
                        nameof(obj)
                    );
            }
        }

        public IEnumerator<byte> GetEnumerator()
        {
            return ((IEnumerable<byte>)Value).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Value.GetEnumerator();
        }

        [Pure]
        byte[] IKey.EncodeAsByteArray()
        {
            byte[] dest = new byte[Value.Length];
            Array.Copy(Value, dest, Value.Length);
            return dest;
        }

        [Pure]
        public IEnumerable<byte[]> EncodeIntoChunks()
        {
            yield return Encoding.ASCII.GetBytes(Value.Length.ToString());
            yield return new byte[1] { 0x3a };  // ':'
            yield return ((IKey)this).EncodeAsByteArray();
        }

        [Pure]
        public override string ToString()
        {
            return BitConverter.ToString(Value).Replace("-", string.Empty).ToLower();
        }
    }
}
