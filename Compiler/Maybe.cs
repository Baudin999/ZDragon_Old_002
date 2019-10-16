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
        private T value;
        public Just(T value)
        {
            this.value = value;
        }

        public override T Get()
        {
            return this.value;
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
