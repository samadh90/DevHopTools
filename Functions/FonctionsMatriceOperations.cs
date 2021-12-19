using System;

namespace DevHopTools.Functions
{
    public static partial class FonctionsMatriceOperations
    {
        /// <summary>
        /// Tente d'additionner les deux matrices spécifiées
        /// </summary>
        /// <param name="A">Matrice opérande de gauche de cette opération</param>
        /// <param name="B">Matrice opérande de droite de cette opération</param>
        /// <param name="C">Matrice résultant de l'opération tentée, sinon null</param>
        /// <returns>Vrai si cette opération a pu être réalisée, sinon faux</returns>
        public static bool Additionner(double[,] A, double[,] B, out double[,] C)
        {
            C = Additionner(A, B);
            return (C != null);
        }

        /// <summary>
        /// Tente d'additionner les deux matrices spécifiées
        /// </summary>
        /// <param name="A">Matrice opérande de gauche de cette opération</param>
        /// <param name="B">Matrice opérande de droite de cette opération</param>
        /// <returns>Matrice résultant de l'opération tentée, sinon null</returns>
        public static double[,] Additionner(double[,] A, double[,] B)
        {
            int nlA = FonctionsMatriceDefinitions.NombreLignes(A);
            int ncA = FonctionsMatriceDefinitions.NombreColonnes(A);
            int nlB = FonctionsMatriceDefinitions.NombreLignes(B);
            int ncB = FonctionsMatriceDefinitions.NombreColonnes(B);
            if ((nlA == 0) || (nlB != nlA) || (ncB != ncA)) return null;
            int nlC = nlA;
            int ncC = ncA;
            double[,] C = new double[nlC, ncC];
            for (int i = 0; i < nlC; i++)
            {
                for (int j = 0; j < ncC; j++)
                {
                    C[i, j] = A[i, j] + B[i, j];
                }
            }
            return C;
        }

        /// <summary>
        /// Tente de soustraire les deux matrices spécifiées
        /// </summary>
        /// <param name="A">Matrice opérande de gauche de cette opération</param>
        /// <param name="B">Matrice opérande de droite de cette opération</param>
        /// <param name="C">Matrice résultant de l'opération tentée, sinon null</param>
        /// <returns>Vrai si cette opération a pu être réalisée, sinon faux</returns>
        public static bool Soustraire(double[,] A, double[,] B, out double[,] C)
        {
            C = Soustraire(A, B);
            return (C != null);
        }

        /// <summary>
        /// Tente de soustraire les deux matrices spécifiées
        /// </summary>
        /// <param name="A">Matrice opérande de gauche de cette opération</param>
        /// <param name="B">Matrice opérande de droite de cette opération</param>
        /// <returns>Matrice résultant de l'opération tentée, sinon null</returns>
        public static double[,] Soustraire(double[,] A, double[,] B)
        {
            int nlA = FonctionsMatriceDefinitions.NombreLignes(A);
            int ncA = FonctionsMatriceDefinitions.NombreColonnes(A);
            int nlB = FonctionsMatriceDefinitions.NombreLignes(B);
            int ncB = FonctionsMatriceDefinitions.NombreColonnes(B);
            if ((nlA == 0) || (nlB != nlA) || (ncB != ncA)) return null;
            int nlC = nlA;
            int ncC = ncA;
            double[,] C = new double[nlC, ncC];
            for (int i = 0; i < nlC; i++)
            {
                for (int j = 0; j < ncC; j++)
                {
                    C[i, j] = A[i, j] - B[i, j];
                }
            }
            return C;
        }

        /// <summary>
        /// Tente de multiplier les deux matrices spécifiées
        /// </summary>
        /// <param name="A">Matrice opérande de gauche de cette opération</param>
        /// <param name="B">Matrice opérande de droite de cette opération</param>
        /// <param name="C">Matrice résultant de l'opération tentée, sinon null</param>
        /// <returns>Vrai si cette opération a pu être réalisée, sinon faux</returns>
        public static bool Multiplier(double[,] A, double[,] B, out double[,] C)
        {
            C = Multiplier(A, B);
            return (C != null);
        }

