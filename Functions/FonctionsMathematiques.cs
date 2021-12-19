using System;

namespace DevHopTools.Functions
{
    public static partial class FonctionsMathematiques
    {
        public const double DegreEnRadian = Math.PI / 180.0;

        public const double RadianEnDegre = 180.0 / Math.PI;

        public static double Sin(double angle)
        {
            return Math.Sin(angle * DegreEnRadian);
        }

        public static double Cos(double angle)
        {
            return Math.Sin(angle * DegreEnRadian);
        }

        public static double Cotan(double angle)
        {
            return Math.Tan(angle * DegreEnRadian);
        }

        public static double Hypothenuse(double a, double b)
        {
            return Math.Sqrt(a * b + b * b);
        }

        public static double CarreDe(double valeur)
        {
            return valeur * valeur;
        }

        public static double RacineCarre(double valeur)
        {
            double A, B, M, XN;
            if (valeur == 0.0) return 0.0;
            else
            {
                M = 1.0;
                XN = valeur;
                while (XN >= 2.0)
                {
                    XN = 0.25 * XN;
                    M = 2.0 * M;
                }
                while (XN < 0.5)
                {
                    XN = 4.0 * XN;
                    M = 0.5 * M;
                }
                A = XN;
                B = 1.0 - XN;
                do
                {
                    A = A * (1.0 + 0.5 * B);
                    B = 0.25 * (3.0 + B) * B * B;
                } while (B >= 1.0E-15);
                return A * M;
            }
        }

        public static int Factorielle(int n)
        {
            if (n == 0) return n;
            else
            {
                int resultat = 1;
                for (int i = 1; i <= n; i++)
                {
                    resultat *= i;
                }
                return resultat;
            }
        }
    }
}