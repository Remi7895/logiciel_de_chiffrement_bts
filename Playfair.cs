using System;
using System.Collections.Generic;
using System.Text;

namespace Chiffrement
{
    internal static class Playfair
    {

        #region ATTRIBUTS

        private const char LettreDeRemplissage = 'X';
        private const string Alphabet = "ABCDEFGHIKLMNOPQRSTUVWXYZ"; // J fusionné avec I car la matrice playfair utilie 25 (5*5) cases et que l'alphabet comporte 26 lettres.

        #endregion

        #region METHODES PUBLIQUES  
        public static string Chiffrer(string texte, string cle) // Chiffrer un texte.
        {
            return Traiter(texte, cle, chiffrement: true);
        }

        public static string Dechiffrer(string texte, string cle) // Déchiffirer un texte.
        {
            return Traiter(texte, cle, chiffrement: false);
        }
        #endregion

        #region METHODES PRIVEES
        private static string Traiter(string message, string cle, bool chiffrement) // Méthode principale qui applique l'algorithme Playfair (chiffrement ou déchiffrement)
        {
            if (message == null) // Empêche un crash de l'application si le message est null.
                throw new ArgumentNullException(nameof(message));

            char[,] grille = GenererGrille(cle); // Génère la grille Playfair (5*5) à partir de la clé
            string messageNettoye = NettoyerMessage(message); // Nettoie le message (format Playfair : majuscules, suppression des caractères non valides).

            if (messageNettoye.Length % 2 != 0)  // Ajoute une lettre de remplissage si le message contient un nombre impair de caractères.
                messageNettoye += LettreDeRemplissage;

            StringBuilder resultat = new StringBuilder(messageNettoye.Length); // Initialise le résultat avec une capacité optimisée

            for (int i = 0; i < messageNettoye.Length; i += 2) // Parcourt le message deux lettres par deux lettres (digrammes)
            {
                char c1 = messageNettoye[i];
                char c2 = messageNettoye[i + 1];

                TrouverPosition(grille, c1, out int ligne1, out int colonne1);
                TrouverPosition(grille, c2, out int ligne2, out int colonne2); // Trouve la position des deux lettres dans la grille

                int decalage = chiffrement ? 1 : -1; // Définit le sens du décalage (droite/bas pour chiffrer, gauche/haut pour déchiffrer)

                if (ligne1 == ligne2 && colonne1 == colonne2)
                {
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
             
            return RetoucherChaine(message, resultat.ToString()); // Ajuste le résultat pour correspondre au format du message d'origine
        }

        private static char[,] GenererGrille(string cle) // Génère la grille Playfair 5x5 à partir de la clé fournie
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

        private static string SupprimerDoublons(string valeur) // Supprime les caractères en double tout en conservant l'ordre d'apparition
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

        private static string NettoyerMessage(string message) // Nettoie le message pour le rendre compatible avec Playfair
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

        private static void TrouverPosition(char[,] grille, char c, out int ligne, out int colonne) // Recherche la position (ligne, colonne) d'une lettre dans la grille
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

        private static int TesterDepassement(int rang, char[,] grille) // Gère les dépassements de la grille (effet circulaire)
        {
            if (rang == grille.GetLength(0))
                return 0;

            if (rang == -1)
                return grille.GetLength(0) - 1;

            return rang;
        }

        private static string RetoucherChaine(string input, string output) // Réintègre la mise en forme d'origine (espaces, minuscules, ponctuation)
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
