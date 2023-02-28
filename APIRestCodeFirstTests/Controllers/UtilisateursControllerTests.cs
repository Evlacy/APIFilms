using Microsoft.VisualStudio.TestTools.UnitTesting;
using APIRestCodeFirst.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using APIRestCodeFirst.Models.EntityFramework;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System.Text.RegularExpressions;
using System.Reflection;

namespace APIRestCodeFirst.Controllers.Tests
{
    [TestClass()]
    public class UtilisateursControllerTests
    {
        private UtilisateursController controller;
        private FilmRatingsDBContext context;

        [TestInitialize]
        public void InitialisationDesTests()
        {
            var builder = new DbContextOptionsBuilder<FilmRatingsDBContext>().UseNpgsql("Server = localhost; port = 5432; Database = lesfilms; uid = postgres; password = postgres;");
            context = new FilmRatingsDBContext(builder.Options);
            controller = new UtilisateursController(context);
        }

        [TestMethod()]
        public void GetUtilisateursTest()
        {
            var lesUtilisateurs = context.Utilisateurs.ToList();
            var result = controller.GetUtilisateurs();

            Assert.IsInstanceOfType(result.Result, typeof(ActionResult<IEnumerable<Utilisateur>>), "Pas un ActionResult");
            CollectionAssert.AreEqual(result.Result.Value.ToList(), lesUtilisateurs, "Les utilisateurs ne se correspondent pas");
        }

        [TestMethod()]
        public void GetUtilisateursByIdTestAsync()
        {
            var result = controller.GetUtilisateurById(1).Result;

            Assert.IsInstanceOfType(result, typeof(ActionResult<Utilisateur>), "Pas un ActionResult");
            Assert.IsNull(result.Result, "Erreur est pas null");
            Assert.IsInstanceOfType(result.Value, typeof(Utilisateur), "Pas un Utilisateur");

            var utilisateur = context.Utilisateurs.Where(c => c.UtilisateurId == 1).FirstOrDefault();
            Assert.AreEqual(utilisateur, result.Value, "Utilisateurs non identiques");
        }

        [TestMethod()]
        public void GetUtilisateursByIdEmailAsync()
        {
            var result = controller.GetUtilisateurByEmail("clilleymd@last.fm").Result;

            Assert.IsInstanceOfType(result, typeof(ActionResult<Utilisateur>), "Pas un ActionResult");
            Assert.IsNull(result.Result, "Erreur est pas null");
            Assert.IsInstanceOfType(result.Value, typeof(Utilisateur), "Pas un Utilisateur");

            var utilisateur = context.Utilisateurs.Where(c => c.Mail == "clilleymd@last.fm").FirstOrDefault();
            Assert.AreEqual(utilisateur, result.Value, "Utilisateurs non identiques");
        }

        [TestMethod]
        public void Postutilisateur_ModelValidated_CreationOK()
        {
            // Arrange
            Random rnd = new Random();
            int chiffre = rnd.Next(1, 1000000000);

            // Le mail doit être unique donc 2 possibilités :
            // 1. on s'arrange pour que le mail soit unique en concaténant un random ou un timestamp
            // 2. On supprime le user après l'avoir créé. Dans ce cas, nous avons besoin d'appeler la méthode DELETE de l’API ou remove du DbSet.
           
            Utilisateur userAtester = new Utilisateur()
            {
                Nom = "MACHIN",
                Prenom = "Luc",
                Mobile = "0606070809",
                Mail = "machin" + chiffre + "@gmail.com",
                Pwd = "Toto1234!",
                Rue = "Chemin de Bellevue",
                CodePostal = "74940",
                Ville = "Annecy-le-Vieux",
                Pays = "France",
                Latitude = null,
                Longitude = null
            };
            
            // Act
            var result = controller.PostUtilisateur(userAtester).Result; // .Result pour appeler la méthode async de manière synchrone, afin d'attendre l’ajout
            
            // Assert
            Utilisateur userRecupere = context.Utilisateurs.Where(u => u.Mail.ToUpper() == userAtester.Mail.ToUpper()).FirstOrDefault(); // On récupère l'utilisateur créé directement dans la BD grace à son mail unique
            // On ne connait pas l'ID de l’utilisateur envoyé car numéro automatique.
            // Du coup, on récupère l'ID de celui récupéré et on compare ensuite les 2 users

            userAtester.UtilisateurId = userRecupere.UtilisateurId;
            Assert.AreEqual(userRecupere, userAtester, "Utilisateurs pas identiques");
        }


        [TestMethod]
        public void Post_InvalidObjectPassed_ReturnsBadRequest()
        {
            Random rnd = new Random();
            int chiffre = rnd.Next(1, 1000000000);

            Utilisateur utilisateur = new Utilisateur()
            {
                Nom = "MACHIN",
                Prenom = "Luc",
                Mobile = "1",
                Mail = "machin" + chiffre + "@gmail.com",
                Pwd = "Toto1234!",
                Rue = "Chemin de Bellevue",
                CodePostal = "74940",
                Ville = "Annecy-le-Vieux",
                Pays = "France",
                Latitude = null,
                Longitude = null
            };

            string PhoneRegex = @"^0[0-9]{9}$";
            Regex regex = new Regex(PhoneRegex);

            if (!regex.IsMatch(utilisateur.Mobile))
            {
                controller.ModelState.AddModelError("Mobile", "Le n° de mobile doit contenir 10 chiffres"); 
            }

            var result = controller.PostUtilisateur(utilisateur);

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void Delete_Ok_ReturnsRightItem()
        {
            Random rnd = new Random();
            int chiffre = rnd.Next(1, 1000000000);

            Utilisateur utilisateur = new Utilisateur()
            {
                Nom = "MACHIN",
                Prenom = "Luc",
                Mobile = "1",
                Mail = "machin" + chiffre + "@gmail.com",
                Pwd = "Toto1234!",
                Rue = "Chemin de Bellevue",
                CodePostal = "74940",
                Ville = "Annecy-le-Vieux",
                Pays = "France",
                Latitude = null,
                Longitude = null
            };

            context.Utilisateurs.Add(utilisateur);
            context.SaveChanges();

            int id = utilisateur.UtilisateurId;

            _ = controller.DeleteUtilisateur(id);

            Utilisateur userdelete = context.Utilisateurs.Where(u => u.UtilisateurId == id).FirstOrDefault(); 
            Assert.IsNull(userdelete);
        }
    }
}