using System;
using System.Collections.Generic;
using System.Text;

namespace LeBot.Functions
{
    class TimeCalculation
    {
        public bool TimeClaculations(int lastday = 0, int lastmonth = 0, int lastyear = 0)
        {
            if (lastday != 0 && lastmonth != 0 && lastyear != 0)
            {
                var today = DateTime.Now.Day;

                // Check if todays day is bigger than lastday that the user used the command
                if (today > lastday)
                {
                    return true;
                }
                else
                {
                    // If it isnt, it could be a different month. So we check if last month number is smaller than this month
                    int thisMonth = DateTime.Now.Month;
                    //  6        <     7
                    if(lastmonth < thisMonth)
                    {
                        return true;
                    }
                    else
                    {
                        // If it isnt, it could be a new year! ~ yeah ~ . Checks if last year number was smaller than this years number
                        int thisYear = DateTime.Now.Year;
                        //   2017   <    2018
                        if(lastyear < thisYear)
                        {
                            return true;
                        }
                        else
                        {
                            // It isnt none of those? So this shit is wrong. Just return false...
                            return false;
                        }
                    }
                }
            }
            else
            {
                return false;
            }
        }
    }
}
