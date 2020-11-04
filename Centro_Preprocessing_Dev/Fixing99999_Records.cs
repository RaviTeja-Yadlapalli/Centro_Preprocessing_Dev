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
    class Fixing99999_Records
    {
        /// <summary>
        /// 
        /// </summary>
        public void fixing99999Records()
        {
            DateTime succeddingTime_Arrive = DateTime.Now;
            DateTime predeedingTime_Arrive = DateTime.Now;
            DateTime stop_Time = DateTime.Now;
            int preceedingRecord = 0, succeedingRecord = 0;
            List<Centro_Preprocessing_UNIQUE> records_99999 = new List<Centro_Preprocessing_UNIQUE>();
            using (Centro_PreprocessingEntities entities = new Centro_PreprocessingEntities())
            {
                IQueryable<Centro_Preprocessing_UNIQUE> allStops = entities.Centro_Preprocessing_UNIQUE.Select(x => x).Where(x => x.ROUTE_NUMBER == 374);
                //selecting all records having STOPID as 99999
                records_99999 = allStops.Where(x => x.STOP_ID == 99999).ToList();
                foreach (Centro_Preprocessing_UNIQUE stop in records_99999)
                {

                    for (int preceed = stop.UNIQUE_ID - 1; preceed > 0; preceed--)
                    {
                        if (allStops.First(x => x.UNIQUE_ID == preceed).STOP_ID != 99999)
                        {
                            preceedingRecord = preceed;
                            break;
                        }
                    }

                    for (int succeed = stop.UNIQUE_ID + 1; succeed > 0; succeed--)
                    {
                        if (allStops.First(x => x.UNIQUE_ID == succeed).STOP_ID != 99999)
                        {
                            preceedingRecord = succeed;
                            break;
                        }
                    }

                    if (stop.SORT_ORDER.Equals(allStops.First(x => x.UNIQUE_ID == preceedingRecord).SORT_ORDER) || stop.SORT_ORDER.Equals(allStops.First(x => x.UNIQUE_ID == succeedingRecord).SORT_ORDER))
                    {
                        if (stop.FIRST_LAST_STOP == 3)
                        {
                            for (int y = stop.UNIQUE_ID - 1; y > 0; y--)
                            {
                                if (allStops.First(x => x.UNIQUE_ID == y).TIMEPOINT == 0 && allStops.First(x => x.UNIQUE_ID == y).STOP_ID != 99999)
                                {
                                    //adding PASSENGERS_ON to preceeding record
                                    allStops.First(x => x.UNIQUE_ID == y).PASSENGERS_ON = allStops.First(x => x.UNIQUE_ID == y).PASSENGERS_ON + allStops.First(x => x.UNIQUE_ID == stop.UNIQUE_ID).PASSENGERS_ON;
                                    //adding PASSENGERS_OFF to preceeding record
                                    allStops.First(x => x.UNIQUE_ID == y).PASSENGERS_OFF = allStops.First(x => x.UNIQUE_ID == y).PASSENGERS_OFF + allStops.First(x => x.UNIQUE_ID == stop.UNIQUE_ID).PASSENGERS_OFF;
                                    // marking record as modified                                    
                                    allStops.First(x => x.UNIQUE_ID == y).MODIFIED_FLAG = true;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            stop_Time = DateTime.SpecifyKind(allStops.First(x => x.UNIQUE_ID == stop.UNIQUE_ID).TIME_ACTUAL_ARRIVE ?? DateTime.Now, DateTimeKind.Utc);
                            //selecting preceeding record
                            for (int preceed = stop.UNIQUE_ID - 1; preceed > 0; preceed--)
                            {
                                if (allStops.First(x => x.UNIQUE_ID == preceed).TIMEPOINT == 0 && allStops.First(x => x.UNIQUE_ID == preceed).TIME_ACTUAL_ARRIVE != null && allStops.First(x => x.UNIQUE_ID == preceed).STOP_ID != 99999)
                                {
                                    predeedingTime_Arrive = DateTime.SpecifyKind(allStops.First(x => x.UNIQUE_ID == preceed).TIME_ACTUAL_ARRIVE ?? DateTime.Now, DateTimeKind.Utc);
                                    preceedingRecord = preceed;
                                    break;
                                }
                            }
                            //selecting succeeding record
                            for (int succeed = stop.UNIQUE_ID + 1; succeed > 0; succeed++)
                            {
                                if (allStops.First(x => x.UNIQUE_ID == succeed).TIMEPOINT == 0 && allStops.First(x => x.UNIQUE_ID == succeed).TIME_ACTUAL_ARRIVE != null && allStops.First(x => x.UNIQUE_ID == succeed).STOP_ID != 99999)
                                {
                                    succeddingTime_Arrive = DateTime.SpecifyKind(allStops.First(x => x.UNIQUE_ID == succeed).TIME_ACTUAL_ARRIVE ?? DateTime.Now, DateTimeKind.Utc);
                                    succeedingRecord = succeed;
                                    break;
                                }
                            }

                            if (Math.Abs(stop_Time.Subtract(predeedingTime_Arrive).Ticks) < Math.Abs(stop_Time.Subtract(succeddingTime_Arrive).Ticks))
                            {
                                //adding PASSENGERS_ON value to selected record
                                allStops.First(x => x.UNIQUE_ID == preceedingRecord).PASSENGERS_ON = allStops.First(x => x.UNIQUE_ID == stop.UNIQUE_ID).PASSENGERS_ON + allStops.First(x => x.UNIQUE_ID == preceedingRecord).PASSENGERS_ON;

                                //adding PASSENGERS_OFF value to selected record
                                allStops.First(x => x.UNIQUE_ID == preceedingRecord).PASSENGERS_OFF = allStops.First(x => x.UNIQUE_ID == stop.UNIQUE_ID).PASSENGERS_OFF + allStops.First(x => x.UNIQUE_ID == preceedingRecord).PASSENGERS_OFF;

                                //substituting PASSENGERS_IN of selected record with 99999 record's value
                                allStops.First(x => x.UNIQUE_ID == preceedingRecord).PASSENGERS_IN = allStops.First(x => x.UNIQUE_ID == stop.UNIQUE_ID).PASSENGERS_IN;
                                //marking as modified
                                allStops.First(x => x.UNIQUE_ID == preceedingRecord).MODIFIED_FLAG = true;
                            }
                            else
                            {
                                //adding PASSENGERS_ON value to selected record
                                allStops.First(x => x.UNIQUE_ID == succeedingRecord).PASSENGERS_ON = allStops.First(x => x.UNIQUE_ID == stop.UNIQUE_ID).PASSENGERS_ON + allStops.First(x => x.UNIQUE_ID == succeedingRecord).PASSENGERS_ON;

                                //adding PASSENGERS_OFF value to selected record
                                allStops.First(x => x.UNIQUE_ID == succeedingRecord).PASSENGERS_OFF = allStops.First(x => x.UNIQUE_ID == stop.UNIQUE_ID).PASSENGERS_OFF + allStops.First(x => x.UNIQUE_ID == succeedingRecord).PASSENGERS_OFF;

                                //marking as modified
                                allStops.First(x => x.UNIQUE_ID == succeedingRecord).MODIFIED_FLAG = true;
                            }
                        }
                        //marking for deletion
                        allStops.First(x => x.UNIQUE_ID == stop.UNIQUE_ID).To_Be_Deleted = true;
                    }
                    //in case of SORT_ORDER of 99999 record do not match with either preceeding or succeeding column
                    else
                    {
                        //finding a record with same COMMENTS as of selected 99999 column
                        var matchedRecord = allStops.Where(x => x.COMMENTS == stop.COMMENTS).First();
                        allStops.First(x => x.UNIQUE_ID == stop.UNIQUE_ID).STOP_ID = matchedRecord.STOP_ID;
                        allStops.First(x => x.UNIQUE_ID == stop.UNIQUE_ID).MAIN_CROSS_STREET = matchedRecord.MAIN_CROSS_STREET;
                        allStops.First(x => x.UNIQUE_ID == stop.UNIQUE_ID).TRAVEL_DIRECTION = matchedRecord.TRAVEL_DIRECTION;
                        allStops.First(x => x.UNIQUE_ID == stop.UNIQUE_ID).SEGMENT_MILES = matchedRecord.SEGMENT_MILES;

                        //marking MODIFIED_FLAG column
                        allStops.First(x => x.UNIQUE_ID == stop.UNIQUE_ID).MODIFIED_FLAG = true;
                    }

                }

                //entities.SaveChanges();

            }
        }

    }
}
