using System;
using System.Collections.Generic;
using System.Linq;

namespace Centro_Preprocessing_Dev
{
    /// <summary>
    /// Author: Ravi Teja Yadlapalli
    /// Date: 23 - OCT - 2020
    /// Last Modified: 03 - NOV - 2020
    /// </summary>
    class FixTimePointMinus1
    {
        public void fixTimepointMinus1()
        {
            List<Centro_Preprocessing_UNIQUE> timepoint_minus1 = new List<Centro_Preprocessing_UNIQUE>();
            using (Centro_PreprocessingEntities entities = new Centro_PreprocessingEntities())
            {
                IQueryable<Centro_Preprocessing_UNIQUE> allStops = entities.Centro_Preprocessing_UNIQUE.Select(x => x).Where(x => x.ROUTE_NUMBER == 374);
                //Selecting all records with TIMEPOINT -1
                timepoint_minus1 = allStops.Where(x => x.TIMEPOINT == -1).ToList();
                foreach (Centro_Preprocessing_UNIQUE stop in timepoint_minus1)
                {
                    //finidng a succeeding record
                    for (int y = stop.UNIQUE_ID + 1; y > 0; y++)
                    {
                        //checking succeeding record wich is not having TIMEPOINT as -1 and not having STOPID as 99999
                        if (allStops.First(x => x.UNIQUE_ID == y).TIMEPOINT != -1 && allStops.First(x => x.UNIQUE_ID == y).STOP_ID != 99999)
                        {
                            //substituting succeeding record TIME_SCHEDULE 
                            // allStops.First(x => x.UNIQUE_ID == y).TIME_SCHEDULED
                            var a = allStops.First(x => x.UNIQUE_ID == stop.UNIQUE_ID).TIME_SCHEDULED;
                            //substituting succeeding record RUNNING_TIME_ACTUAL  
                            // allStops.First(x => x.UNIQUE_ID == y).RUNNING_TIME_ACTUAL 
                            var b = allStops.First(x => x.UNIQUE_ID == stop.UNIQUE_ID).RUNNING_TIME_ACTUAL;
                            //adding succeeding record PASSENGERS_ON   
                            // allStops.First(x => x.UNIQUE_ID == y).PASSENGERS_ON 
                            var c = allStops.First(x => x.UNIQUE_ID == y).PASSENGERS_ON + allStops.First(x => x.UNIQUE_ID == stop.UNIQUE_ID).PASSENGERS_ON;
                            //adding succeeding record PASSENGERS_OFF    
                            //  allStops.First(x => x.UNIQUE_ID == y).PASSENGERS_OFF 
                            var d = allStops.First(x => x.UNIQUE_ID == y).PASSENGERS_OFF + allStops.First(x => x.UNIQUE_ID == stop.UNIQUE_ID).PASSENGERS_OFF;
                            //substituting succeeding record TIMEPOINT_MILES     
                            // allStops.First(x => x.UNIQUE_ID == y).TIMEPOINT_MILES
                            var e = allStops.First(x => x.UNIQUE_ID == stop.UNIQUE_ID).TIMEPOINT_MILES;

                            //Marking as To be Deleted
                            allStops.First(x => x.UNIQUE_ID == stop.UNIQUE_ID).To_Be_Deleted = true;
                            //Marking as changed
                            allStops.First(x => x.UNIQUE_ID == y).MODIFIED_FLAG = true;
                            break;
                        }
                    }
                }

                entities.SaveChanges();

            }
        }
    }
}
