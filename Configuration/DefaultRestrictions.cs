using System;
namespace Configuration
{
    public class DefaultRestrictions
    {
        public StringRestrictions StringRestrictions { get; set; } = new StringRestrictions();
        public NumberRestrictions NumberRestrictions { get; set; } = new NumberRestrictions();
        public DecimalRestrictions DecimalRestrictions { get; set; } = new DecimalRestrictions();
        public ListRestrictions ListRestrictions { get; set; } = new ListRestrictions();
    }

    public class StringRestrictions
    {
        public int Min { get; set; } = 0;
        public int Max { get; set; } = 100;
    }

    public class NumberRestrictions
    {
        public int Min { get; set; } = -1000;
        public int Max { get; set; } = 1000;
    }

    public class DecimalRestrictions
    {
        public int Min { get; set; } = -1000;
        public int Max { get; set; } = 1000;
        public int Decimals { get; set; } = 3;
    }

    public class ListRestrictions
    {
        public int Min { get; set; } = 0;
        public int Max { get; set; } = 10;
    }
}
