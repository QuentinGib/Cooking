drop database cooking;
create database cooking;
use cooking;
set sql_safe_updates=0;
--
drop table if exists rCommandee;
drop table if exists Ingredient;
drop table if exists produit;
drop table if exists recette;
drop table if exists commande;
drop table if exists CdR;
drop table if exists client;
--
CREATE TABLE `cooking`.`client` (
  `NomC` VARCHAR(20) NOT NULL,
  `mdp` VARCHAR(20) NOT NULL,
  `numtelC` VARCHAR(10) NOT NULL,
  PRIMARY KEY (`NomC`) );

  CREATE TABLE `cooking`.`CdR` (
  `NomCdR` VARCHAR(20) NOT NULL,
  `NomC` VARCHAR(20) NOT NULL,
  `Solde` INT NOT NULL DEFAULT(0), 
  PRIMARY KEY (`NomCdR`),
   CONSTRAINT `NomC1` FOREIGN KEY (`NomC`)
		REFERENCES `cooking`.`client` (`NomC`)
		ON DELETE CASCADE
		ON UPDATE NO ACTION);

CREATE TABLE `cooking`.`commande` (
  `numCommande` INT NOT NULL AUTO_INCREMENT,
  `prix` INT NOT NULL,
  `date` DATE NULL DEFAULT(NOW()),
  `NomC` VARCHAR(20) NOT NULL,
  PRIMARY KEY (`NumCommande`),
  CONSTRAINT `NomC2` FOREIGN KEY (`NomC`)
		REFERENCES `cooking`.`client` (`NomC`)
		ON DELETE CASCADE
		ON UPDATE NO ACTION);

CREATE TABLE `cooking`.`recette` (
  `NomR` VARCHAR(30) NOT NULL,
  `type` VARCHAR(10) NOT NULL,
  `descriptif` VARCHAR(256) NOT NULL,
  `PdV_C` INT NOT NULL,
  `Remu_CdR` INT NOT NULL,
  `Compteur` INT DEFAULT(0),
  `NomCdR` VARCHAR(20) NOT NULL,
  PRIMARY KEY (`NomR`),
   CONSTRAINT `NomCdR` FOREIGN KEY (`NomCdR`)
		REFERENCES `cooking`.`CdR` (`NomCdR`)
		ON DELETE CASCADE
		ON UPDATE NO ACTION );

CREATE TABLE `cooking`.`produit` (
  `NomP` VARCHAR(20) NOT NULL,
  `categorie` VARCHAR(10) NOT NULL,
  `unite` VARCHAR(20) NOT NULL,
  `stock` INT NOT NULL,
  `stock_min` INT NOT NULL,
  `stock_max` INT NOT NULL,
  `NomF` VARCHAR(20) NOT NULL,
  `refF` VARCHAR(10) NOT NULL,
  `numtelF` VARCHAR(10) NOT NULL,
  PRIMARY KEY (`NomP`) ); 
   
CREATE TABLE `cooking`.`rCommandee` (
  `numCommande` INT NOT NULL,
  `NomR` VARCHAR(30) NOT NULL,
  `quantite` INT NOT NULL,
  PRIMARY KEY (`numCommande`, `NomR`),
  CONSTRAINT `NomR1` FOREIGN KEY (`NomR`)
		REFERENCES `cooking`.`recette` (`NomR`)
		ON DELETE CASCADE
		ON UPDATE NO ACTION,
   CONSTRAINT `numCommande` FOREIGN KEY (`numCommande`)
		REFERENCES `cooking`.`commande` (`numCommande`)
		ON DELETE CASCADE
		ON UPDATE NO ACTION );

CREATE TABLE `cooking`.`ingredient` (
  `NomR` VARCHAR(30) NOT NULL,
  `NomP` VARCHAR(20) NOT NULL,
  `quantite` INT NOT NULL,
  PRIMARY KEY (`NomP`, `NomR`),
  CONSTRAINT `NomR2` FOREIGN KEY (`NomR`)
		REFERENCES `cooking`.`recette` (`NomR`)
		ON DELETE CASCADE
		ON UPDATE NO ACTION,
   CONSTRAINT `NomP` FOREIGN KEY (`NomP`)
		REFERENCES `cooking`.`produit` (`NomP`)
		ON DELETE CASCADE
		ON UPDATE NO ACTION );
