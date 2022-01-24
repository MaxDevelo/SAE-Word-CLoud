/*
SAE-PROG-COM-WEB: BUTS1
Auteur: WAWRZYNIAK Maxime.
Description: Le programme permet de récupérer les mots dans les différents discours des président lors de la fin d'année, afin 
afin d'en faire un nuage de mot et donc de générer les pages HTML 
*/

using System;
using System.IO;
using System.Collections.Generic;

class NuageDeMot{

	static void Main()
	{	
		//lancement du programme
		debutProgramme();	
	}
	
	/*
	debutProgramme: proc: la procédure permet d'ouvrir les fichiers et de filtrer afin de mettre en place 
	la génération des pages.
	*/
	
	
	public static void debutProgramme()
	{
	//===========================================================================		
	// Génération du nuage de mot (par date)
	//===========================================================================	
		//Initialisation d'une liste président: dossier par président.
		List<string> president = new List<string>(){"../présidents/Emmanuel-Macron","../présidents/François-Hollande","../présidents/François-Mitterrand","../présidents/Jacques-Chirac","../présidents/Nicolas-Sarkozy","../présidents/Valéry-Giscard-DEstaing"};
		Dictionary<string, int> dict = new Dictionary<string, int>();
		int compteur = 0;

		//Boucle qui parcours chaque dossier de chaque président.
		for(int i=0; i<president.Count; i++)
		{
			//Stockage de tous les fichier d'un dossier dans un tableau: date
			string[] date = Directory.GetFiles(president[i]);
			
			//Boucle qui parcours le tableau: date
			foreach(string fichier in date)
			{
				//On traitre chaque dates (fichiers)
				StreamReader sr = File.OpenText(fichier);
				dict = suppPetiteOcc(ouvrir_fichier(sr));
				GenerationPage(dict,fichier); //Génère la page html qui contient le nuage de mot
				compteur++; //Incrémante si la page html est terminé
				Console.WriteLine(compteur);
			}
			
		}
	//===========================================================================		
	// Génération du nuage de mot (par période)
	//===========================================================================	

		//Initialisation d'une liste président: dossier par président.
		string dossier = "../discours";
		Dictionary<string, int> dict2 = new Dictionary<string, int>();
		//Boucle qui parcours chaque dossier de chaque président.

			//Stockage de tous les fichier d'un dossier dans un tableau: date2
			string[] date2 = Directory.GetFiles(dossier);

			//Boucle qui parcours le tableau: date2
			for(int j=0; j<date2.Length; j=j+2)
			{
				
				string[] tab1 = date2[j].Split('/'); //On récupère la première date. Ex: 2020.txt
				string[] tab2 = date2[j+1].Split('/'); //On récupère la deuxième date. Ex: 2021.txt
				//Récuperation de la date2: Ex: '2012.txt' : '2012' avec fichDate1[0]
				string[] fichDate1 = tab1[2].Split('.'); //On prend la date donc: 2020
				string[] fichDate2 = tab2[2].Split('.'); //On prend la date donc: 2021
				//On défini son répertoire et le nom du fichier .txt
				string nouveauFichier = "../periodes/" + fichDate1[0] + "-" +  fichDate2[0] + ".txt";

				string fichier1 = dossier + "/" + fichDate1[0] + ".txt"; 
				string fichier2 = dossier + "/" + fichDate2[0] + ".txt"; 
				//Stockage de chaque figle dans un tableau.
				string[] fich1 = File.ReadAllLines(fichier1);
				string[] fich2 = File.ReadAllLines(fichier2);
				//Créer et écrire un fichier texte
				using (StreamWriter writer = File.CreateText(nouveauFichier))
				{
					//Déclaration d'un entier pour incrémenter
					int lineNum = 0;
					while(lineNum < fich1.Length || lineNum < fich2.Length)
					{
						if(lineNum < fich1.Length)
						writer.WriteLine(fich1[lineNum]);
						if(lineNum < fich2.Length)
						writer.WriteLine(fich2[lineNum]);
						lineNum++;
					}
			
				}	
			}

			//Stockage de tous les fichier d'un dossier dans un tableau: date
			string[] xdate = Directory.GetFiles("../periodes");
			
			//Boucle qui parcours le tableau: date
			foreach(string fichier in xdate)
			{
				StreamReader sr = File.OpenText(fichier);
				
				dict2 = suppPetiteOcc(ouvrir_fichier(sr));
				GenerationPage2(dict2,fichier);
				//On incrémente chaque fois qu'un fichier html 	a été crée
				compteur++;
				Console.WriteLine(compteur);
			}
			//Fin du programme et donc cela affiche le nombre de page html généré
			Console.WriteLine("Les {0} pages html ont été généré.", compteur); 
		
	}


	/*
	ouvrir_fichier: List<int>: la fonction lit le fichier et stocke les lignes dans une liste
	parametre: StreamReader: sr: initaliation dossier.
	retour: renvoie une liste
	*/
	
