namespace DevHopTools.Fonctions
{
    public static partial class FonctionsMatriceDefinitions
    {
        /// <summary>
        /// Retourne le nombre de lignes de la matrice spécifiée
        /// </summary>
        /// <param name="matrice">Matrice dont on veut connaître le nombre de lignes</param>
        /// <returns>Nombre de lignes de la matrice spécifiée si possible, sinon 0</returns>
        public static int NombreLignes(double[,] matrice)
        {
            if (matrice == null) return 0;
            return matrice.GetLength(0);
        }

        /// <summary>
        /// Retourne le nombre de colonnes de la matrice spécifiée
        /// </summary>
        /// <param name="matrice">Matrice dont on veut connaître le nombre de colonnes</param>
        /// <returns>Nombre de colonnes de la matrice spécifiée si possible, sinon 0</returns>
        public static int NombreColonnes(double[,] matrice)
        {
            if (matrice == null) return 0;
            return matrice.GetLength(1);
        }

        /// <summary>
        /// Indique si la matrice spécifiée existe ou pas
        /// <para>Une matrice qui existe comporte au moins une ligne et une colonne</para>
        /// </summary>
        /// <param name="matrice">Matrice dont on veut vérifier l'existence</param>
        /// <returns>Vrai si la matrice spécifiée existe, sinon faux</returns>
        public static bool Existe(double[,] matrice)
        {
            if (matrice == null) return false;
            //return (matrice.GetLength(0) >= 1) /* && (matrice.GetLength(1) >= 1) */; // On vérifie la présence d'au moins une ligne et une colonne ; bien qu'en C#, si un tableau à deux dimensions comporte au moins une ligne, il comporte au moins une colonne
            return (matrice.Length >= 1); // On vérifie la présence d'au moins une cellule
        }

        /// <summary>
        /// Indique si la matrice spécifiée correspond strictement à un vecteur ligne
        /// <para>Un vecteur ligne comporte une et une seule ligne mais au moins deux colonnes</para>
        /// </summary>
        /// <param name="matrice">Matrice dont on veut vérifier si elle correspond à un vecteur ligne</param>
        /// <returns>Vrai si la matrice spécifiée correspond spécifiquement à un vecteur ligne, sinon faux</returns>
        public static bool EstVecteurLigne(double[,] matrice)
        {
            if (matrice == null) return false;
            return (matrice.GetLength(0) == 1) && (matrice.GetLength(1) >= 2);
        }

        /// <summary>
        /// Indique si la matrice spécifiée correspond strictement à un vecteur colonne
        /// <para>Un vecteur colonne comporte une et une seule colonne mais au moins deux lignes</para>
        /// </summary>
        /// <param name="matrice">Matrice dont on veut vérifier si elle correspond à un vecteur colonne</param>
        /// <returns>Vrai si la matrice spécifiée correspond spécifiquement à un vecteur colonne, sinon faux</returns>
        public static bool EstVecteurColonne(double[,] matrice)
        {
            if (matrice == null) return false;
            return (matrice.GetLength(1) == 1) && (matrice.GetLength(0) >= 2);
        }

        /// <summary>
        /// Indique si la matrice spécifiée correspond à une matrice carrée
        /// <para>Une matrice carrée comporte le même nombre de lignes que de colonnes</para>
        /// </summary>
        /// <param name="matrice">Matrice dont on veut vérifier si elle correspond à une matrice carrée</param>
        /// <returns>Vrai si la matrice spécifiée correspond à une matrice carrée, sinon faux</returns>
        public static bool EstCarree(double[,] matrice)
        {
            if (matrice == null) return false;
            return (matrice.Length >= 1) && (matrice.GetLength(0) == matrice.GetLength(1));
        }

        /// <summary>
        /// Indique si la matrice spécifiée correspond à une matrice rectangulaire
        /// <para>Une matrice rectangulaire comporte un nombre de lignes différent du nombre de colonnes</para>
        /// </summary>
        /// <param name="matrice">Matrice dont on veut vérifier si elle correspond à une matrice carrée</param>
        /// <param name="rejetVecteurs">Indique si on doit rejeter les vecteurs en ne les considérant pas comme matrice rectangulaire</param>
        /// <returns>Vrai si la matrice spécifiée correspond à une matrice rectangulaire, sinon faux</returns>
        public static bool EstRectangulaire(double[,] matrice, bool rejetVecteurs = false)
        {
            if (matrice == null) return false;
            if (rejetVecteurs && ((matrice.GetLength(0) == 1) || (matrice.GetLength(1) == 1))) return false;
            return (matrice.Length >= 1) && (matrice.GetLength(0) != matrice.GetLength(1));
        }

