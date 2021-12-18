using System;
using System.IO;
using System.Text;

namespace DevHopTools.Fonctions
{
    public static partial class FonctionsMatriceCycleDeVie
    {
        /// <summary>
        /// Permet de créer une nouvelle matrice en fonction des dimensions spécifiées, et de l'éventuelle valeur de remplissage précisée
        /// </summary>
        /// <param name="nombreLignes">Nombre de lignes de la matrice à créer</param>
        /// <param name="nombreColonnes">Nombre de colonnes de la matrice à créer</param>
        /// <param name="valeurRemplissage">Valeur de remplissage à utiliser pour initialiser les cellules de cette nouvelle matrice</param>
        /// <returns>Nouvelle matrice si possible, sinon null</returns>
        public static double[,] CreerMatrice(int nombreLignes, int nombreColonnes, double valeurRemplissage = 0.0)
        {
            if ((nombreLignes < 0) || (nombreColonnes < 0)) return null;
            if ((nombreLignes == 0) || (nombreColonnes == 0)) return (nombreLignes == nombreColonnes) ? new double[0, 0] : null;
            double[,] matrice = new double[nombreLignes, nombreColonnes];
            if (valeurRemplissage != 0.0)
            {
                for (int i = 0; i < nombreLignes; i++)
                {
                    for (int j = 0; j < nombreColonnes; j++)
                    {
                        matrice[i, j] = valeurRemplissage;
                    }
                }
            }
            return matrice;
        }

        /// <summary>
        /// Permet de créer une matrice Zero en fonction des dimensions spécifiées
        /// </summary>
        /// <param name="nombreLignes">Nombre de lignes de la matrice Zero à créer</param>
        /// <param name="nombreColonnes">Nombre de colonnes de la matrice Zero à créer</param>
        /// <returns>Nouvelle matrice Zero si possible, sinon null</returns>
        public static double[,] CreerZero(int nombreLignes, int nombreColonnes)
        {
            return CreerMatrice(nombreLignes, nombreColonnes);
        }

        /// <summary>
        /// Permet de créer une matrice Identité en fonction des dimensions spécifiées
        /// </summary>
        /// <param name="nombreLignes">Nombre de lignes de la matrice Identité à créer</param>
        /// <param name="nombreColonnes">Nombre de colonnes de la matrice Identité à créer</param>
        /// <returns>Nouvelle matrice Identité si possible, sinon null</returns>
        public static double[,] CreerIdentite(int nombreLignes, int nombreColonnes)
        {
            double[,] matrice = CreerMatrice(nombreLignes, nombreColonnes);
            if (matrice == null) return null;
            for (int i = 0; i < nombreLignes; i++)
            {
                matrice[i, i] = 1.0;
            }
            return matrice;
        }

        /// <summary>
        /// Permet de créer une nouvelle matrice avec les mêmes dimensions et mêmes valeurs que la matrice source spécifiée
        /// </summary>
        /// <param name="source">Matrice à dupliquer</param>
        /// <returns>Nouvelle matrice si possible, sinon null</returns>
        public static double[,] Dupliquer(double[,] source)
        {
            if (source == null) return null;
            int nombreLignes = FonctionsMatriceDefinitions.NombreLignes(source);
            int nombreColonnes = FonctionsMatriceDefinitions.NombreColonnes(source);
            double[,] matrice = new double[nombreLignes, nombreColonnes];
            for (int i = 0; i < nombreLignes; i++)
            {
                for (int j = 0; j < nombreColonnes; j++)
                {
                    matrice[i, j] = source[i, j];
                }
            }
            return matrice;
        }

        /// <summary>
        /// Permet de créer une matrice réduite correspondant à la matrice source spécifiée dont on ne prend ni les données d'une ligne spécifiée, ni les données d'une colonne spécifiée
        /// </summary>
        /// <param name="source">Matrice à dupliquer</param>
        /// <param name="indiceLigneAIgnorer">Indice de la ligne à ignorer lors de la duplication</param>
        /// <param name="indiceColonneAIgnorer">Indice de la colonne à ignorer lors de la duplication</param>
        /// <returns>Matrice réduite si possible, sinon null</returns>
        public static double[,] CreerMatriceReduite(double[,] source, int indiceLigneAIgnorer, int indiceColonneAIgnorer)
        {
            if ((source == null) || (indiceLigneAIgnorer < 0) || (indiceColonneAIgnorer < 0) || FonctionsMatriceDefinitions.EstVecteurLigne(source) || FonctionsMatriceDefinitions.EstVecteurColonne(source)) return null;
            int nombreLignes = FonctionsMatriceDefinitions.NombreLignes(source);
            int nombreColonnes = FonctionsMatriceDefinitions.NombreColonnes(source);
            if ((indiceLigneAIgnorer >= nombreLignes) || (indiceColonneAIgnorer >= nombreColonnes)) return null;
            double[,] matriceReduite = new double[nombreLignes - 1, nombreColonnes - 1];
            for (int iS = 0, iR = 0; iS < nombreLignes; iS++)
            {
                if (iS != indiceLigneAIgnorer)
                {
                    for (int jS = 0, jR = 0; jS < nombreColonnes; jS++)
                    {
                        if (jS != indiceColonneAIgnorer)
                        {
                            matriceReduite[iR, jR] = source[iS, jS];
                            jR++;
                        }
                    }
                    iR++;
                }
            }
            return matriceReduite;
        }

