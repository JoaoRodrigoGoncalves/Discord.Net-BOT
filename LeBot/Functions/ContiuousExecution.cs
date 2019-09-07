using System;
using System.Collections.Generic;
using System.Text;

namespace LeBot.Functions
{
    class ContiuousExecution
    {
        public bool TestContinuousExecution(int lastday = 0, int lastmonth = 0, int lastyear = 0)
        {
            if(lastday != 0 || lastmonth != 0 || lastyear != 0)
            {
                int today = DateTime.Now.Day;
                int month = DateTime.Now.Month;
                int year = DateTime.Now.Year;

                //  1        31            08          07
                if(today - lastday != 1 && month - lastmonth >=2)
                //      = -30           |         = 1
                //       true           |         false
                {
                    if(year - lastyear != 1)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return false;
            }
        }
    }
}
