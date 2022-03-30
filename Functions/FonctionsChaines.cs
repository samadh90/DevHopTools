using System.Linq;
using System.Text;

namespace DevHopTools.Functions
{
    public static partial class FonctionsChaines
    {
        private const string c_LettresMajusculesAccentuees = "ÁÀÂÄÃÉÈÊËÍÌÎÏÓÒÔÖÕÚÙÛÜÝŸÇÑÆŒ";
        private const string c_LettresMinusculesAccentuees = "áàâäãéèêëíìîïóòôöõúùûüýÿçñæ";
        private const string c_LettresMajusculesSansAccent = "AAAAAEEEEIIIIOOOOOUUUUYYCNAO";
        private const string c_LettresMinusculesSansAccent = "aaaaaeeeeiiiiooooouuuuyycnao";

        private const string c_VoyellesMajuscules = "AEIOUYÁÀÂÄÃÉÈÊËÍÌÎÏÓÒÔÖÕÚÙÛÜÝŸ";
        private const string c_VoyellesMinuscules = "aeiouyáàâäãéèêëíìîïóòôöõúùûüýÿ";
        private const string c_Voyelles = c_VoyellesMajuscules + c_VoyellesMinuscules;

        public static bool EstChiffre(char caractere)
        {
            return (caractere >= '0') && (caractere <= '9');
        }

        public static bool EstLettreNonAccentuee(char caractere)
        {
            return ((caractere >= 'A') && (caractere <= 'Z')) || ((caractere >= 'a') && (caractere <= 'z'));
        }

        public static bool EstLettreAccentuee(char caractere)
        {
            return c_LettresMajusculesAccentuees.Contains(caractere) || c_LettresMinusculesAccentuees.Contains(caractere);
        }

        public static bool EstLettre(char caractere)
        {
            return ((caractere >= 'A') && (caractere <= 'Z'))
                || ((caractere >= 'a') && (caractere <= 'z'))
                || c_LettresMajusculesAccentuees.Contains(caractere)
                || c_LettresMinusculesAccentuees.Contains(caractere);
        }

        public static bool EstLettreMinuscule(char caractere)
        {
            return ((caractere >= 'a') && (caractere <= 'z')) || c_LettresMinusculesAccentuees.Contains(caractere);
        }

        public static bool EstLettreMajuscule(char caractere)
        {
            return ((caractere >= 'A') && (caractere <= 'Z')) || c_LettresMajusculesAccentuees.Contains(caractere);
        }

        public static char SansAccent(char caractere)
        {
            int indice = c_LettresMinusculesAccentuees.IndexOf(caractere);
            if (indice >= 0) return c_LettresMinusculesSansAccent[indice];
            indice = c_LettresMajusculesAccentuees.IndexOf(caractere);
            if (indice >= 0) return c_LettresMajusculesSansAccent[indice];
            return caractere;
        }

        public static char EnMiniscule(char caractere)
        {
            if ((caractere >= 'A') && (caractere <= 'Z')) return (char)('a' + (caractere - 'A'));
            int indice = c_LettresMajusculesAccentuees.IndexOf(caractere);
            if (indice >= 0) return c_LettresMinusculesSansAccent[indice];
            return caractere;
        }

        public static char EnMajuscule(char caractere)
        {
            if ((caractere >= 'a') && (caractere <= 'z')) return (char)('A' + (caractere - 'a'));
            int indice = c_LettresMajusculesAccentuees.IndexOf(caractere);
            if (indice >= 0) return c_LettresMajusculesAccentuees[indice];
            return caractere;
        }

        public static bool EstVoyelle(char caractere)
        {
            return c_Voyelles.Contains(caractere);
        }

        public static bool EstConsonne(char caractere)
        {
            return EstLettre(caractere) && !EstVoyelle(caractere);
        }

        public static bool ChaineEgales(string chaine1, string chaine2)
        {
            if (chaine1 == null) return (chaine2 == null);
            if (chaine2 == null) return false;
            if (chaine1.Length != chaine2.Length) return false;
            for (int indice = 0, longueur = chaine1.Length; indice < longueur; indice++)
            {
                char caractere1 = char.ToUpper(SansAccent(chaine1[indice]));
                char caractere2 = char.ToUpper(SansAccent(chaine2[indice]));
                if (caractere1 != caractere2) return false;
            }
            return true;
        }

        public static string EnMajuscule(string chaine)
        {
            if (string.IsNullOrEmpty(chaine)) return chaine;
            StringBuilder resultat = new StringBuilder();
            for (int indice = 0, longueur = chaine.Length; indice < longueur; indice++)
            {
                resultat.Append(EnMajuscule(chaine[indice]));
            }
            return resultat.ToString();
        }

