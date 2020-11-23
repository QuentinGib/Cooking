using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Xml;


namespace Cooking
{
    class Program
    {
        static void Main(string[] args)
        {
            string connectionString = "SERVER=localhost;PORT=3306;DATABASE=cooking;UID=root;PASSWORD=flavie;";
            MySqlConnection connection = new MySqlConnection(connectionString);
            Menu(connection);
        }

        /// <summary>
        /// Affiche le menu pricipal
        /// </summary>
        static void PrintMenu()
        {
            Console.Clear();
            Console.WriteLine("1. Client\n" +
                "2. Créateur de recette\n" +
                "3. Admin\n" +
                "4. Demo\n");
        }

        /// <summary>
        /// Naviguer dans le menu principal
        /// </summary>
        /// <param name="connection">Paramètres de connection à la base de données</param>
        static void Menu(MySqlConnection connection)
        {
            PrintMenu();
            int choix = 0;
            while(choix > 4 || choix < 1)
            {
                choix = Convert.ToInt32(Console.ReadLine());
                switch (choix)
                {
                    case 1:
                        Client(connection);
                        PrintMenu();
                        choix = 0;
                        break;
                    case 2:
                        CdR(connection);
                        PrintMenu();
                        choix = 0;
                        break;
                    case 3:
                        Admin(connection);
                        break;
                    case 4:
                        Demo(connection);
                        break;
                    default:
                        Console.WriteLine("Choix incorrect\n");
                        break;
                }
            }
        }

        /// <summary>
        /// Propose au client de s'identifier ou s'inscrire
        /// </summary>
        /// <param name="connection">Paramètres de connection à la base de données</param>
        static void Client(MySqlConnection connection)
        {
            string NomC = "";
            Console.Clear();
            Console.WriteLine("1. S'identifier\n" +
                "2. S'inscrire\n" +
                "3. Retour");
            int choix = 0;
            while (choix > 3 || choix < 1)
            {
                choix = Convert.ToInt32(Console.ReadLine());
                switch (choix)
                {
                    case 1:
                        NomC = Identification(connection);
                        if(NomC == "-1")
                        {
                            choix = 0;
                        }
                        else
                        {
                            MenuCompteClient(connection, NomC);
                        }
                        break;
                    case 2:
                        Inscription(connection);
                        NomC = Identification(connection);
                        if (NomC == "-1")
                        {
                            choix = 0;
                        }
                        else
                        {
                            MenuCompteClient(connection, NomC);
                        }
                        break;
                    case 3:
                        break;
                    default:
                        Console.WriteLine("Choix incorrect\n");
                        break;
                }
            }
        }

        /// <summary>
        /// Menu affiché après connection du client
        /// </summary>
        /// <param name="connection">Paramètres de connection à la base de données</param>
        /// <param name="NomC">Nom du client connecté</param>
        static void MenuCompteClient(MySqlConnection connection, string NomC)
        {
            string[,,] panier = new string[200, 200, 200];
            int nbRCommand = 0;
            int choix = 1;
            while (choix != 0)
            {
                Console.Clear();
                string[] recettes = printRecettes(connection);
                Console.WriteLine("Choisissez une recette parmi celles présentées" +
                "\n\n" + (recettes.Length + 1) + ". Mon Panier" +
                "\n0. Deconnexion");
                choix = Convert.ToInt32(Console.ReadLine());
                if(choix >= 1 && choix <= recettes.Length)
                {
                    nbRCommand = InfosRecette(connection, choix-1, recettes, panier, nbRCommand);
                }
                else if(choix == recettes.Length + 1)
                {
                    //Afficher panier
                    nbRCommand = MenuPanier(connection, panier, nbRCommand, NomC);
                    choix = 1;
                    Console.Clear();
                }
                else if(choix != 0)
                {
                    Console.Clear();
                    Console.WriteLine("Saisie incorrect");
                }
            }
        }

        /// <summary>
        /// Affiche toutes les recettes disponibles
        /// </summary>
        /// <param name="connection">Paramètres de connection à la base de données</param>
        /// <returns>Tableau contenant tous les noms de recettes</returns>
        static string[] printRecettes(MySqlConnection connection)
        {
            connection.Open();

            MySqlCommand command = connection.CreateCommand();
            command.CommandText = " SELECT COUNT(NomR)"
                                + " FROM recette ;";

            MySqlDataReader reader;
            reader = command.ExecuteReader();

            int nbRecettes = 0;
            if (reader.Read())
            {
                nbRecettes = reader.GetInt32(0);
            }

            reader.Close();

            string[] recette = new string[nbRecettes];
            int i = 1;

            command = connection.CreateCommand();
            command.CommandText = " SELECT NomR"
                                + " FROM recette"
                                + " ORDER BY NomR;";

            reader = command.ExecuteReader();

            while (reader.Read())   // parcours ligne par ligne
            {
                recette[i - 1] = reader.GetString(0);
                Console.WriteLine(i + ". " + recette[i-1]) ;
                i++;
            }
            reader.Close();
            connection.Close();
            return recette;
        }

