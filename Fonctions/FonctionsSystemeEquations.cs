using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DevHopTools.Fonctions
{
    public static partial class FonctionsSystemeEquations
    {
        /// <summary>
        /// Permet de formater sous forme de chaîne de caractères, l'équation spécifiée du système spécifié
        /// </summary>
        /// <param name="A">Matrice des coefficients dépendants des inconnues</param>
        /// <param name="B">Vecteur colonne des coefficients indépendants</param>
        /// <param name="nomsInconnues">Noms des inconnues</param>
        /// <param name="i">Indice de l'équation que l'on veut formater</param>
        /// <returns>Chaîne correspondant à l'équation dûment formatée, sinon une chaîne vide</returns>
        public static string FormaterEquation(double[,] A, double[,] B, string[] nomsInconnues, int i)
        {
            if ((A == null) || (B == null) || (nomsInconnues == null)) return string.Empty;
            int taille = nomsInconnues.Length;
            if ((taille == 0)
                || (FonctionsMatriceDefinitions.NombreLignes(A) != taille)
                || (FonctionsMatriceDefinitions.NombreColonnes(A) != taille)
                || (FonctionsMatriceDefinitions.NombreLignes(B) != taille)
                || (FonctionsMatriceDefinitions.NombreColonnes(B) != 1)) return string.Empty;
            if ((i < 0) || (i >= taille)) return string.Empty;
            double[] coefficients = new double[taille];
            for (int j = 0; j < taille; j++) coefficients[j] = A[i, j];
            return FormaterEquation(coefficients, B[i, 0], nomsInconnues, taille);
        }

        /// <summary>
        /// Permet de formater l'équation spécifiée sous forme d'une chaîne de caractères
        /// </summary>
        /// <param name="A">Tableau des coefficients multiplicateurs des inconnues dans cette équation</param>
        /// <param name="B">Valeur du coefficient indépendant de cette équation</param>
        /// <param name="nomsInconnues">Noms des inconnues</param>
        /// <returns>Chaîne correspondant à l'équation dûment formatée, sinon une chaîne vide</returns>
        public static string FormaterEquation(double[] A, double b, string[] nomsInconnues)
        {
            if ((A == null) || (nomsInconnues == null)) return string.Empty;
            int taille = nomsInconnues.Length;
            if ((taille == 0) || (A.Length != taille)) return string.Empty;
            return FormaterEquation(A, b, nomsInconnues, taille);
        }

        /// <summary>
        /// Permet de formater l'équation spécifiée sous forme d'une chaîne de caractères
        /// <para>Uniquement à usage interne de cette classe FSysteme</para>
        /// </summary>
        /// <param name="A">Tableau des coefficients multiplicateurs des inconnues dans cette équation</param>
        /// <param name="B">Valeur du coefficient indépendant de cette équation</param>
        /// <param name="nomsInconnues">Noms des inconnues</param>
        /// <param name="taille">Indique le nombre d'inconnues, et donc également, le nombre de coefficients multiplicateurs</param>
        /// <returns>Chaîne correspondant à l'équation dûment formatée, sinon une chaîne vide</returns>
        private static string FormaterEquation(double[] A, double b, string[] nomsInconnues, int taille)
        {
            StringBuilder resultat = new StringBuilder();
            int compteurTermes = 0;
            for (int j = 0; j < taille; j++)
            {
                if (A[j] != 0.0)
                {
                    if (compteurTermes == 0)
                    {
                        if (A[j] == -1.0)
                            resultat.Append('-');
                        else if (A[j] != 1.0)
                            resultat.Append($"{A[j]} ");
                    }
                    else
                    {
                        if (A[j] == 1.0)
                        {
                            resultat.Append(" + ");
                        }
                        else
                        {
                            if (A[j] > 0.0)
                                resultat.Append($" + {A[j]} ");
                            else if (A[j] == -1.0)
                                resultat.Append(" - ");
                            else
                                resultat.Append($" - {-A[j]} ");
                        }
                    }
                    resultat.Append($"{nomsInconnues[j]}");
                    compteurTermes++;
                }
            }
            if (compteurTermes == 0) Console.Write("0");
            resultat.Append($" = {b}");
            return resultat.ToString();
        }

        /// <summary>
        /// Permet de vérifier la validité d'un nom d'inconnue
        /// </summary>
        /// <param name="nom">Nom d'une inconnue</param>
        /// <returns>Vrai si le nom spécifié respecte les règles de validité d'un nom d'inconnue dans un système d'équations, sinon faux</returns>
        public static bool TesterValiditeNom(string nom)
        {
            if (string.IsNullOrWhiteSpace(nom)) return false;
            for (int i = 0; i < nom.Length; i++)
            {
                if (!char.IsLetter(nom[i]))
                {
                    if (i == 0) return false;
                    if (!char.IsDigit(nom[i])) return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Tente de charger un système de N équations linéaires à N inconnues en valeur réelle, à partir d'un fichier texte
        /// </summary>
        /// <param name="nomFichier">Nom du fichier à charger si possible</param>
        /// <param name="A">Matrice des coefficients dépendants des inconnues, si possible, sinon null</param>
        /// <param name="B">Vecteur colonne des coefficients indépendants, si possible, sinon null</param>
        /// <param name="nomsInconnues">Noms des inconnues, si possible, sinon null</param>
        /// <param name="tailleMaximaleAutorisee">Taille maximale autorisée pour le système à charger</param>
        /// <returns>Vrai si le chargement a pu se faire, sinon faux</returns>
        public static bool Charger(string nomFichier, out double[,] A, out double[,] B, out string[] nomsInconnues, int tailleMaximaleAutorisee = 1000)
        {
            return Charger_Normalise(nomFichier, out A, out B, out nomsInconnues, tailleMaximaleAutorisee)
                || Charger_FormatLibre(nomFichier, out A, out B, out nomsInconnues, tailleMaximaleAutorisee);
        }

        /// <summary>
        /// Tente de charger un système de N équations linéaires à N inconnues en valeur réelle, à partir d'un fichier texte au format NORMALISE
        /// </summary>
        /// <param name="nomFichier">Nom du fichier à charger si possible</param>
        /// <param name="A">Matrice des coefficients dépendants des inconnues, si possible, sinon null</param>
        /// <param name="B">Vecteur colonne des coefficients indépendants, si possible, sinon null</param>
        /// <param name="nomsInconnues">Noms des inconnues, si possible, sinon null</param>
        /// <param name="tailleMaximaleAutorisee">Taille maximale autorisée pour le système à charger</param>
        /// <returns>Vrai si le chargement a pu se faire, sinon faux</returns>
        private static bool Charger_Normalise(string nomFichier, out double[,] A, out double[,] B, out string[] nomsInconnues, int tailleMaximaleAutorisee = 1000)
        {
            char[] separateurs = new char[] { '\t' };
            try
            {
                using (StreamReader fichier = new StreamReader(nomFichier, Encoding.Default, true))
                {
                    #region Traitement de la première ligne => taille
                    if (fichier.EndOfStream) throw new Exception("Impossible de lire la ligne n°1 qui doit préciser la taille du système !");
                    string[] champs = fichier.ReadLine().Split(separateurs, 2);
                    int taille;
                    if (!FonctionsChaines.TenterConvertir(champs[0], out taille)) throw new Exception("A la ligne n°1, le premier champ ne correspond pas à une valeur entière ; il devait normalement préciser la taille du système !");
                    if (taille < 1) throw new Exception("A la ligne n°1, le premier champ précise une taille nulle ou négative pour le système ; ce qui n'a aucun sens !");
                    if (taille > tailleMaximaleAutorisee) throw new Exception($"A la ligne n°1, le premier champ précise une taille excessive pour le système ; la taille indiquée est de {taille} pour une taille maximale de {tailleMaximaleAutorisee} !");
                    #endregion
                    #region Traitement de la seconde ligne => nomsInconnues
                    if (fichier.EndOfStream) throw new Exception("Impossible de lire la ligne n°2 qui doit préciser les noms des inconnues du système !");
                    champs = fichier.ReadLine().Split(separateurs, taille + 1);
                    string plurielTaille = (taille >= 2) ? "s" : "";
                    if (champs.Length < taille)
                    {
                        string plurielEffectif = (champs.Length >= 2) ? "s" : "";
                        throw new Exception($"A la ligne n°2, il faudrait au minimum {taille} champ{plurielTaille} utile{plurielTaille} afin de définir les noms des inconnues du système ; il n'y a malheureusement que {champs.Length} champ{plurielEffectif} !");
                    }
                    champs = champs.Take(taille).Select(champ => champ.Trim()).Where(champ => champ.Length >= 1).ToArray();
                    if (champs.Length < taille)
                    {
                        string plurielEffectif = (champs.Length >= 2) ? "s" : "";
                        throw new Exception($"A la ligne n°2, il faudrait {taille} champ{plurielTaille} défini{plurielTaille} (c'est à dire non vide après suppression des espaces de début et de fin) afin de définir correctement les noms des inconnues du système ; il n'y a malheureusement que {champs.Length} champ{plurielEffectif} significati{plurielEffectif} !");
                    }
                    if (champs.Any(champ => !TesterValiditeNom(champ))) throw new Exception("A la ligne n°2, on remarque qu'il existe au moins un nom d'inconnue qui ne respecte pas les règles de nomenclature attendue ; à savoir, une lettre comme premier caractère, et éventuellement suivi de lettre(s) et/ou chiffre(s) !");
                    champs = champs.Distinct().ToArray();
                    if (champs.Length < taille)
                    {
                        string plurielEffectif = (champs.Length >= 2) ? "s" : "";
                        throw new Exception($"A la ligne n°2, on remarque qu'il existe au moins un nom d'inconnue en doublon d'un autre ; ce qui est interdit, car cela rend impossible l'identification d'une inconnue au sein du système !");
                    }
                    nomsInconnues = champs.ToArray();
                    #endregion
                    #region Traitement des (taille) lignes suivantes : conjointement les lignes de la matrice A et du vecteur colonne B
                    A = new double[taille, taille];
                    B = new double[taille, 1];
                    for (int i = 0; i < taille; i++)
                    {
                        if (fichier.EndOfStream) throw new Exception($"Impossible de lire la ligne n°{3 + i} qui doit détailler les coefficients de l'équation n°{i + 1} du système !");
                        champs = fichier.ReadLine().Split(separateurs, (taille + 1) + 1);
                        if (champs.Length < (taille + 1))
                        {
                            string plurielEffectif = (champs.Length >= 2) ? "s" : "";
                            throw new Exception($"A la ligne n°{3 + i}, correspondant à l'équation n°{i + 1}, il faudrait au minimum {taille + 1} champs afin de définir les valeurs des coefficients dépendants des inconnues du système ainsi que la valeur du coefficient indépendant au sein de cette équation ; il n'y a malheureusement que {champs.Length} champ{plurielEffectif} !");
                        }
                        double valeur;
                        double[] coefficients = champs.Take(taille + 1).Select(champ => FonctionsChaines.TenterConvertir(champ, out valeur) ? valeur : double.NaN).ToArray();
                        if (coefficients.Any(coefficient => double.IsNaN(coefficient)))
                            throw new Exception($"A la ligne n°{3 + i}, correspondant à l'équation n°{i + 1}, on remarque la présence d'au moins un coefficient non numérique !");
                        for (int j = 0; j < taille; j++) A[i, j] = coefficients[j];
                        B[i, 0] = coefficients[taille];
                    }
                    #endregion
                }
                return true;
            }
            catch (Exception erreur)
            {
                System.Diagnostics.Debug.WriteLine($"\nErreur lors du chargement du système {nomFichier} au format normalisé :\n{erreur.Message}\n");
                A = null;
                B = null;
                nomsInconnues = null;
                return false;
            }
        }

        /// <summary>
        /// Tente de charger un système de N équations linéaires à N inconnues en valeur réelle, à partir d'un fichier texte au format LIBRE
        /// </summary>
        /// <param name="nomFichier">Nom du fichier à charger si possible</param>
        /// <param name="A">Matrice des coefficients dépendants des inconnues, si possible, sinon null</param>
        /// <param name="B">Vecteur colonne des coefficients indépendants, si possible, sinon null</param>
        /// <param name="nomsInconnues">Noms des inconnues, si possible, sinon null</param>
        /// <param name="tailleMaximaleAutorisee">Taille maximale autorisée pour le système à charger</param>
        /// <returns>Vrai si le chargement a pu se faire, sinon faux</returns>
        private static bool Charger_FormatLibre(string nomFichier, out double[,] A, out double[,] B, out string[] nomsInconnues, int tailleMaximaleAutorisee = 1000)
        {
            try
            {
                using (StreamReader fichier = new StreamReader(nomFichier, Encoding.Default, true))
                {
                    List<string> noms = new List<string>(); // Noms des inconnues
                    List<List<double>> cd = new List<List<double>>(); // Coefficients dépendants des inconnues par équation
                    List<double> ci = new List<double>(); // Coefficient indépendant par équation
                    while (!fichier.EndOfStream)
                    {
                        string ligne = fichier.ReadLine();
                        string[] parties = ligne.Split('=');
                        if (parties.Length != 2) continue;
                        double valeurCI = 0.0;
                        Dictionary<string, double> valeursCD = new Dictionary<string, double>();
                        bool erreur = false;
                        for (int iP = 0; iP < 2; iP++)
                        {
                            double multiplicateurP = (iP == 0) ? 1.0 : -1.0;
                            string expression = parties[iP].Trim();
                            if (expression.Length == 0)
                            {
                                erreur = true;
                                break;
                            }
                            if (expression.StartsWith("-")) expression = $"0 {expression}"; else expression = $"0 + {expression}";
                            string[] termes = expression.Replace("-", " + -").Split('+').Select(terme => terme.Replace(" ", "")).ToArray();
                            for (int iT = 0; iT < termes.Length; iT++)
                            {
                                double coefficient;
                                string nomInconnue;
                                if (!DecomposerTerme(termes[iT], out coefficient, out nomInconnue))
                                {
                                    erreur = true;
                                    break;
                                }
                                if (nomInconnue == null)
                                {
                                    valeurCI -= multiplicateurP * coefficient;
                                }
                                else
                                {
                                    if (!valeursCD.ContainsKey(nomInconnue)) valeursCD.Add(nomInconnue, 0.0);
                                    valeursCD[nomInconnue] += multiplicateurP * coefficient;
                                }
                            }
                            if (erreur) break;
                        }
                        if (erreur) continue;
                        // Arrivé ici, la ligne lue correspond à une équation syntaxiquement valable
                        foreach (string nom in valeursCD.Keys)
                        {
                            AjouterInconnue(nom, noms, cd);
                        }
                        List<double> coefficients = new List<double>();
                        for (int j = 0; j < noms.Count; j++)
                        {
                            double coefficient;
                            coefficients.Add(valeursCD.TryGetValue(noms[j], out coefficient) ? coefficient : 0.0);
                        }
                        cd.Add(coefficients);
                        ci.Add(valeurCI);
                    }
                    if (noms.Count == 0) throw new Exception("Ce système ne référence aucune inconnue !");
                    if (noms.Count != cd.Count) throw new Exception($"Ce système référence un nombre d'inconnues différent du nombre d'équations qui le composent : {cd.Count} équation(s) pour {noms.Count} inconnue(s) !");
                    int taille = noms.Count;
                    nomsInconnues = noms.ToArray();
                    A = new double[taille, taille];
                    B = new double[taille, 1];
                    for (int i = 0; i < taille; i++)
                    {
                        for (int j = 0; j < taille; j++)
                        {
                            A[i, j] = cd[i][j];
                        }
                        B[i, 0] = ci[i];
                    }
                }
                return true;
            }
            catch (Exception erreur)
            {
                System.Diagnostics.Debug.WriteLine($"\nErreur lors du chargement du système {nomFichier} au format libre :\n{erreur.Message}\n");
                A = null;
                B = null;
                nomsInconnues = null;
                return false;
            }
        }

        /// <summary>
        /// Tente de décomposer un terme d'une expression arithmétique pouvant contenir un coefficient et/ou un nom d'inconnue
        /// </summary>
        /// <param name="terme">Terme à décomposer</param>
        /// <param name="coefficient">Valeur du coefficient (multiplicateur)</param>
        /// <param name="nomInconnue">Nom de l'inconnue</param>
        /// <returns>Vrai si ce terme est syntaxiquement correct et a pu être décomposé, sinon faux</returns>
        private static bool DecomposerTerme(string terme, out double coefficient, out string nomInconnue)
        {
            coefficient = double.NaN;
            nomInconnue = null;
            if (terme.Length == 0) return false;
            double multiplicateur = 1.0;
            int i = 0;
            if (terme[i] == '-')
            {
                if (terme.Length == 1) return false;
                multiplicateur = -1.0;
                i++;
            }
            int iDebut = i;
            while ((i < terme.Length) && (char.IsDigit(terme[i]) || (terme[i] == ',') || (terme[i] == '.'))) i++;
            string texteCoefficient = terme.Substring(iDebut, i - iDebut);
            if (texteCoefficient.Length == 0)
            {
                if (i == terme.Length) return false;
                coefficient = multiplicateur;
            }
            else
            {
                if (!FonctionsChaines.TenterConvertir(texteCoefficient, out coefficient)) return false;
                coefficient *= multiplicateur;
            }
            if (i < terme.Length)
            {
                nomInconnue = terme.Substring(i);
                if (!TesterValiditeNom(nomInconnue)) return false;
            }
            return true;
        }

        /// <summary>
        /// Permet d'ajouter si nécessaire le nom de l'inconnue spécifiée, tout en corrigeant les "lignes" précédentes de la matrice A représentée par la List&lt;List&lt;double&gt;&gt;
        /// </summary>
        /// <param name="nomInconnue">Nom de l'inconnue à prendre en compte</param>
        /// <param name="noms">Liste des noms des inconnues</param>
        /// <param name="cd">List&lt;List&lt;double&gt;&gt; représentant la matrice A dans son état actuel</param>
        /// <returns>Vrai si cette inconnue a du être ajoutée, sinon faux</returns>
        private static bool AjouterInconnue(string nomInconnue, List<string> noms, List<List<double>> cd)
        {
            int indiceInsertion;
            if (FonctionsAvancees.RechercherParDichotomie(noms, nomInconnue, out indiceInsertion)) return false;
            noms.Insert(indiceInsertion, nomInconnue);
            for (int i = 0; i < cd.Count; i++)
            {
                cd[i].Insert(indiceInsertion, 0.0);
            }
            return true;
        }

        /// <summary>
        /// Méthode de résolution d'un système de N équations linéaires à N inconnues en valeur réelle
        /// </summary>
        public enum Resolution
        {
            /// <summary>
            /// Résolution "classique" par le produit matriciel de l'inverse de la matrice des coefficients dépendants, avec le vecteur colonne des coefficients indépendants
            /// </summary>
            InverseMatriciel,
            /// <summary>
            /// Résolution "optimisée" par la décomposition LUP, suivie par des substitutions avant et arrière
            /// </summary>
            DecompositionLUP
        }

        /// <summary>
        /// Tente de résoudre le système défini par la matrice des coefficients dépendants (A) et le vecteur colonne des coefficients indépendants (B)
        /// </summary>
        /// <param name="A">Matrice des coefficients dépendants des inconnues</param>
        /// <param name="B">Vecteur colonne des coefficients indépendants</param>
        /// <param name="solution">Vecteur colonne de la solution, si possible, sinon null</param>
        /// <param name="resolution">Méthode de résolution à appliquer</param>
        /// <returns>Vrai si le système spécifié par A et B a pu être résolu, sinon faux</returns>
        public static bool Resoudre(double[,] A, double[,] B, out double[,] solution, Resolution resolution)
        {
            if (!FonctionsMatriceDefinitions.EstCarree(A)
                || !FonctionsMatriceDefinitions.EstVecteurColonne(B)
                || (FonctionsMatriceDefinitions.NombreLignes(A) != FonctionsMatriceDefinitions.NombreLignes(B)))
            {
                solution = null;
                return false;
            }
            switch (resolution)
            {
                case Resolution.InverseMatriciel:
                    return Resoudre_ParInverse(A, B, out solution);
                case Resolution.DecompositionLUP:
                    return Resoudre_ParLUP(A, B, out solution);
                default:
                    solution = null;
                    return false;
            }
        }

        /// <summary>
        /// Tente de résoudre le système défini par la matrice des coefficients dépendants (A) et le vecteur colonne des coefficients indépendants (B)
        /// <para>Résolution par le produit matriciel de l'inverse de la matrice des coefficients dépendants, avec le vecteur colonne des coefficients indépendants</para>
        /// </summary>
        /// <param name="A">Matrice des coefficients dépendants des inconnues</param>
        /// <param name="B">Vecteur colonne des coefficients indépendants</param>
        /// <param name="solution">Vecteur colonne de la solution, si possible, sinon null</param>
        /// <returns>Vrai si le système spécifié par A et B a pu être résolu, sinon faux</returns>
        private static bool Resoudre_ParInverse(double[,] A, double[,] B, out double[,] solution)
        {
            int taille = FonctionsMatriceDefinitions.NombreLignes(A);
            double[,] invA;
            if (!FonctionsMatriceOperations.Inverser(A, out invA, FonctionsMatriceOperations.CalculOptimiseConseille(A, 0.1))
             || !FonctionsMatriceOperations.Multiplier(invA, B, out solution))
            {
                solution = FonctionsMatriceCycleDeVie.CreerMatrice(taille, 1, double.NaN);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Tente de résoudre le système défini par la matrice des coefficients dépendants (A) et le vecteur colonne des coefficients indépendants (B)
        /// <para>Résolution par la décomposition LUP, suivie par des substitutions avant et arrière</para>
        /// </summary>
        /// <param name="A">Matrice des coefficients dépendants des inconnues</param>
        /// <param name="B">Vecteur colonne des coefficients indépendants</param>
        /// <param name="solution">Vecteur colonne de la solution, si possible, sinon null</param>
        /// <returns>Vrai si le système spécifié par A et B a pu être résolu, sinon faux</returns>
        private static bool Resoudre_ParLUP(double[,] A, double[,] B, out double[,] solution)
        {
            solution = null;
            // Si on n'arrive pas à décomposer la matrice A en LUP, alors le système n'a pas de solution (unique)
            if (!RealiserDecompositionLUP(A, out int n, out double[,] L, out double[,] U, out int[] indicesPermutation)) return false;
            // Dimensionnement des deux vecteurs colonnes à calculer : X et Y
            double[,] X = new double[n, 1];
            double[,] Y = new double[n, 1];
            // Résolution du système L.Y = B par substitution avant
            /*
            POUR i ALLANT DE 1 A n FAIRE
                Y[i] = B[indicesPermutation[i]] – SOMME DE L[i,j]*Y[j] POUR j ALLANT DE 1 A i-1)
            FIN POUR i
            */
            for (int i = 0; i < n; i++)
            {
                Y[i, 0] = B[indicesPermutation[i], 0] - SommePourSubstitution(L, Y, i, 0, i - 1);
            }
            // Résolution du système U.X = Y par substitution après
            /*
            POUR i ALLANT DE n A 1 FAIRE
                X[i] = (Y[i] – SOMME DE U[i,j]*X[j] POUR j ALLANT DE i+1 A n) / U[i,i]
            FIN POUR i
            */
            for (int i = n - 1; i >= 0; i--)
            {
                X[i, 0] = (Y[i, 0] - SommePourSubstitution(U, X, i, i + 1, n - 1)) / U[i, i];
            }
            // Affectation du vecteur colonne X comme solution du système initial : A.X = B
            solution = X;
            return true;
        }

        /// <summary>
        /// Permet d'effectuer un calcul de somme dans le cadre d'une substitution avant ou arrière lors de la résolution d'un système dont la matrice des coefficients dépendants est une matrice triangulaire (inférieure ou supérieure)
        /// </summary>
        /// <param name="matrice">Matrice triangulaire</param>
        /// <param name="vecteurColonne">Vecteur colonne</param>
        /// <param name="i">Indice d'équation en cours de résolution</param>
        /// <param name="jMin">Borne minimale comprise sur l'indice variant dans ce calcul de somme</param>
        /// <param name="jMax">Borne maximale comprise sur l'indice variant dans ce calcul de somme</param>
        /// <returns>Valeur de la somme calculée</returns>
        private static double SommePourSubstitution(double[,] matrice, double[,] vecteurColonne, int i, int jMin, int jMax)
        {
            double somme = 0.0;
            for (int j = jMin; j <= jMax; j++)
            {
                somme += matrice[i, j] * vecteurColonne[j, 0];
            }
            return somme;
        }

        /// <summary>
        /// Tente de réaliser la décomposition LUP de la matrice A des coefficients dépendants des inconnues d'un système d'équations (A.X = B)
        /// </summary>
        /// <param name="A">Matrice des coefficients dépendants des inconnues</param>
        /// <param name="n">Taille du système</param>
        /// <param name="L">Matrice triangulaire inférieure résultant de la décomposition LUP de la matrice A</param>
        /// <param name="U">Matrice triangulaire supérieure résultant de la décomposition LUP de la matrice A</param>
        /// <param name="indicesPermutation">Egalement noté Pi, correspond au tableau des indices reflétant les permutations d'équations effectuées pour obtenir les matrices L et U</param>
        /// <returns>Vrai si la décomposition LUP a pu se faire complètement, sinon faux</returns>
        private static bool RealiserDecompositionLUP(double[,] A, out int n, out double[,] L, out double[,] U, out int[] indicesPermutation)
        {
            indicesPermutation = null;
            L = null;
            U = null;
            n = FonctionsMatriceDefinitions.NombreLignes(A);
            if (n < 1) return false;
            // Initialiser par défaut le tableau de permutation (dans cette situation, il représente une matrice identité en terme de permutation - c'est à dire, aucune permutation)
            /*
            POUR k ALLANT DE 1 A n FAIRE
                π[k] = k
            FIN POUR k
            */
            indicesPermutation = new int[n];
            for (int k = 0; k < n; k++)
            {
                indicesPermutation[k] = k;
            }
            // Transformer le contenu de la matrice A afin qu'elle contienne au final (après ce mécanisme de décomposition) les valeurs de L et U
            /*
            POUR k ALLANT DE 1 A n FAIRE
                p = 0
                POUR i ALLANT DE k A n FAIRE
                    SI Abs(A[i,k]) > p ALORS
                        p = Abs(A[i,k])
                        k’ = i
                    FIN SI
                FIN POUR i
                SI p == 0 ALORS
                    realisable = FAUX
                    FIN DE L’ALGORITHME
                FIN SI
                PERMUTER LES VALEURS DE π[k] ET π[k’]
                POUR j ALLANT DE 1 A n FAIRE
                    PERMUTER LES VALEURS DE A[k,j] ET A[k’,j]
                FIN POUR j
                POUR i ALLANT DE k+1 A n FAIRE
                    A[i,k] = A[i,k] / A[k,k]
                    POUR j ALLANT DE k+1 A n FAIRE
                        A[i,j] = A[i,j] - A[i,k]*A[k,j] 
                    FIN POUR j
                FIN POUR i
            FIN POUR k
            */
            double[,] Adecompose = FonctionsMatriceCycleDeVie.Dupliquer(A);
            for (int k = 0; k < n; k++)
            {
                // Recherche de p, la valeur absolue strictement positive et qui est la plus grande dans la colonne k de la matrice A en cours de décomposition : au final, il s'agit surtout de déterminer dans quelle ligne se trouve cette valeur absolue strictement positive la plus grande possible dans la colonne k ; l'indice de cette ligne est noté k' (kPrime)
                double p = 0.0;
                int kPrime = -1;
                for (int i = k; i < n; i++)
                {
                    double valeurAbsolue = Math.Abs(Adecompose[i, k]);
                    if (valeurAbsolue > p)
                    {
                        p = valeurAbsolue;
                        kPrime = i;
                    }
                }
                if (p == 0.0) return false;
                // On notifie une permutation des lignes k et k' (kPrime)
                FonctionsAvancees.Permuter(ref indicesPermutation[k], ref indicesPermutation[kPrime]);
                // On réalise dans A la permutation des valeurs des lignes k et k' (kPrime)
                for (int j = 0; j < n; j++)
                {
                    FonctionsAvancees.Permuter(ref Adecompose[k, j], ref Adecompose[kPrime, j]);
                }
                // On réalise la transformation dite de "la règle du pivot"
                for (int i = k + 1; i < n; i++)
                {
                    Adecompose[i, k] = Adecompose[i, k] / Adecompose[k, k];
                    for (int j = k + 1; j < n; j++)
                    {
                        Adecompose[i, j] = Adecompose[i, j] - Adecompose[i, k] * Adecompose[k, j];
                    }
                }
            }
            // INITIALISER L A 0
            L = new double[n, n];
            // INITIALISER U A 0
            U = new double[n, n];
            // Remplir les parties intéressantes (non nulles par définition) des matrices L (triangulaire inférieure) et U (triangulaire supérieure)
            /*
            POUR i ALLANT DE 1 A n FAIRE
                POUR j ALLANT DE 1 A n FAIRE
                    SI i > j ALORS
                        L[i,j] = A[i,j]
                    SINON
                        U[i,j] = A[i,j]
                    FIN SI
                FIN POUR j
            FIN POUR i
            POUR i ALLANT DE 1 A n FAIRE
                L[i,i] = 1
            FIN POUR i
            */
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    if (i > j)
                    {
                        // Si la cellule en i,j se trouve en dessous de la diagonale principale, c'est donc une position "intéressante" pour la matrice triangulaire inférieure : L
                        L[i, j] = Adecompose[i, j];
                    }
                    else
                    {
                        // Sinon la cellule en i,j se trouve sur, ou dessus de, la diagonale principale, c'est donc une position "intéressante" pour la matrice triangulaire supérieure : U
                        U[i, j] = Adecompose[i, j];
                    }
                }
            }
            // Remplir de 1 la diagonale principale de la matrice L (qui n'a pas profité de la double boucle précédente pour recevoir des valeurs éventuellement non nulles sur sa diagonale principale)
            for (int i = 0; i < n; i++)
            {
                L[i, i] = 1.0;
            }
            return true;
        }
    }
}