using System;
using System.Collections.Generic;
using System.Text;
using TUIO;

namespace Lbi
{
    namespace Macys
    {

        public class CalibrationPoint
        {
            public float x, y;
            public long symbolId;
            public CalibrationPoint(float _x, float _y, long _symbolId)
            {
                x = _x; y = _y; symbolId = _symbolId;
            }
        }
        /**
         * Point block analyzer takes a sequence of fiduciary Markers
         * and arranges them to  a grid and computes the edge  as approached from some specific direction.
         * The direction representing the body is refered to as the affinity.
         * represented as [-1  or 1]  .
         * -1 => left,1 => right in the X
         * -1 => top , 1 => bottom in Y
         * Essentiall if one where to scan form the affinity side ot the non affinity side
         * what will the first X be.
         */
          
        public class PointBlockAnalyzer
        {
            //Fiduciary Symbol Ids
            long  startId = 0;
            long endId = 0;
            float x, y;
            bool good=false;
            //Number of columns in a block and rows  in a block.
            int blockColCount = 3;
            int blockRowCount = 8;
            //Final Position for the point.
            public float X { get { return x; } set { x = value; } }
            public float Y { get { return y; } set { y = value; } }
            public bool Good { get { return good; } set { good = value; }  }
            int _xAff=-1, _yAff=-1;

            
            public PointBlockAnalyzer(long start,long end ,int xAff,int yAff)
            {
                x = y = -1;
                good = false;
                startId = start;
                endId=end;
                updateBlockFrame();
                _xAff = xAff;
                _yAff = yAff;
            }

            /**
             * Recompute the sequence of Ids we should check for this segment.
             */
            void updateBlockFrame()
            {
                //endId = blockColCount * blockRowCount + startId;
            }


            void updateDemoObject(TuioDemoObject o)
            {
                TuioPoint p=o.getPosition();
                switch (_xAff)
                {
                    case -1:
                        if (x < 0 || p.getX() < x) x = p.getX();
                        break;
                    case 1:
                        if (x>1 || p.getX() >x) x=p.getX();
                        break;
                }
                switch (_yAff)
                {
                    case -1:
                        if (y < 0 || p.getY() < y) x = p.getY();
                        break;
                    case 1:
                        if (y > 1 || p.getY() > y) y = p.getY();
                        break;
                }
                good = (x >= 0.0 && y >= 0.0);
                
            }

            /**
             * Given current positions and the 
             * list of calibrated locations spit out and X and Y position 
             * for the points on the screen.
             */
            public void update(Dictionary<long,TuioDemoObject> items,
                Dictionary<long,CalibrationPoint> refData){
                x = y = -1;
                good = false;
                long start = startId;
                long end = endId;
                float xTotal=0.0f, yTotal = 0.0f;
                //Set of elements we can still see.
                Dictionary<long, bool> contained = new Dictionary<long, bool>();
                foreach(TuioDemoObject o in items.Values ){

                    if (o.getSymbolID() < start || o.getSymbolID() > end) continue;
                    
                    //If the identifer exists then 
                    contained[o.getSymbolID()] = true;
                }
                //Calculate a Linear Centroid of the elements that are covered.

                int count = 0;
                foreach (CalibrationPoint p in refData.Values)
                {
                    if (p.symbolId < start || p.symbolId > end) continue;
                    if (contained.ContainsKey(p.symbolId)) continue;
                    xTotal += p.x;
                    yTotal += p.y;
                    count++;
                }
                good = count > 3;
                x = xTotal / count;
                y = yTotal / count;
                
            }



        }
    }
}
