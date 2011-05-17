using System;
using GeoAPI.Geometries;
using GisSharpBlog.NetTopologySuite.Geometries;
using GisSharpBlog.NetTopologySuite.IO;
using GisSharpBlog.NetTopologySuite.Utilities;
using GisSharpBlog.NetTopologySuite.Geometries.Utilities;

namespace NetTopologySuite.Tests.NUnit.Geometries.Prepared
{
    public abstract class StressTestHarness
    {
        const int MAX_ITER = 10000;

        static readonly PrecisionModel pm = new PrecisionModel();
        static readonly GeometryFactory fact = new GeometryFactory(pm, 0);
        static WKTReader _wktRdr = new WKTReader(fact);
        static WKTWriter _wktWriter = new WKTWriter();

        private int _numTargetPts = 1000;

        public StressTestHarness()
        {
        }

        public int TargetSize
        {
            get { return _numTargetPts; }
            set { _numTargetPts = value; }
        }

        public void Run(int nIter)
        {
            Console.WriteLine("Running " + nIter + " tests");
            //  	Geometry poly = createCircle(new Coordinate(0, 0), 100, nPts);
            IGeometry poly = CreateSineStar(new Coordinate(0, 0), 100, _numTargetPts);
            Console.WriteLine(poly);

            Console.WriteLine();
            //System.out.println("Running with " + nPts + " points");
            Run(nIter, poly);
        }

        static IGeometry CreateCircle(ICoordinate origin, double size, int nPts)
        {
            GeometricShapeFactory gsf = new GeometricShapeFactory();
            gsf.Centre = origin;
            gsf.Size = size;
            gsf.NumPoints = nPts;
            IGeometry circle = gsf.CreateCircle();
            // Polygon gRect = gsf.createRectangle();
            // Geometry g = gRect.getExteriorRing();
            return circle;
        }

        static IGeometry CreateSineStar(Coordinate origin, double size, int nPts)
        {
            SineStarFactory gsf = new SineStarFactory();
            gsf.Centre = origin;
            gsf.Size = size;
            gsf.NumPoints = nPts;
            gsf.ArmLengthRatio = 0.1;
            gsf.NumArms = 20;
            IGeometry poly = gsf.CreateSineStar();
            return poly;
        }

        static IGeometry CreateRandomTestGeometry(IEnvelope env, double size, int nPts)
        {
            Random rnd = new Random(1);
            double width = env.Width;
            double xOffset = width * rnd.NextDouble();
            double yOffset = env.Height * rnd.NextDouble();
            ICoordinate basePt = new Coordinate(
                            env.MinX + xOffset,
                            env.MinY + yOffset);
            IGeometry test = CreateTestCircle(basePt, size, nPts);
            if (test is IPolygon && rnd.NextDouble() > 0.5)
            {
                test = test.Boundary;
            }
            return test;
        }

        static IGeometry CreateTestCircle(ICoordinate origin, double size, int nPts)
        {
            GeometricShapeFactory gsf = new GeometricShapeFactory();
            gsf.Centre = origin;
            gsf.Size = size;
            gsf.NumPoints = nPts;
            IGeometry circle = gsf.CreateCircle();
            //    System.out.println(circle);
            return circle;
        }

        public void Run(int nIter, IGeometry target)
        {
            int count = 0;
            while (count < nIter)
            {
                count++;
                IGeometry test = CreateRandomTestGeometry(target.EnvelopeInternal, 10, 20);

                //      System.out.println("Test # " + count);
                //  		System.out.println(line);
                Console.WriteLine("Test[" + count + "] " + target.GetType().FullName + "/" + test.GetType().FullName);
                bool isResultCorrect = CheckResult(target, test);
                if (!isResultCorrect)
                {
                    throw new Exception("Invalid result found");
                }
            }
        }

        public abstract bool CheckResult(IGeometry target, IGeometry test);
    }
}