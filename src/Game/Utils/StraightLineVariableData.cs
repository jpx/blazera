namespace BlazeraLib
{
    public class StraightLineVariableData : VariableData<float>
    {
        public StraightLineVariableData(float boundData, int precision = DEFAULT_PRECISION)
            : base(boundData, precision)
        {
        }

        public StraightLineVariableData(StraightLineVariableData copy)
            : base(copy)
        {
        }

        protected override float ComputeData(int level)
        {
            Vector2I bounds = GetBounds(level);

            if (bounds.X == bounds.Y)
                return GetKeyData(bounds.X);

            float percent = (float)(level - bounds.X) / (float)(bounds.Y - bounds.X);

            return GetKeyData(bounds.X) + (GetKeyData(bounds.Y) - GetKeyData(bounds.X)) * percent;
        }
    }
}
