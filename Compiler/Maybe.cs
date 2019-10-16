using System;
namespace Compiler
{
    public interface IMaybe { }
    public interface IMaybe<T> : IMaybe
    {
        T Get();
    }

    public abstract class Maybe<T> : IMaybe<T>
    {
        public abstract T Get();
    }


    public class Just<T> : Maybe<T>
    {
        private readonly T _value;
        public Just(T value)
        {
            this._value = value;
        }

        public override T Get()
        {
            return this._value;
        }
    }


    public class Nothing<T> : Maybe<T>
    {
        public override T Get()
        {
            throw new InvalidOperationException("Cannot get a value from Nothing.");
        }

    }

}