        /// <summary>
        /// Affiche les infos d'une recette et ajoute au panier si nécessaire
        /// </summary>
        /// <param name="connection">Paramètres de connection à la base de données</param>
        /// <param name="choix">Numéro de la recette à afficher choisie</param>
        /// <param name="recettes">Tableau contenant tous les noms de recettes</param>
        /// <param name="panier">Matrice contenant le nom de recettes commandées, le prix à l'unité et les quantités</param>
        /// <param name="nbRCommand">Nombre de recettes commandées</param>
        /// <returns></returns>
        static int InfosRecette(MySqlConnection connection, int choix, string[] recettes, string[,,] panier, int nbRCommand)
        {
            connection.Open();
            string nom = "";
            string prix = "";

            MySqlCommand command = connection.CreateCommand();
            command.CommandText = " SELECT NomR, type, descriptif, PdV_C"
                                + " FROM recette "
                                + "WHERE NomR LIKE '" + recettes[choix] + "';";

            MySqlDataReader reader;
            reader = command.ExecuteReader();
            if (reader.Read())
            {
                Console.Clear();
                nom = reader.GetString(0);
                prix = reader.GetString(3);
                Console.WriteLine("          " + reader.GetString(0)
                    + "\nType de plat : " + reader.GetString(1) + "        Prix : " + reader.GetString(3) + " cooks"
                    + "\nDescription : " + reader.GetString(2));
            }

            int suite = 0;
            while(suite < 1 || suite > 2)
            {
                Console.WriteLine("\n1. Retour \n2. Commander");
                suite = Convert.ToInt32(Console.ReadLine());
                if(suite == 2)
                {
                    nbRCommand++;
                    Console.WriteLine("Combien d'unités en voulez vous ? ");
                    string nombre = Console.ReadLine();
                    panier[nbRCommand, 0, 0] = nom;
                    panier[0, nbRCommand, 0] = prix;
                    panier[0, 0, nbRCommand] = nombre;
                }
            }

            reader.Close();
            connection.Close();
            return nbRCommand;

        }

        /// <summary>
        /// Affiche le panier
        /// </summary>
        /// <param name="connection">Paramètres de connection à la base de données</param>
        /// <param name="panier">Matrice contenant le nom de recettes commandées, le prix à l'unité et les quantités</param>
        /// <param name="nbRCommand">Nombre de recettes commandées</param>
        /// <param name="NomC">Nom du client connecté</param>
        /// <returns>le nombre de recettes dans le panier</returns>
        /// 
        static int MenuPanier(MySqlConnection connection, string[,,] panier, int nbRCommand, string NomC)
        {
            double prixT = 0;
            bool reussi = false;
            int choix = 0;
            Console.Clear();
            Console.WriteLine("----------------------- Panier ------------------------" +
                "\n\nPlat                            Prix/unité     Quantité\n");
            for(int i = 1; i<=nbRCommand; i++)
            {
                Console.WriteLine("{0,-32}{1,6}cooks{2,13}",panier[i, 0, 0], panier[0, i, 0], panier[0, 0, i]);
                prixT = prixT + Convert.ToDouble(panier[0, i, 0]) * Convert.ToInt32(panier[0, 0, i]);
            }
            Console.WriteLine("\nPrix total : " + prixT + " cooks" +
                "\n\n1. Payer" +
                "\n2. Retour");
            choix = Convert.ToInt32(Console.ReadLine());
            if(choix == 1)
            {
                reussi = Paiement(connection, panier, nbRCommand, prixT, NomC);
                if(reussi == true)
                {
                    nbRCommand = 0;
                }
            }
            return nbRCommand;
        }
        /// <summary>
        /// Permet de réaliser la commande du panier, payer, décrémenter les stocks et le solde du client si CdR
        /// </summary>
        /// <param name="connection">Paramètres de connection à la base de données</param>
        /// <param name="panier">Matrice contenant le nom de recettes commandées, le prix à l'unité et les quantités</param>
        /// <param name="nbRCommand">Nombre de recettes commandées</param>
        /// <param name="prixT">Prix total du panier</param>
        /// <param name="NomC">Nom du client connecté</param>
        /// <returns>true si le paiement a été réussi, false sinon</returns>
        /// 
        static bool Paiement(MySqlConnection connection, string[,,] panier, int nbRCommand, double prixT, string NomC)
        {
            bool reussi = false;
            int compteurR = 0;
            int numCommande = 0;
            int solde = 0;
            int[] quantiteP = new int[100];
            string[] nomP = new string[100];
            int j = 0;
            Console.WriteLine("Prix à payer : " + prixT + " cooks" +
                "\nValider ? (oui ou non)");
            string choix = Console.ReadLine();
            if(choix == "oui")
            {
                connection.Open();
                MySqlCommand command = connection.CreateCommand();
                MySqlDataReader reader;
                
                //Création d'une nouvelle commande dans la bdd
                command.CommandText = " INSERT INTO `cooking`.`commande` (`prix`,`date`,`NomC`) " +
                    " VALUES (" + prixT + ", Now(), '" + NomC + "');";
                command.ExecuteNonQuery();

                //On lie les recettes commandés aux commandes dans la table rCommandee
                command.CommandText = "SELECT max(numCommande) FROM commande;";
                reader = command.ExecuteReader();
                if (reader.Read())
                {
                    numCommande = Convert.ToInt32(reader.GetString(0));
                }
                reader.Close();
                
                for (int i = 1; i <= nbRCommand; i++)
                {
                    command.CommandText = "INSERT INTO `cooking`.`rCommandee` (`numCommande`, `NomR`, `quantite`) " +
                        " VALUES (" + numCommande + ",'" + panier[i,0,0] +"', " + panier[0,0,i] + ");";
                    command.ExecuteNonQuery();
                }

                command.CommandText = "select solde from CdR cr, client c where c.NomC = cr.NomC;";
                reader = command.ExecuteReader();
                if (reader.Read())
                {
                    solde = Convert.ToInt32(reader.GetString(0));
                    reader.Close();
                    if (solde >= prixT)
                    {
                        command.CommandText = "UPDATE CdR SET solde = " + (solde - prixT) +
                            " WHERE NomC = '" + NomC + "'; ";
                        command.ExecuteNonQuery();
                    }
                    else
                    {
                        Console.WriteLine("Solde insuffisant, nous completerons avec votre carte banquaire.");
                        choix = Console.ReadLine();
                        if(choix == "oui")
                        {
                            command.CommandText = "UPDATE CdR SET solde = 0 WHERE NomC = '" + NomC + "'; ";
                            command.ExecuteNonQuery();
                        }
                    }
                    reussi = true;
                }
                
                for(int i = 1; i<= nbRCommand; i++)
                {
                    //Actions sur le prix de la recette
                    command.CommandText = "UPDATE recette SET Compteur = Compteur + " + panier[0,0,i] +
                        " WHERE NomR LIKE '" + panier[i, 0, 0] + "';";
                    command.ExecuteNonQuery();
                    command.CommandText = "SELECT Compteur FROM recette WHERE NomR LIKE '" + panier[i, 0, 0] + "';";
                    reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        compteurR = Convert.ToInt32(reader.GetString(0));
                        reader.Close();
                        Console.WriteLine(compteurR);
                        if (compteurR > 50)
                        {
                            Console.WriteLine(panier[i,0,0]);
                            command.CommandText = "UPDATE recette SET PdV_C = PdV_C + 5, Remu_CdR = 4" +
                                " WHERE NomR LIKE '" + panier[i, 0, 0] + "';";
                            command.ExecuteNonQuery();
                        }
                        else if(compteurR > 10)
                        {
                            command.CommandText = "UPDATE recette SET PdV_C = PdV_C + 2" +
                                " WHERE NomR LIKE '" + panier[i, 0, 0] + "';";
                            command.ExecuteNonQuery();
                            Console.WriteLine(panier[i, 0, 0]);
                        }
                        Console.ReadKey();
                    }

                    //Actions sur les stocks de produits
                    command.CommandText = "SELECT i.quantite, i.NomP FROM recette r, ingredient i, produit p" +
                        " WHERE i.NomR LIKE r.NomR and r.NomR LIKE '" + panier[i,0,0] + "';";
                    reader = command.ExecuteReader();
                    j = 0;
                    while (reader.Read())   // parcours ligne par ligne
                    {
                        quantiteP[j] = Convert.ToInt32(reader.GetString(0));
                        nomP[j] = reader.GetString(1);
                        j++;
                    }
                    reader.Close();
                    for(int n = 0; n<j; n++)
                    {
                        command.CommandText = "UPDATE produit SET stock = stock - " + quantiteP[n] +
                            " WHERE NomP LIKE '" + nomP[n] + "';";
                        command.ExecuteNonQuery();
                    }
                }
                connection.Close();
            }
            return reussi;
        }

