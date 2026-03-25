using System;
using System.Collections.Generic;
using System.Text;

namespace Chiffrement
{
    internal static class Playfair
    {

        #region ATTRIBUTS
        private const char LettreDeRemplissage = 'X';
        private const string Alphabet = "ABCDEFGHIKLMNOPQRSTUVWXYZ"; // J fusionné avec I
        #endregion
            
        #region METHODES PUBLIQUES  
        public static string Chiffrer(string texte, string cle)
        {
            return Traiter(texte, cle, chiffrement: true);
        }

        public static string Dechiffrer(string texte, string cle)
        {
            return Traiter(texte, cle, chiffrement: false);
        }
        #endregion

        #region METHODES PRIVEES
        private static string Traiter(string message, string cle, bool chiffrement)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            char[,] grille = GenererGrille(cle);
            string messageNettoye = NettoyerMessage(message);

            if (messageNettoye.Length % 2 != 0)
                messageNettoye += LettreDeRemplissage;

            StringBuilder resultat = new StringBuilder(messageNettoye.Length);

            for (int i = 0; i < messageNettoye.Length; i += 2)
            {
                char c1 = messageNettoye[i];
                char c2 = messageNettoye[i + 1];

                TrouverPosition(grille, c1, out int ligne1, out int colonne1);
                TrouverPosition(grille, c2, out int ligne2, out int colonne2);

                int decalage = chiffrement ? 1 : -1;

                if (ligne1 == ligne2 && colonne1 == colonne2)
                {
                    // Cas particulier demandé par le sujet : deux lettres identiques
                    char remplacement = grille[TesterDepassement(ligne1 + decalage, grille),
                                              TesterDepassement(colonne1 + decalage, grille)];
                    resultat.Append(remplacement);
                    resultat.Append(remplacement);
                }
                else if (ligne1 == ligne2)
                {
                    resultat.Append(grille[ligne1, TesterDepassement(colonne1 + decalage, grille)]);
                    resultat.Append(grille[ligne2, TesterDepassement(colonne2 + decalage, grille)]);
                }
                else if (colonne1 == colonne2)
                {
                    resultat.Append(grille[TesterDepassement(ligne1 + decalage, grille), colonne1]);
                    resultat.Append(grille[TesterDepassement(ligne2 + decalage, grille), colonne2]);
                }
                else
                {
                    resultat.Append(grille[ligne1, colonne2]);
                    resultat.Append(grille[ligne2, colonne1]);
                }
            }

            return RetoucherChaine(message, resultat.ToString());
        }

        private static char[,] GenererGrille(string cle)
        {
            string cleNormalisee = string.IsNullOrWhiteSpace(cle) ? "CIPHER" : cle;
            cleNormalisee = cleNormalisee.ToUpperInvariant().Replace('J', 'I');

            StringBuilder travail = new StringBuilder(cleNormalisee.Length + Alphabet.Length);
            travail.Append(cleNormalisee);
            travail.Append(Alphabet);

            string chaineSansDoublon = SupprimerDoublons(travail.ToString());

            char[,] grille = new char[5, 5];
            for (int i = 0; i < 25; i++)
            {
                grille[i / 5, i % 5] = chaineSansDoublon[i];
            }

            return grille;
        }

        private static string SupprimerDoublons(string valeur)
        {
            HashSet<char> dejaVus = new HashSet<char>();
            StringBuilder resultat = new StringBuilder(valeur.Length);

            foreach (char c in valeur)
            {
                if (char.IsLetter(c) && dejaVus.Add(c))
                    resultat.Append(c);
            }

            return resultat.ToString();
        }

        private static string NettoyerMessage(string message)
        {
            StringBuilder resultat = new StringBuilder(message.Length);

            foreach (char c in message)
            {
                if (!char.IsLetter(c))
                    continue;

                char lettre = char.ToUpperInvariant(c);
                if (lettre == 'J')
                    lettre = 'I';

                resultat.Append(lettre);
            }

            return resultat.ToString();
        }

        private static void TrouverPosition(char[,] grille, char c, out int ligne, out int colonne)
        {
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    if (grille[i, j] == c)
                    {
                        ligne = i;
                        colonne = j;
                        return;
                    }
                }
            }

            throw new ArgumentException($"Caractère introuvable dans la grille : {c}", nameof(c));
        }

        private static int TesterDepassement(int rang, char[,] grille)
        {
            if (rang == grille.GetLength(0))
                return 0;

            if (rang == -1)
                return grille.GetLength(0) - 1;

            return rang;
        }

        private static string RetoucherChaine(string input, string output)
        {
            StringBuilder resultat = new StringBuilder(output);
            int indexLettres = 0;

            for (int i = 0; i < input.Length; i++)
            {
                if (!char.IsLetter(input[i]))
                {
                    resultat.Insert(indexLettres, input[i]);
                    indexLettres++;
                    continue;
                }

                if (char.IsLower(input[i]))
                    resultat[indexLettres] = char.ToLowerInvariant(resultat[indexLettres]);

                indexLettres++;
            }

            return resultat.ToString();
        }
        #endregion
    }
}