        /// <summary>
        /// Tente de créer une matrice en en chargeant les données à partir d'un fichier texte au format CSV prévu à cet effet
        /// </summary>
        /// <param name="nomFichier">Nom du fichier texte contenant les données définissant une matrice</param>
        /// <returns>Matrice résultant du chargement réussi, sinon null</returns>
        public static double[,] Charger(string nomFichier)
        {
            if (string.IsNullOrEmpty(nomFichier)) return null;
            try
            {
                double[,] matrice = null;
                using (StreamReader fichier = new StreamReader(nomFichier, Encoding.Default, true))
                {
                    if (!FonctionsChaines.TenterConvertir(fichier.ReadLine().Split('\t')[0].Trim(), out int nombreLignes)) throw new Exception("Erreur de lecture du nombre de lignes de la matrice !");
                    if (nombreLignes < 0) throw new Exception("Un nombre de lignes d'une matrice ne peut pas être négatif !");
                    if (!FonctionsChaines.TenterConvertir(fichier.ReadLine().Split('\t')[0].Trim(), out int nombreColonnes)) throw new Exception("Erreur de lecture du nombre de colonnes de la matrice !");
                    if (nombreColonnes < 0) throw new Exception("Un nombre de colonnes d'une matrice ne peut pas être négatif !");
                    if ((nombreLignes == 0) || (nombreColonnes == 0))
                    {
                        if (nombreLignes != nombreColonnes) throw new Exception($"Une matrice vide doit avoir à la fois 0 ligne et 0 colonne, et pas être déclarée {nombreLignes} x {nombreColonnes} !");
                        return new double[0, 0];
                    }
                    matrice = new double[nombreLignes, nombreColonnes];
                    for (int i = 0; i < nombreLignes; i++)
                    {
                        string[] champs = fichier.ReadLine().Split('\t');
                        if (champs.Length < nombreColonnes) throw new Exception($"La ligne n°{3 + i} du fichier ne contient pas au moins {nombreColonnes} champs !");
                        for (int j = 0; j < nombreColonnes; j++)
                        {
                            if (!FonctionsChaines.TenterConvertir(champs[j].Trim(), out double valeur)) throw new Exception($"Erreur de lecture de la valeur en ({i + 1}, {j + 1}) de la matrice !");
                            matrice[i, j] = valeur;
                        }
                    }
                }
                return matrice;
            }
            catch (Exception erreur)
            {
                System.Diagnostics.Debug.WriteLine($"\nErreur lors du chargement d'une matrice à partir du fichier {nomFichier} !\n{erreur.Message}\n");
                return null;
            }
        }

        /// <summary>
        /// Tente de sauvegarder la matrice spécifiée dans un fichier texte au format CSV prévu à cet effet
        /// </summary>
        /// <param name="nomFichier">Nom du fichier texte devant contenir la matrice à sauvegarder</param>
        /// <param name="matrice">Matrice à sauvegarder</param>
        /// <returns>Vrai si la sauvegarde a réussi, sinon faux</returns>
        public static bool Sauvegarder(string nomFichier, double[,] matrice)
        {
            if (string.IsNullOrEmpty(nomFichier) || (matrice == null)) return false;
            try
            {
                using (StreamWriter fichier = new StreamWriter(nomFichier, false, Encoding.Default))
                {
                    int nombreLignes = FonctionsMatriceDefinitions.NombreLignes(matrice);
                    int nombreColonnes = FonctionsMatriceDefinitions.NombreColonnes(matrice);
                    fichier.WriteLine(nombreLignes);
                    fichier.WriteLine(nombreColonnes);
                    for (int i = 0; i < nombreLignes; i++)
                    {
                        for (int j = 0; j < nombreColonnes; j++)
                        {
                            if (j > 0) fichier.Write('\t');
                            fichier.Write(matrice[i, j]);
                        }
                        fichier.WriteLine();
                    }
                }
                return true;
            }
            catch (Exception erreur)
            {
                System.Diagnostics.Debug.WriteLine($"\nErreur lors de la sauvegarde d'une matrice dans le fichier {nomFichier} !\n{erreur.Message}\n");
                return false;
            }
        }
    }
}