-- insertion dans la table client(10)
INSERT INTO `cooking`.`client` (`NomC`,`mdp`,`numtelC`) VALUES ('LEROUX','Louis','0612345670');
INSERT INTO `cooking`.`client` (`NomC`,`mdp`,`numtelC`) VALUES ('COLIN','Emma','0612345671');
INSERT INTO `cooking`.`client` (`NomC`,`mdp`,`numtelC`) VALUES ('VIDAL','Raphaël','0612345672');
INSERT INTO `cooking`.`client` (`NomC`,`mdp`,`numtelC`) VALUES ('PICARD','Jade','0612345673');
INSERT INTO `cooking`.`client` (`NomC`,`mdp`,`numtelC`) VALUES ('ROGER','Adam','0612345674');
INSERT INTO `cooking`.`client` (`NomC`,`mdp`,`numtelC`) VALUES ('PERROT','Alice','0612345675');
INSERT INTO `cooking`.`client` (`NomC`,`mdp`,`numtelC`) VALUES ('PREVOST','Jules','0612345676');
INSERT INTO `cooking`.`client` (`NomC`,`mdp`,`numtelC`) VALUES ('LANGLOIS','Léa','0612345677');
INSERT INTO `cooking`.`client` (`NomC`,`mdp`,`numtelC`) VALUES ('BRETON','Lucas','0612345678');
INSERT INTO `cooking`.`client` (`NomC`,`mdp`,`numtelC`) VALUES ('LEVEQUE','Camille','0612345679');		

-- insertion dans la table Créateur de recette(6)
INSERT INTO `cooking`.`CdR` (`NomCdR`,`NomC`,`Solde`) VALUES ('ChefLeroux','LEROUX',0);
INSERT INTO `cooking`.`CdR` (`NomCdR`,`NomC`,`Solde`) VALUES ('PetitColin','COLIN',20);
INSERT INTO `cooking`.`CdR` (`NomCdR`,`NomC`,`Solde`) VALUES ('ApprentiVidal','VIDAL',50);
INSERT INTO `cooking`.`CdR` (`NomCdR`,`NomC`,`Solde`) VALUES ('SuperPerrot','PERROT',30);
INSERT INTO `cooking`.`CdR` (`NomCdR`,`NomC`,`Solde`) VALUES ('J.Prevost','PREVOST',0);
INSERT INTO `cooking`.`CdR` (`NomCdR`,`NomC`,`Solde`) VALUES ('MamanLéa','LANGLOIS',10);
		
-- insertion dans la table commande(8)
INSERT INTO `cooking`.`commande` (`numCommande`,`prix`,`date`,`NomC`) VALUES (1,40,NOW(),'VIDAL');
INSERT INTO `cooking`.`commande` (`numCommande`,`prix`,`date`,`NomC`) VALUES (2,20,NOW(),'LEROUX');
INSERT INTO `cooking`.`commande` (`numCommande`,`prix`,`date`,`NomC`) VALUES (3,60,NOW(),'BRETON');
INSERT INTO `cooking`.`commande` (`numCommande`,`prix`,`date`,`NomC`) VALUES (4,30,NOW(),'BRETON');
INSERT INTO `cooking`.`commande` (`numCommande`,`prix`,`date`,`NomC`) VALUES (5,30,NOW(),'ROGER');
INSERT INTO `cooking`.`commande` (`numCommande`,`prix`,`date`,`NomC`) VALUES (6,60,NOW(),'VIDAL');
INSERT INTO `cooking`.`commande` (`numCommande`,`prix`,`date`,`NomC`) VALUES (7,20,NOW(),'LEVEQUE');
INSERT INTO `cooking`.`commande` (`numCommande`,`prix`,`date`,`NomC`) VALUES (8,20,NOW(),'PICARD');		