        /// <summary>
        /// Propose aux Créateur de recette un choix d'actions qu'il faudra choisir
        /// </summary>
        /// <param name="connection">Paramètres de connection à la base de données</param>
        static void CdR(MySqlConnection connection)
        {
            Console.Clear();
            string NomCdR= IdentificationCdR(connection);
            if (NomCdR != "")
            {
                Console.Clear();
                int choix = 0;
                bool retour = false;
                while (choix > 4 || choix < 1)
                {
                    Console.WriteLine("1. Saisir une recette\n" +
               "2. Consulter son solde de cook\n" +
               "3. Afficher la liste de ses recettes\n" +
               "4. Retour\n");
                    choix = Convert.ToInt32(Console.ReadLine());
                    switch (choix)
                    {
                        case 1:
                            Console.Clear();
                            string nom;
                            string type;
                            string description;
                            string nomProduit;
                            double quantite;
                            double prix;
                            bool listage = false;
                            string tempData;

                            Console.WriteLine("Nom de la recette: ");
                            nom = Console.ReadLine();
                            Console.WriteLine("Type (descriptif en un mot): ");
                            type= Console.ReadLine();
                            Console.WriteLine("Description (256 caractères max): ");
                            description = Console.ReadLine();
                            Console.WriteLine("Prix de vente: ");
                            prix = Convert.ToInt64(Console.ReadLine());

                            InscriptionRecette(nom, type, description, Convert.ToInt32(prix), 2, NomCdR, connection); 
                                                                                                                      
                            connection.Open();
                            MySqlCommand command = connection.CreateCommand();                                                                                          
                            MySqlDataReader reader;
                            while (listage == false)
                            {
                                Console.WriteLine("Nom de l'ingrédient: ");
                                nomProduit = Console.ReadLine();
                                Console.WriteLine("Quantité: ");
                                quantite = Convert.ToInt64(Console.ReadLine());

                                //___________vérification dans la BDD Produit_________
                                
                                command.CommandText = "SELECT NomP FROM produit WHERE NomP = '"+nomProduit+"';";
                                
                                reader = command.ExecuteReader();

                                if (reader.Read())
                                {
                                    reader.Close();
                                    connection.Close();
                                    // Pas de rajouter dans la BDD


                                }
                                else
                                {
                                    reader.Close();
                                    connection.Close();
                                    string categorie;
                                    string unite;
                                    // Rajout dans la BDD
                                    Console.WriteLine("Votre produit va être rajouté dans notre base de données, veuillez répondre aux questions suivantes: ");
                                    Console.WriteLine("A quelle catégorie appartient votre produit? (viande, poisson, légume…),");
                                    categorie = Console.ReadLine();
                                    Console.WriteLine("Quelle unité de mesure dit-on utiliser avec votre produit? (ml, g, unité, feuille, cuillière…),");
                                    unite = Console.ReadLine();

                                    InscriptionProduit(nomProduit, categorie, unite, Convert.ToInt32(quantite), Convert.ToInt32(quantite * 2), Convert.ToInt32(quantite * 3), connection);

                                    Console.WriteLine("Votre produit vient d'être ajouté à la base de donnée.");

                                   Console.ReadKey();
                                }

                                InscriptionIngredientsRecette(nom, nomProduit, quantite, connection);

                                Console.WriteLine("Souhaitez vous rajouter un autre ingrédient ? (O ou N)");
                                tempData = Console.ReadLine();
                                if (tempData == "N")
                                {
                                    listage = true;
                                }
                            }
                            Console.Clear();

                            // REMUNERATION DE 2 COOK POUR LA RECETTE

                            connection.Open();
                            command.CommandText = "UPDATE CdR SET Solde = Solde+2 WHERE NomCdR = '" + NomCdR + "';";

                            command.ExecuteNonQuery();
                            connection.Close();
                            break;

                        case 2:
                            Console.Clear();
                            double data;
                            connection.Open();

                            command = connection.CreateCommand();
                            command.CommandText = "SELECT solde FROM CdR WHERE NomCdR = '" + NomCdR + "';";

                            reader = command.ExecuteReader();

                            while (reader.Read())
                            {
                                Console.Clear();
                                data = Convert.ToInt64(reader.GetString(0));
                                Console.WriteLine("Solde cook: "+data);
                                Console.WriteLine("\nAppuyez sur une touche pour retourner en arrière.");

                            }

                            Console.ReadKey();
                            Console.Clear();
                            reader.Close();
                            connection.Close();


                            break;
                        case 3:
                            Console.Clear();

                            string[] recettes = new string[50];
                            string[,] recettesETquantitees = new string[50, 50];
                            int indexI = 0;
                            int nbrecettes = 0;
                            connection.Open();

                            command = connection.CreateCommand();
                            command.CommandText = "SELECT NomR FROM recette WHERE NomCdR = '"+NomCdR+"';"; // Demande la liste des recettes
                            reader = command.ExecuteReader();
                            indexI = 0;
                            while (reader.Read())
                            {
                                recettes[indexI] = reader.GetString(0); // Stock des recettes dans un tableau
                                indexI++;
                                nbrecettes++;
                            }
                            reader.Close();

                            command = connection.CreateCommand();
                            command.CommandText = "SELECT NomR, sum(quantite) FROM rcommandee GROUP BY NomR;"; // Demande la liste des recettes commandé avec sa quantité
                            reader = command.ExecuteReader();
                            indexI = 0;
                            while (reader.Read())
                            {
                                recettesETquantitees[indexI, 0] = reader.GetString(0); // Stock des noms des recettes dans un tableau
                                recettesETquantitees[indexI, 1] = reader.GetString(1); // Stock des quantités des commandes de la recette associé dans un tableau
                                indexI++;
                            }
                            reader.Close();

                            Console.WriteLine("Liste des recettes:");
                            bool test = false;
                            bool test2 = false;
                            int compteur = 1;
                            int index = 0;
                            while(test == false)
                            {
                                Console.Write(" " + compteur + ") "+recettes[compteur-1]+" commandé ");

                                for(int i=0; i< indexI; i++)
                                {
                                    if(recettes[compteur-1]== recettesETquantitees[i, 0])
                                    {
                                        test2 = true;
                                        index = i;
                                    }
                                }
                                if (test2 == true)
                                {
                                    Console.Write(recettesETquantitees[index,1] + "fois.\n");
                                }
                                else
                                {
                                    Console.Write("0 fois.\n");
                                }

                                test2 =false;
                                if (compteur == nbrecettes)
                                {
                                    test = true; // Sortie boucle
                                }
                                compteur++;
                            }
                            
                            
                            Console.ReadKey();
                            Console.Clear();
                            
                            connection.Close();
                            break;
                        case 4:
                            retour = true;
                            break;
                        default:
                            Console.WriteLine("Choix incorrect\n");
                            break;
                    }
                    if (retour==false)
                    {
                        choix = 0;
                    }

                }
            }

        }

