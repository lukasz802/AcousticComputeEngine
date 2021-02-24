using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Compute_Engine
{
    #region Public methods

    public static class MathOperation
    {
        public static double DecibelAdd(double db1, double db2)
        {
            return 10 * Math.Log10(Math.Pow(10, db1 / 10) + Math.Pow(10, db2 / 10));
        }

        public static double[] OctaveAdd(double[] db1, double[] db2)
        {
            double[] val = new double[8];

            for (int i = 0; i < val.Length; i++)
            {
                val[i] = db1[i] + db2[i];
            }
            return val;
        }

        public static double[] OctaveMinus(double[] db1, double[] db2)
        {
            double[] val = new double[8];

            for (int i = 0; i < val.Length; i++)
            {
                val[i] = db1[i] - db2[i];
            }
            return val;
        }

        public static double[] OctaveDecibelAdd(double[] db1, double[] db2)
        {
            double[] val = new double[8];

            for (int i = 0; i < val.Length; i++)
            {
                val[i] = 10 * Math.Log10(Math.Pow(10, db1[i] / 10) + Math.Pow(10, db2[i] / 10));
            }
            return val;
        }

        public static double OctaveSum(params double[] oct)
        {
            double val = -1000;

            for (int i = 0; i < oct.Length; i++)
            {
                val = 10 * Math.Log10(Math.Pow(10, val / 10) + Math.Pow(10, oct[i] / 10));
            }
            return val;
        }

        public static double DecibelMinus(double db1, double db2)
        {
            return 10 * Math.Log10(Math.Pow(10, db1 / 10) - Math.Pow(10, db2 / 10));
        }

        public static double DecibelMultiply(double dbBase, double dbMultiplier)
        {
            return 10 * Math.Log10(Math.Pow(10, dbBase / 10) * dbMultiplier);
        }

        public static double DecibelDevide(double dbBase, double dbDevider)
        {
            return 10 * Math.Log10(Math.Pow(10, dbBase / 10) / dbDevider);
        }

        public static double CalcResult(List<string> row)
        {
            double result;
            List<string> temp = new List<string>();
            string last_element = string.Empty;

            if (row.Count <= 3) { return Convert.ToDouble(row[0]); }
            else
            {
                for (int i = 0; i < row.Count; i++)
                {
                    if (row[i].Contains("×"))
                    {
                        if (last_element == "×" || last_element == "÷")
                        {
                            temp[temp.Count - 1] = DecibelMultiply(Convert.ToDouble(temp[temp.Count - 1]), Convert.ToDouble(row[i + 1])).ToString();
                        }
                        else 
                        {
                            temp.Add(DecibelMultiply(Convert.ToDouble(row[i - 1]), Convert.ToDouble(row[i + 1])).ToString());
                        }
                        last_element = "×";
                        i++;
                    }
                    else if (row[i].Contains("÷"))
                    {
                        if (last_element == "×" || last_element == "÷")
                        {
                            temp[temp.Count - 1] = DecibelDevide(Convert.ToDouble(temp[temp.Count - 1]), Convert.ToDouble(row[i + 1])).ToString();
                        }
                        else
                        {
                            temp.Add(DecibelDevide(Convert.ToDouble(row[i - 1]), Convert.ToDouble(row[i + 1])).ToString());
                        }
                        last_element = "÷";
                        i++;
                    }
                    else if ((row[i].Contains("+") || row[i].Contains("-")) && !Regex.IsMatch(row[i], @"[0-9]"))
                    {
                        if (i == 1) { temp.Add(row[i - 1]); }
                        temp.Add(row[i]);
                        temp.Add(row[i + 1]);
                        last_element = "÷";
                        i++;
                    }
                }

                result = Convert.ToDouble(temp[0]);
                for (int i = 1; i < temp.Count; i++)
                {
                    if (temp[i].Contains("+") && !Regex.IsMatch(temp[i], @"[0-9]"))
                    {
                        result = DecibelAdd(result, Convert.ToDouble(temp[i + 1]));
                    }
                    else if (temp[i].Contains("-") && !Regex.IsMatch(temp[i], @"[0-9]"))
                    {
                        result = DecibelMinus(result, Convert.ToDouble(temp[i + 1]));
                    }
                }
                return result;
            }
        }
    }

    #endregion
}