-- insertion dans la table recette(3)
INSERT INTO `cooking`.`recette` (`NomR`,`type`,`descriptif`,`PdV_C`,`Remu_CdR`,`Compteur`,`NomCdR`) VALUES ('Tacos mexicains','Plat','A la poêle, faire dorer l oignon émincé dans un peu d huile d olive. Rajouter la viande, assaisonner et laisser cuire 5 min. Laver les feuilles de laitue. Couper les tomates et le poivron en petits dés. [...]',30,2,0,'MamanLéa');
INSERT INTO `cooking`.`recette` (`NomR`,`type`,`descriptif`,`PdV_C`,`Remu_CdR`,`Compteur`,`NomCdR`) VALUES ('Pain','Entrée','Mélangez la farine, l huile d olive, le sachet de levure, le sel et ajoutez l eau. Malaxez jusqu à l obstention d une pâte homogène. Le geste est important: faites comme si vous étiez en train de plier un mouchoir avec la pâte.[...]',10,2,0,'J.Prevost');
INSERT INTO `cooking`.`recette` (`NomR`,`type`,`descriptif`,`PdV_C`,`Remu_CdR`,`Compteur`,`NomCdR`) VALUES ('Poireau à la vinaigrette','Entrée','Éplucher et laver la première couche de chaque poireau puis couper la base et la partie verte abîmée. Les faire cuivre 15 minutes, mettre huile vinaigre et jaune d œuf. Disposer',20,2,0,'ApprentiVidal');

-- insertion dans la table produit(12)
INSERT INTO `cooking`.`produit`(`NomP`,`categorie`,`unite`,`stock`,`stock_min`,`stock_max`,`NomF`,`refF`,`numtelF`) VALUES('tortillas','féculent','feuille',30,8,50,'Auchan','auch','0123456789');
INSERT INTO `cooking`.`produit`(`NomP`,`categorie`,`unite`,`stock`,`stock_min`,`stock_max`,`NomF`,`refF`,`numtelF`) VALUES('oignon blanc','légume','unité',5,2,20,'Auchan','auch','0123456789');
INSERT INTO `cooking`.`produit`(`NomP`,`categorie`,`unite`,`stock`,`stock_min`,`stock_max`,`NomF`,`refF`,`numtelF`) VALUES('tomates','fruit','unité',5,2,20,'Monoprix','monop','0987654321');
INSERT INTO `cooking`.`produit`(`NomP`,`categorie`,`unite`,`stock`,`stock_min`,`stock_max`,`NomF`,`refF`,`numtelF`) VALUES('poivron','légume','unité',5,2,20,'Auchan','auch','0123456789');
INSERT INTO `cooking`.`produit`(`NomP`,`categorie`,`unite`,`stock`,`stock_min`,`stock_max`,`NomF`,`refF`,`numtelF`) VALUES('laitue','légume','feuille',5,2,20,'Monoprix','monop','0987654321');
INSERT INTO `cooking`.`produit`(`NomP`,`categorie`,`unite`,`stock`,`stock_min`,`stock_max`,`NomF`,`refF`,`numtelF`) VALUES('bœuf haché','viande','gramme',1000,250,2500,'Auchan','auch','0123456789');
INSERT INTO `cooking`.`produit`(`NomP`,`categorie`,`unite`,`stock`,`stock_min`,`stock_max`,`NomF`,`refF`,`numtelF`) VALUES('farine','féculent','gramme',1000,250,2500,'Monoprix','monop','0987654321');
INSERT INTO `cooking`.`produit`(`NomP`,`categorie`,`unite`,`stock`,`stock_min`,`stock_max`,`NomF`,`refF`,`numtelF`) VALUES('levure','féculent','sachet',2,1,5,'Monoprix','monop','0987654321');
INSERT INTO `cooking`.`produit`(`NomP`,`categorie`,`unite`,`stock`,`stock_min`,`stock_max`,`NomF`,`refF`,`numtelF`) VALUES('huile','corps gras','cuillère à soupe',5,2,15,'Monoprix','monop','0987654321');
INSERT INTO `cooking`.`produit`(`NomP`,`categorie`,`unite`,`stock`,`stock_min`,`stock_max`,`NomF`,`refF`,`numtelF`) VALUES('échalote','légume','unité',5,2,20,'Auchan','auch','0123456789');
INSERT INTO `cooking`.`produit`(`NomP`,`categorie`,`unite`,`stock`,`stock_min`,`stock_max`,`NomF`,`refF`,`numtelF`) VALUES('oeuf','oeuf','unité',5,2,20,'Auchan','auch','0123456789');
INSERT INTO `cooking`.`produit`(`NomP`,`categorie`,`unite`,`stock`,`stock_min`,`stock_max`,`NomF`,`refF`,`numtelF`) VALUES('vinaigre','corps gras','cuillière à soupe',5,2,15,'Auchan','auch','0123456789');   
  		