        /// <summary>
        /// Indique si la matrice spécifiée correspond à une matrice zéro
        /// <para>Une matrice est considérée comme matrice "zéro" si toutes ces cellules sont égales à 0</para>
        /// </summary>
        /// <param name="matrice">Matrice dont on veut vérifier si elle correspond à une matrice zéro</param>
        /// <returns>Vrai si la matrice spécifiée correspond à une matrice zéro, sinon faux</returns>
        public static bool EstZero(double[,] matrice)
        {
            // On rejete les cas d'absence de matrice ou de matrice vide
            if ((matrice == null) || (matrice.Length == 0)) return false;
            // On détermine le nombre de lignes : nl
            int nl = matrice.GetLength(0);
            // On détermine le nombre de colonnes : nc
            int nc = matrice.GetLength(1);
            // Parcours des lignes (de la première jusqu'à la dernière par "pas" de 1)
            for (int i = 0; i < nl; i++)
            {
                // Parcours des colonnes (de la première jusqu'à la dernière par "pas" de 1)
                for (int j = 0; j < nc; j++)
                {
                    // Si pour la cellule en ligne i et en colonne j, la règle de valeur de cellule = 0 (pour toute cellule) n'est pas respectée, cela suffit à déterminer que la matrice n'est pas de type matrice "zéro"
                    if (matrice[i, j] != 0.0) return false;
                }
            }
            // Si on a pu remarquer que la condition attendue pour toute cellule était effectivement respectée, cela prouve que la matrice est effectivement une matriuce "zéro"
            return true;
        }