        /// <summary>
        /// Tente de multiplier les deux matrices spécifiées
        /// </summary>
        /// <param name="A">Matrice opérande de gauche de cette opération</param>
        /// <param name="B">Matrice opérande de droite de cette opération</param>
        /// <returns>Matrice résultant de l'opération tentée, sinon null</returns>
        public static double[,] Multiplier(double[,] A, double[,] B)
        {
            int nlA = FonctionsMatriceDefinitions.NombreLignes(A);
            int ncA = FonctionsMatriceDefinitions.NombreColonnes(A);
            int nlB = FonctionsMatriceDefinitions.NombreLignes(B);
            int ncB = FonctionsMatriceDefinitions.NombreColonnes(B);
            if ((nlA == 0) || (ncA != nlB)) return null;
            int nlC = nlA;
            int ncC = ncB;
            double[,] C = new double[nlC, ncC];
            for (int i = 0; i < nlC; i++)
            {
                for (int j = 0; j < ncC; j++)
                {
                    #region Somme pour k allant de 0 à NCA-1 de l'expression Ai,k * Bk,j
                    double somme = 0.0;
                    for (int k = 0; k < ncA; k++)
                    {
                        somme += A[i, k] * B[k, j];
                    }
                    #endregion
                    C[i, j] = somme;
                }
            }
            return C;
        }

        /// <summary>
        /// Retourne si possible la valeur du déterminant de la matrice spécifiée
        /// </summary>
        /// <param name="A">Matrice dont on veut calculer le déterminant</param>
        /// <param name="determinant">Valeur du déterminant de la matrice spécifiée si possible, sinon double.NaN</returns>
        /// <param name="calculOptimise">Indique si on tente de réaliser le calcul du déterminant de manière optimisée (par exploitation des zéros)</param>
        /// <returns>Vrai si le déterminant a pu être calculé, sinon faux</returns>
        public static bool Determinant(double[,] A, out double determinant, bool calculOptimise = false)
        {
            determinant = Determinant(A, calculOptimise);
            return !double.IsNaN(determinant);
        }

        /// <summary>
        /// Retourne si possible la valeur du déterminant de la matrice spécifiée
        /// </summary>
        /// <param name="A">Matrice dont on veut calculer le déterminant</param>
        /// <param name="calculOptimise">Indique si on tente de réaliser le calcul du déterminant de manière optimisée (par exploitation des zéros)</param>
        /// <returns>Valeur du déterminant de la matrice spécifiée si possible, sinon double.NaN</returns>
        public static double Determinant(double[,] A, bool calculOptimise = false)
        {
            if (!FonctionsMatriceDefinitions.EstCarree(A)) return double.NaN;
            int taille = FonctionsMatriceDefinitions.NombreLignes(A);
            if (taille >= 2)
            {
                return calculOptimise ? DeterminantOptimise(A, taille) : Determinant(A, taille);
            }
            else // (taille == 1)
            {
                return A[0, 0];
            }
        }

        /// <summary>
        /// Méthode à usage spécifiquement interne afin de calculer le déterminant d'une matrice que l'on sait être carrée et de taille &gt;= 2
        /// </summary>
        /// <param name="A">Matrice carrée de taille &gt;= 2</param>
        /// <param name="taille">Taille de cette matrice carrée dont on veut calculer le déterminant</param>
        /// <returns>Valeur du déterminant</returns>
        private static double Determinant(double[,] A, int taille)
        {
            if (taille > 2)
            {
                int i = 0; // On choisit arbitrairement la première ligne (alors que l'on pouvait choisir n'importe quelle ligne ou n'importe quelle colonne
                double signe = ((i % 2) == 0) ? 1.0 : -1.0;
                double detA = 0.0;
                for (int j = 0; j < taille; j++)
                {
                    detA += signe * A[i, j] * Determinant(FonctionsMatriceCycleDeVie.CreerMatriceReduite(A, i, j), taille - 1);
                    signe = -signe;
                }
                return detA;
            }
            else // (taille == 2)
            {
                return A[0, 0] * A[1, 1] - A[0, 1] * A[1, 0];
            }
        }


