using System;

namespace Chiffrement
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            string texte = "Hello World";
            string cle = "cipher";

            string texteChiffre = Playfair.Chiffrer(texte, cle);
            string texteDechiffre = Playfair.Dechiffrer(texteChiffre, cle);

            Console.WriteLine("Texte clair    : " + texte);
            Console.WriteLine("Texte chiffré  : " + texteChiffre);
            Console.WriteLine("Texte déchiffré: " + texteDechiffre);

            Console.ReadKey();
        }
    }
}
