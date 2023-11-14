namespace CarDealers.Util
{
    public class Converter
    {
        public static int ParseInt(string s)
        {
            int myInt;
            Int32.TryParse(s, out myInt);
            return myInt;
        }
    }
}