        /// <summary>
        /// Méthode à usage spécifiquement interne afin de calculer, de manière optimisée, le déterminant d'une matrice que l'on sait être carrée et de taille &gt;= 2
        /// </summary>
        /// <param name="A">Matrice carrée de taille &gt;= 2</param>
        /// <param name="taille">Taille de cette matrice carrée dont on veut calculer le déterminant</param>
        /// <returns>Valeur du déterminant</returns>
        private static double DeterminantOptimise(double[,] A, int taille)
        {
            if (taille > 2)
            {
                #region Recherche de la ligne contenant un maximum de zéros
                int indiceLigneContenantMaximumZeros;
                int maximumZerosEnLigne = MaximumZerosEnLigne(A, out indiceLigneContenantMaximumZeros);
                #endregion

                #region Recherche de la colonne contenant un maximum de zéros
                int indiceColonneContenantMaximumZeros;
                int maximumZerosEnColonne = MaximumZerosEnColonne(A, out indiceColonneContenantMaximumZeros);
                #endregion

                #region Choix entre la ligne contenant un maximum de zéros et la colonne contenant un maximum de zéros
                bool uneLigneEstChoisie = (maximumZerosEnLigne >= maximumZerosEnColonne);
                #endregion

                #region Calcul du déterminant
                double detA = 0.0;
                if (uneLigneEstChoisie)
                {
                    int i = indiceLigneContenantMaximumZeros;
                    double signe = ((i % 2) == 0) ? 1.0 : -1.0;
                    for (int j = 0; j < taille; j++)
                    {
                        if (A[i, j] != 0.0)
                        {
                            detA += signe * A[i, j] * DeterminantOptimise(FonctionsMatriceCycleDeVie.CreerMatriceReduite(A, i, j), taille - 1);
                        }
                        signe = -signe;
                    }
                }
                else // if (!uneLigneEstChoisie)
                {
                    int j = indiceColonneContenantMaximumZeros;
                    double signe = ((j % 2) == 0) ? 1.0 : -1.0;
                    for (int i = 0; i < taille; i++)
                    {
                        if (A[i, j] != 0.0)
                        {
                            detA += signe * A[i, j] * DeterminantOptimise(FonctionsMatriceCycleDeVie.CreerMatriceReduite(A, i, j), taille - 1);
                        }
                        signe = -signe;
                    }
                }
                return detA;
                #endregion
            }
            else // (taille == 2)
            {
                return A[0, 0] * A[1, 1] - A[0, 1] * A[1, 0];
            }
        }

        /// <summary>
        /// Retourne le nombre maximal de zéros que l'on peut trouver dans les lignes de la matrice spécifiée, et indique également l'indice de la première ligne contenant cette quantité maximale de zéros
        /// </summary>
        /// <param name="A">Matrice analysée</param>
        /// <param name="indiceLigneOptimale">Indique l'indice de la première ligne contenant un maximum de zéros en ligne</param>
        /// <returns>Nombre de zéros présents au maximum dans une ligne de la matrice spécifiée, si possible, sinon 0</returns>
        public static int MaximumZerosEnLigne(double[,] A, out int indiceLigneOptimale)
        {
            indiceLigneOptimale = -1;
            if ((A == null) || (A.Length == 0)) return 0;
            int NL = FonctionsMatriceDefinitions.NombreLignes(A);
            int NC = FonctionsMatriceDefinitions.NombreColonnes(A);
            int maximumZeros = -1;
            for (int i = 0; i < NL; i++)
            {
                int compteurZeros = 0;
                for (int j = 0; j < NC; j++)
                {
                    if (A[i, j] == 0.0) compteurZeros++;
                }
                if (compteurZeros > maximumZeros)
                {
                    maximumZeros = compteurZeros;
                    indiceLigneOptimale = i;
                }
            }
            return maximumZeros;
        }

