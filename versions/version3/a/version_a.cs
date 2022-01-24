using System;
using System.IO;
using System.Collections.Generic;

class version3a{
	
	

	static void Main()
	{	
			debutProgramme();	
	}
		/*
		programme: proc: Le programme va permettre à l'utilisateur de rentrer un nom de president et la date
		afin de faire un nuage de mot. Et donc si le dossier ou fichier n'existe pas, 
		cela renvoie une erreur à l'utilisateur.
	*/
	
	public static void debutProgramme()
	{
		int compteur = 0;
		//Initialisation d'une liste président: dossier par président.
		List<string> president = new List<string>(){"../../../présidents/Emmanuel-Macron","../../../présidents/François-Hollande","../../../présidents/François-Mitterrand","../../../présidents/Jacques-Chirac","../../../présidents/Nicolas-Sarkozy","../../../présidents/Valéry-Giscard-DEstaing"};
		//Boucle qui parcours chaque dossier de chaque président.
		for(int i=0; i<president.Count; i++)
		{
			//Stockage de tous les fichier d'un dossier dans un tableau: date
			string[] date = Directory.GetFiles(president[i]);
			
			//Boucle qui parcours le tableau: date
			foreach(string fichier in date)
			{
				StreamReader sr = File.OpenText(fichier);
				
				//Affichage du dictionnaire
				affiche_dictionnaire(ouvrir_fichier(sr));
				compteur++;
				Console.WriteLine("total: " + compteur + " discours traité(s).");
			}
		}
	
	}
	
	/*
	verificationDossier: bool: la fonction vérifie si le dossier existe.
	parametre: StreamReader: sr: initaliation dossier.
			   string: xPresident: dossier president.
	retour: renvoie booleen
	*/
	
	public static bool verificationDossier(string xPresident)
	{
		
		 bool verifie = false;
		 
		 if(Directory.Exists(xPresident) == true)
          {
			verifie = true;
          }

		  return verifie;
	}
	

	
	/*
	ouvrir_fichier: List<int>: la fonction lit le fichier et stocke les lignes dans une liste
	parametre: StreamReader: sr: initaliation dossier.
	retour: renvoie une liste
	*/
	
	public static Dictionary<string, int> ouvrir_fichier(StreamReader xFichier)
	{
		string mot = "";
		Dictionary<string, int> dict = new Dictionary<string, int>();
		//Ouverture des 3 fichiers etapes. 
		StreamReader fich = File.OpenText("../../../etape1.txt");
		StreamReader fich1 = File.OpenText("../../../etape2.txt");
		StreamReader fich2 = File.OpenText("../../../etape3.txt");
		List<string[]> terminaison = new List<string[]>();
		List<string> motSupp = new List<string>();
		//Ouvertire du fichier texte: stopwords.txt
		StreamReader fichSupp = File.OpenText("../../../stopwords.txt");
		motSupp = LireFichier(fichSupp);
		terminaison = LireFichier(fich,fich1, fich2);
		//Stockage de toutes les ponctuations et chiffres et caractères spéciaux dans une liste.
		List<char> ponctuation = new List<char>(){'',',', ';', '-', '_' ,'?', '!','’', '/', '.',' ','\'', ' ', ':','0','1','2','3','4','5','6','7','8','9','\n','\r'};
		//Lecture du premier caractère
		int caractere_lu = xFichier.Read() ;
		//Lecture caractere par caractere du fichier
		while(caractere_lu != -1)
		{
				//On regarde si le caractere est dans la liste ponctuation ou non
				if(!(ponctuation.Contains((char)caractere_lu)))
				{
					mot += (char)caractere_lu; //Ajout du caractere au string
				} 
				else if(mot != ""){
						mot = mot.ToLower();  // transformer mot en minuscule
						
						if(suppMot(mot, motSupp) == false) //Si le mot est dans la liste des mot
						{
							mot = radical(mot, terminaison);
							if(mot !="") 
							{
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
			caractere_lu = xFichier.Read() ; //On conitue à lire le caractère suivant
		}
		
	

		return dict;
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
	affiche_dictionnaire: proc: la procedure affiche un dictionnaire
	parametre: Dictionary<string, int>: Xocc_mot: dictionnaire.
	*/
	public static void affiche_dictionnaire(Dictionary<string, int> Xocc_mot)
	{
		
		foreach(KeyValuePair<string, int> val in Xocc_mot)
		{
			Console.Write(val.Key +","+val.Value);

			Console.WriteLine();
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
		while((mot = xFichier.ReadLine())!=null)
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
						if(terminaison[v][1] == "epsilon" && xmot.Length > terminaison[v][0].Length)
						{
							if(xmot.Substring((xmot.Length - terminaison[v][0].Length), terminaison[v][0].Length) == terminaison[v][0])
							{
									nouveauMot = xmot.Substring(0, (xmot.Length - terminaison[v][0].Length)+1);
									return nouveauMot;
							}	
						
						}
						else if (xmot.Length > terminaison[v][0].Length)
						{
							// 2ieme colonne: on supprime la terminaison afin de le remplacer par la 3ieme colonne. Ex: "que" en "c", Olympique = Olympic
							if(xmot.Substring((xmot.Length - terminaison[v][0].Length), terminaison[v][0].Length) == terminaison[v][0])
							{
								nouveauMot = xmot.Substring(0, ((xmot.Length - terminaison[v][0].Length) )) + terminaison[v][1];
								return nouveauMot;
							}					
					   }
							
				
				}
			
		
		return nouveauMot;
		}

		
	/*
	LireFichier:  List<string[]>: fonct: la fonction permet de mettre les 3 etapes dans une liste.
	paramètre: StreamReader: fichier: etape1.txt
	           StreamReader: fichier1: etape2.txt
			   StreamReader: fichier2: etape3.txt
	retour: renvoie une liste avec un tableau
	*/
		
		public static List<string[]> LireFichier(StreamReader fichier,StreamReader fichier1,StreamReader fichier2)
		{
			List<string[]> term = new List<string[]>();
			string line;
			string newLine = "";
			// Insération de l'etape 1 dans la liste
			while(!(fichier.EndOfStream))
			{
				line = fichier.ReadLine();
				for(int i=2; i<line.Length; i++)
				{
					newLine += line[i];
				
				}
				// Permet de mettre les 3 colonnes dans un tableau
				term.Add(newLine.Split(' '));
				newLine = "";
			}	
			// Insération de l'etape 2 dans la liste
			while(!(fichier1.EndOfStream))
			{
				line = fichier1.ReadLine();
				for(int i=2; i<line.Length; i++)
				{
					newLine += line[i];
				
				}
		
				term.Add(newLine.Split(' '));
				newLine = "";
			}	
			// Insération de l'etape 3 dans la liste
			while(!(fichier2.EndOfStream))
			{
				line = fichier2.ReadLine();
				for(int i=2; i<line.Length; i++)
				{
					newLine += line[i];
				
				}
		
				term.Add(newLine.Split(' '));
				newLine = "";
			}	
			return term;
		}
	

}
