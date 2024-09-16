using BattleshipLite_Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace TestsUnitaires_Client
{
    [TestClass]
    public class TestsGeneraux
    {
        [TestMethod]
        public void CoupHorsGrille()
        {
            Partie partie = new Partie();
            partie.Demarrer(ref partie, 4, 4);
            bool result = partie.Joueurs.FirstOrDefault().JouerCoup(new Connexion(80), partie.Joueurs.FirstOrDefault().Plateau, "E5");
            Assert.IsTrue(!result);
        }
        [TestMethod]
        public void Victoire()
        {
            //TODO: Implémenter le test de victoire lorsque la méthode de vérification de la grille est implémentée
            Assert.IsTrue(true);
        }
        [TestMethod]
        public void Defaite()
        {
            //TODO: Implémenter le test de victoire lorsque la méthode de vérification de la grille est implémentée
            Assert.IsTrue(true);
        }
        [TestMethod]
        public void CoupDejaJoue()
        {
            Partie partie = new Partie();
            partie.Demarrer(ref partie, 4, 4);
            //Ajoute un coup à la liste de coups du joueur
            partie.Joueurs.FirstOrDefault().Coups.Add(new Coup { Case = partie.Joueurs.FirstOrDefault().Plateau.Grille[0][0], EstReussi = true });

            //Essaie de jouer un coup déjà joué
            bool result = partie.Joueurs.FirstOrDefault().JouerCoup(new Connexion(80), partie.Joueurs.FirstOrDefault().Plateau, "A1");

            //Teste si le coup a réussi
            Assert.IsTrue(!result);
        }
        [TestMethod]
        public void RecommencerPartie()
        {
            Partie partie = new Partie();
            partie.Demarrer(ref partie, 10, 10, 2);
            partie.Joueurs[0].Plateau.Grille[0][0].ToucheCase();

            //Démarre une nouvelle partie
            partie.Demarrer(ref partie, 10, 10, 2);

            //Teste si la case est réinitialisée
            Assert.IsTrue(!partie.Joueurs[0].Plateau.Grille[0][0].EstTouche);
        }
        [TestMethod]
        public void PlacementBateauInvalide()
        {
            Partie partie = new Partie();
            partie.Demarrer(ref partie, 4, 4);
            Bateau bateau = new Bateau("Kayak", new List<Case>());
            bool estPlace = partie.Joueurs[0].PlacerBateaux(bateau, "G5", "G6");

            //Teste si le bateau est placé sur une case invalide
            Assert.IsTrue(!partie.Joueurs[0].PlacerBateaux(bateau, "G5", "G6"));
        }

    }
}
