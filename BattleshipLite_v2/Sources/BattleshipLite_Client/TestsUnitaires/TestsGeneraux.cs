using BattleshipLite_Client;

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
            bool result = partie.Joueurs.FirstOrDefault().JouerCoup(new Connexion(), partie.Joueurs.FirstOrDefault().Plateau, "E5");
            Assert.IsTrue(!result);
        }
        [TestMethod]
        public void Victoire()
        {
            //TODO: Impl�menter le test de victoire lorsque la m�thode de v�rification de la grille est impl�ment�e
            Assert.IsTrue(true);
        }
        [TestMethod]
        public void Defaite()
        {
            //TODO: Impl�menter le test de victoire lorsque la m�thode de v�rification de la grille est impl�ment�e
            Assert.IsTrue(true);
        }
        [TestMethod]
        public void CoupDejaJoue()
        {
            Partie partie = new Partie();
            partie.Demarrer(ref partie, 4, 4);
            //Ajoute un coup � la liste de coups du joueur
            partie.Joueurs.FirstOrDefault().Coups.Add(new Coup { Case = partie.Joueurs.FirstOrDefault().Plateau.Grille[0][0], EstReussi = true });

            //Essaie de jouer un coup d�j� jou�
            bool result = partie.Joueurs.FirstOrDefault().JouerCoup(new Connexion(), partie.Joueurs.FirstOrDefault().Plateau, "A1");

            //Teste si le coup a r�ussi
            Assert.IsTrue(!result);
        }
        [TestMethod]
        public void RecommencerPartie()
        {
            Partie partie = new Partie();
            partie.Demarrer(ref partie, 10, 10, 2);
            partie.Joueurs[0].Plateau.Grille[0][0].ToucheCase();

            //D�marre une nouvelle partie
            partie.Demarrer(ref partie, 10, 10, 2);

            //Teste si la case est r�initialis�e
            Assert.IsTrue(!partie.Joueurs[0].Plateau.Grille[0][0].EstTouche);
        }
        [TestMethod]
        public void PlacementBateauInvalide()
        {
            Partie partie = new Partie();
            partie.Demarrer(ref partie, 4, 4);
            Bateau bateau = new Bateau("Kayak", new List<Case>());
            bool estPlace = partie.Joueurs[0].PlacerBateaux(bateau, "G5", "G6");

            //Teste si le bateau est plac� sur une case invalide
            Assert.IsTrue(!partie.Joueurs[0].PlacerBateaux(bateau, "G5", "G6"));
        }
    }
}