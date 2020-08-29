using System;
using System.Collections.Generic;

namespace Web.Types.Values
{
    public class TinyType<T, TValue> : IEquatable<T> where T : TinyType<T, TValue>
    {
        public TinyType(TValue value)
        {
            Value = value;
        }

        public TValue Value { get; }

        public bool Equals(T other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return EqualityComparer<TValue>.Default.Equals(Value, other.Value);
        }

        public bool Equals(TValue other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(Value, other)) return true;
            return EqualityComparer<TValue>.Default.Equals(Value, other);
        }

        public override bool Equals(object? obj) => (obj is T o && Equals(o)) || (obj is TValue v && Equals(v));

        public override int GetHashCode() => EqualityComparer<TValue>.Default.GetHashCode(Value);

        public static bool operator ==(TinyType<T, TValue> left, TinyType<T, TValue> right) => Equals(left, right);

        public static bool operator !=(TinyType<T, TValue> left, TinyType<T, TValue> right) => !Equals(left, right);

        public override string ToString() => $"{Value}";
    }
}
