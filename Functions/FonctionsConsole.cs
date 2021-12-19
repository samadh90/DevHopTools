using System;
using System.Globalization;
using System.Linq;

namespace DevHopTools.Functions
{
    public static partial class FonctionsConsole
    {
        private static readonly CultureInfo c_CultureAnglaise = CultureInfo.GetCultureInfo("EN-US");

        private const NumberStyles c_StyleReel = NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint;

        public static void DefinirTailleConsole(int largeur, int hauteur)
        {
            if (largeur > Console.WindowWidth)
            {
                Console.BufferWidth = largeur;
                Console.WindowWidth = Math.Min(largeur, Console.LargestWindowWidth);
            }
            else
            {
                Console.WindowWidth = Math.Min(largeur, Console.LargestWindowWidth);
                Console.BufferWidth = largeur;
            }
            if (hauteur > Console.WindowHeight)
            {
                Console.BufferHeight = hauteur;
                Console.WindowHeight = Math.Min(hauteur, Console.LargestWindowHeight);
            }
            else
            {
                Console.WindowHeight = Math.Min(hauteur, Console.LargestWindowHeight);
                Console.BufferHeight = hauteur;
            }
        }

        public static void DefinirTailleConsole(int largeur, int hauteur, int largeurContenu, int hauteurContenu)
        {
            DefinirTailleConsole(largeur, hauteur);
            if (largeurContenu > largeur) Console.BufferWidth = largeurContenu;
            if (hauteurContenu > hauteur) Console.BufferHeight = hauteurContenu;
        }

        public static void DefinirCouleurs(ConsoleColor couleurFond, ConsoleColor couleurTexte)
        {
            Console.BackgroundColor = couleurFond;
            Console.ForegroundColor = couleurTexte;
        }

        public static void EffacerEcran()
        {
            Console.Clear();
        }

        public static void EffacerEcran(ConsoleColor couleurFond)
        {
            Console.BackgroundColor = couleurFond;
            Console.Clear();
        }

        public static void EffacerEcran(ConsoleColor couleurFond, ConsoleColor couleurTexte)
        {
            Console.BackgroundColor = couleurFond;
            Console.ForegroundColor = couleurTexte;
            Console.Clear();
        }

        public static string GererMenu(params string[] points)
        {
            if ((points == null) || (points.Length == 0)) return string.Empty;

            int nombrePoints = 0;
            for (int indice = 0; indice < points.Length; indice++)
            {
                if (!string.IsNullOrWhiteSpace(points[indice]))
                {
                    nombrePoints++;
                }
            }
            if (nombrePoints == 0) return string.Empty;

            string formatPointMenu = string.Format("{{0,{0}}} - {{1}}", nombrePoints.ToString().Length);
            // ---> Exemple de format produit par l'instruction précédente sur un tableau comprenant entre 10 et 99 points acceptables : "{0,2} - {1}"

            int numero = 0;
            for (int indice = 0; indice < points.Length; indice++)
            {
                if (!string.IsNullOrWhiteSpace(points[indice]))
                {
                    numero++;
                    Console.WriteLine(formatPointMenu, numero, points[indice]);
                }
            }

            int numeroChoisi = EncoderEntier(1, nombrePoints);

            numero = 0;
            for (int indice = 0; indice < points.Length; indice++)
            {
                if (!string.IsNullOrWhiteSpace(points[indice]))
                {
                    numero++;
                    if (numero == numeroChoisi)
                    {
                        return points[indice];
                    }
                }
            }
            return string.Empty;
        }

        public static string EncoderLibelle(bool supprimerEspacesSuperflus = true, int longueurMinimale = 0, int longueurMaximale = int.MaxValue, bool lettresAutorisees = true, bool chiffresAutorises = true, bool espaceAutorise = true, bool ponctuationAutorisee = true, string caracteresAutorises = null)
        {
            while (true)
            {
                Console.Write("> ");
                string encodage = Console.ReadLine();
                if (supprimerEspacesSuperflus) encodage = encodage.Trim();
                if (encodage.Length < longueurMinimale)
                {
                    string caractere = (longueurMinimale >= 2) ? "s" : "";
                    Console.WriteLine($"Vous devez encoder un texte d'au moins {longueurMinimale} caractère{caractere} !");
                }
                else if (encodage.Length > longueurMaximale)
                {
                    string caractere = (longueurMinimale >= 2) ? "s" : "";
                    Console.WriteLine($"Vous devez encoder un texte d'au plus {longueurMaximale} caractère{caractere} !");
                }
                else
                {
                    if (!lettresAutorisees || !chiffresAutorises || !espaceAutorise || !ponctuationAutorisee || (caracteresAutorises != null))
                    {
                        bool erreurPresente = false;
                        for (int indice = 0; indice < encodage.Length; indice++)
                        {
                            char caractere = encodage[indice];
                            if (FonctionsChaines.EstLettre(caractere))
                            {
                                if (!lettresAutorisees && ((caracteresAutorises == null) || !caracteresAutorises.Contains(caractere)))
                                {
                                    erreurPresente = true;
                                    break;
                                }
                            }
                            else if (FonctionsChaines.EstChiffre(caractere))
                            {
                                if (!chiffresAutorises && ((caracteresAutorises == null) || !caracteresAutorises.Contains(caractere)))
                                {
                                    erreurPresente = true;
                                    break;
                                }
                            }
                            else if ((caractere == ' ') && ((caracteresAutorises == null) || !caracteresAutorises.Contains(caractere)))
                            {
                                if (!espaceAutorise)
                                {
                                    erreurPresente = true;
                                    break;
                                }
                            }
                            else if (".,;:-?!".Contains(caractere))
                            {
                                if (!ponctuationAutorisee && ((caracteresAutorises == null) || !caracteresAutorises.Contains(caractere)))
                                {
                                    erreurPresente = true;
                                    break;
                                }
                            }
                            else if ((caracteresAutorises == null) || !caracteresAutorises.Contains(caractere))
                            {
                                erreurPresente = true;
                                break;
                            }
                        }
                        if (erreurPresente)
                        {
                            Console.WriteLine("Vous ne devez encoder que des caractères autorisés !");
                        }
                        else
                        {
                            return encodage;
                        }
                    }
                    else
                    {
                        return encodage;
                    }
                }
            }
        }