-- insertion dans la table rCommandee(8)
INSERT INTO `cooking`.`rCommandee`(`numCommande`,`NomR`,`quantite`) VALUES(1,'Tacos mexicains',2);
INSERT INTO `cooking`.`rCommandee`(`numCommande`,`NomR`,`quantite`) VALUES(2,'Pain',1);
INSERT INTO `cooking`.`rCommandee`(`numCommande`,`NomR`,`quantite`) VALUES(3,'Tacos mexicains',3);
INSERT INTO `cooking`.`rCommandee`(`numCommande`,`NomR`,`quantite`) VALUES(4,'Pain',2);
INSERT INTO `cooking`.`rCommandee`(`numCommande`,`NomR`,`quantite`) VALUES(5,'Pain',2);
INSERT INTO `cooking`.`rCommandee`(`numCommande`,`NomR`,`quantite`) VALUES(6,'Poireau à la vinaigrette',3);
INSERT INTO `cooking`.`rCommandee`(`numCommande`,`NomR`,`quantite`) VALUES(7,'Tacos mexicains',1);
INSERT INTO `cooking`.`rCommandee`(`numCommande`,`NomR`,`quantite`) VALUES(8,'Poireau à la vinaigrette',1);
-- insertion dans la table ingredient(13)
INSERT INTO `cooking`.`ingredient`(`NomR`,`NomP`,`quantite`) VALUES('Tacos mexicains','tortillas',8);
INSERT INTO `cooking`.`ingredient`(`NomR`,`NomP`,`quantite`) VALUES('Tacos mexicains','oignon blanc',1);
INSERT INTO `cooking`.`ingredient`(`NomR`,`NomP`,`quantite`) VALUES('Tacos mexicains','tomates',1);
INSERT INTO `cooking`.`ingredient`(`NomR`,`NomP`,`quantite`) VALUES('Tacos mexicains','laitue',1);
INSERT INTO `cooking`.`ingredient`(`NomR`,`NomP`,`quantite`) VALUES('Tacos mexicains','bœuf haché',250);
INSERT INTO `cooking`.`ingredient`(`NomR`,`NomP`,`quantite`) VALUES('Tacos mexicains','poivron',1);
INSERT INTO `cooking`.`ingredient`(`NomR`,`NomP`,`quantite`) VALUES('Pain','farine',500);
INSERT INTO `cooking`.`ingredient`(`NomR`,`NomP`,`quantite`) VALUES('Pain','levure',1);
INSERT INTO `cooking`.`ingredient`(`NomR`,`NomP`,`quantite`) VALUES('Pain','huile',1);
INSERT INTO `cooking`.`ingredient`(`NomR`,`NomP`,`quantite`) VALUES('Poireau à la vinaigrette','échalote',2);
INSERT INTO `cooking`.`ingredient`(`NomR`,`NomP`,`quantite`) VALUES('Poireau à la vinaigrette','oeuf',2);
INSERT INTO `cooking`.`ingredient`(`NomR`,`NomP`,`quantite`) VALUES('Poireau à la vinaigrette','huile',3);
INSERT INTO `cooking`.`ingredient`(`NomR`,`NomP`,`quantite`) VALUES('Poireau à la vinaigrette','vinaigre',1);