	public static Dictionary<string, int> ouvrir_fichier(StreamReader xFichier)
	{
		string mot = "";
		//Initialisation d'un premier dictionnaire
		Dictionary<string, int> dict = new Dictionary<string, int>();
		//Initialisation d'un deuxième dictionnaire
		Dictionary<string, int> dict2 = new Dictionary<string, int>();
		List<StreamReader> fichiers = new List<StreamReader>(new StreamReader[] { File.OpenText("../etape1.txt"), File.OpenText("../etape2.txt"), File.OpenText("../etape3.txt") });
		List<string[]> terminaison = new List<string[]>();
		terminaison = LireFichier2(fichiers);	
		//Stockage de toutes les ponctuations et chiffres et caractères spéciaux dans une liste.
		List<char> ponctuation = new List<char>(){'','<','>','=', ',', ';', '-', '_' ,'?', '!','’', '/', '.',' ','\'', ' ', ':','0','1','2','3','4','5','6','7','8','9','\n','\r'};
		//Liste qui va stocker les radicaux
		List<string> rad = new List<string>();
		//Liste qui va stocker les mots du texte
		List<string> motTexte = new List<string>();
		List<string> motSupp = new List<string>();
		//Ouvertire du fichier texte: stopwords.txt
		StreamReader fich = File.OpenText("../stopwords.txt");
		motSupp = LireFichier(fich);
		//Initialisation d'un dictionnaire qui va contenir: son radical et le mot du texte
		Dictionary<string, string> radMot = new Dictionary<string, string>();
		//Lecture du premier caractère
		int caractere_lu = xFichier.Read() ;
		//Lecture caractere par caractere du fichier
		while(caractere_lu != -1)
		{
				//On regarde si le caractere est dans la liste ponctuation ou non
				if(!(ponctuation.Contains((char)caractere_lu))) // False
				{
					mot += (char)caractere_lu; //Ajout du caractere au string
				} 
				else if(mot != ""){
						mot = mot.ToLower(); // transformer mot en minuscule
						
						if(suppMot(mot, motSupp) == false) //Si le mot est dans la liste des mot
						{
							motTexte.Add(mot); 
							mot = radical(mot, terminaison);
							if(mot != "")
							{
								rad.Add(mot);
								if(!(dict.ContainsKey(mot)))
								{
									dict.Add(mot,1); //Ajout du mot dans le dictionnaire avec une occurrence à 1.
								}
								else{
									dict[mot]++; //Incrémentation de la valeur dans le dictionnaire
								}
							}
						}
						mot = ""; // On vide le mot
					}
			caractere_lu = xFichier.Read() ; //On conitue à lire le caractère suiva,t
		}
		
		/*
		Boucle qui met le mot et son radical dans un dictionnaire.
		*/
		
		for(int i=0; i<motTexte.Count; i++)
		{
			string val = compareMotRad(motTexte[i], rad);
			if(!radMot.ContainsKey(motTexte[i]) && val != "")
			{
				radMot.Add(motTexte[i], val);
			}
		}
		
		/*
		Boucle qui met le mot et son nombre occurence dans un nouveau dictionnaire
		*/
		

			foreach(KeyValuePair<string, string> val in radMot)
			{
				//Si le dictionnaire: dict contien la clé
				if(dict.ContainsKey(val.Value))
				{
					int valeur = dict[val.Value];
					string cle = val.Key;
					//Ajoutd ans le dictionnaire: dict2
					dict2.Add(cle, valeur);
					//Supprime le mot dans l'ancien dictionnaire
					dict.Remove(val.Value);
				}

			}
		

		return dict2; // nouveau dicionnaire
	}


	
	/*
	GenerationPage: proc: la procedure génère la page web par date.
	parametre: Dictionary<string, int>: xDict: dictionnaire.
			   string: NomPresident: nom du président
	*/
	
	
      public static void GenerationPage(Dictionary<string, int> xDict, string NomPresident)
		{
			//Dicttionnaire constitué de 6mots.
			Dictionary<string, int> grandOcc = new Dictionary<string, int>();
			grandOcc = PrendMaxOcc(xDict);
			string[] tab = NomPresident.Split("/");
			string[] xDate = tab[3].Split(".");
			string nomFichierpresident = xDate[0] ;
			//On crée le nom du fichier (html)
			string nomFichier = nomFichierpresident + ".html";
			List<string> datePrésidentiel = new List<string>{"2017-2021","2012-2016","1981-1994","1995-2006","2007-2011","1974-1980"};
			//Liste de couleur
			List<string> couleur = new List<string>{"#955251","#B565A7","#009B77","#DD4124","#D65076","#F24333" ,"#5B2333","#45B8AC","#745296","#632A50","#073B3A","#EB5E55","#D81E5B","#188FA7","#20A4F3" ,"#5B5EA6","#9B2335","#E15D44","#C3447A"};
			//Initialisation du Random
			Random rnd = new Random();
			//Emplacement du fichier html
			string fichier = "../nuage de mot/présidents/" + tab[2] + "/" + nomFichier;
			int maximum = Maximmum(xDict);

			//Verfification si le fichier existe
			if(File.Exists(fichier)) { File.Delete(fichier); }
			//Génération des pages HTML.
			using(StreamWriter s = new StreamWriter(fichier))
			{

				s.WriteLine("{0}","<!DOCTYPE html>");
				s.WriteLine("{0}", "<html lang='fr'>");
				s.WriteLine("{0}", "<head>");
				s.WriteLine("{0}","<meta charset='utf-8'>");
				//Titre de la page web
				s.WriteLine("<title>{0} : 31 Décembre {1}</title>", tab[2], xDate[0]);
				//Mise en place de Bootstrap (pour les barres de progression)
				s.WriteLine("{0}","<link rel='stylesheet' href='https://stackpath.bootstrapcdn.com/bootstrap/4.3.1/css/bootstrap.min.css' integrity='sha384-ggOyR0iXCbMQv3Xipma34MD+dH/1fQ784/j6cY/iJTQUOhcWr7x9JvoRxT2MZw1T' crossorigin='anonymous'>");
				//Rélier html et fichier css
				s.WriteLine("{0}" ,"<link rel='stylesheet' href='../../css/styles1.css' />");
				//Logo de la page web
				s.WriteLine("{0}","<link rel='icon' href='../../images/logo.png' />");
				s.WriteLine("{0}", "</head>");
				s.WriteLine("{0}", "<body>");
				//----------Barre de navigation----------
				// @ permet de ne pas avoir de problème lorsque l'on retourne à la ligne pour continuer à écrire dans le fichier.
				s.WriteLine("{0}",@"<nav id='navigation'>
			<ol>
				<li><a href='../../index.html'>Accueil</a></li>
				<li>
				<a href='#'>Périodes▼</a>
				<ul class='nav-list'>
					<li>
					<a href='../../periodes/1974-1979.html'>1974-1979 </a>
					<ul class='periode'>
						<li><a href='../../periodes/1974-1975.html'>1974-1975</a></li>
						<li><a href='../../periodes/1976-1977.html'>1976-1977</a></li>
						<li><a href='../../periodes/1978-1979.html'>1978-1979</a></li>
					</ul>
					</li>

					<li>
					<a href='../../periodes/1980-1989.html'>1980-1989 </a>
					<ul class='periode'>
						<li><a href='../../periodes/1980-1981.html'>1980-1981</a></li>
						<li><a href='../../periodes/1982-1983.html'>1982-1983</a></li>
						<li><a href='../../periodes/1984-1985.html'>1984-1985</a></li>
						<li><a href='../../periodes/1986-1987.html'>1986-1987</a></li>
						<li><a href='../../periodes/1988-1989.html'>1988-1989</a></li>
					</ul>
					</li>

					<li>
					<a href='../../periodes/1990-1999.html'>1990-1999 </a>
					<ul class='periode'>
						<li><a href='../../periodes/1990-1991.html'>1990-1991</a></li>
						<li><a href='../../periodes/1992-1993.html'>1992-1993</a></li>
						<li><a href='../../periodes/1994-1995.html'>1994-1995</a></li>
						<li><a href='../../periodes/1996-1997.html'>1996-1997</a></li>
						<li><a href='../../periodes/1998-1999.html'>1998-1999</a></li>
					</ul>
					</li>

					<li>
					<a href='../../periodes/2000-2009.html'>2000-2009 </a>
					<ul class='periode'>
						<li><a href='../../periodes/2000-2001.html'>2000-2001</a></li>
						<li><a href='../../periodes/2002-2003.html'>2002-2003</a></li>
						<li><a href='../../periodes/2004-2005.html'>2004-2005</a></li>
						<li><a href='../../periodes/2006-2007.html'>2006-2007</a></li>
						<li><a href='../../periodes/2008-2009.html'>2008-2009</a></li>
					</ul>
					</li>

					<li>
					<a href='../../periodes/2010-2021.html'>2010-2021 </a>
					<ul class='periode'>
						<li><a href='../../periodes/2010-2011.html'>2010-2011</a></li>
						<li><a href='../../periodes/2012-2013.html'>2012-2013</a></li>
						<li><a href='../../periodes/2014-2015.html'>2014-2015</a></li>
						<li><a href='../../periodes/2016-2017.html'>2016-2017</a></li>
						<li><a href='../../periodes/2018-2019.html'>2018-2019</a></li>
						<li><a href='../../periodes/2020-2021.html'>2020-2021</a></li>
					</ul>
					</li>
				</ul>
				</li>
				<li>
				<a href='#'>Présidents▼</a>
				<ul class='nav-list'>
					<li>
					<a href='../../présidents/Emmanuel-Macron/2017-2021.html'>Emmanuel Macron </a>
					<ul class='datePresident'>
						<li><a href='../../présidents/Emmanuel-Macron/2017.html'>31 Décembre 2017</a></li>
						<li><a href='../../présidents/Emmanuel-Macron/2018.html'>31 Décembre 2018</a></li>
						<li><a href='../../présidents/Emmanuel-Macron/2019.html'>31 Décembre 2019</a></li>
						<li><a href='../../présidents/Emmanuel-Macron/2020.html'>31 Décembre 2020</a></li>
						<li><a href='../../présidents/Emmanuel-Macron/2021.html'>31 Décembre 2021</a></li>
						<li><a href='../../présidents/Emmanuel-Macron/2022.html'>31 Décembre 2022</a></li>
					</ul>
					</li>
					<li>
					<a href='../../présidents/François-Hollande/2012-2016.html'>François Hollande </a>
					<ul class='datePresident'>
						<li><a href='../../présidents/François-Hollande/2016.html'>31 Décembre 2016</a></li>
						<li><a href='../../présidents/François-Hollande/2015.html'>31 Décembre 2015</a></li>
						<li><a href='../../présidents/François-Hollande/2014.html'>31 Décembre 2014</a></li>
						<li><a href='../../présidents/François-Hollande/2013.html'>31 Décembre 2013</a></li>
						<li><a href='../../présidents/François-Hollande/2012.html'>31 Décembre 2012</a></li>
					</ul>
					</li>
					<li>
					<a href='../../présidents/Nicolas-Sarkozy/2007-2011.html'>Nicolas Sarkozy </a>
					<ul class='datePresident'>
						<li><a href='../../présidents/Nicolas-Sarkozy/2011.html'>31 Décembre 2011</a></li>
						<li><a href='../../présidents/Nicolas-Sarkozy/2010.html'>31 Décembre 2010</a></li>
						<li><a href='../../présidents/Nicolas-Sarkozy/2009.html'>31 Décembre 2009</a></li>
						<li><a href='../../présidents/Nicolas-Sarkozy/2008.html'>31 Décembre 2008</a></li>
						<li><a href='../../présidents/Nicolas-Sarkozy/2007.html'>31 Décembre 2007</a></li>
					</ul>
					</li>
					<li>
					<a href='../../présidents/Jacques-Chirac/1995-2006.html'>Jacques Chirac </a>
					<ul class='datePresident'>
						<li><a href='../../présidents/Jacques-Chirac/2006.html'>31 Décembre 2006</a></li>
						<li><a href='../../présidents/Jacques-Chirac/2005.html'>31 Décembre 2005</a></li>
						<li><a href='../../présidents/Jacques-Chirac/2004.html'>31 Décembre 2004</a></li>
						<li><a href='../../présidents/Jacques-Chirac/2003.html'>31 Décembre 2003</a></li>
						<li><a href='../../présidents/Jacques-Chirac/2002.html'>31 Décembre 2002</a></li>
						<li><a href='../../présidents/Jacques-Chirac/2001.html'>31 Décembre 2001</a></li>
						<li><a href='../../présidents/Jacques-Chirac/2000.html'>31 Décembre 2000</a></li>
						<li><a href='../../présidents/Jacques-Chirac/1999.html'>31 Décembre 1999</a></li>
						<li><a href='../../présidents/Jacques-Chirac/1998.html'>31 Décembre 1998</a></li>
						<li><a href='../../présidents/Jacques-Chirac/1997.html'>31 Décembre 1997</a></li>
						<li><a href='../../présidents/Jacques-Chirac/1996.html'>31 Décembre 1996</a></li>
						<li><a href='../../présidents/Jacques-Chirac/1995.html'>31 Décembre 1995</a></li>
					</ul>
					</li>
					<li>
					<a href='../../présidents/François-Mitterrand/1981-1994.html'>François Mitterrand</a>
					<ul class='datePresident'>
						<li><a href='../../présidents/François-Mitterrand/1981.html'>31 Décembre 1981</a></li>
						<li><a href='../../présidents/François-Mitterrand/1982.html'>31 Décembre 1982</a></li>
						<li><a href='../../présidents/François-Mitterrand/1983.html'>31 Décembre 1983</a></li>
						<li><a href='../../présidents/François-Mitterrand/1984.html'>31 Décembre 1984</a></li>
						<li><a href='../../présidents/François-Mitterrand/1985.html'>31 Décembre 1985</a></li>
						<li><a href='../../présidents/François-Mitterrand/1986.html'>31 Décembre 1986</a></li>
						<li><a href='../../présidents/François-Mitterrand/1987.html'>31 Décembre 1987</a></li>
						<li><a href='../../présidents/François-Mitterrand/1988.html'>31 Décembre 1988</a></li>
						<li><a href='../../présidents/François-Mitterrand/1989.html'>31 Décembre 1989</a></li>
						<li><a href='../../présidents/François-Mitterrand/1990.html'>31 Décembre 1990</a></li>
						<li><a href='../../présidents/François-Mitterrand/1991.html'>31 Décembre 1991</a></li>
						<li><a href='../../présidents/François-Mitterrand/1992.html'>31 Décembre 1992</a></li>
						<li><a href='../../présidents/François-Mitterrand/1993.html'>31 Décembre 1993</a></li>
						<li><a href='../../présidents/François-Mitterrand/1994.html'>31 Décembre 1994</a></li>
					</ul>
					</li>
					<li>
					<a href='../../présidents/Valéry-Giscard-DEstaing/1974-1980.html'>Valéry Giscard DEstaing </a>
					<ul class='datePresident'>
						<li><a href='../../présidents/Valéry-Giscard-DEstaing/1980.html'>31 Décembre 1980</a></li>
						<li><a href='../../présidents/Valéry-Giscard-DEstaing/1979.html'>31 Décembre 1979</a></li>
						<li><a href='../../présidents/Valéry-Giscard-DEstaing/1978.html'>31 Décembre 1978</a></li>
						<li><a href='../../présidents/Valéry-Giscard-DEstaing/1977.html'>31 Décembre 1977</a></li>
						<li><a href='../../présidents/Valéry-Giscard-DEstaing/1976.html'>31 Décembre 1976</a></li>
						<li><a href='../../présidents/Valéry-Giscard-DEstaing/1975.html'>31 Décembre 1975</a></li>
						<li><a href='../../présidents/Valéry-Giscard-DEstaing/1974.html'>31 Décembre 1974</a></li>
					</ul>
					</li>
				</ul>
				</li>
				<li><a href='../../infos.html'>Aide</a></li>
			</ol>

			</nav>");
				//-------------------------------
				//Mise en place du titre de la page: Si le nom du fichier n'est pas dans la liste
				if(!datePrésidentiel.Contains(nomFichierpresident)){s.WriteLine("<h1>Voeux : {0} : 31 Décembre {1}</h1>", tab[2], xDate[0]);}
				//Mise en place du titre de la page: Si le nom du fichier est dans la liste
				if(datePrésidentiel.Contains(nomFichierpresident)){s.WriteLine("<h1>Voeux : {0} : {1}</h1>", tab[2], xDate[0]);}
				//Mise en place du nuage de mot
				s.WriteLine("{0}","<div class='wordCLoud'>");	
				foreach(KeyValuePair<string, int> val in xDict)
				{	
					//Génération de page pour les différentes date
					if(!datePrésidentiel.Contains(nomFichierpresident))
					{
						if(val.Value == maximum)
						{
							//Mot dans le nuage de mot où sa taille est multiplié par 20 et on rajoute 20. La valeur sera en pixel (px).
							//Le mot sera de couleur noir (le plus grand occurrence)
							s.WriteLine("<p style='font-size: {0}px;font-weight:bold; color:black;'>{1}</p>", val.Value*5+20, val.Key);
						}
						else{
							//Choix d'un index aléatoire au niveau de la liste des couleurs
							int index = rnd.Next(couleur.Count);
							//Stockage de la couleur (RVB) dans un string
							string xCouleur = couleur[index];
							//Le mot sera de couleur aléatoire
							s.WriteLine("<p style='font-size: {0}px; color:{1}'>{2}</p>", val.Value*5+20, xCouleur, val.Key);
						}
					}
					//Génération de page pour les discours présidents global
					else{		
							if(val.Value == maximum)
							{
								s.WriteLine("<p style='font-size: {0}px;font-weight:bold; color:black;'>{1}</p>", val.Value*1.5, val.Key);
							}
							else{
								int index = rnd.Next(couleur.Count);
								string xCouleur = couleur[index];
								s.WriteLine("<p style='font-size: {0}px; color:{1}'>{2}</p>", val.Value*1.5, xCouleur, val.Key);
							}
					}
					
				} 
				s.WriteLine("{0}", "</div>"); 
				//Statistique
				s.WriteLine("{0}", "<div class='stats'>");
				s.WriteLine("{0}", "<h2>Les Plus Grands Themes (stats)</h2>");
				foreach(KeyValuePair<string, int> val in grandOcc)
				{
					s.WriteLine("{0}", "<div class='progress'>");	
					s.WriteLine("<div class='progress-bar progress-bar-striped progress-bar-animated' role='progressbar' aria-valuenow='{0}' aria-valuemin='0' aria-valuemax='{1}' style='width: {2}%'> {3} : {4} occurrences</div>",val.Value, maximum,val.Value, val.Key, val.Value);
					s.WriteLine("{0}", "</div>");
					s.WriteLine("{0}", "<br>");
				}
				s.WriteLine("{0}", "</div>");
				s.WriteLine("{0}", "</body>");
				s.WriteLine("{0}", "</html>");
			 }
			
	}	

    /*
	GenerationPage2: proc: la procedure génère la page web par période.
	parametre: Dictionary<string, int>: xDict: dictionnaire.
			   string: NomPresident: nom du président
	*/
	

    public static void GenerationPage2(Dictionary<string, int> xDict, string NomPresident)
	{
			//Dicttionnaire constitué de 6mots.
			Dictionary<string, int> grandOcc = new Dictionary<string, int>();
			grandOcc = PrendMaxOcc(xDict);
			string[] tab = NomPresident.Split("/");
			string[] xDate = tab[2].Split(".");
			string nomFichier = xDate[0] + ".html";
			int maximum = Maximmum(xDict);
			string fichier = "../nuage de mot/periodes/"  + nomFichier;
			List<string> datePeriode = new List<string>{"1974-1980","1981-1990","1991-2001","2002-2010","2011-2021"};
			//Liste de couleur
			List<string> couleur = new List<string>{"#955251","#B565A7","#009B77","#DD4124","#D65076","#F24333" ,"#5B2333","#45B8AC","#745296","#632A50","#073B3A","#EB5E55","#D81E5B","#188FA7","#20A4F3" ,"#5B5EA6","#9B2335","#E15D44","#C3447A"};
			//Initialisation du Random
			Random rnd = new Random();
			//Verfification si le fichier existe
			if(File.Exists(fichier)) { File.Delete(fichier); }
			//Génération des pages HTML.
            using (StreamWriter s = new StreamWriter(fichier))
            {

               s.WriteLine("{0}","<!DOCTYPE html>");
                s.WriteLine("{0}", "<html>");
				s.WriteLine("{0}", "<head>");
                s.WriteLine("<title>{0} : {1}</title>", tab[1], xDate[0]);
				s.WriteLine("{0}","<link rel='stylesheet' href='https://stackpath.bootstrapcdn.com/bootstrap/4.3.1/css/bootstrap.min.css' integrity='sha384-ggOyR0iXCbMQv3Xipma34MD+dH/1fQ784/j6cY/iJTQUOhcWr7x9JvoRxT2MZw1T' crossorigin='anonymous'>");
				s.WriteLine("{0}" ,"<link rel='stylesheet' href='../css/styles1.css' />");
				s.WriteLine("{0}","<link rel='icon' href='../images/logo.png' />");
                s.WriteLine("{0}", "</head>");
                s.WriteLine("{0}", "<body>");
				//----------Barre de navigation----------
				s.WriteLine("{0}",@"<nav id='navigation'>
			<ol>
				<li><a href='../index.html'>Accueil</a></li>
				<li>
				<a href='#'>Périodes▼</a>
				<ul class='nav-list'>
					<li>
					<a href='../periodes/1974-1979.html'>1974-1979 </a>
					<ul class='periode'>
						<li><a href='../periodes/1974-1975.html'>1974-1975</a></li>
						<li><a href='../periodes/1976-1977.html'>1976-1977</a></li>
						<li><a href='../periodes/1978-1979.html'>1978-1979</a></li>
					</ul>
					</li>

					<li>
					<a href='../periodes/1980-1989.html'>1980-1989 </a>
					<ul class='periode'>
						<li><a href='../periodes/1980-1981.html'>1980-1981</a></li>
						<li><a href='../periodes/1982-1983.html'>1982-1983</a></li>
						<li><a href='../periodes/1984-1985.html'>1984-1985</a></li>
						<li><a href='../periodes/1986-1987.html'>1986-1987</a></li>
						<li><a href='../periodes/1988-1989.html'>1988-1989</a></li>
					</ul>
					</li>

					<li>
					<a href='../periodes/1990-1999.html'>1990-1999 </a>
					<ul class='periode'>
						<li><a href='../periodes/1990-1991.html'>1990-1991</a></li>
						<li><a href='../periodes/1992-1993.html'>1992-1993</a></li>
						<li><a href='../periodes/1994-1995.html'>1994-1995</a></li>
						<li><a href='../periodes/1996-1997.html'>1996-1997</a></li>
						<li><a href='../periodes/1998-1999.html'>1998-1999</a></li>
					</ul>
					</li>

					<li>
					<a href='../periodes/2000-2009.html'>2000-2009 </a>
					<ul class='periode'>
						<li><a href='../periodes/2000-2001.html'>2000-2001</a></li>
						<li><a href='../periodes/2002-2003.html'>2002-2003</a></li>
						<li><a href='../periodes/2004-2005.html'>2004-2005</a></li>
						<li><a href='../periodes/2006-2007.html'>2006-2007</a></li>
						<li><a href='../periodes/2008-2009.html'>2008-2009</a></li>
					</ul>
					</li>

					<li>
					<a href='../periodes/2010-2021.html'>2010-2021 </a>
					<ul class='periode'>
						<li><a href='../periodes/2010-2011.html'>2010-2011</a></li>
						<li><a href='../periodes/2012-2013.html'>2012-2013</a></li>
						<li><a href='../periodes/2014-2015.html'>2014-2015</a></li>
						<li><a href='../periodes/2016-2017.html'>2016-2017</a></li>
						<li><a href='../periodes/2018-2019.html'>2018-2019</a></li>
						<li><a href='../periodes/2020-2021.html'>2020-2021</a></li>
					</ul>
					</li>
				</ul>
				</li>
				<li>
				<a href='#'>Présidents▼</a>
				<ul class='nav-list'>
					<li>
					<a href='../présidents/Emmanuel-Macron/2017-2021.html'>Emmanuel Macron </a>
					<ul class='datePresident'>
						<li><a href='../présidents/Emmanuel-Macron/2017.html'>31 Décembre 2017</a></li>
						<li><a href='../présidents/Emmanuel-Macron/2018.html'>31 Décembre 2018</a></li>
						<li><a href='../présidents/Emmanuel-Macron/2019.html'>31 Décembre 2019</a></li>
						<li><a href='../présidents/Emmanuel-Macron/2020.html'>31 Décembre 2020</a></li>
						<li><a href='../présidents/Emmanuel-Macron/2021.html'>31 Décembre 2021</a></li>
					</ul>
					</li>
					<li>
					<a href='../présidents/François-Hollande/2012-2016.html'>François Hollande </a>
					<ul class='datePresident'>
						<li><a href='../présidents/François-Hollande/2016.html'>31 Décembre 2016</a></li>
						<li><a href='../présidents/François-Hollande/2015.html'>31 Décembre 2015</a></li>
						<li><a href='../présidents/François-Hollande/2014.html'>31 Décembre 2014</a></li>
						<li><a href='../présidents/François-Hollande/2013.html'>31 Décembre 2013</a></li>
						<li><a href='../présidents/François-Hollandey/2012.html'>31 Décembre 2012</a></li>
					</ul>
					</li>
					<li>
					<a href='../présidents/Nicolas-Sarkozy/2007-2011.html'>Nicolas Sarkozy </a>
					<ul class='datePresident'>
						<li><a href='../présidents/Nicolas-Sarkozy/2011.html'>31 Décembre 2011</a></li>
						<li><a href='../présidents/Nicolas-Sarkozy/2010.html'>31 Décembre 2010</a></li>
						<li><a href='../présidents/Nicolas-Sarkozy/2009.html'>31 Décembre 2009</a></li>
						<li><a href='../présidents/Nicolas-Sarkozy/2008.html'>31 Décembre 2008</a></li>
						<li><a href='../présidents/Nicolas-Sarkozy/2007.html'>31 Décembre 2007</a></li>
					</ul>
					</li>
					<li>
					<a href='../présidents/Jacques-Chirac/1995-2006.html'>Jacques Chirac </a>
					<ul class='datePresident'>
						<li><a href='../présidents/Jacques-Chirac/2006.html'>31 Décembre 2006</a></li>
						<li><a href='../présidents/Jacques-Chirac/2005.html'>31 Décembre 2005</a></li>
						<li><a href='../présidents/Jacques-Chirac/2004.html'>31 Décembre 2004</a></li>
						<li><a href='../présidents/Jacques-Chirac/2003.html'>31 Décembre 2003</a></li>
						<li><a href='../présidents/Jacques-Chirac/2002.html'>31 Décembre 2002</a></li>
						<li><a href='../présidents/Jacques-Chirac/2001.html'>31 Décembre 2001</a></li>
						<li><a href='../présidents/Jacques-Chirac/2000.html'>31 Décembre 2000</a></li>
						<li><a href='../présidents/Jacques-Chirac/1999.html'>31 Décembre 1999</a></li>
						<li><a href='../présidents/Jacques-Chirac/1998.html'>31 Décembre 1998</a></li>
						<li><a href='../présidents/Jacques-Chirac/1997.html'>31 Décembre 1997</a></li>
						<li><a href='../présidents/Jacques-Chirac/1996.html'>31 Décembre 1996</a></li>
						<li><a href='../présidents/Jacques-Chirac/1995.html'>31 Décembre 1995</a></li>
					</ul>
					</li>
					<li>
					<a href='../présidents/François-Mitterrand/1981-1994.html'>François Mitterrand</a>
					<ul class='datePresident'>
						<li><a href='../présidents/François-Mitterrand/1981.html'>31 Décembre 1981</a></li>
						<li><a href='../présidents/François-Mitterrand/1982.html'>31 Décembre 1982</a></li>
						<li><a href='../présidents/François-Mitterrand/1983.html'>31 Décembre 1983</a></li>
						<li><a href='../présidents/François-Mitterrand/1984.html'>31 Décembre 1984</a></li>
						<li><a href='../présidents/François-Mitterrand/1985.html'>31 Décembre 1985</a></li>
						<li><a href='../présidents/François-Mitterrand/1986.html'>31 Décembre 1986</a></li>
						<li><a href='../présidents/François-Mitterrand/1987.html'>31 Décembre 1987</a></li>
						<li><a href='../présidents/François-Mitterrand/1988.html'>31 Décembre 1988</a></li>
						<li><a href='../présidents/François-Mitterrand/1989.html'>31 Décembre 1989</a></li>
						<li><a href='../présidents/François-Mitterrand/1990.html'>31 Décembre 1990</a></li>
						<li><a href='../présidents/François-Mitterrand/1991.html'>31 Décembre 1991</a></li>
						<li><a href='../présidents/François-Mitterrand/1992.html'>31 Décembre 1992</a></li>
						<li><a href='../présidents/François-Mitterrand/1993.html'>31 Décembre 1993</a></li>
						<li><a href='../présidents/François-Mitterrand/1994.html'>31 Décembre 1994</a></li>
					</ul>
					</li>
					<li>
					<a href='../présidents/Valéry-Giscard-DEstaing/1974-1980.html'>Valéry Giscard DEstaing </a>
					<ul class='datePresident'>
						<li><a href='../présidents/Valéry-Giscard-DEstaing/1980.html'>31 Décembre 1980</a></li>
						<li><a href='../présidents/Valéry-Giscard-DEstaing/1979.html'>31 Décembre 1979</a></li>
						<li><a href='../présidents/Valéry-Giscard-DEstaing/1978.html'>31 Décembre 1978</a></li>
						<li><a href='../présidents/Valéry-Giscard-DEstaing/1977.html'>31 Décembre 1977</a></li>
						<li><a href='../présidents/Valéry-Giscard-DEstaing/1976.html'>31 Décembre 1976</a></li>
						<li><a href='../présidents/Valéry-Giscard-DEstaing/1975.html'>31 Décembre 1975</a></li>
						<li><a href='../présidents/Valéry-Giscard-DEstaing/1974.html'>31 Décembre 1974</a></li>
					</ul>
					</li>
				</ul>
				</li>
				<li><a href='../infos.html'>Aide</a></li>
			</ol>

			</nav>");
				//-------------------------------
				s.WriteLine("<h1>Voeux : {0} : {1}</h1>", tab[1], xDate[0]);
				s.WriteLine("{0}","<div class='wordCLoud'>");
				//Mise en place des mots sur la page
				foreach(KeyValuePair<string, int> val in xDict)
				{	
			
					//Génération de page pour les différentes date
					if(!datePeriode.Contains(xDate[0]))
					{
						if(val.Value == maximum)
						{
							s.WriteLine("<p style='font-size: {0}px;font-weight:bold; color:black;'>{1}</p>", val.Value*4+20, val.Key);
						}
						else{
							int index = rnd.Next(couleur.Count);
							string xCouleur = couleur[index];
							s.WriteLine("<p style='font-size: {0}px; color:{1}'>{2}</p>", val.Value*4+20, xCouleur, val.Key);
						}
					}
					//Génération de page pour les discours présidents global
					else{		
							if(val.Value == maximum)
							{
								s.WriteLine("<p style='font-size: {0}px;font-weight:bold; color:black;'>{1}</p>", val.Value*1.5+20, val.Key);
							}
							else{
								int index = rnd.Next(couleur.Count);
								string xCouleur = couleur[index];
								s.WriteLine("<p style='font-size: {0}px; color:{1}'>{2}</p>", val.Value*1.5+20, xCouleur, val.Key);
							}
					}
				  
				}  
				s.WriteLine("{0}", "</div>");
				//Statistique
				s.WriteLine("{0}", "<div class='stats'>");
				s.WriteLine("{0}", "<h2>Les Plus Grands Themes (stats)</h2>");
				foreach(KeyValuePair<string, int> val in grandOcc)
				{
					s.WriteLine("{0}", "<div class='progress'>");	
					s.WriteLine("<div class='progress-bar progress-bar-striped progress-bar-animated' role='progressbar' aria-valuenow='{0}' aria-valuemin='0' aria-valuemax='{1}' style='width: {2}%'> {3} : {4} occurrences</div>",val.Value, maximum,val.Value, val.Key, val.Value);
					s.WriteLine("{0}", "</div>");
					s.WriteLine("{0}", "<br>");
				}
				s.WriteLine("{0}", "</div>");
                s.WriteLine("{0}", "</body>");
                s.WriteLine("{0}", "</html>");
            }
	}


	/*
	compareMotRad: string: le programme permet de recuprer le radical d'un mot à partir de la liste.
	parametre: string: xMot: mot à regarder
			   List<string>: xRad: liste des radicals
	retour: renvoie le radical ou rien
	*/
	public static string compareMotRad(string xMot, List<string> xRad)
	{
		for(int j=0; j<xRad.Count; j++)
		{
			//Regarde si la longueur du radical est plus petite que la longueur du mot.
			if(xRad[j].Length < xMot.Length)
			{
				/*
				 Explication: 
				 xMot = vivre et xRad[j] = viv. Donc vivre sera transformé en viv. Donc viv == viv, cela est vrai
				*/
				if(xMot.Substring(0, xRad[j].Length) == xRad[j])
				{
					return xRad[j];
				}
			}
				
		}
		
		return ""; //Retourne une chaine de caractere vide
	}
	
	/*
	suppMot: fonct: bool: verifie si le mot se situe dans la liste des mots
	inutiles.
	parametre: string: xmot: mot à verifier.
	retour: envoie vrai ou faux.
	*/
	public static bool suppMot(string xmot, List<string> motSupp)
	{
		bool verifie = false;

		//Si le mot est dans la liste: alors c'est vrai.
		if(motSupp.Contains(xmot))
		{
			verifie = true;
		}
		
		return verifie; //Retourne VRAI ou FAUX
	}

	/*
	stokerResultat: proc: la procedure permet stocker lr resultat du dictionnaire dans un fichier texte.
	parametre: Dictionary<string, int>: Xocc_mot: dictionnaire.
			   string: fichier: nom du fichier a traiter
	*/
	public static void stokerResultat(Dictionary<string, int> Xocc_mot, string fichier, string nomDossier)
	{
		string nom = "../../../résultatCompilation/" + nomDossier + "/" + "resultat-" + fichier;
		if(File.Exists(nom)){File.Delete(nom);}
		//Créer fichier .txt pour sticker resultat.
		FileStream fs = File.Create(nom);
		
		using(StreamWriter writetext = new StreamWriter(fs))
		{
			foreach(KeyValuePair<string, int> val in Xocc_mot)
			{
				//On écrit le mot et son occurrence sur  le fichier
				writetext.WriteLine(val.Key + "(" + val.Value + ")");	
			}
		 
		}
	}

	/*
	LireFichier: List<string>: la fonction permet de stocker le mot dans le fichier dans une liste.
	parametre: StreamReader: xFichier: fichier .
	retour: renvoie une liste
	*/
	
	public static List<string> LireFichier(StreamReader xFichier)
	{
		string mot;
		List<string> xmot = new List<string>();
		while((mot = xFichier.ReadLine())!=null) //Tant qu'on n'arrive pas à la fin du fichier
		{
			xmot.Add(mot);

		}
		return xmot;
	}
	
	/*
	radical: string: fonct: la fonction permet de transformer un mot en radical via les 3 etapes.
	paramètre: string: mot
	retour: renvoie le mot transformé en radical.
	*/


	public static string  radical(string xmot, List<string[]> terminaison)
	{
			string nouveauMot = "";
				for(int v = 0; v<terminaison.Count; v++)
				{
						// 3ieme colonne = Epsilone. Donc on supprime la "terminaison"
						if(terminaison[v][2] == "epsilon" && xmot.Length > terminaison[v][1].Length)
						{
							if(xmot.Substring((xmot.Length - terminaison[v][1].Length), terminaison[v][1].Length) == terminaison[v][1])
							{
									nouveauMot = xmot.Substring(0, (xmot.Length - terminaison[v][1].Length));
									if(validation(nouveauMot,int.Parse(terminaison[v][0])))
									{
										return nouveauMot;
									}
									
							}	
						
						}
						// 2ieme colonne: on supprime la terminaison afin de le remplacer par la 3ieme colonne. Ex: "que" en "c", Olympique = Olympic
						else if (xmot.Length > terminaison[v][1].Length)
						{
							if(xmot.Substring((xmot.Length - terminaison[v][1].Length), terminaison[v][1].Length) == terminaison[v][1])
						
								nouveauMot = xmot.Substring(0, ((xmot.Length - terminaison[v][1].Length) )) + terminaison[v][2];
								if(validation(nouveauMot,int.Parse(terminaison[v][0])))
								{
									return nouveauMot;
								}
							}					
					   }
							
				return nouveauMot;
		}
		


	/*
	LireFichier2: List<string[]>: fonct: permet de stocker les etapes dans une liste.
	parametre: StreamReader: fichiers: 3 etapes.
	retour: renvoie une liste
	*/
		public static List<string[]> LireFichier2(List<StreamReader> fichiers)
		{
			List<string[]> sorted = new List<string[]>();
			for (int j = 0; j < fichiers.Count; j++) {
				StreamReader fichier = fichiers[j];
				while(!(fichier.EndOfStream))
				{
					string line = fichier.ReadLine();
					sorted.Add(line.Split(' '));
				}
			}

			return sorted;
		}

	/*
	validation: bool: fonct: Permet de vérifier le critère de validité.
	parametre: StreamReader: xFichier: fichier .
	retour: renvoie une liste
	*/
	public static bool validation(string xmot, int critere)
	{
		//Initialisation d'une liste avec toute les voyelles
		List<char> voyelle = new List<char>(){'a','e','y','u','i','o'};
		//IInitialisation de l'entier qu'on devra incrémenter
		int m = 0;
		bool verification = false;
		if(critere == 0) //Si l'entier est égal à 0
		{
			for(int i=1; i<xmot.Length; i++)
			{	
				//Si le mot contient une voyelle suivit de  un ou plusieurs consonnes.
				if(voyelle.Contains(xmot[i-1]) && !voyelle.Contains(xmot[i]))
				{
					m++; //Incrémentation
				}
			}
			if(m > 0) // Si l'entier est supérieur à 0
			{
				verification = true; //Vraie
			}
			else{
				verification = false; //Faux
			}
		}
		else //alors l'entier est egal à 1 
		{
			for(int i=1; i<xmot.Length; i++)
			{	
				if(voyelle.Contains(xmot[i-1]) && !voyelle.Contains(xmot[i]))
				{
					m++;
				}
			}
			if(m > 1)
			{
				verification = true;
			}
			else{
				verification = false;
			}
		}

		return verification;

	}
	
	/*
	suppPetiteOcc: Dictionary<string, int>: fonct: crée un nouveau dictionnaire en prenant 30mots.
	parametre: Dictionary<string, int>: xDict: dictionnaire à traité .
	retour: renvoie un dictionnaire
	*/
	public static Dictionary<string, int> suppPetiteOcc(Dictionary<string, int> xDict)
    {
        Dictionary<string, int> nouveauDictTrie = new Dictionary<string, int>();
		List<int> list = new List<int>();

		foreach(KeyValuePair<string, int> mot in xDict)
		{
			list.Add(mot.Value);
		}

		//Trie par insertion
		int val, prec;
		for(int i=1; i<list.Count; i++)
		{
			val = list[i];
			prec = i-1;
			while(prec >=0 && list[prec]>val)
			{
				list[prec+1] = list[prec--];
				list[prec+1] = val;
			}
		}

		//Insertion de 50mots dans un nouveau dictionnaire
		for(int j=list.Count-1; j>list.Count-1-50; j--)
		{
			foreach(KeyValuePair<string, int> vl in xDict)
			{
				if(list[j] == vl.Value && !nouveauDictTrie.ContainsKey(vl.Key))
				{
					nouveauDictTrie.Add(vl.Key, vl.Value);
					break;
				}
			}
		}
		nouveauDictTrie = melanger(nouveauDictTrie);
		return nouveauDictTrie;
	}

	/*
	melanger: Dictionary<string, int>: fonct: permet de mélanger le dictionnaire
	parametre: Dictionary<string, int>: xDict: dictionnaire à traité .
	retour: renvoie un dictionnaire mélangé
	*/

	public static Dictionary<string, int> melanger(Dictionary<string, int> xDict)
	{
		List<string> mot = new List<string>();
		Dictionary<string, int> DictMelange = new Dictionary<string, int>();
		string temp = "";
		foreach(KeyValuePair<string, int> val in xDict)
		{
			mot.Add(val.Key);
		}

		for(int i=0; i<mot.Count; i++)
		{
			temp = mot[randomIndic(i, mot.Count)];
			mot.RemoveAt(mot.IndexOf(temp));
			mot.Insert(i, temp);
		}

			for(int i=0; i<mot.Count; i++)
			{
				if(xDict.ContainsKey(mot[i]))
				{
					DictMelange.Add(mot[i], xDict[mot[i]]);
				}
			}

			return DictMelange;
	}

	/*
	randomIndic: int: fonct: permet de prendre une valeur aléatoire entre une borne inférieur et supérieur
	parametre: xInf: int: borne inférieur
			   xSup: int: borne supérieur
	retour: renvoie un nombre aléatoire
	*/
	public static int randomIndic(int xInf, int xSup)
	{
		Random rd = new Random();
		 return rd.Next(xInf, xSup);
	}
	

	/*
		Maximmum: fonct: int: La fonction recupère  le maximum
	*/

	public static int Maximmum(Dictionary<string, int> xmot)
	{	

		int max = 0;
		
		foreach(KeyValuePair<string, int> g in xmot)
		{
			if(max < g.Value)
			{
				max = g.Value;
			}
		}
		return max;
	}

	/*
	PrendMaxOcc: Dictionary<string, int>: fonct: crée un nouveau dictionnaire en prenant 6mots.
	parametre: Dictionary<string, int>: xDict: dictionnaire à traité .
	retour: renvoie un dictionnaire
	*/
	public static Dictionary<string, int> PrendMaxOcc(Dictionary<string, int> xDict)
    {
        Dictionary<string, int> nouveauDictTrie = new Dictionary<string, int>();
		List<int> list = new List<int>();

		foreach(KeyValuePair<string, int> mot in xDict)
		{
			list.Add(mot.Value);
		}

		//Trie par insertion
		int val, prec;
		for(int i=1; i<list.Count; i++)
		{
			val = list[i];
			prec = i-1;
			while(prec >=0 && list[prec]>val)
			{
				list[prec+1] = list[prec--];
				list[prec+1] = val;
			}
		}

		//Insertion de 50mots dans un nouveau dictionnaire
		for(int j=list.Count-1; j>list.Count-1-7; j--)
		{
			foreach(KeyValuePair<string, int> vl in xDict)
			{
				if(list[j] == vl.Value && !nouveauDictTrie.ContainsKey(vl.Key))
				{
					nouveauDictTrie.Add(vl.Key, vl.Value);
					break;
				}
			}
		}
		return nouveauDictTrie;
	}

}
