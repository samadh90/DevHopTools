using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;

namespace DevHopTools.Fonctions
{
    public static partial class FonctionsDocuments
    {
        private static readonly CultureInfo c_CultureAnglaise = CultureInfo.GetCultureInfo("EN-US");

        private const NumberStyles c_StyleReel = NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint;

        public delegate void MethodeGenerationHtml(StreamWriter fichier);

        /// <summary>
        /// Permet de lire à partir d'un objet StreamReader correspondant à un fichier texte en cours de lecture, une valeur entière strictement positive constituée par le contenu du premier champ de la ligne courante
        /// </summary>
        /// <param name="fichier">Objet StreamReader dont on veut lire la ligne courante et dont on veut en récupérer une valeur entière strictement positive à partir du premier champ lu</param>
        /// <param name="nombre">Valeur entière strictement positive correctement récupérée, sinon 0</param>
        /// <returns>Vrai si la récupération de la valeur entière strictement positive a pu se faire correctement, sinon faux</returns>
        public static bool LireNombre(StreamReader fichier, out int nombre)
        {
            try
            {
                string ligne = fichier.ReadLine();
                if (ligne == null) throw new Exception("Ligne manquante dans le fichier !");
                nombre = int.Parse(ligne.Split('\t')[0].Trim());
                if (nombre <= 0) throw new Exception($"Valeur entière récupérée mais négative ou nulle : {nombre}");
                return true;
            }
            catch (Exception erreur)
            {
                System.Diagnostics.Debug.WriteLine($"\nErreur de récupération d'une valeur entière strictement positive à partir du premier champ de la ligne courante :\n{erreur.Message}\n");
                nombre = 0;
                return false;
            }
        }

        /// <summary>
        /// Permet de lire à partir d'un objet StreamReader correspondant à un fichier texte en cours de lecture, une série de valeurs entières strictement positives constituée par le contenu des champs (ou des nombreValeurs premiers champs) de la ligne courante
        /// </summary>
        /// <param name="fichier">Objet StreamReader dont on veut lire la ligne courante et dont on veut en récupérer une série de valeurs réelles</param>
        /// <param name="valeurs">Tableau des valeurs entières strictement positives correctement récupérées, sinon null</param>
        /// <param name="nombreValeurs">Soit le nombre de valeurs entières strictement positives à récupérer, soit une valeur négative afin d'ignorer cette contrainte, et donc de prendre tous les champs en compte</param>
        /// <returns>Vrai si la récupération de la série de valeurs entières strictement positives a pu se faire correctement, sinon faux</returns>
        public static bool LireNombres(StreamReader fichier, out int[] valeurs, int nombreValeurs = -1)
        {
            try
            {
                string ligne = fichier.ReadLine();
                if (ligne == null) throw new Exception("Ligne manquante dans le fichier !");
                string[] champs = ligne.Split('\t');
                if ((nombreValeurs >= 0) && (champs.Length < nombreValeurs)) throw new Exception($"Pas suffisamment de champs dans la ligne lue : {champs.Length} champs en lieu et place d'un minimum de {nombreValeurs} champs");
                valeurs = new int[(nombreValeurs >= 0) ? nombreValeurs : champs.Length];
                for (int indice = 0; indice < valeurs.Length; indice++)
                {
                    int valeur;
                    if (!int.TryParse(champs[indice].Trim(), out valeur)) throw new Exception($"Le champ n°{indice + 1} n'est pas représentatif d'une valeur entière : {champs[indice]}");
                    if (valeur <= 0) throw new Exception($"Le champ n°{indice + 1} contient une valeur entière négative ou nulle : {valeur}");
                    valeurs[indice] = valeur;
                }
                return true;
            }
            catch (Exception erreur)
            {
                System.Diagnostics.Debug.WriteLine($"\nErreur de récupération d'une série de valeurs réelles entières strictement positives à partir de la ligne courante :\n{erreur.Message}\n");
                valeurs = null;
                return false;
            }
        }