        /// <summary>
        /// Inscription d'une recette dans la BDD Mysql
        /// </summary>
        /// <returns></returns>
        static bool InscriptionRecette(string NomR, string type, string descriptif, int PdV_C,int Remu_CdR, string NomCdR, MySqlConnection connection)
        {
            connection.Open();
            MySqlCommand command = connection.CreateCommand();
            command.CommandText = "INSERT INTO `cooking`.`recette` (`NomR`,`type`,`descriptif`,`PdV_C`,`Remu_CdR`,`Compteur`,`NomCdR`) VALUES('" + NomR + "','" + type + "','" + descriptif + "'," + PdV_C + "," + Remu_CdR + ","+0+",'" + NomCdR + "');";
            command.ExecuteNonQuery();
            connection.Close();
            return true;
        }

        /// <summary>
        /// Inscription d'un produit dans la BDD Mysql
        /// </summary>
        /// <returns>true si l'inscription a été réussi, false sinon</returns>
        static bool InscriptionProduit(string NomP, string categorie, string unite, int stock, int stock_min, int stock_max, MySqlConnection connection)
        {
            connection.Open();
            MySqlCommand command = connection.CreateCommand();
            command.CommandText = "INSERT INTO `cooking`.`produit`(`NomP`,`categorie`,`unite`,`stock`,`stock_min`,`stock_max`,`NomF`,`refF`,`numtelF`) VALUES('"+ NomP + "','"+ categorie + "','"+ unite + "',"+ stock + ","+ stock_min + ","+ stock_max + ",'Auchan','auch','0123456789');";
            command.ExecuteNonQuery();
            connection.Close();
            return true;
        }

