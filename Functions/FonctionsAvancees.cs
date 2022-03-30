using System;
using System.Collections.Generic;
using System.Linq;

namespace DevHopTools.Functions
{
    public static partial class FonctionsAvancees
    {
        public static void Permuter<T>(ref T a, ref T b)
        {
            T tmp = a;
            a = b;
            b = tmp;
        }

        public static void Permuter<T>(List<T> liste, int indice1, int indice2, bool verifierValiditeParametres = true)
        {
            if (!verifierValiditeParametres || ((liste != null) && (indice1 >= 0) && (indice1 < liste.Count) && (indice2 >= 0) && (indice2 < liste.Count)))
            {
                T tmp = liste[indice1];
                liste[indice1] = liste[indice2];
                liste[indice2] = tmp;
            }
        }

        public delegate bool MethodeTestMeilleur(int indiceElementTeste, int indiceMeilleurElementActuel);

        public static bool Trier(params object[] listes)
        {
            if ((listes == null) || (listes.Length < 2)) return false;
            int nombreListes = listes.Length - 1;
            int nombreElements = 0;
            for (int indice = 0; indice < nombreListes; indice++)
            {
                if (listes[indice] == null) return false;
                var typeParametre = listes[indice].GetType();
                if (!typeParametre.FullName.StartsWith("System.Collections.Generic.List")) return false;
                var methodeGetCount = typeParametre.GetProperty("Count").GetGetMethod();
                int nombreElementsDeCetteListe = (int)methodeGetCount.Invoke(listes[indice], null);
                if (indice == 0)
                {
                    nombreElements = nombreElementsDeCetteListe;
                }
                else
                {
                    if (nombreElementsDeCetteListe != nombreElements) return false;
                }
            }
            if (!(listes[listes.Length - 1] is MethodeTestMeilleur)) return false;
            MethodeTestMeilleur testerMeilleur = listes[listes.Length - 1] as MethodeTestMeilleur;
            for (int indiceDebutNonTrie = 0; indiceDebutNonTrie < nombreElements; indiceDebutNonTrie++)
            {
                int indiceMeilleur = indiceDebutNonTrie;
                for (int indice = indiceDebutNonTrie + 1; indice < nombreElements; indice++)
                {
                    if (testerMeilleur(indice, indiceMeilleur))
                    {
                        indiceMeilleur = indice;
                    }
                }
                if (indiceMeilleur != indiceDebutNonTrie)
                {
                    for (int indice = 0; indice < nombreListes; indice++)
                    {
                        var typeParametre = listes[indice].GetType();
                        var proprieteItem = typeParametre.GetProperty("Item");
                        var methodeGetItem = proprieteItem.GetGetMethod();
                        var methodeSetItem = proprieteItem.GetSetMethod();
                        object elementMeilleur = methodeGetItem.Invoke(listes[indice], new object[] { indiceMeilleur });
                        object elementDebut = methodeGetItem.Invoke(listes[indice], new object[] { indiceDebutNonTrie });
                        methodeSetItem.Invoke(listes[indice], new object[] { indiceMeilleur, elementDebut });
                        methodeSetItem.Invoke(listes[indice], new object[] { indiceDebutNonTrie, elementMeilleur });
                    }
                }
            }
            return true;
        }

        public static bool RechercherParDichotomie<T>(List<T> elements, T element, out int indice)
            where T : IComparable
        {
            if (elements.Count == 0)
            {
                indice = 0;
                return false;
            }
            int i1 = 0;
            int i2 = elements.Count - 1;
            int iM = (i1 + i2) / 2;
            do
            {
                if (element.CompareTo(iM) <= 0)
                {
                    i2 = iM;
                }
                else
                {
                    i1 = iM + 1;
                }
                iM = (i1 + i2) / 2;
            } while (i1 < i2);
            if (element.CompareTo(elements[iM]) <= 0)
            {
                indice = iM;
                return element.CompareTo(elements[iM]) == 0;
            }
            else
            {
                indice = iM + 1;
                return false;
            }
        }

        public static IEnumerable<T> SansDoublon<T>(this IEnumerable<T> ensemble, Func<T, T, bool> comparateur)
        {
            if ((ensemble == null) || (comparateur == null)) yield break;
            List<T> elementsRetournes = new List<T>();
            foreach (T element in ensemble)
            {
                if (!elementsRetournes.Any(e => comparateur(e, element)))
                {
                    elementsRetournes.Add(element);
                    yield return element;
                }
            }
        }

        public class DescriptionAttribute : Attribute
        {
            private string _description;
            public string Texte => _description;
            public DescriptionAttribute(string description)
            {
                _description = description;
            }
        }

        public static string DescriptionDe<T>(T valeur)
            where T : struct, IConvertible, IComparable
        {
            var typeEnumere = typeof(T);
            var nomValeur = valeur.ToString();
            var informationChamp = typeEnumere.GetField(nomValeur);
            if (informationChamp == null) return nomValeur;
            var description = informationChamp.GetCustomAttributes(typeof(DescriptionAttribute), false).FirstOrDefault() as DescriptionAttribute;
            return (description != null) ? description.Texte : nomValeur;
        }

        public static string Description<T>(this T valeur)
            where T : struct, IConvertible, IComparable
        {
            return DescriptionDe<T>(valeur);
        }

        public static IEnumerable<string> DescriptionDe<T>()
            where T : struct, IConvertible, IComparable
        {
            var typeEnumere = typeof(T);
            Array valeurs;
            try
            {
                valeurs = Enum.GetValues(typeEnumere);
            }
            catch
            {
                yield break;
            }
            foreach (var valeur in valeurs.OfType<T>())
            {
                var nomValeur = valeur.ToString();
                var informationChamp = typeEnumere.GetField(nomValeur);
                if (informationChamp == null) yield break;
                var description = informationChamp.GetCustomAttributes(typeof(DescriptionAttribute), false).FirstOrDefault() as DescriptionAttribute;
                yield return (description != null) ? description.Texte : nomValeur;
            }
        }
    }
}