        public static string UniformiserNom(string chaine)
        {
            if (string.IsNullOrWhiteSpace(chaine)) return chaine;
            StringBuilder resultat = new StringBuilder();
            for (int indice = 0; indice < chaine.Length; indice++)
            {
                if (((indice == 0) && EstLettre(chaine[indice])) || ((indice > 0) && !EstLettre(chaine[indice - 1]) && EstLettre(chaine[indice])))
                {
                    resultat.Append(EnMajuscule(chaine[indice]));
                }
                else
                {
                    resultat.Append(EnMiniscule(chaine[indice]));
                }
            }
            return resultat.ToString();
        }

        public static string PremiereLettreMajuscule(string chaine)
        {
            if (string.IsNullOrWhiteSpace(chaine)) return chaine;
            StringBuilder resultat = new StringBuilder();
            bool lettreEnMajuscule = false;
            for (int indice = 0; indice < chaine.Length; indice++)
            {
                if (!lettreEnMajuscule && ((indice == 0) && (EstLettre(chaine[indice])) || ((indice > 0) && !EstLettre(chaine[indice - 1]) && EstLettre(chaine[indice]))))
                {
                    lettreEnMajuscule = true;
                    resultat.Append(EnMajuscule(chaine[indice]));
                }
                else
                {
                    resultat.Append(EnMiniscule(chaine[indice]));
                }
            }
            return resultat.ToString();
        }

        public static bool TenterConvertir(string chaine, out sbyte valeur, sbyte valeurParDefaut = default(sbyte))
        {
            if (!string.IsNullOrEmpty(chaine) && sbyte.TryParse(chaine.Trim(), out valeur)) return true;
            valeur = valeurParDefaut;
            return false;
        }

        public static bool TenterConvertir(string chaine, out byte valeur, byte valeurParDefaut = default(byte))
        {
            if (!string.IsNullOrEmpty(chaine) && byte.TryParse(chaine.Trim(), out valeur)) return true;
            valeur = valeurParDefaut;
            return false;
        }

        public static bool TenterConvertir(string chaine, out short valeur, short valeurParDefaut = default(short))
        {
            if (!string.IsNullOrEmpty(chaine) && short.TryParse(chaine.Trim(), out valeur)) return true;
            valeur = valeurParDefaut;
            return false;
        }

        public static bool TenterConvertir(string chaine, out ushort valeur, ushort valeurParDefaut = default(ushort))
        {
            if (!string.IsNullOrEmpty(chaine) && ushort.TryParse(chaine.Trim(), out valeur)) return true;
            valeur = valeurParDefaut;
            return false;
        }

        public static bool TenterConvertir(string chaine, out int valeur, int valeurParDefaut = default(int))
        {
            if (!string.IsNullOrEmpty(chaine) && int.TryParse(chaine.Trim(), out valeur)) return true;
            valeur = valeurParDefaut;
            return false;
        }

        public static bool TenterConvertir(string chaine, out uint valeur, uint valeurParDefaut = default(uint))
        {
            if (!string.IsNullOrEmpty(chaine) && uint.TryParse(chaine.Trim(), out valeur)) return true;
            valeur = valeurParDefaut;
            return false;
        }

        public static bool TenterConvertir(string chaine, out long valeur, long valeurParDefaut = default(long))
        {
            if (!string.IsNullOrEmpty(chaine) && long.TryParse(chaine.Trim(), out valeur)) return true;
            valeur = valeurParDefaut;
            return false;
        }

        public static bool TenterConvertir(string chaine, out ulong valeur, ulong valeurParDefaut = default(ulong))
        {
            if (!string.IsNullOrEmpty(chaine) && ulong.TryParse(chaine.Trim(), out valeur)) return true;
            valeur = valeurParDefaut;
            return false;
        }

        public static bool TenterConvertir(string chaine, out double valeur, double valeurParDefaut = default(double))
        {
            if (!string.IsNullOrEmpty(chaine) && double.TryParse(chaine.Trim(), out valeur)) return true;
            valeur = valeurParDefaut;
            return false;
        }

        public static bool TenterConvertir(string chaine, out float valeur, float valeurParDefaut = default(float))
        {
            if (!string.IsNullOrEmpty(chaine) && float.TryParse(chaine.Trim(), out valeur)) return true;
            valeur = valeurParDefaut;
            return false;
        }

        public static bool TenterConvertir(string chaine, out decimal valeur, decimal valeurParDefaut = default(decimal))
        {
            if (!string.IsNullOrEmpty(chaine) && decimal.TryParse(chaine.Trim(), out valeur)) return true;
            valeur = valeurParDefaut;
            return false;
        }
    }
}