        /// <summary>
        /// Permet de lire à partir d'un objet StreamReader correspondant à un fichier texte en cours de lecture, une valeur réelle constituée par le contenu du premier champ de la ligne courante
        /// </summary>
        /// <param name="fichier">Objet StreamReader dont on veut lire la ligne courante et dont on veut en récupérer une valeur réelle à partir du premier champ lu</param>
        /// <param name="valeur">Valeur réelle correctement récupérée, sinon NaN</param>
        /// <param name="accepterNegatif">Indique si on accepte ou pas toute valeur réelle négative</param>
        /// <param name="accepterZero">Indique si on accepte ou pas la valeur réelle zéro</param>
        /// <returns>Vrai si la récupération de la valeur réelle a pu se faire correctement, sinon faux</returns>
        public static bool LireValeur(StreamReader fichier, out double valeur, bool accepterNegatif = true, bool accepterZero = true)
        {
            try
            {
                string ligne = fichier.ReadLine();
                if (ligne == null) throw new Exception("Ligne manquante dans le fichier !");
                valeur = double.Parse(ligne.Split('\t')[0].Trim().Replace(',', '.'), c_StyleReel, c_CultureAnglaise);
                if (!accepterNegatif && (valeur < 0.0)) throw new Exception($"Valeur réelle récupérée mais négative : {valeur}");
                if (!accepterZero && (valeur == 0.0)) throw new Exception("Valeur réelle récupérée mais nulle");
                return true;
            }
            catch (Exception erreur)
            {
                System.Diagnostics.Debug.WriteLine($"\nErreur de récupération d'une valeur réelle à partir du premier champ de la ligne courante :\n{erreur.Message}\n");
                valeur = double.NaN;
                return false;
            }
        }

        /// <summary>
        /// Permet de lire à partir d'un objet StreamReader correspondant à un fichier texte en cours de lecture, une série de valeurs réelles constituée par le contenu des champs (ou des nombreValeurs premiers champs) de la ligne courante
        /// </summary>
        /// <param name="fichier">Objet StreamReader dont on veut lire la ligne courante et dont on veut en récupérer une série de valeurs réelles</param>
        /// <param name="valeurs">Tableau des valeurs réelles correctement récupérées, sinon null</param>
        /// <param name="accepterNegatif">Indique si on accepte ou pas toute valeur réelle négative</param>
        /// <param name="accepterZero">Indique si on accepte ou pas la valeur réelle zéro</param>
        /// <param name="nombreValeurs">Soit le nombre de valeurs réelles à récupérer, soit une valeur négative afin d'ignorer cette contrainte, et donc de prendre tous les champs en compte</param>
        /// <returns>Vrai si la récupération de la série de valeurs réelles a pu se faire correctement, sinon faux</returns>
        public static bool LireValeurs(StreamReader fichier, out double[] valeurs, bool accepterNegatif = true, bool accepterZero = true, int nombreValeurs = -1)
        {
            try
            {
                string ligne = fichier.ReadLine();
                if (ligne == null) throw new Exception("Ligne manquante dans le fichier !");
                string[] champs = ligne.Split('\t');
                if ((nombreValeurs >= 0) && (champs.Length < nombreValeurs)) throw new Exception($"Pas suffisamment de champs dans la ligne lue : {champs.Length} champs en lieu et place d'un minimum de {nombreValeurs} champs");
                valeurs = new double[(nombreValeurs >= 0) ? nombreValeurs : champs.Length];
                for (int indice = 0; indice < valeurs.Length; indice++)
                {
                    double valeur;
                    if (!double.TryParse(champs[indice].Trim().Replace(',', '.'), c_StyleReel, c_CultureAnglaise, out valeur)) throw new Exception($"Le champ n°{indice + 1} n'est pas représentatif d'une valeur réelle : {champs[indice]}");
                    if (!accepterNegatif && (valeur < 0.0)) throw new Exception($"Le champ n°{indice + 1} contient une valeur réelle négative : {valeur}");
                    if (!accepterZero && (valeur == 0.0)) throw new Exception($"Le champ n°{indice + 1} contient une valeur réelle nulle");
                    valeurs[indice] = valeur;
                }
                return true;
            }
            catch (Exception erreur)
            {
                System.Diagnostics.Debug.WriteLine($"\nErreur de récupération d'une série de valeurs réelles à partir de la ligne courante :\n{erreur.Message}\n");
                valeurs = null;
                return false;
            }
        }