        /// <summary>
        /// Inscription d'un ingrédient d'une recette dans la BDD Mysql
        /// </summary>
        static void InscriptionIngredientsRecette(string nom, string nomProduit, double quantitées, MySqlConnection connection)
        {
            connection.Open();
            MySqlCommand command = connection.CreateCommand();
            command.CommandText = "INSERT INTO `cooking`.`ingredient` (`NomR`,`NomP`,`quantite`) VALUES('" + nom + "','" + nomProduit + "'," + quantitées + ");";
            command.ExecuteNonQuery();
            connection.Close();
        }

        /// <summary>
        /// Permet de vérifier les identifiants et mot de passe d'un Créateur de recette
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
        static string IdentificationCdR(MySqlConnection connection)
        {
            string NomC;
            string mdp;
            string NomCdR = "";

            string fin;

            bool valide = false;
            bool valide2 = true;
            
            while (valide == false)
            {
                Console.Clear();
                Console.WriteLine("Identifiant client:");
                NomC = Console.ReadLine();
                Console.WriteLine("Mot de passe client:");
                mdp = Console.ReadLine();
                valide = ConnectCompte(NomC, mdp, connection);
                
                if (valide == false)
                {
                    Console.WriteLine("Identifiant client incorrect ! Retour ? (oui ou non)");
                    fin = Console.ReadLine();
                    if (fin == "oui")
                    {
                        break;
                    }
                }
                if (valide == true)
                {
                    Console.WriteLine("Identifiant créateur de recette:");
                    NomCdR = Console.ReadLine();
                    valide2 = ConnectCdR(NomCdR, NomC, connection);

                    if (valide2 == false)
                    {
                        InscriptionCdR(NomCdR, NomC, connection);
                        Console.WriteLine("Vous n'aviez pas de compte créateur de recette. Il vient de vous être créé.");
                        Console.WriteLine("\nAppuyez sur une touche pour accèder à votre compte.");
                        Console.ReadKey();
                    }
                    if(valide2 == true)
                    {
                        Console.WriteLine("Connexion réussi. ");
                        Console.WriteLine("\nAppuyez sur une touche pour accèder à votre compte.");
                        Console.ReadKey();
                    }
                }
                
            }

            return NomCdR;
        }

