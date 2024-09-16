using BattleshipLite_Serveur;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace TestsUnitaires
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
        public void Gagnant()
        {

            Partie partie = new Partie();
            partie.Demarrer(ref partie, 4, 4);
            Joueur joueur1 = partie.Joueurs.FirstOrDefault();
            Joueur joueur2 = partie.Joueurs.LastOrDefault();
            joueur1.PlacerBateaux(new Bateau("Kayak", new List<Case>()), "A1", "A2");
            joueur2.PlacerBateaux(new Bateau("Chaloupe", new List<Case>()), "A1", "A2");

            bool result = partie.CheckIfWinner(partie, out joueur1);
            if (result)
            {
                Assert.Fail();
            }
            result = partie.CheckIfWinner(partie, out joueur2);
            if (result)
            {
                Assert.Fail();
            }

            //Ajoute des coups à la liste de coups du joueur
            joueur1.Coups.Add(new Coup { Case = joueur1.Plateau.Grille[0][0], EstReussi = true });
            joueur1.Coups.Add(new Coup { Case = joueur1.Plateau.Grille[0][1], EstReussi = true });

            //Teste si le joueur a gagné
            result = partie.CheckIfWinner(partie, out joueur1);
            Assert.IsTrue(result);
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