        public static bool ExporterEnHtml(string nomFichier, string titre, MethodeGenerationHtml generationHtml)
        {
            return ExporterEnHtml(nomFichier, Encoding.Default, titre, null, generationHtml);
        }

        public static bool ExporterEnHtml(string nomFichier, string titre, string style, MethodeGenerationHtml generationHtml)
        {
            return ExporterEnHtml(nomFichier, Encoding.Default, titre, style, generationHtml);
        }

        public static bool ExporterEnHtml(string nomFichier, Encoding encodageCaracteres, string titre, MethodeGenerationHtml genetaionHtml)
        {
            return ExporterEnHtml(nomFichier, encodageCaracteres, titre, null, genetaionHtml);
        }

        public static bool ExporterEnHtml(string nomFichier, Encoding encodageCaracteres, string titre, string style, MethodeGenerationHtml generationHtml)
        {
            try
            {
                using (StreamWriter fichier = new StreamWriter(nomFichier, false, encodageCaracteres))
                {
                    string jeuCaracteres = fichier.Encoding.Equals(Encoding.UTF8) ? "utf-8" : "iso-8859-1";
                    DateTime instant = DateTime.Now;
                    fichier.WriteLine("<!doctype html>");
                    fichier.WriteLine("");
                    fichier.WriteLine("<html>");
                    fichier.WriteLine("\t<head>");
                    fichier.WriteLine($"\t\t<meta charset=\"{jeuCaracteres}\"/>");
                    fichier.WriteLine($"\t\t<meta http-equiv=\"content-type\" content=\"text/html;charset={jeuCaracteres}\"/>");
                    if (!string.IsNullOrWhiteSpace(titre)) fichier.WriteLine($"\t\t<title>{titre.Trim()}</title>");
                    if (!string.IsNullOrWhiteSpace(titre))
                    {
                        fichier.WriteLine("\t\t<style>");
                        fichier.WriteLine(style);
                        fichier.WriteLine("\t\t</style>");
                    }
                    fichier.WriteLine("\t</head>");
                    fichier.WriteLine();
                    fichier.WriteLine("\t<body>");
                    fichier.WriteLine("\t\t<header>");
                    if (!string.IsNullOrWhiteSpace(titre)) fichier.WriteLine($"\t\t\t{titre.Trim()}");
                    fichier.WriteLine("\t\t</header>");
                    fichier.WriteLine("\t\t<main>");
                    if (generationHtml != null) generationHtml(fichier);
                    fichier.WriteLine("\t\t</main>");
                    fichier.WriteLine("\t\t<footer>");
                    fichier.WriteLine($"\t\t\t<p>Généré {instant:dddd le d/MM/yyyy à H:mm:ss}</p>");
                    fichier.WriteLine("\t\t</footer>");
                    fichier.WriteLine("\t</body>");
                    fichier.WriteLine("</html>");
                }
                return true;
            }
            catch (Exception erreur)
            {
                Debug.WriteLine($"\nErreur de création du document HTML {nomFichier} :\n{erreur.Message}\n");
                return false;
            }
        }

        public static void OuvrirDocument(string nomFichier)
        {
            try
            {
                Process.Start("explorer.exe", nomFichier.Replace("/", "\\"));
            }
            catch
            {
            }
        }
    }
}