        /// <summary>
        /// Permet la gestion de Cooking
        /// </summary>
        static void Admin(MySqlConnection connection)
        {
            Console.Clear();
            int choix = 0;
            bool retour = false;
            while (choix < 1)
            {
                connection.Open();
                MySqlCommand command = connection.CreateCommand();
                MySqlDataReader reader;

                retour = false;
                Console.WriteLine("1. Le tableau de bord de la semaine\n" +
                                  "2. Le réapprovisionnement hebdomadaire des produits\n" +
                                  "3. Supprimer une recette\n" +
                                  "4. Supprimer un cuisiner (et toutes ses recettes)\n");
                choix = Convert.ToInt32(Console.ReadLine());
                switch (choix)
                {
                    case 1:
                        Console.Clear();
                        Console.WriteLine("1. Le CdR de la semaine\n" +
                                          "2. Le top 5 recettes\n" +
                                          "3. Le CdR d'Or\n"+
                                          "4. Retour");
                        choix = Convert.ToInt32(Console.ReadLine());
                        int compteur = 1;
                        bool exit = false;
                        switch (choix)
                        {
                            case 1:
                                Console.Clear();
                                //CdR ayant le + été commandé
                                command = connection.CreateCommand();
                                command.CommandText = "SELECT SUM(c.quantite),c.NomR, r.NomCdR FROM rCommandee c, recette r  WHERE c.NomR=r.NomR GROUP BY NomR ORDER BY SUM(quantite) DESC;";
                                
                                reader = command.ExecuteReader();
                                exit = false;
                                while (reader.Read() && exit == false)   // parcours une ligne, celle qu'il faut
                                {
                                    Console.WriteLine( "Le créateur de la semaine est " + reader.GetString(2)+
                                                       " avec " + reader.GetString(0) +" commandes pour son plat " + reader.GetString(1) +"."); 
                                    exit = true;
                                }
                                reader.Close();
                                Console.WriteLine("\nAppuyez sur une touche pour continuer.");
                                Console.ReadKey();
                                break;
                            case 2:
                                Console.Clear();
                                command = connection.CreateCommand();
                                command.CommandText = "SELECT SUM(c.quantite),c.NomR, r.NomCdR, r.type FROM rCommandee c, recette r  WHERE c.NomR=r.NomR GROUP BY NomR ORDER BY SUM(quantite) DESC;";

                                reader = command.ExecuteReader();
                                exit = false;
                                
                                Console.WriteLine("Le top 5 recettes de la semaine:");
                                while (reader.Read() && exit == false)   // parcours 5 lignes max
                                {
                                    Console.WriteLine(" "+compteur+") "+reader.GetString(1)+ ": "+ reader.GetString(3)+" créé par "+ reader.GetString(2)+" et commandé "+ reader.GetString(0)+" fois.");
                                    compteur++;
                                    if (compteur == 5)
                                    {
                                        exit = true;
                                    }
                                }
                                reader.Close();
                                Console.WriteLine("\nAppuyez sur une touche pour continuer.");
                                Console.ReadKey();
                                break;
                            case 3:
                                Console.Clear();
                                //CdR ayant le + été commandé
                                command = connection.CreateCommand();
                                command.CommandText = "SELECT SUM(c.quantite),c.NomR, r.NomCdR FROM rCommandee c, recette r  WHERE c.NomR=r.NomR GROUP BY NomR ORDER BY SUM(quantite) DESC;";

                                reader = command.ExecuteReader();
                                exit = false;
                                string cdror = "";
                                while (reader.Read() && exit == false)   // parcours une ligne, celle qu'il faut
                                {
                                    Console.WriteLine("Le créateur d'or est " + reader.GetString(2) + ".");
                                    cdror = reader.GetString(2); // Récupère son nom 
                                    exit = true;
                                }
                                reader.Close();
                                command = connection.CreateCommand();
                                command.CommandText = "SELECT SUM(c.quantite),c.NomR, r.NomCdR FROM rCommandee c, recette r  WHERE c.NomR=r.NomR AND r.NomCdR = '"+cdror+"' ORDER BY SUM(quantite) DESC;";
                                reader = command.ExecuteReader();
                                exit = false;
                                compteur = 1;
                                Console.WriteLine("Ses recettes les plus commandées: ");
                                while (reader.Read() && exit == false)   // parcours une ligne, celle qu'il faut
                                {
                                    Console.WriteLine(" " + compteur + ") "+reader.GetString(1) + ", commandé " + reader.GetString(0) + " fois.");
                                    compteur++;
                                    if (compteur == 5)
                                    {
                                        exit = true;
                                    }
                                }
                                reader.Close();

                                Console.WriteLine("\nAppuyez sur une touche pour continuer.");
                                Console.ReadKey();



                                break;
                            case 4:
                                Console.Clear();
                                //retour = true;
                                break;

                        }
                        Console.Clear();
                        choix = 0;
                        break;

                    case 2:
                        Console.Clear();
                        Console.WriteLine("1. Mise à jour des Qte max et Qte min des produits\n" +
                                          "2. Edition de la liste des commandes de la semaine au format XML\n"+
                                          "3. Retour");
                        choix = Convert.ToInt32(Console.ReadLine());
                        switch (choix)
                        {
                            case 1:
                                Console.Clear();
                                command.CommandText = "SELECT p.NomP FROM produit p, ingredient i, recette r , rCommandee rc, commande c WHERE DATEDIFF( NOW(), c.date ) <=30 and i.NomP = p.NomP and i.NomR = r.NomR and rc.NomR = r.NomR and rc.numCommande = c.NumCommande;";
                                command.ExecuteNonQuery();

                                Console.WriteLine("Les quantités maximum et minimum viennent d'être mis à jour.");
                                Console.ReadKey();
                                Console.Clear();
                                retour = true;
                                break;
                            case 2:
                                Console.Clear();
                                int nbLignes = 0;
                                command = connection.CreateCommand();
                                command.CommandText = "SELECT count(NomP) FROM produit WHERE stock < stock_min ORDER BY NomF AND NomP;";
                                reader = command.ExecuteReader();
                                while (reader.Read()){
                                    nbLignes = reader.GetInt32(0); // Recupère taille matrice
                                }
                                reader.Close();
                                string[,] matriceproduits = new string[nbLignes, 4];
                                command = connection.CreateCommand();
                                command.CommandText = "SELECT NomP, stock, stock_max, NomF FROM produit WHERE stock<stock_min ORDER BY NomF AND NomP; ";
                                reader = command.ExecuteReader();

                                XmlWriter xmlWriter = XmlWriter.Create("Commande.xml");
                                xmlWriter.WriteStartDocument();
                                xmlWriter.WriteStartElement("Commande");

                                while (reader.Read())
                                {
                                    xmlWriter.WriteStartElement("Fournisseur");
                                    xmlWriter.WriteString(reader.GetString(3));
                                    xmlWriter.WriteEndElement();

                                    xmlWriter.WriteStartElement("Produit");
                                    xmlWriter.WriteString(reader.GetString(0));
                                    xmlWriter.WriteEndElement();

                                    xmlWriter.WriteStartElement("Quantitée");
                                    xmlWriter.WriteString(Convert.ToString(reader.GetInt32(2)-reader.GetInt32(1)));
                                    xmlWriter.WriteEndElement();
                                }

                                xmlWriter.WriteEndElement();
                                xmlWriter.WriteEndDocument();
                                xmlWriter.Close();

                                reader.Close();

                                Console.WriteLine("Creation du fichier Commande.xml réussi." 
                                                 +"\n\nLecture du fichier: \n");

                                XmlDocument docXml = new XmlDocument();
                                docXml.Load("Commande.xml");
                                XmlElement racine = docXml.DocumentElement;
                                Console.WriteLine(" racine : " + racine.Name);
                                foreach (XmlNode e in racine)
                                {
                                    Console.WriteLine("balise : " + e.Name);
                                    if (e.ChildNodes != null)
                                    {
                                        foreach (XmlAttribute a in e.Attributes)
                                        {
                                            Console.Write(" attribute : ");
                                            Console.Write(" name = " + a.Name);
                                            Console.WriteLine(", value : " + a.Value);
                                        }
                                        foreach (XmlNode e2 in e)
                                        {
                                            if (e2.NodeType == XmlNodeType.Text)
                                            {
                                                Console.WriteLine(" InnerText : " + e2.InnerText);
                                            }
                                            else
                                            {
                                                Console.WriteLine(" type : " + e2.NodeType + " , nom : " + e2.Name);
                                            }
                                        }
                                    }
                                }
                                
                                Console.WriteLine("Appuyez sur une touche pour continuer.");
                                Console.ReadKey();
                                Console.Clear();
                                
                                retour = true;
                                break;
                            case 3:
                                Console.Clear();
                                retour = true;
                                break;
                        }
                        if (retour == true)
                        {
                            choix = 0;
                        }

                        break;
                    case 3:
                        Console.Clear();
                        //____________________________________Lister les plats et proposer quelle recette supprimer
                        command = connection.CreateCommand();
                        command.CommandText = "SELECT NomR FROM recette;";

                        reader = command.ExecuteReader();
                        int val = 0;
                        Console.WriteLine("Quelle recette souhaitez vous supprimer ?");
                        compteur = 1;
                        string[] listePlat = new string[50];
                        while (reader.Read()) 
                        {
                            Console.WriteLine(" "+compteur+") " + reader.GetString(0));
                            listePlat[compteur] = reader.GetString(0);
                            compteur++;
                        }
                        reader.Close();
                        val = Convert.ToInt32(Console.ReadLine());
                        //__________________________________Suppression recette
                        command.CommandText = "DELETE FROM recette WHERE NomR='"+listePlat[val]+"';";
                        command.ExecuteNonQuery();

                        Console.WriteLine("\nLa recette vient d'être supprimé. Appuyez sur une touche pour continuer.");
                        Console.ReadKey();
                        Console.Clear();
                        choix = 0;
                        break;
                    case 4:
                        Console.Clear();
                        //____________________________________Lister les chefs et proposer quel chef supprimer
                        command = connection.CreateCommand();
                        command.CommandText = "SELECT NomCdR FROM CdR;";
                        reader = command.ExecuteReader();
                        int valeurChef = 0;
                        Console.WriteLine("Quel chef souhaitez vous supprimer ?");
                        compteur = 1;
                        string[] listeChef = new string[50];
                        while (reader.Read())
                        {
                            Console.WriteLine(" " + compteur + ") " + reader.GetString(0));
                            listeChef[compteur] = reader.GetString(0);
                            compteur++;
                        }
                        reader.Close();
                        valeurChef = Convert.ToInt32(Console.ReadLine());
                        //__________________________________Suppression chef
                        command.CommandText = "DELETE FROM CdR WHERE NomCdR='"+ listeChef[valeurChef] + "';";
                        command.ExecuteNonQuery();
                        //__________________________________Suppression de ses recettes
                        command.CommandText = "DELETE FROM recette WHERE NomCdR='"+ listeChef[valeurChef] + "';";
                        command.ExecuteNonQuery();

                        Console.WriteLine("\nLe chef, ainsi que ses recettes, vient d'être supprimé. Appuyez sur une touche pour continuer.");
                        Console.ReadKey();

                        Console.Clear();
                        choix = 0;
                        break;
                    default:
                        Console.WriteLine("Choix incorrect\n");
                        break;
                }
                connection.Close();
            }
        }

