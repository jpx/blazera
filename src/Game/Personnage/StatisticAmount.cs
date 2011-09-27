namespace BlazeraLib
{
    public class StatisticAmount
    {
        #region Constants

        const uint DEFAULT_VALUE = 0;

        #endregion

        #region Members

        public uint Value { get; set; }

        #endregion

        public StatisticAmount(uint value = DEFAULT_VALUE)
        {
            Value = value;
        }
    }
}