        /// <summary>
        /// Retourne le nombre maximal de zéros que l'on peut trouver dans les colonnes de la matrice spécifiée, et indique également l'indice de la première colonne contenant cette quantité maximale de zéros
        /// </summary>
        /// <param name="A">Matrice analysée</param>
        /// <param name="indiceColoneOptimale">Indique l'indice de la première colonne contenant un maximum de zéros en ligne</param>
        /// <returns>Nombre de zéros présents au maximum dans une colonne de la matrice spécifiée, si possible, sinon 0</returns>
        public static int MaximumZerosEnColonne(double[,] A, out int indiceColoneOptimale)
        {
            indiceColoneOptimale = -1;
            if ((A == null) || (A.Length == 0)) return 0;
            int NL = FonctionsMatriceDefinitions.NombreLignes(A);
            int NC = FonctionsMatriceDefinitions.NombreColonnes(A);
            int maximumZeros = -1;
            for (int j = 0; j < NC; j++)
            {
                int compteurZeros = 0;
                for (int i = 0; i < NL; i++)
                {
                    if (A[i, j] == 0.0) compteurZeros++;
                }
                if (compteurZeros > maximumZeros)
                {
                    maximumZeros = compteurZeros;
                    indiceColoneOptimale = j;
                }
            }
            return maximumZeros;
        }

        /// <summary>
        /// Retourne le nombre de valeurs zéros présentes dans la ligne spécifiée de la matrice spécifiée
        /// </summary>
        /// <param name="A">Matrice analysée</param>
        /// <param name="indiceLigne">Indice de ligne</param>
        /// <returns>Nombre de zéros présents dans la ligne spécifiée de la matrice spécifiée, si possible, sinon 0</returns>
        public static int NombreZerosEnLigne(double[,] A, int indiceLigne)
        {
            if ((A == null) || (A.Length == 0)) return 0;
            int NL = FonctionsMatriceDefinitions.NombreLignes(A);
            int NC = FonctionsMatriceDefinitions.NombreColonnes(A);
            if ((indiceLigne < 0) || (indiceLigne >= NL)) return 0;
            int compteurZeros = 0;
            for (int j = 0; j < NC; j++)
            {
                if (A[indiceLigne, j] == 0.0) compteurZeros++;
            }
            return compteurZeros;
        }

        /// <summary>
        /// Retourne le nombre de valeurs zéros présentes dans la colonne spécifiée de la matrice spécifiée
        /// </summary>
        /// <param name="A">Matrice A</param>
        /// <param name="indiceColonne">Indice de colonne</param>
        /// <returns>Nombre de zéros présents dans la colonne spécifiée de la matrice spécifiée, si possible, sinon 0</returns>
        public static int NombreZerosEnColonne(double[,] A, int indiceColonne)
        {
            if ((A == null) || (A.Length == 0)) return 0;
            int NL = FonctionsMatriceDefinitions.NombreLignes(A);
            int NC = FonctionsMatriceDefinitions.NombreColonnes(A);
            if ((indiceColonne < 0) || (indiceColonne >= NC)) return 0;
            int compteurZeros = 0;
            for (int i = 0; i < NL; i++)
            {
                if (A[i, indiceColonne] == 0.0) compteurZeros++;
            }
            return compteurZeros;
        }

