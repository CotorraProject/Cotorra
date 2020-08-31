using Cotorra.Core.Utils;
using System;
using System.Collections.Generic; 

namespace Cotorra.Core
{
    public static class FiscalStringsUtils
    {
        public static List<string> GenerateRFCs(int laps)
        {
            List<string> Generates = new List<string>();

            for (int i = 0; i < laps; i++)
            {               
                var num = RandomSecure.RandomIntFromRNG(65, 90);
                char let = (char)(num);
                num = RandomSecure.RandomIntFromRNG(65, 90);
                char let2 = (char)(num);
                num = RandomSecure.RandomIntFromRNG(65, 90);
                char let3 = (char)(num);
                num = RandomSecure.RandomIntFromRNG(65, 90);
                char let4 = (char)(num);
                num = RandomSecure.RandomIntFromRNG(0, 99);
                var stryear = num < 10 ? "0" + num : num.ToString();
                num = RandomSecure.RandomIntFromRNG(1, 12);
                var strmonth = num < 10 ? "0" + num : num.ToString();
                num = RandomSecure.RandomIntFromRNG(1, 28);
                var streay = num < 10 ? "0" + num : num.ToString();
                num = RandomSecure.RandomIntFromRNG(65, 90);
                char let5 = (char)(num);
                num = RandomSecure.RandomIntFromRNG(65, 90);
                char let6 = (char)(num);
                num = RandomSecure.RandomIntFromRNG(65, 90);
                char let7 = (char)(num);

                Generates.Add($"{let}{let2}{let3}{let4}{stryear}{strmonth}{streay}{let5}{let6}{let7}");

            }
            return Generates;
        }

        public static List<string> GenerateCURPs(int laps)
        {

            List<string> Generates = new List<string>();

            for (int i = 0; i < laps; i++)
            {               
                var num = RandomSecure.RandomIntFromRNG(65, 90);
                char let = (char)(num);
                num = RandomSecure.RandomIntFromRNG(65, 90);
                char let2 = (char)(num);
                num = RandomSecure.RandomIntFromRNG(65, 90);
                char let3 = (char)(num);
                num = RandomSecure.RandomIntFromRNG(65, 90);
                char let4 = (char)(num);
                num = RandomSecure.RandomIntFromRNG(0, 99);
                var stryear = num < 10 ? "0" + num : num.ToString();
                num = RandomSecure.RandomIntFromRNG(1, 12);
                var strmonth = num < 10 ? "0" + num : num.ToString();
                num = RandomSecure.RandomIntFromRNG(1, 28);
                var streay = num < 10 ? "0" + num : num.ToString();
                num = RandomSecure.RandomIntFromRNG(65, 90);
                char let5 = (char)(num);
                num = RandomSecure.RandomIntFromRNG(65, 90);
                char let6 = (char)(num);
                num = RandomSecure.RandomIntFromRNG(65, 90);
                char let7 = (char)(num);
                num = RandomSecure.RandomIntFromRNG(65, 90);
                char let8 = (char)(num);
                num = RandomSecure.RandomIntFromRNG(65, 90);
                char hc1 = (char)(num);
                num = RandomSecure.RandomIntFromRNG(65, 90);
                char hc2 = (char)(num);
                num = RandomSecure.RandomIntFromRNG(0, 99);
                var gc3 = num < 10 ? "0" + num : num.ToString();


                Generates.Add($"{let}{let2}{let3}{let4}{stryear}{strmonth}{streay}{let5}{let6}{let7}{let8}{hc1}{hc2}{gc3}");

            }
            return Generates;
        }

        public static List<string> GenerateNSS(int laps)
        {

            List<string> Generates = new List<string>();

            for (int i = 0; i < laps; i++)
            {              
                var num = RandomSecure.RandomIntFromRNG(1, 9);
                var n1 = num.ToString();
                num = RandomSecure.RandomIntFromRNG(1, 9);
                var n2 = num.ToString();
                num = RandomSecure.RandomIntFromRNG(1, 9);
                var n3 = num.ToString();
                var n4 = num.ToString();
                num = RandomSecure.RandomIntFromRNG(1, 9);
                var n5 = num.ToString();
                num = RandomSecure.RandomIntFromRNG(1, 9);
                var n6 = num.ToString();
                var n7 = num.ToString();
                num = RandomSecure.RandomIntFromRNG(1, 9);
                var n8 = num.ToString();
                num = RandomSecure.RandomIntFromRNG(1, 9);
                var n9 = num.ToString();



                Generates.Add($"{n1}{n2}{n3}{n4}{n5}{n6}{n7}{n8}{n9}");

            }
            return Generates;
        }
    }
}
