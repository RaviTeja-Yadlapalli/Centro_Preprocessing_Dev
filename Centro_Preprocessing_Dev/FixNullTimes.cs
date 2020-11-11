using System;
using System.Collections.Generic;
using System.Linq;

namespace Centro_Preprocessing_Dev
{
    /// <summary>
    /// Author: Ravi Teja Yadlapalli
    /// Date: 17 - OCT - 2020
    /// Last Modified: 03 - NOV - 2020
    /// </summary>
    class FixNullTimes
    {
        public void fixArriveDepartTimes()
        {
            List<Centro_Preprocessing_UNIQUE> time_actual_null = new List<Centro_Preprocessing_UNIQUE>();
            List<Centro_Preprocessing_UNIQUE> time_depart_null = new List<Centro_Preprocessing_UNIQUE>();
            DateTime succeddingTime_Arrive = DateTime.Now;
            DateTime predeedingTime_Arrive = DateTime.Now;
            DateTime succeddingTime_Depart = DateTime.Now;
            DateTime predeedingTime_Depart = DateTime.Now;
            double segmentMiles = 0.0;
            using (Centro_PreprocessingEntities entities = new Centro_PreprocessingEntities())
            {                
                //get the data set of specific bus route
                IQueryable<Centro_Preprocessing_UNIQUE> allStops = entities.Centro_Preprocessing_UNIQUE.Select(x => x).Where(x => x.ROUTE_NUMBER == 374);
                //filtering the dataset having time point 0 and time actual arrive is null and not having stp id 99999
                time_actual_null = allStops.Where(x => x.TIMEPOINT == 0).Where(x => x.TIME_ACTUAL_ARRIVE == null).Where(x => x.STOP_ID != 99999).ToList();
                foreach (Centro_Preprocessing_UNIQUE stop in time_actual_null)
                {
                    if (stop.FIRST_LAST_STOP == 2)
                    {
                        //selecting preceeding record
                        for (int y = stop.UNIQUE_ID - 1; y > 0; y--)
                        {
                            if (allStops.First(x => x.UNIQUE_ID == y).TIMEPOINT == 0 && allStops.First(x => x.UNIQUE_ID == y).TIME_ACTUAL_ARRIVE != null && allStops.First(x => x.UNIQUE_ID == y).STOP_ID != 99999)
                            {
                                predeedingTime_Arrive = DateTime.SpecifyKind(allStops.First(x => x.UNIQUE_ID == y).TIME_ACTUAL_ARRIVE ?? DateTime.Now, DateTimeKind.Utc);                               
                                break;
                            }
                        }
                        //selecting succeeding record
                        for (int y = stop.UNIQUE_ID + 1; y > 0; y++)
                        {
                            if (allStops.First(x => x.UNIQUE_ID == y).TIMEPOINT == 0 && allStops.First(x => x.UNIQUE_ID == y).TIME_ACTUAL_ARRIVE != null && allStops.First(x => x.UNIQUE_ID == y).STOP_ID != 99999)
                            {
                                succeddingTime_Arrive = DateTime.SpecifyKind(allStops.First(x => x.UNIQUE_ID == y).TIME_ACTUAL_ARRIVE ?? DateTime.Now, DateTimeKind.Utc);                               
                                break;
                            }
                        }
                        //filling the NULL TIME_ACTUAL_ARRIVE with average of preceeding and succeeding records
                        allStops.First(x => x.UNIQUE_ID == stop.UNIQUE_ID).TIME_ACTUAL_ARRIVE  = new DateTime((predeedingTime_Arrive.Ticks + succeddingTime_Arrive.Ticks) / 2);
                    }
                    else if(stop.FIRST_LAST_STOP == 1)
                    {
                        //selecting the succeeding record
                        for (int y = stop.UNIQUE_ID + 1; y > 0; y++)
                        {
                            if (allStops.First(x => x.UNIQUE_ID == y).TIMEPOINT == -1 && allStops.First(x => x.UNIQUE_ID == y).TIME_ACTUAL_ARRIVE != null && allStops.First(x => x.UNIQUE_ID == y).STOP_ID != 99999)
                            {
                                // allStops.First(x => x.UNIQUE_ID == stop.UNIQUE_ID).TIME_ACTUAL_ARRIVE  
                                succeddingTime_Arrive = allStops.First(x => x.UNIQUE_ID == y).TIME_ACTUAL_ARRIVE ?? DateTime.Now;                                
                                break;
                            }
                        }
                        //segment miles of the record having  TIME_ACTUAL_ARRIVE NULL
                        segmentMiles = (allStops.First(x => x.UNIQUE_ID == stop.UNIQUE_ID).SEGMENT_MILES) ?? segmentMiles;
                        //updating the TIME_ACTUAL_ARRIVE column
                        allStops.First(x => x.UNIQUE_ID == stop.UNIQUE_ID).TIME_ACTUAL_ARRIVE = succeddingTime_Arrive.AddSeconds(-120* segmentMiles);
                        //updating TIME_ACTUAL_DEPART column
                        allStops.First(x => x.UNIQUE_ID == stop.UNIQUE_ID).TIME_ACTUAL_DEPART = succeddingTime_Arrive.AddSeconds(-120* segmentMiles);
                    }
                    // if FIRST_LAST_STOP is 3
                    else
                    {
                        //selecting the preceeding record
                        for (int y = stop.UNIQUE_ID - 1; y > 0; y--)
                        {
                            if (allStops.First(x => x.UNIQUE_ID == y).TIMEPOINT == -1 && allStops.First(x => x.UNIQUE_ID == y).TIME_ACTUAL_DEPART != null && allStops.First(x => x.UNIQUE_ID == y).STOP_ID != 99999)
                            {
                                // allStops.First(x => x.UNIQUE_ID == stop.UNIQUE_ID).TIME_ACTUAL_ARRIVE  
                                predeedingTime_Arrive = allStops.First(x => x.UNIQUE_ID == y).TIME_ACTUAL_DEPART ?? DateTime.Now;
                                break;
                            }
                        }
                        //segment miles of the record having  TIME_ACTUAL_ARRIVE NULL
                        segmentMiles = (allStops.First(x => x.UNIQUE_ID == stop.UNIQUE_ID).SEGMENT_MILES) ?? segmentMiles;
                        //updating the TIME_ACTUAL_ARRIVE column
                        allStops.First(x => x.UNIQUE_ID == stop.UNIQUE_ID).TIME_ACTUAL_ARRIVE = predeedingTime_Arrive.AddSeconds(120* segmentMiles);
                        //updating TIME_ACTUAL_DEPART column
                        allStops.First(x => x.UNIQUE_ID == stop.UNIQUE_ID).TIME_ACTUAL_DEPART = predeedingTime_Arrive.AddSeconds(120* segmentMiles);
                    }
                    //marking as modified
                    allStops.First(x => x.UNIQUE_ID == stop.UNIQUE_ID).MODIFIED_FLAG = true;
                }


                ////for records having TIME_ACTUAL_DEPART as NULL
                //filtering the dataset having time point 0 and TIME_ACTUAL_DEPART as NULL and not having STOP_ID 99999
                time_depart_null = allStops.Where(x => x.TIMEPOINT == 0).Where(x => x.TIME_ACTUAL_DEPART == null).Where(x => x.STOP_ID != 99999).ToList();
                foreach (Centro_Preprocessing_UNIQUE stop in time_actual_null)
                {
                    if (stop.FIRST_LAST_STOP == 2)
                    {
                        //selecting preceeding record
                        for (int y = stop.UNIQUE_ID - 1; y > 0; y--)
                        {
                            if (allStops.First(x => x.UNIQUE_ID == y).TIMEPOINT == 0  && allStops.First(x => x.UNIQUE_ID == y).TIME_ACTUAL_DEPART != null && allStops.First(x => x.UNIQUE_ID == y).STOP_ID != 99999)
                            {                        
                                predeedingTime_Depart = DateTime.SpecifyKind(allStops.First(x => x.UNIQUE_ID == y).TIME_ACTUAL_DEPART ?? DateTime.Now, DateTimeKind.Utc);
                                break;
                            }
                        }
                        //selecting succeeding record
                        for (int y = stop.UNIQUE_ID + 1; y > 0; y++)
                        {
                            if (allStops.First(x => x.UNIQUE_ID == y).TIMEPOINT == 0 && allStops.First(x => x.UNIQUE_ID == y).TIME_ACTUAL_DEPART != null && allStops.First(x => x.UNIQUE_ID == y).STOP_ID != 99999)
                            {                          
                                succeddingTime_Depart = DateTime.SpecifyKind(allStops.First(x => x.UNIQUE_ID == y).TIME_ACTUAL_DEPART ?? DateTime.Now, DateTimeKind.Utc);
                                break;
                            }
                        }
                        //filling the NULL TIME_ACTUAL_DEPART with average of preceeding and succeeding records
                        allStops.First(x => x.UNIQUE_ID == stop.UNIQUE_ID).TIME_ACTUAL_DEPART = new DateTime((predeedingTime_Depart.Ticks + succeddingTime_Depart.Ticks) / 2);
                    }
                    else if (stop.FIRST_LAST_STOP == 1)
                    {
                        //selecting the succeeding record
                        for (int y = stop.UNIQUE_ID + 1; y > 0; y++)
                        {
                            if (allStops.First(x => x.UNIQUE_ID == y).TIMEPOINT == -1 && allStops.First(x => x.UNIQUE_ID == y).TIME_ACTUAL_ARRIVE != null && allStops.First(x => x.UNIQUE_ID == y).STOP_ID != 99999)
                            {
                                // allStops.First(x => x.UNIQUE_ID == stop.UNIQUE_ID).TIME_ACTUAL_ARRIVE  
                                succeddingTime_Arrive = allStops.First(x => x.UNIQUE_ID == y).TIME_ACTUAL_ARRIVE ?? DateTime.Now;
                                break;
                            }
                        }
                        //segment miles of the record having  TIME_ACTUAL_ARRIVE NULL
                        segmentMiles = (allStops.First(x => x.UNIQUE_ID == stop.UNIQUE_ID).SEGMENT_MILES) ?? segmentMiles;
                        //updating the TIME_ACTUAL_ARRIVE column
                        allStops.First(x => x.UNIQUE_ID == stop.UNIQUE_ID).TIME_ACTUAL_ARRIVE = succeddingTime_Arrive.AddSeconds(-120 * segmentMiles);
                        //updating TIME_ACTUAL_DEPART column
                        allStops.First(x => x.UNIQUE_ID == stop.UNIQUE_ID).TIME_ACTUAL_DEPART = succeddingTime_Arrive.AddSeconds(-120 * segmentMiles);
                    }
                    // if FIRST_LAST_STOP is 3
                    else
                    {
                        //selecting the preceeding record
                        for (int y = stop.UNIQUE_ID - 1; y > 0; y--)
                        {
                            if (allStops.First(x => x.UNIQUE_ID == y).TIMEPOINT == -1 && allStops.First(x => x.UNIQUE_ID == y).TIME_ACTUAL_DEPART != null && allStops.First(x => x.UNIQUE_ID == y).STOP_ID != 99999)
                            {
                                // allStops.First(x => x.UNIQUE_ID == stop.UNIQUE_ID).TIME_ACTUAL_ARRIVE  
                                predeedingTime_Arrive = allStops.First(x => x.UNIQUE_ID == y).TIME_ACTUAL_DEPART ?? DateTime.Now;
                                break;
                            }
                        }
                        //segment miles of the record having  TIME_ACTUAL_ARRIVE NULL
                        segmentMiles = (allStops.First(x => x.UNIQUE_ID == stop.UNIQUE_ID).SEGMENT_MILES) ?? segmentMiles;
                        //updating the TIME_ACTUAL_ARRIVE column
                        allStops.First(x => x.UNIQUE_ID == stop.UNIQUE_ID).TIME_ACTUAL_ARRIVE = predeedingTime_Arrive.AddSeconds(120 * segmentMiles);
                        //updating TIME_ACTUAL_DEPART column
                        allStops.First(x => x.UNIQUE_ID == stop.UNIQUE_ID).TIME_ACTUAL_DEPART = predeedingTime_Arrive.AddSeconds(120 * segmentMiles);
                    }

                    allStops.First(x => x.UNIQUE_ID == stop.UNIQUE_ID).MODIFIED_FLAG = true;
                }

                entities.SaveChanges();
            }
        }
    }
}
