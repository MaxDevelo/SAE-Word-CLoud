using System;
using System.IO;
using System.Collections.Generic;

class version3b{
	

	static void Main()
	{	
		//lancement du programme
		debutProgramme();	

	}
		
	/*
	debutProgramme: proc: la procédure permet d'ourvrir les fichier et de filtrer.
	retour: renvoie booleen
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
	ouvrir_fichier: List<int>: la fonction lit le fichier et stocke les lignes dans une liste
	parametre: StreamReader: sr: initaliation dossier.
	retour: renvoie une liste
	*/
	
	public static Dictionary<string, int> ouvrir_fichier(StreamReader xFichier)
	{
		string mot = "";
		Dictionary<string, int> dict = new Dictionary<string, int>();
		StreamReader fich = File.OpenText("../../../etape1.txt");
		StreamReader fich1 = File.OpenText("../../../etape2.txt");
		StreamReader fich2 = File.OpenText("../../../etape3.txt");

		List<string[]> terminaison = new List<string[]>();
		List<string> motSupp = new List<string>();
		StreamReader fichSupp = File.OpenText("../../../stopwords.txt");
		motSupp = LireFichier_MotSupp(fichSupp);
		terminaison = LireFichier(fich,fich1, fich2);
		List<char> ponctuation = new List<char>(){'',',', ';', '-', '_' ,'?', '!','’', '/', '.',' ','\'', ' ', ':','0','1','2','3','4','5','6','7','8','9','\n','\r'};
		int caractere_lu = xFichier.Read() ;
		while(caractere_lu != -1)
		{
				if(!(ponctuation.Contains((char)caractere_lu)))
				{
					mot += (char)caractere_lu;
				} 
				else if(mot != ""){
						mot = mot.ToLower();
						
						if(suppMot(mot, motSupp) == false)
						{
							mot = radical(mot,terminaison);
							if(mot !="")
							{
								if(!(dict.ContainsKey(mot)))
								{
									dict.Add(mot,1);
								}
								else{
									dict[mot]++;
								}
							}
						}
						mot = "";
					}
			caractere_lu = xFichier.Read() ;
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

		if(motSupp.Contains(xmot))
		{
			verifie = true;
		}
		
		return verifie;
	}
	
	/*
	LireFichier_MotSupp: List<string>: la fonction permet de stocker le mot dans le fichier dans une liste.
	parametre: StreamReader: xFichier: fichier .
	retour: renvoie une liste
	*/
	
	public static List<string> LireFichier_MotSupp(StreamReader xFichier)
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
	supp_ponctuation: string: la fonction permet de supprimer toute les ponctuations.
	parametre: string: phrase: phrase .
	retour: renvoie la phrase sans ponctuation
	*/
	
	public static string supp_ponctuation(string phrase)
	{	
		string resultat = "";
		List<char> ponctuation = new List<char>(){',', '_', '.', '-', '!', '?', ':', '/', '(', ')', '\'','`', ';', '!', '/', '.',' ','\'', ' ', ':','0','1','2','3','4','5','6','7','8','9','\n','\r'};
		for(int i=0; i<phrase.Length; i++)
		{
			if(!ponctuation.Contains(phrase[i]))
			{
				resultat += phrase[i];
			}
		}
		
		return resultat;
	}
	

		/*
	radical: string: fonct: la fonction permet de transformer un mot en radical via les 3 etapes.
	paramètre: string: mot
	retour: renvoie le mot transformé en radical.
	*/

	public static string  radical(string xmot, List<string[]>  terminaison)
	{	
	
			string nouveauMot = "";
				for(int v = 0; v<terminaison.Count; v++)
				{
						//Si 3ieme colonne est Epsilone. Supprimer la terminaison
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
						
						//Remplacer la terminaison
						}
						else if (xmot.Length > terminaison[v][1].Length)
						{
							if(xmot.Substring((xmot.Length - terminaison[v][1].Length), terminaison[v][1].Length) == terminaison[v][1])
							{
								nouveauMot = xmot.Substring(0, ((xmot.Length - terminaison[v][1].Length) )) + terminaison[v][2];
								if(validation(nouveauMot,int.Parse(terminaison[v][0])))
								{
									return nouveauMot;
								}

							}					
					   }
				}
		return nouveauMot;
		}
		
	/*
	LireFichier: List<string[]>: fonct: permet de stocker les etapes dans une liste.
	parametre: StreamReader: fichier: etape1.txt .
			   StreamReader: fichier1: etape2.txt .
			   StreamReader: fichier2: etape3.txt .
	retour: renvoie une liste
	*/
		public static List<string[]> LireFichier(StreamReader fichier,StreamReader fichier1,StreamReader fichier2)
		{
			List<string[]> term = new List<string[]>();
			string line;
			string newLine = "";
			//Parcours du fichier etape1.txt
			while(!(fichier.EndOfStream))
			{
				line = fichier.ReadLine();
				for(int i=0; i<line.Length; i++)
				{
					newLine += line[i];
				
				}
		
				term.Add(newLine.Split(' '));
				newLine = "";
			}	
			//Parcours du fichier etape2.txt
			while(!(fichier1.EndOfStream))
			{
				line = fichier1.ReadLine();
				for(int i=0; i<line.Length; i++)
				{
					newLine += line[i];
				
				}
		
				term.Add(newLine.Split(' '));
				newLine = "";
			}	
			//Parcours du fichier etape3.txt
			while(!(fichier2.EndOfStream))
			{
				line = fichier2.ReadLine();
				for(int i=0; i<line.Length; i++)
				{
					newLine += line[i];
				
				}
		
				term.Add(newLine.Split(' '));
				newLine = "";
			}	
			return term;
		}

	/*
	validation: bool: fonct: Permet de vérifier le critère de validité.
	parametre: StreamReader: xFichier: fichier .
	retour: renvoie une liste
	*/
	public static bool validation(string xmot, int critere)
	{

		List<char> voyelle = new List<char>(){'a','e','y','u','i','o'};
		int m = 0;
		bool verification = false;
		if(critere == 0)
		{
			for(int i=1; i<xmot.Length; i++)
			{	
				if(voyelle.Contains(xmot[i-1]) && !voyelle.Contains(xmot[i]))
				{
					m++;
				}
			}
			if(m > 0)
			{
				verification = true;
			}
		}
		else
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
		}

		return verification;

	}

}