        /// <summary>
        /// Tente de calculer l'inverse de la matrice spécifiée
        /// </summary>
        /// <param name="A">Matrice dont on veut calculer l'inverse</param>
        /// <param name="B">Matrice résultant de l'opération tentée, sinon null</param>
        /// <param name="calculOptimise">Indique si on tente de réaliser le calcul du déterminant de manière optimisée (par exploitation des zéros)</param>
        /// <returns>Vrai si cette opération a pu être réalisée, sinon faux</returns>
        public static bool Inverser(double[,] A, out double[,] B, bool calculOptimise = false)
        {
            B = Inverser(A, calculOptimise);
            return (B != null);
        }

        /// <summary>
        /// Tente de calculer l'inverse de la matrice spécifiée
        /// </summary>
        /// <param name="A">Matrice dont on veut calculer l'inverse</param>
        /// <param name="calculOptimise">Indique si on tente de réaliser le calcul du déterminant de manière optimisée (par exploitation des zéros)</param>
        /// <returns>Matrice résultant de l'opération tentée, sinon null</returns>
        public static double[,] Inverser(double[,] A, bool calculOptimise = false)
        {
            double detA;
            if (!Determinant(A, out detA, calculOptimise) || (detA == 0.0)) return null;
            int taille = FonctionsMatriceDefinitions.NombreLignes(A);
            if (taille >= 2)
            {

                double[,] B = new double[taille, taille];
                int tailleReduite = taille - 1;
                for (int i = 0; i < taille; i++)
                {
                    double signe = ((i % 2) == 0) ? 1.0 : -1.0;
                    for (int j = 0; j < taille; j++)
                    {
                        double[,] matriceReduite = FonctionsMatriceCycleDeVie.CreerMatriceReduite(A, j, i);
                        double detReduit = (tailleReduite == 1)
                                         ? matriceReduite[0, 0]
                                         : (calculOptimise ? DeterminantOptimise(matriceReduite, tailleReduite) : Determinant(matriceReduite, tailleReduite));
                        B[i, j] = signe * detReduit / detA;
                        signe = -signe;
                    }
                }
                return B;
            }
            else
            {
                return new double[,] { { 1.0 / A[0, 0] } };
            }
        }

        /// <summary>
        /// Retourne le nombre de cellules contenant la valeur 0 au sein de la matrice spécifiée
        /// </summary>
        /// <param name="A">Matrice dont on compte le nombre de zéros</param>
        /// <returns>Nombre de zéros de la matrice spécifiée, sinon 0</returns>
        public static int NombreZeros(double[,] A)
        {
            if ((A == null) || (A.Length == 0)) return 0;
            int compteurZeros = 0;
            int NL = FonctionsMatriceDefinitions.NombreLignes(A);
            int NC = FonctionsMatriceDefinitions.NombreColonnes(A);
            for (int i = 0; i < NL; i++)
            {
                for (int j = 0; j < NC; j++)
                {
                    if (A[i, j] == 0.0) compteurZeros++;
                }
            }
            return compteurZeros;
        }

        /// <summary>
        /// Donne un conseil sur le fait d'utiliser ou pas le calcul optimisé (par exploitation des zéros) des déterminants et inverses sur la matrice spécifiée
        /// </summary>
        /// <param name="A">Matrice pour laquelle on désire avoir un conseil d'optimisation des calculs de déterminant et d'inverse</param>
        /// <param name="seuilTolerance">Seuil de tolérance en pourcentage (une valeur réelle comprise entre 0.0 et 1.0)</param>
        /// <returns>Vrai si le calcul optimisé est conseillé, sinon faux</returns>
        public static bool CalculOptimiseConseille(double[,] A, double seuilTolerance = 0.0)
        {
            int compteurZeros = NombreZeros(A);
            if (compteurZeros == 0) return false;
            if (seuilTolerance < 0.0) seuilTolerance = 0.0;
            if (seuilTolerance > 1.0) seuilTolerance = 1.0;
            int minimumZerosDesires = (int)Math.Floor(Math.Max(A.GetLength(0), A.GetLength(1)) * (1.0 - seuilTolerance));
            return (compteurZeros >= minimumZerosDesires);
        }
    }
}