        /// <summary>
        /// Permet de vérifier les identifiants et mot de passe d'un client
        /// </summary>
        /// <param name="connection"></param>
        /// <returns>"-1" si retour, nomC si on s'est bien identifié</returns>
        static string Identification(MySqlConnection connection)
        {
            string NomC;
            string mdp;
            string fin;
            bool valide = false;
            string retour = "-1";
            Console.Clear();
            while (valide == false)
            {
                Console.WriteLine("Identifiant :");
                NomC = Console.ReadLine();
                Console.WriteLine("Mot de passe :");
                mdp = Console.ReadLine();
                valide = ConnectCompte(NomC, mdp, connection);
                retour = NomC;
                if (valide == false)
                {
                    Console.WriteLine("Incorrect ! Retour ? (oui ou non)");
                    fin = Console.ReadLine();
                    if(fin == "oui")
                    {
                        retour = "-1";
                        valide = true;
                    }
                }
            }
            return retour;
        }

        /// <summary>
        /// Permet l'inscription d'un client
        /// </summary>
        static void Inscription(MySqlConnection connection)
        {
            connection.Open();
            Console.Clear();
            Console.WriteLine("Nom :");
            string NomC = Console.ReadLine();
            Console.WriteLine("Mot de passe :");
            string mdp = Console.ReadLine();
            Console.WriteLine("Numéro de téléphone :");
            string numtel = Console.ReadLine();

            MySqlCommand command = connection.CreateCommand();
            command.CommandText = "INSERT INTO `cooking`.`client` (`NomC`,`mdp`,`numtelC`) VALUES('" + NomC + "', '" + mdp + "', '" + numtel + "');";
            command.ExecuteNonQuery();
            Console.WriteLine("\n Inscription effectuée ! Appuyez sur une touche pour accèder à votre compte.");
            Console.ReadKey();
            connection.Close();
        }