        public static sbyte EncoderEntierB(string messageInvite)
        {
            return EncoderEntierB(messageInvite, sbyte.MinValue, sbyte.MaxValue, true);
        }

        public static sbyte EncoderEntierB(string messageInvite, bool zeroAutorise)
        {
            return EncoderEntierB(messageInvite, sbyte.MinValue, sbyte.MaxValue, zeroAutorise);
        }

        public static sbyte EncoderEntierB(string messageInvite, sbyte borneMinimale, sbyte borneMaximale)
        {
            return EncoderEntierB(messageInvite, borneMinimale, borneMaximale, true);
        }

        public static sbyte EncoderEntierB(string messageInvite, sbyte borneMinimale, sbyte borneMaximale, bool zeroAutorise)
        {
            ConsoleColor couleurTexteParDefaut = Console.ForegroundColor;
            Console.WriteLine(messageInvite);
            while (true)
            {
                Console.Write("> ");
                string chaineEncodee = Console.ReadLine();
                sbyte valeurConvertie;
                if (sbyte.TryParse(chaineEncodee.Trim(), out valeurConvertie))
                {
                    if (valeurConvertie < borneMinimale)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"Vous auriez du encoder une valeur plus grande ou égale à {borneMinimale}");
                    }
                    else if (valeurConvertie > borneMaximale)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"Vous auriez du encoder une valeur plus petite ou égale à {borneMaximale}");
                    }
                    else if (!zeroAutorise && valeurConvertie == 0)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Vous auriez du encoder une valeur non null !");
                    }
                    else
                    {
                        return valeurConvertie;
                    }
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Vous auriez du encoder une valeur entière !");
                }
                Console.ForegroundColor = couleurTexteParDefaut;
                Console.WriteLine("Veuillez recommencer cet encodage :");
            }
        }

        public static short EncoderEntierS(string messageInvite)
        {
            return EncoderEntierS(messageInvite, short.MinValue, short.MaxValue, true);
        }

        public static short EncoderEntierS(string messageInvite, bool zeroAutorise)
        {
            return EncoderEntierS(messageInvite, short.MinValue, short.MaxValue, zeroAutorise);
        }

        public static short EncoderEntierS(string messageInvite, short borneMinimale, short borneMaximale)
        {
            return EncoderEntierS(messageInvite, borneMinimale, borneMaximale, true);
        }

        public static short EncoderEntierS(string messageInvite, short borneMinimale, short borneMaximale, bool zeroAutorise)
        {
            ConsoleColor couleurTexteParDefaut = Console.ForegroundColor;
            Console.WriteLine(messageInvite);
            while (true)
            {
                Console.Write("> ");
                string chaineEncodee = Console.ReadLine();
                short valeurConvertie;
                if (short.TryParse(chaineEncodee.Trim(), out valeurConvertie))
                {
                    if (valeurConvertie < borneMinimale)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"Vouz auriez du encoder une valeur plus grande ou égale à {borneMinimale}");
                    }
                    else if (valeurConvertie > borneMaximale)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"Vouz auriez du encoder une valeur plus petite ou égale à {borneMaximale}");
                    }
                    else if (!zeroAutorise && valeurConvertie == 0)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Vous auriez du encoder uen valeur non null !");
                    }
                    else
                    {
                        return valeurConvertie;
                    }
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Vous auriez du encoder une valeur entière !");
                }
                Console.ForegroundColor = couleurTexteParDefaut;
                Console.WriteLine("Veuillez recommencer cet encodage :");
            }
        }

        public static int EncoderEntier(string messageInvite)
        {
            return EncoderEntier(messageInvite, int.MinValue, int.MaxValue, true);
        }

        public static int EncoderEntier(string messageInvite, bool zeroAutorise)
        {
            return EncoderEntier(messageInvite, int.MinValue, int.MaxValue, zeroAutorise);
        }

        public static int EncoderEntier(string messageInvite, int borneMinimale, int borneMaximale)
        {
            return EncoderEntier(messageInvite, borneMinimale, borneMaximale, true);
        }

        public static int EncoderEntier(string messageInvite, int borneMinimale, int borneMaximale, bool zeroAutorise)
        {
            ConsoleColor couleurTexteParDefaut = Console.ForegroundColor;
            Console.WriteLine(messageInvite);
            while (true)
            {
                Console.Write("> ");
                string chaineEncodee = Console.ReadLine();
                int valeurConvertie;
                if (int.TryParse(chaineEncodee.Trim(), out valeurConvertie))
                {
                    if (valeurConvertie < borneMinimale)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"Vouz auriez du encoder une valeur plus grande ou égale à {borneMinimale}");
                    }
                    else if (valeurConvertie > borneMaximale)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"Vouz auriez du encoder une valeur plus petite ou égale à {borneMaximale}");
                    }
                    else if (!zeroAutorise && valeurConvertie == 0)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Vous auriez du encoder uen valeur non null !");
                    }
                    else
                    {
                        return valeurConvertie;
                    }
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Vous auriez du encoder une valeur entière !");
                }
                Console.ForegroundColor = couleurTexteParDefaut;
                Console.WriteLine("Veuillez recommencer cet encodage :");
            }
        }

        private static int EncoderEntier(int BorneMinimale = int.MinValue, int BorneMaximale = int.MaxValue)
        {
            while (true)
            {
                Console.Write("> ");

                string Encodage = Console.ReadLine().Trim();

                int Valeur;

                if (!int.TryParse(Encodage, out Valeur))
                {
                    Console.WriteLine("Vous devez encoder une valeur entière !");
                }
                else
                {
                    if (Valeur < BorneMinimale)
                    {
                        Console.WriteLine("Vous devez encoder une valeur plus grande ou égale à {0} !", BorneMinimale);
                    }
                    else if (Valeur > BorneMaximale)
                    {
                        Console.WriteLine("Vous devez encoder une valeur plus petite ou égale à {0} !", BorneMaximale);
                    }
                    else
                    {
                        return Valeur;
                    }
                }
            }
        }

        public static long EncoderEntierL(string messageInvite)
        {
            return EncoderEntierL(messageInvite, long.MinValue, long.MaxValue, true);
        }

        public static long EncoderEntierL(string messageInvite, bool zeroAutorise)
        {
            return EncoderEntierL(messageInvite, long.MinValue, long.MaxValue, zeroAutorise);
        }

        public static long EncoderEntierL(string messageInvite, long borneMinimale, long borneMaximale)
        {
            return EncoderEntierL(messageInvite, borneMinimale, borneMaximale, true);
        }

        public static long EncoderEntierL(string messageInvite, long borneMinimale, long borneMaximale, bool zeroAutorise)
        {
            ConsoleColor couleurTexteParDefaut = Console.ForegroundColor;
            Console.WriteLine(messageInvite);
            while (true)
            {
                Console.Write("> ");
                string chaineEncodee = Console.ReadLine();
                long valeurConvertie;
                if (long.TryParse(chaineEncodee.Trim(), out valeurConvertie))
                {
                    if (valeurConvertie < borneMinimale)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"Vouz auriez du encoder une valeur plus grande ou égale à {borneMinimale}");
                    }
                    else if (valeurConvertie > borneMaximale)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"Vouz auriez du encoder une valeur plus petite ou égale à {borneMaximale}");
                    }
                    else if (!zeroAutorise && valeurConvertie == 0)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Vous auriez du encoder uen valeur non null !");
                    }
                    else
                    {
                        return valeurConvertie;
                    }
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Vous auriez du encoder une valeur entière !");
                }
                Console.ForegroundColor = couleurTexteParDefaut;
                Console.WriteLine("Veuillez recommencer cet encodage :");
            }
        }

        public static decimal EncoderDecimal(string messageInvite)
        {
            return EncoderDecimal(messageInvite, decimal.MinValue, decimal.MaxValue, true);
        }

        public static decimal EncoderDecimal(string messageInvite, bool zeroAutorise)
        {
            return EncoderDecimal(messageInvite, decimal.MinValue, decimal.MaxValue, zeroAutorise);
        }

        public static decimal EncoderDecimal(string messageInvite, decimal borneMinimale, decimal borneMaximale)
        {
            return EncoderDecimal(messageInvite, borneMinimale, borneMaximale, true);
        }

        public static decimal EncoderDecimal(string messageInvite, decimal bornMinimale, decimal borneMaximale, bool zeroAutorise)
        {
            ConsoleColor couleurTexteParDefaut = Console.ForegroundColor;
            Console.WriteLine(messageInvite);
            while (true)
            {
                Console.Write("> ");
                string chaineEncodee = Console.ReadLine().Trim();
                decimal valeurConvertie;
                if (decimal.TryParse(chaineEncodee.Replace(',', '.'), c_StyleReel, c_CultureAnglaise, out valeurConvertie))
                {
                    if (valeurConvertie < bornMinimale)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"Vous auriez du encoder une valeur plus grande ou égale à {bornMinimale}");
                    }
                    else if (valeurConvertie > borneMaximale)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"Vous auriez du encoder une valeur plus petite ou égale à {borneMaximale}");
                    }
                    else if (!zeroAutorise && valeurConvertie == 0m)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Vous auriez du encoder une valeur non nulle !");
                    }
                    else
                    {
                        return valeurConvertie;
                    }
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Vous auriez du encoder une valeur numérique");
                }
                Console.ForegroundColor = couleurTexteParDefaut;
                Console.WriteLine("Veuillez recommencer cet encodage :");
            }
        }

        public static decimal EncoderMontant(string messageInvite, bool AutoriserZero = true, decimal BorneMinimale = decimal.MinValue, bool BorneMinimaleComprise = true, decimal BorneMaximale = decimal.MaxValue, bool BorneMaximaleComprise = true, int MaximumChiffresDecimaux = 2)
        {
            ConsoleColor couleurTexteParDefaut = Console.ForegroundColor;
            Console.WriteLine(messageInvite);
            while (true)
            {
                Console.Write("> ");
                string Encodage = Console.ReadLine().Trim().Replace(',', '.');

                decimal Valeur;

                if (!decimal.TryParse(Encodage, NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint, CultureInfo.GetCultureInfo("EN-US"), out Valeur))
                {
                    Console.WriteLine("Vous devez encoder une valeur monétaire !");
                }
                else
                {
                    int NombreChiffresDecimaux = 0;
                    int PositionSeparateur = Encodage.IndexOf('.');
                    if (PositionSeparateur >= 0)
                    {
                        /*
                                              0 1 2 3 4 5 6
                                             +-+-+-+-+-+-+-+
                        Exemple : Encodage = |5|2|7|8|.|9|5|
                                             +-+-+-+-+-+-+-+
                                  PositionSeparateur = 4
                                  Encodage.Length = 7
                                  NombreChiffresDecimaux = Encodage.Length - PositionSeparateur - 1 = 7 - 4 - 1 = 2
                        */
                        NombreChiffresDecimaux = Encodage.Length - PositionSeparateur - 1;
                    }
                    if (NombreChiffresDecimaux > MaximumChiffresDecimaux)
                    {
                        Console.WriteLine("Vous devez encoder une valeur monétaire avec un maximum de {0} chiffre{1} dans la partie décimale !",
                            MaximumChiffresDecimaux,
                            (MaximumChiffresDecimaux >= 2) ? "s" : "");
                    }
                    else if (Valeur < BorneMinimale)
                    {
                        Console.WriteLine("Vous devez encoder une valeur {0} à {1} !",
                            BorneMinimaleComprise ? "supérieure ou égale" : "strictement supérieure",
                            BorneMinimale);
                    }
                    else if ((Valeur == BorneMinimale) && (!BorneMinimaleComprise))
                    {
                        Console.WriteLine("Vous devez encoder une valeur strictement supérieure à {0} !", BorneMinimale);
                    }
                    else if (Valeur > BorneMaximale)
                    {
                        Console.WriteLine("Vous devez encoder une valeur {0} à {1} !",
                            BorneMaximaleComprise ? "inférieure ou égale" : "strictement inférieure",
                            BorneMaximale);
                    }
                    else if ((Valeur == BorneMaximale) && (!BorneMaximaleComprise))
                    {
                        Console.WriteLine("Vous devez encoder une valeur strictement inférieure à {0} !", BorneMaximale);
                    }
                    else if ((Valeur == 0.0m) && (!AutoriserZero))
                    {
                        Console.WriteLine("Vous devez encoder une valeur différente de zéro !");
                    }
                    else
                    {
                        return Valeur;
                    }
                }
            }
        }

        public static double EncoderReel(string messageInvite)
        {
            return EncoderReel(messageInvite, double.MinValue, double.MaxValue, true);
        }

        public static double EncoderReel(string messageInvite, bool zeroAutorise)
        {
            return EncoderReel(messageInvite, double.MinValue, true, double.MaxValue, true, zeroAutorise);
        }

        public static double EncoderReel(string messageInvite, double borneMinimale, double borneMaximale)
        {
            return EncoderReel(messageInvite, borneMinimale, true, borneMaximale, true, true);
        }

        public static double EncoderReel(string messageInvite, double borneMinimale, double borneMaximale, bool zeroAutorise)
        {
            return EncoderReel(messageInvite, borneMinimale, true, borneMaximale, true, zeroAutorise);
        }

        public static double EncoderReel(string messageInvite, double borneMinimale, bool borneMinimaleComprise, double borneMaximale, bool borneMaximaleComprise, bool zeroAutorise)
        {
            ConsoleColor couleurTexteParDefaut = Console.ForegroundColor;
            Console.WriteLine(messageInvite);
            while (true)
            {
                Console.Write("> ");
                string chaineEncodee = Console.ReadLine().Trim();
                double valeurConvertie;
                if (double.TryParse(chaineEncodee.Replace(',', '.'), c_StyleReel, c_CultureAnglaise, out valeurConvertie))
                {
                    if (borneMinimaleComprise && (valeurConvertie < borneMinimale))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"Vous auriez du encoder une valeur plus grande ou égale à {borneMinimale}");
                    }
                    else if (!borneMinimaleComprise && (valeurConvertie <= borneMinimale))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"Vous auriez du encoder une valeur strictement plus grande que {borneMinimale} !");
                    }
                    else if (borneMaximaleComprise && (valeurConvertie > borneMaximale))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"Vous auriez du encoder une valeur plus petite ou égale à {borneMaximale}");
                    }
                    else if (!borneMaximaleComprise && (valeurConvertie >= borneMaximale))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"Vous auriez du encoder une valeur strictement plus petite que {borneMaximale} !");
                    }
                    else if (!zeroAutorise && valeurConvertie == 0.0)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Vous auriez du encoder une valeur non null !");
                    }
                    else
                    {
                        return valeurConvertie;
                    }
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Vous auriez du encoder une valeur numérique !");
                }
                Console.ForegroundColor = couleurTexteParDefaut;
                Console.WriteLine("Veuillez recommencer cet encodage :");
            }
        }

        public static float EncoderReelF(string messageInvite)
        {
            return EncoderReelF(messageInvite, float.MinValue, true, float.MaxValue, true, true);
        }

        public static float EncoderReelF(string messageInvite, bool zeroAutorise)
        {
            return EncoderReelF(messageInvite, float.MinValue, true, float.MaxValue, true, zeroAutorise);
        }

        public static float EncoderReelF(string messageInvite, float borneMinimale, float borneMaximale)
        {
            return EncoderReelF(messageInvite, borneMinimale, true, borneMaximale, true, true);
        }

        public static float EncoderReelF(string messageInvite, float borneMinimale, float borneMaximale, bool zeroAutorise)
        {
            return EncoderReelF(messageInvite, borneMinimale, true, borneMaximale, true, zeroAutorise);
        }

        public static float EncoderReelF(string messageInvite, float borneMinimale, bool borneMinimaleComprise, float borneMaximale, bool borneMaximaleComprise, bool zeroAutorise)
        {
            ConsoleColor couleurTexteParDefaut = Console.ForegroundColor;
            Console.WriteLine(messageInvite);
            while (true)
            {
                Console.Write("> ");
                string chaineEncodee = Console.ReadLine().Trim();
                float valeurConvertie;
                if (float.TryParse(chaineEncodee.Replace(',', '.'), c_StyleReel, c_CultureAnglaise, out valeurConvertie))
                {
                    if (borneMinimaleComprise && (valeurConvertie < borneMinimale))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"Vous auriez du encoder une valeur plus grande ou égale à {borneMinimale} !");
                    }
                    else if (!borneMinimaleComprise && (valeurConvertie <= borneMinimale))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"Vous auriez du encoder une valeur strictement plus grande que {borneMinimale} !");
                    }
                    else if (borneMaximaleComprise && (valeurConvertie > borneMaximale))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"Vous auriez du encoder une valeur plus petite ou égale à {borneMaximale} !");
                    }
                    else if (!borneMaximaleComprise && (valeurConvertie >= borneMaximale))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"Vous auriez du encoder une valeur strictement plus petite que {borneMaximale} !");
                    }
                    else if (!zeroAutorise && valeurConvertie == 0.0)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Vous auriez du encoder une valeur non nulle !");
                    }
                    else
                    {
                        return valeurConvertie;
                    }
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Vous auriez du encoder une valeur numérique !");
                }
                Console.ForegroundColor = couleurTexteParDefaut;
                Console.WriteLine("Veuillez recommencer cet encodage :");
            }
        }

        public static DateTime EncoderMoment(string messageInvite)
        {
            return EncoderMoment(messageInvite, DateTime.MinValue, DateTime.MaxValue);
        }

        public static DateTime EncoderMoment(string messageInvite, DateTime borneMinimale)
        {
            return EncoderMoment(messageInvite, borneMinimale, DateTime.MaxValue);
        }

        public static DateTime EncoderMoment(string messageInvite, DateTime borneMinimale, DateTime borneMaximale)
        {
            ConsoleColor couleurTexteParDefaut = Console.ForegroundColor;
            Console.WriteLine(messageInvite);
            while (true)
            {
                Console.Write("> ");
                string chaineEncodee = Console.ReadLine().Trim();
                string[] parties = chaineEncodee.Split('/', ' ', ':');
                if (parties.Length == 5)
                {
                    parties = $"{chaineEncodee}:00".Split('/', ' ', ':');
                }
                if (parties.Length != 6)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Vous auriez du encoder un moment selon le format J/M/A h:m ou J/M/A h:m:s !");
                }
                else
                {
                    int valeur;
                    int[] valeurs = new int[6];
                    for (int indice = 0; indice < valeurs.Length; indice++)
                    {
                        valeurs[indice] = int.TryParse(parties[indice], out valeur) ? valeur : -1;
                    }
                    if ((valeurs[2] < 1) || (valeurs[2] > 9999))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("L'année doit être comprise entre 1 et 9999 !");
                    }
                    else if ((valeurs[1] < 1) || (valeurs[1] > 12))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Le mois doit être compris entre 1 et 12 !");
                    }
                    else if ((valeurs[0] < 1) || (valeurs[0] > DateTime.DaysInMonth(valeurs[2], valeurs[1])))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"Dans ce cas, le jour doit être compris entre 1 et {DateTime.DaysInMonth(valeurs[2], valeurs[1])} !");
                    }
                    else if ((valeurs[3] < 0) || (valeurs[3] > 23))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("L'heure doit être comprise entre 0 et 23 !");
                    }
                    else if ((valeurs[4] < 0) || (valeurs[4] > 59))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Les minutes doivent être comprises entre 0 et 59 !");
                    }
                    else if ((valeurs[5] < 0) || (valeurs[5] > 59))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Les secondes doivent être comprises entre 0 et 59 !");
                    }
                    else
                    {
                        DateTime moment = new DateTime(valeurs[2], valeurs[1], valeurs[0], valeurs[3], valeurs[4], valeurs[5]);
                        if (moment < borneMinimale)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine($"Vous auriez du encoder un moment postérieur ou égal à {borneMinimale.ToString("d/MM/yyyy H:mm:ss").Replace('-', '/')} !");
                        }
                        else if (moment > borneMaximale)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine($"Vous auriez du encoder un moment antérieur ou égal à {borneMaximale.ToString("d/MM/yyyy H:mm:ss").Replace('-', '/')} !");
                        }
                        else
                        {
                            return moment;
                        }
                    }
                }
                Console.ForegroundColor = couleurTexteParDefaut;
                Console.WriteLine("Veuillez recommencer cet encodage :");
            }
        }

        public static bool RepondreOuiNon(string question)
        {
            Console.Write($"{question} (O/N)");
            while (true)
            {
                ConsoleKey touche = Console.ReadKey(true).Key;
                if (touche == ConsoleKey.O)
                {
                    Console.WriteLine("Oui");
                    return true;
                }
                if (touche == ConsoleKey.N)
                {
                    Console.WriteLine("Non");
                    return false;
                }
            }
        }

        public static void AttendreTouche(string explications, ConsoleKey toucheAttendu)
        {
            Console.Write($"\nVeuillez appuyer sur la touche {toucheAttendu} pour {explications}");
            while (Console.ReadKey(true).Key != toucheAttendu) ;
            Console.WriteLine();
        }

        public static int Selectionner(string[] elements, bool autoriserAnnulation = false, int indiceDebut = 0, int nombreElements = -1)
        {
            if ((elements == null) || (nombreElements > elements.Length)) return -1;
            if (nombreElements < 0) nombreElements = elements.Length;
            for (int indice = indiceDebut; indice < nombreElements; indice++)
            {
                Console.WriteLine($"{indice + 1} - {elements[indice]}");
            }
            if (nombreElements == 0) autoriserAnnulation = true;
            int borneMinimale = autoriserAnnulation ? 0 : 1;
            string annulationOptionnelle = autoriserAnnulation ? " (0 pour annuler)" : "";
            return EncoderEntier($"Veuillez encoder le numéro correspondant à votre sélection{annulationOptionnelle} :", borneMinimale, nombreElements) - 1;
        }

        public static void AfficherTitre(string titre, char soulignement = '=', bool centrageFenetre = true)
        {
            int longueurTitre = titre.Length;
            int largeurUtile;
            if (centrageFenetre)
            {
                largeurUtile = Console.WindowWidth - 1;
            }
            else
            {
                largeurUtile = Console.BufferWidth - 1;
            }
            if (longueurTitre > largeurUtile)
            {
                titre = $"{titre.Substring(0, largeurUtile - 4)} ...";
                longueurTitre = titre.Length;
            }
            string espacement = new string(' ', (largeurUtile - longueurTitre + 1) / 2);
            Console.Write(espacement);
            Console.WriteLine(titre);
            Console.Write(espacement);
            Console.WriteLine(new string(soulignement, longueurTitre));
        }

        public static int[] AfficherTableau(int nombreElements, bool affichageNumerotation, string titre, string[] entetesColonnes, string[] formatsDonnees, bool[] alignementGaucheDonnees, params object[] tableaux)
        {
            if ((nombreElements < 0)
                || ((entetesColonnes != null) && (entetesColonnes.Length == 0))
                || ((formatsDonnees != null) && (formatsDonnees.Length == 0))
                || ((entetesColonnes != null) && (formatsDonnees != null) && (entetesColonnes.Length != formatsDonnees.Length))
                || ((alignementGaucheDonnees != null) && (alignementGaucheDonnees.Length == 0))
                || ((entetesColonnes != null) && (alignementGaucheDonnees != null) && (entetesColonnes.Length != alignementGaucheDonnees.Length))
                || ((formatsDonnees != null) && (alignementGaucheDonnees != null) && (formatsDonnees.Length != alignementGaucheDonnees.Length))
                || (tableaux == null)
                || (tableaux.Length == 0)
                || ((entetesColonnes != null) && (entetesColonnes.Length != tableaux.Length))
                || ((formatsDonnees != null) && (formatsDonnees.Length != tableaux.Length)))
            {
                return null;
            }
            int nombreColonnes = tableaux.Length;
            for (int indiceColonne = 0; indiceColonne < nombreColonnes; indiceColonne++)
            {
                int tailleTableauDonnees = 0;
                if (tableaux[indiceColonne] is string[])
                {
                    tailleTableauDonnees = (tableaux[indiceColonne] as string[]).Length;
                }
                else if (tableaux[indiceColonne] is int[])
                {
                    tailleTableauDonnees = (tableaux[indiceColonne] as int[]).Length;
                }
                else if (tableaux[indiceColonne] is double[])
                {
                    tailleTableauDonnees = (tableaux[indiceColonne] as double[]).Length;
                }
                else if (tableaux[indiceColonne] is decimal[])
                {
                    tailleTableauDonnees = (tableaux[indiceColonne] as decimal[]).Length;
                }
                else if (tableaux[indiceColonne] is bool[])
                {
                    tailleTableauDonnees = (tableaux[indiceColonne] as bool[]).Length;
                }
                else
                {
                    return null;
                }
                if (tailleTableauDonnees < nombreColonnes)
                {
                    return null;
                }
            }

            string[] formatsCellules = new string[nombreColonnes];
            int[] largueurColonnes = new int[nombreColonnes];

            if (entetesColonnes != null)
            {
                for (int indiceColonne = 0; indiceColonne < nombreColonnes; indiceColonne++)
                {
                    largueurColonnes[indiceColonne] = entetesColonnes[indiceColonne].Length;
                }
            }

            for (int indiceColonne = 0; indiceColonne < nombreColonnes; indiceColonne++)
            {
                string[] tableauChaines = null;
                int[] tableauEntiers = null;
                double[] tableauReels = null;
                decimal[] tableauDecimaux = null;
                bool[] tableauBooleens = null;
                if (tableaux[indiceColonne] is string[])
                {
                    tableauChaines = tableaux[indiceColonne] as string[];
                }
                else if (tableaux[indiceColonne] is int[])
                {
                    tableauEntiers = tableaux[indiceColonne] as int[];
                }
                else if (tableaux[indiceColonne] is double[])
                {
                    tableauReels = tableaux[indiceColonne] as double[];
                }
                else if (tableaux[indiceColonne] is decimal[])
                {
                    tableauDecimaux = tableaux[indiceColonne] as decimal[];
                }
                else if (tableaux[indiceColonne] is bool[])
                {
                    tableauBooleens = tableaux[indiceColonne] as bool[];
                }
                string formatCellule = (formatsDonnees == null) ? "{0}" : string.Format("{{0:{0}}}", formatsDonnees[indiceColonne]);
                for (int indiceElement = 0; indiceElement < nombreElements; indiceElement++)
                {
                    int largueurCellule = 0;
                    if (tableauChaines != null)
                    {
                        largueurCellule = string.Format(formatCellule, tableauChaines[indiceElement]).Length;
                    }
                    else if (tableauEntiers != null)
                    {
                        largueurCellule = string.Format(formatCellule, tableauEntiers[indiceElement]).Length;
                    }
                    else if (tableauReels != null)
                    {
                        largueurCellule = string.Format(formatCellule, tableauReels[indiceElement]).Length;
                    }
                    else if (tableauDecimaux != null)
                    {
                        largueurCellule = string.Format(formatCellule, tableauDecimaux[indiceElement]).Length;
                    }
                    else if (tableauBooleens != null)
                    {
                        largueurCellule = string.Format(formatCellule, tableauBooleens[indiceElement]).Length;
                    }
                    if (largueurCellule > largueurColonnes[indiceElement])
                    {
                        largueurColonnes[indiceElement] = largueurCellule;
                    }
                }
                formatsCellules[indiceColonne] = formatCellule;
            }

            int largeurEnteteLigne = 0;
            string formatEnteteLigne = string.Empty;
            if (affichageNumerotation)
            {
                largeurEnteteLigne = nombreElements.ToString().Length;
                formatEnteteLigne = string.Format("{{0,{0}}}", largeurEnteteLigne);
            }

            int largeurTableau = largeurEnteteLigne
                               + 3 * nombreColonnes
                               + largueurColonnes.Sum()
                               + 2;

            string separateurHorizontal = string.Format(formatEnteteLigne, string.Empty) + " ";
            for (int indiceColonne = 0; indiceColonne < nombreColonnes; indiceColonne++)
            {
                separateurHorizontal += "+-" + new string('-', largueurColonnes[indiceColonne]) + "-";
            }
            separateurHorizontal += "+";

            if (!string.IsNullOrWhiteSpace(titre))
            {
                Console.WriteLine(separateurHorizontal.Replace("-+-", "---"));
                int LargeurBarreTitre = 3 * (nombreColonnes - 1) + largueurColonnes.Sum();
                if (LargeurBarreTitre < 4)
                {
                    largueurColonnes[0] += 4 - LargeurBarreTitre;
                    LargeurBarreTitre = 4;
                }
                if (titre.Length > LargeurBarreTitre)
                {
                    titre = titre.Substring(0, LargeurBarreTitre - 3) + "...";
                }
                int NombreEspaces = LargeurBarreTitre - titre.Length;
                Console.Write(formatEnteteLigne, string.Empty);
                Console.WriteLine(" | {0}{1}{2} |", new string(' ', NombreEspaces / 2), titre, new string(' ', NombreEspaces - NombreEspaces / 2));
                Console.WriteLine(separateurHorizontal);
            }
            else
            {
                Console.WriteLine(separateurHorizontal);
            }

            if (entetesColonnes != null)
            {
                Console.Write(formatEnteteLigne + " ", string.Empty);
                for (int IndiceColonne = 0; IndiceColonne < nombreColonnes; IndiceColonne++)
                {
                    int NombreEspaces = largueurColonnes[IndiceColonne] - entetesColonnes[IndiceColonne].Length;
                    Console.Write("| {0}{1}{2} ", new string(' ', NombreEspaces / 2), entetesColonnes[IndiceColonne], new string(' ', NombreEspaces - NombreEspaces / 2));
                }
                Console.WriteLine("|");
                Console.WriteLine(separateurHorizontal);
            }

            for (int IndiceElement = 0; IndiceElement < nombreElements; IndiceElement++)
            {
                Console.Write(formatEnteteLigne + " ", affichageNumerotation ? (IndiceElement + 1).ToString() : string.Empty);
                for (int IndiceColonne = 0; IndiceColonne < nombreColonnes; IndiceColonne++)
                {
                    string[] TableauChaines = null;
                    int[] TableauEntiers = null;
                    double[] TableauReels = null;
                    decimal[] TableauDecimaux = null;
                    bool[] TableauBooleens = null;
                    if (tableaux[IndiceColonne] is string[])
                    {
                        TableauChaines = tableaux[IndiceColonne] as string[];
                    }
                    else if (tableaux[IndiceColonne] is int[])
                    {
                        TableauEntiers = tableaux[IndiceColonne] as int[];
                    }
                    else if (tableaux[IndiceColonne] is double[])
                    {
                        TableauReels = tableaux[IndiceColonne] as double[];
                    }
                    else if (tableaux[IndiceColonne] is decimal[])
                    {
                        TableauDecimaux = tableaux[IndiceColonne] as decimal[];
                    }
                    else if (tableaux[IndiceColonne] is bool[])
                    {
                        TableauBooleens = tableaux[IndiceColonne] as bool[];
                    }
                    string TexteCellule = string.Empty;
                    if (TableauChaines != null)
                    {
                        TexteCellule = string.Format(formatsCellules[IndiceColonne], TableauChaines[IndiceElement]);
                    }
                    else if (TableauEntiers != null)
                    {
                        TexteCellule = string.Format(formatsCellules[IndiceColonne], TableauEntiers[IndiceElement]);
                    }
                    else if (TableauReels != null)
                    {
                        TexteCellule = string.Format(formatsCellules[IndiceColonne], TableauReels[IndiceElement]);
                    }
                    else if (TableauDecimaux != null)
                    {
                        TexteCellule = string.Format(formatsCellules[IndiceColonne], TableauDecimaux[IndiceElement]);
                    }
                    else if (TableauBooleens != null)
                    {
                        TexteCellule = string.Format(formatsCellules[IndiceColonne], TableauBooleens[IndiceElement]);
                    }
                    if (TexteCellule.Length < largueurColonnes[IndiceColonne])
                    {
                        string Espaces = new string(' ', largueurColonnes[IndiceColonne] - TexteCellule.Length);
                        if (alignementGaucheDonnees != null)
                        {
                            if (alignementGaucheDonnees[IndiceColonne])
                                TexteCellule += Espaces;
                            else
                                TexteCellule = Espaces + TexteCellule;
                        }
                        else
                        {
                            if (TableauChaines != null)
                                TexteCellule += Espaces;
                            else
                                TexteCellule = Espaces + TexteCellule;
                        }
                    }
                    Console.Write("| {0} ", TexteCellule);
                }
                Console.WriteLine("|");
            }

            Console.WriteLine(separateurHorizontal);

            return largueurColonnes;
        }
    }
}