        /// <summary>
        /// Indique si la matrice spécifiée correspond à une matrice identité
        /// <para>Une matrice est considérée comme matrice "identité" si toutes les cellules de sa diagonale principale sont égales à 1 et toutes les autres cellules sont égales à 0</para>
        /// </summary>
        /// <param name="matrice">Matrice dont on veut vérifier si elle correspond à une matrice identité</param>
        /// <returns>Vrai si la matrice spécifiée correspond à une matrice identité, sinon faux</returns>
        public static bool EstIdentite(double[,] matrice)
        {
            if ((matrice == null) || (matrice.Length == 0)) return false;
            int nl = matrice.GetLength(0);
            int nc = matrice.GetLength(1);
            for (int i = 0; i < nl; i++)
            {
                for (int j = 0; j < nc; j++)
                {
                    if (i == j)
                    { // La cellule à tester en (i, j) est sur la diagonale principale
                        if (matrice[i, j] != 1.0) return false;
                    }
                    else
                    { // La cellule à tester en (i, j) n'est pas sur la diagonale principale
                        if (matrice[i, j] != 0.0) return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Indique si la matrice spécifiée correspond à une matrice triangulaire inférieure
        /// <para>Une matrice est considérée comme matrice "triangulaire inférieure" si toutes les cellules au dessus de sa diagonale principale sont égales à 0, et au moins une cellule en dessous de sa diagonale principale est différente de 0</para>
        /// </summary>
        /// <param name="matrice">Matrice dont on veut vérifier si elle correspond à une matrice triangulaire inférieure</param>
        /// <returns>Vrai si la matrice spécifiée correspond à une matrice triangulaire inférieure, sinon faux</returns>
        public static bool EstTriangulaireInferieure(double[,] matrice)
        {
            if ((matrice == null) || (matrice.Length == 0)) return false;
            int nl = matrice.GetLength(0);
            int nc = matrice.GetLength(1);
            bool auMoinsUneCelluleSignificativeEnDessous = false;
            for (int i = 0; i < nl; i++)
            {
                for (int j = 0; j < nc; j++)
                {
                    if (i < j)
                    { // La cellule à tester en (i, j) est au dessus de la diagonale principale
                        if (matrice[i, j] != 0.0) return false; // Règle du "pour tout" non vérifiée (au moins une fois), donc on a la preuve que la règle globale n'est pas respectée
                    }
                    else if (i > j)
                    { // La cellule à tester en (i, j) est en dessous de la diagonale principale
                        if (matrice[i, j] != 0.0) auMoinsUneCelluleSignificativeEnDessous = true; // Règle du "il existe" vérifiée, donc on a la preuve que cette règle est respectée ... mais on ne peut pas arrêter les tests parce qu'il reste peut-être d'autres cellules de la règle du "pour tout" à vérifier
                    }
                }
            }
            return auMoinsUneCelluleSignificativeEnDessous;
        }

        /// <summary>
        /// Indique si la matrice spécifiée correspond à une matrice triangulaire supérieure
        /// <para>Une matrice est considérée comme matrice "triangulaire supérieure" si toutes les cellules en dessous de sa diagonale principale sont égales à 0, et au moins une cellule au dessus de sa diagonale principale est différente de 0</para>
        /// </summary>
        /// <param name="matrice">Matrice dont on veut vérifier si elle correspond à une matrice triangulaire supérieure</param>
        /// <returns>Vrai si la matrice spécifiée correspond à une matrice triangulaire supérieure, sinon faux</returns>
        public static bool EstTriangulaireSuperieure(double[,] matrice)
        {
            if ((matrice == null) || (matrice.Length == 0)) return false;
            int nl = matrice.GetLength(0);
            int nc = matrice.GetLength(1);
            bool auMoinsUneCelluleSignificativeAuDessus = false;
            for (int i = 0; i < nl; i++)
            {
                for (int j = 0; j < nc; j++)
                {
                    if (i > j)
                    { // La cellule à tester en (i, j) est en dessous de la diagonale principale
                        if (matrice[i, j] != 0.0) return false; // Règle du "pour tout" non vérifiée (au moins une fois), donc on a la preuve que la règle globale n'est pas respectée
                    }
                    else if (i < j)
                    { // La cellule à tester en (i, j) est au dessus de la diagonale principale
                        if (matrice[i, j] != 0.0) auMoinsUneCelluleSignificativeAuDessus = true; // Règle du "il existe" vérifiée, donc on a la preuve que cette règle est respectée ... mais on ne peut pas arrêter les tests parce qu'il reste peut-être d'autres cellules de la règle du "pour tout" à vérifier
                    }
                }
            }
            return auMoinsUneCelluleSignificativeAuDessus;
        }

        /// <summary>
        /// Indique si, en terme de valeurs contenues, la matrice spécifiée est "quelconque"
        /// <para>Une matrice est considérée quelconque si elle n'est ni "zéro", ni "identité", ni "triangulaire inférieure", ni "triangulaire supérieure"</para>
        /// </summary>
        /// <param name="matrice">Matrice dont on veut vérifier qu'elle n'a rien de "particulier" en terme de valeurs contenues</param>
        /// <returns>Vrai si la matrice spécifiée peut être considérée comme "quelconque", sinon faux</returns>
        public static bool EstQuelconque(double[,] matrice)
        {
            return !EstZero(matrice)
                && !EstIdentite(matrice)
                && !EstTriangulaireInferieure(matrice)
                && !EstTriangulaireSuperieure(matrice);
        }

        /// <summary>
        /// Identifie un type de matrice en fonction de ses dimensions
        /// </summary>
        public enum Forme
        {
            /// <summary>
            /// Matrice inexistante ou vide
            /// </summary>
            [FonctionsAvancees.Description("Matrice inexistante ou vide")]
            Inexistante,
            /// <summary>
            /// Vecteur ligne
            /// <para>Une seule ligne, plusieurs colonnes</para>
            /// </summary>
            [FonctionsAvancees.Description("Vecteur ligne")]
            VecteurLigne,
            /// <summary>
            /// Vecteur colonne
            /// <para>Une seule colonne, plusieurs lignes</para>
            /// </summary>
            [FonctionsAvancees.Description("Vecteur colonne")]
            VecteurColonne,
            /// <summary>
            /// Matrice carrée
            /// <para>Au moins une ligne et au moins une colonne, en même quantité</para>
            /// </summary>
            [FonctionsAvancees.Description("Matrice carrée")]
            Carre,
            /// <summary>
            /// Matrice rectangulaire
            /// <para>Au moins une ligne et au moins une colonne, en quantité différente</para>
            /// </summary>
            [FonctionsAvancees.Description("Matrice rectangulaire")]
            Rectangulaire
        }

        /// <summary>
        /// Retourne le type de la matrice spécifiée en terme de dimension
        /// </summary>
        /// <param name="matrice">Matrice dont on veut déterminer la forme</param>
        /// <returns>Indicateur de forme de la matrice spécifiée</returns>
        public static Forme FormeDe(double[,] matrice)
        {
            if ((matrice == null) || (matrice.Length == 0)) return Forme.Inexistante;
            int nombreLignes = matrice.GetLength(0);
            int nombreColonnes = matrice.GetLength(1);
            if (nombreLignes == 1)
            {
                return (nombreColonnes >= 2) ? Forme.VecteurLigne : Forme.Carre;
            }
            else if (nombreColonnes == 1)
            {
                return Forme.VecteurColonne;
            }
            else
            {
                return (nombreLignes == nombreColonnes) ? Forme.Carre : Forme.Rectangulaire;
            }
        }

        /// <summary>
        /// Identifie un type de matrice en fonction des valeurs contenues
        /// </summary>
        public enum Type
        {
            /// <summary>
            /// Matrice inexistante ou vide
            /// </summary>
            [FonctionsAvancees.Description("Matrice inexistante ou vide")]
            Inexistante,
            /// <summary>
            /// Matrice "Zéro"
            /// <para>Contenant uniquement 0</para>
            /// </summary>
            [FonctionsAvancees.Description("Matrice zéro")]
            Zero,
            /// <summary>
            /// Matrice "Identité"
            /// <para>Contenant 0 partout, excepté sur la diagonale principale où il n'y a que des 1</para>
            /// </summary>
            [FonctionsAvancees.Description("Matrice identité")]
            Identite,
            /// <summary>
            /// Matrice triangulaire inférieure
            /// <para>Où toutes les cellules au dessus de la diagonale principale contiennent 0, et pour laquelle il existe au moins une cellule différente de 0 en dessous de cette diagonale</para>
            /// </summary>
            [FonctionsAvancees.Description("Matrice triangulaire inférieure")]
            TriangulaireInferieure,
            /// <summary>
            /// Matrice triangulaire supérieure
            /// <para>Où toutes les cellules en dessous de la diagonale principale contiennent 0, et pour laquelle il existe au moins une cellule différente de 0 au dessus de cette diagonale</para>
            /// </summary>
            [FonctionsAvancees.Description("Matrice triangulaire supérieure")]
            TriangulaireSuperieure,
            /// <summary>
            /// Matrice quelconque
            /// <para>Ne possède aucune particularité "standard" en terme de valeurs contenues</para>
            /// </summary>
            [FonctionsAvancees.Description("Matrice quelconque")]
            Quelconque
        }

        /// <summary>
        /// Retourne le type de la matrice spécifiée en terme de valeurs contenues
        /// </summary>
        /// <param name="matrice">Matrice dont on veut déterminer le type</param>
        /// <returns>Indicateur de type de la matrice spécifiée</returns>
        public static Type TypeDe(double[,] matrice)
        {
            if ((matrice == null) || (matrice.Length == 0)) return Type.Inexistante;
            int nombreLignes = matrice.GetLength(0);
            int nombreColonnes = matrice.GetLength(1);
            bool zeroPI = true; // Indicateur de l'unique présence de 0 en dessous de la diagonale principale
            bool zeroPS = true; // Indicateur de l'unique présence de 0 au dessus de la diagonale principale
            bool zeroDP = true; // Indicateur de l'unique présence de 0 sur la diagonale principale
            bool unDP = true; // Indicateur de l'unique présence de 1 sur la diagonale principale
            for (int i = 0; i < nombreLignes; i++)
            {
                for (int j = 0; j < nombreColonnes; j++)
                {
                    if (i > j)
                    {
                        if (matrice[i, j] != 0.0) zeroPI = false;
                    }
                    else if (i < j)
                    {
                        if (matrice[i, j] != 0.0) zeroPS = false;
                    }
                    else // (i == j)
                    {
                        if (matrice[i, j] != 0.0)
                        {
                            zeroDP = false;
                            if (matrice[i, j] != 1.0) unDP = false;
                        }
                        else
                        {
                            unDP = false;
                        }
                    }
                }
            }
            if (zeroPI) // La partie inférieure ne contient que des 0
            {
                if (zeroPS) // La partie supérieure ne contient que des 0
                {
                    if (zeroDP) return Type.Zero;
                    else if (unDP) return Type.Identite;
                    else return Type.Quelconque;
                }
                else
                {
                    return Type.TriangulaireSuperieure;
                }
            }
            else
            {
                if (zeroPS) return Type.TriangulaireInferieure;
                else return Type.Quelconque;
            }
        }
    }
}