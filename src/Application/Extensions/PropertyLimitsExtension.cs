using Domain.Entities;

namespace Application.Extensions
{
    public static class PropertyLimitsExtension
    {

        public static bool IntOverLimits(this int prop, double startingLimit, double endingLimit)
        {
            if (prop >= startingLimit && prop <= endingLimit)
            {
                return true;
            }
            else
                return false;


        }
        public static bool DoubleOverLimits(this double prop, double startingLimit, double endingLimit)
        {
            if (prop >= startingLimit && prop <= endingLimit)
            {
                return true;
            }
            else
                return false;

        }

        public static bool DecimalOverLimits(this decimal prop, double startingLimit, double endingLimit)
        {
            if ((double)prop >= startingLimit && (double)prop <= endingLimit)
            {
                return true;
            }
            else
                return false;

        }
    }
}