        /// <summary>
        /// Permet l'inscription d'un créateur de recette
        /// </summary>
        static void InscriptionCdR(string NomCdR, string NomC,MySqlConnection connection)
        {
            connection.Open();
            Console.Clear();
            MySqlCommand command = connection.CreateCommand();
            command.CommandText = "INSERT INTO `cooking`.`CdR` (`NomCdR`,`NomC`) VALUES('" + NomCdR + "', '" + NomC + "');";
            command.ExecuteNonQuery();
            connection.Close();
        }
        
        /// <summary>
        /// Verification des identifiants d'un client fourni avec la base de donnée
        /// </summary>
        /// <param name="id"></param>
        /// <param name="mdp"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        static bool ConnectCompte(string id, string mdp, MySqlConnection connection)
        {
            connection.Open();
            bool valide = false;
            string idRentree;
            string mdpRentree;

            MySqlCommand command = connection.CreateCommand();
            command.CommandText = "select nomC, mdp from client;";

            MySqlDataReader reader;
            reader = command.ExecuteReader();

            while (reader.Read())    // parcours ligne par ligne
            {
                idRentree = reader.GetValue(0).ToString();
                mdpRentree = reader.GetValue(1).ToString();
                if (id == idRentree && mdp == mdpRentree)
                {
                    valide = true;
                }
            }
            connection.Close();

            return valide;
        }

        /// <summary>
        /// Verification des identifiants d'un créateur de recette fourni avec la base de donnée
        /// </summary>
        /// <param name="NomCdR"></param>
        /// <param name="NomC"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        static bool ConnectCdR(string NomCdR, string NomC,MySqlConnection connection)
        {
            connection.Open();
            bool valide = false;
            string NomCdRRentree;
            string NomCRentree;

            MySqlCommand command = connection.CreateCommand();
            command.CommandText = "select NomCdR, NomC from CdR;";

            MySqlDataReader reader;
            reader = command.ExecuteReader();

            while (reader.Read())    // parcours ligne par ligne
            {
                NomCdRRentree = reader.GetValue(0).ToString();
                NomCRentree = reader.GetValue(1).ToString();
                if (NomCdR == NomCdRRentree && NomC == NomCRentree)
                {
                    valide = true;
                }
            }
            connection.Close();

            return valide;
        }

        /// <summary>
        /// Mode de demo
        /// </summary>
        /// <param name="connection">connection à la base de donnée</param>
        static void Demo(MySqlConnection connection)
        {
            connection.Open();
            Console.Clear();
            MySqlCommand command = connection.CreateCommand();
            command.CommandText = " SELECT count(NomC) FROM client;";

            MySqlDataReader reader;
            reader = command.ExecuteReader();

            if (reader.Read())
            {
                Console.WriteLine("Nombre de clients : " + reader.GetInt32(0));
            }

            reader.Close();
            Console.WriteLine("Appuyer sur une touche du clavier pour continuer");
            Console.ReadKey();

            Console.Clear();
            command = connection.CreateCommand();
            command.CommandText = "SELECT count(NomCdR) FROM CdR;";
            reader = command.ExecuteReader();

            if (reader.Read())
            {
                Console.WriteLine("Nombre de CdR : " + reader.GetInt32(0));
            }
            reader.Close();
            command = connection.CreateCommand();
            command.CommandText = "SELECT cdr.NomCdR, sum(c.quantite)" +
                " FROM CdR cdr, rCommandee c, recette r" +
                " WHERE c.NomR = r.NomR and r.NomCdR = cdr.NomCdR" +
                " GROUP BY cdr.NomCdR" +
                " ORDER BY cdr.NomCdR; ";
            reader = command.ExecuteReader();

            while (reader.Read())
            {
                Console.WriteLine("Nom : " + reader.GetValue(0).ToString() + " | Total commandé : " + reader.GetInt32(1));
            }
            reader.Close();
            Console.WriteLine("Appuyer sur une touche du clavier pour continuer");
            Console.ReadKey();

            Console.Clear();
            command = connection.CreateCommand();
            command.CommandText = "SELECT count(NomR) FROM recette;";
            reader = command.ExecuteReader();

            if (reader.Read())
            {
                Console.WriteLine("Nombre de recettes : " + reader.GetInt32(0));
            }
            reader.Close();
            Console.WriteLine("Appuyer sur une touche du clavier pour continuer");
            Console.ReadKey();

            Console.Clear();
            command = connection.CreateCommand();
            command.CommandText = "SELECT p1.NomP" +
                " FROM produit p1, produit p2" +
                " WHERE p1.stock <= (2 * p2.stock_min) and p1.NomP = p2.NomP; ";
            reader = command.ExecuteReader();
            Console.WriteLine("Recette dont le stock est faible :");
            while (reader.Read())
            {
                Console.WriteLine(reader.GetValue(0).ToString());
            }
            reader.Close();
            Console.WriteLine("Appuyer sur une touche du clavier pour continuer");
            Console.ReadKey();


            Console.Clear();
            Console.WriteLine("Choisissez un produit :");
            string produit = Console.ReadLine();
            command = connection.CreateCommand();
            command.CommandText = "SELECT r.NomR, quantite i FROM recette r, produit p, ingredient i" +
                " WHERE r.NomR = i.NomR and i.NomP = p.NomP and p.NomP LIKE '" + produit + "';";
            reader = command.ExecuteReader();
            while (reader.Read())
            {
                Console.WriteLine("Recette : " + reader.GetValue(0).ToString() + " | quantité : " + reader.GetInt32(1));
            }
            reader.Close();
            Console.WriteLine("Appuyer sur une touche du clavier pour terminer");
            Console.ReadKey();
        }
    }
}
