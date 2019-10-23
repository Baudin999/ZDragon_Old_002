using System;
using System.Collections.Generic;
using System.Text;

namespace Compiler
{
    /// <summary>
    /// Represents an input for parsing.
    /// </summary>
    public class Input
    {

        private readonly string _source;
        private int _position;
        private int _line;
        private int _column;
        private readonly int _length;

        /// <summary>
        /// Initializes a new instance of the <see cref="Input" /> class.
        /// </summary>
        /// <param name="source">The source.</param>
        public Input(string source)
            : this(source, 0)
        {
        }

        internal Input(string source, int position, int line = 1, int column = 1)
        {
            _source = source;
            _position = position;
            _line = line;
            _column = column;
            _length = source.Length;
        }

        internal bool IsEqualTo(string keyword)
        {
            // TODO: get some performance gain by checking the keywords instead of
            // calling this method over and over....
            var length = keyword.Length;
            if (_length <= _position + length)
            {
                return false;
            }

            for (var i = 0; i < length; ++i)
            {
                if (Peek(i) != keyword[i]) return false;
            }
            return true;
        }

        public bool HasNext() => _position < _length;
        public bool HasPeek(int pos) => _position + pos < _length;

        public char Current() => _source[_position];

        public char Next() {
            if (HasNext())
            {
                var c = _source[_position++];
                if (c == '↓')
                {
                    _line++;
                    _column = 1;
                } else
                {
                    _column++;
                }
                return c;
            }
            else
            {
                throw new InvalidOperationException("There is no next");
            }
        }



        public char Peek(int position = 1) => _source[_position + position];
        public int Position {  get { return _position; } }
        public int Column {  get { return _column; } }
        public int Line {  get { return _line; } }


    }

   
}