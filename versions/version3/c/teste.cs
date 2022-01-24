using System;
using System.IO;
using System.Collections.Generic;

class teste{
	

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
        string fichier = "../../../test.txt";
		StreamReader sr = File.OpenText(fichier);		
		afficheResultat(ouvrir_fichier(sr));
	}
	
	/*
	ouvrir_fichier: List<int>: la fonction lit le fichier et stocke les lignes dans une liste
	parametre: StreamReader: sr: initaliation dossier.
	retour: renvoie une liste
	*/
	
	public static Dictionary<string, string[]> ouvrir_fichier(StreamReader xFichier)
	{
		string mot = "";
		//Initialisation d'un  dictionnaire
		Dictionary<string, int> dict = new Dictionary<string, int>();
		//Va permetre d'éviter de faire 3 while pour lire les 3 etapes
		List<StreamReader> fichiers = new List<StreamReader>(new StreamReader[] { File.OpenText("../../../etape1.txt"), File.OpenText("../../../etape2.txt"), File.OpenText("../../../etape3.txt") });
		List<string[]> terminaison = new List<string[]>();
		terminaison = LireFichier2(fichiers);	
        //Initialisation d'un dictionnaire qui va stocker le radical, le mot et son nombre occurrence.
        Dictionary<string, string[]> MotTraite = new Dictionary<string, string[]>();
		List<string> motSupp = new List<string>();
		StreamReader fich = File.OpenText("../../../stopwords.txt");
		motSupp = LireFichier(fich);
		//Stockage de toutes les ponctuations et chiffres et caractères spéciaux dans une liste.
		List<char> ponctuation = new List<char>(){'','*','%','—','<','>','=', ',', ';', '-', '_' ,'?', '!','’', '/', '.',' ','\'', ' ', ':','0','1','2','3','4','5','6','7','8','9','\n','\r'};
		//Liste qui va stocker les radicaux
		List<string> rad = new List<string>();
		//Liste qui va stocker les mots du texte
		List<string> motTexte = new List<string>();
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
			caractere_lu = xFichier.Read() ; //On conitue à lire le caractère suivant
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
		Boucle qui met le mot et son nombre occurence
		*/
			foreach(KeyValuePair<string, string> val in radMot)
			{
                string[] tabRadical = new string[2];
				if(dict.ContainsKey(val.Value))
				{
					int valeur = dict[val.Value];
					string cle = val.Key;
                    tabRadical[0] = cle; //Contient le mot
                    tabRadical[1] = valeur.ToString(); //Contient le nombre occurrence (converti en string)
                    MotTraite.Add(val.Value, tabRadical); //Stockage du radical et du tableau dans un dictionnaire.
					dict.Remove(val.Value);
				}

			}
		stokerResultat(MotTraite);
		return MotTraite;
	}

    /*
	afficheResultat: procédure : Affichage du radical, mot et son nombre occurrence.
	parametre: Dictionary<string, string[]>: RadMotOcc: dictionnaire.
	*/
	public static void afficheResultat(Dictionary<string, string[]> RadMotOcc)
	{
        foreach(KeyValuePair<string, string[]> val in RadMotOcc)
		{
            Console.Write(val.Key);
            foreach(string x in val.Value)
            {
                Console.Write(" " + x);
            }
            Console.WriteLine();
        }
	}

    	/*
	stokerResultat: proc: la procedure permet stocker resulta du dictionnaire dans un fichier.
	parametre: Dictionary<string, string[]>: Xocc_mot: dictionnaire.
	*/
	public static void stokerResultat(Dictionary<string, string[]> Xocc_mot)
	{
		string nom = "../../../résultatCompilation/teste/Resultat.txt";
		
         if(File.Exists(nom)){File.Delete(nom);}
		//Créer fichier .txt pour sticker resultat.
		 FileStream fs = File.Create(nom);
		
		using(StreamWriter writetext = new StreamWriter(fs))
        {
            foreach(KeyValuePair<string, string[]> val in Xocc_mot)
            {
                writetext.WriteLine(val.Key + " " + val.Value[0] + " " + val.Value[1]);
            }
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
			if(xRad[j].Length < xMot.Length)
			{
				if(xMot.Substring(0, xRad[j].Length) == xRad[j])
				{
					return xRad[j];
				}
			}
				
		}
		return xMot;
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

		for(int v = 0; v<terminaison.Count; v++)
		{
            string nouveauMot = "";
			// 3ieme colonne = Epsilone. Donc on supprime la "terminaison"
			if(terminaison[v][2] == "epsilon" && xmot.Length > terminaison[v][1].Length)
			{
				if(xmot.Substring((xmot.Length - terminaison[v][1].Length), terminaison[v][1].Length) == terminaison[v][1])
				{
						//On contruit le redical
						nouveauMot = xmot.Substring(0, (xmot.Length - terminaison[v][1].Length));
						if(validation(nouveauMot,int.Parse(terminaison[v][0]))) //On vérifie sa validation
						{
							return nouveauMot;
						}
				}	
			
			}
			// 2ieme colonne: on supprime la terminaison afin de le remplacer par la 3ieme colonne. Ex: "que" en "c", Olympique = Olympic
			else if (xmot.Length > terminaison[v][1].Length)
			{
				if(xmot.Substring((xmot.Length - terminaison[v][1].Length), terminaison[v][1].Length) == terminaison[v][1])
				{
					//On contruit le redical
					nouveauMot = xmot.Substring(0, ((xmot.Length - terminaison[v][1].Length) )) + terminaison[v][2];
					if(validation(nouveauMot,int.Parse(terminaison[v][0])))//On vérifie sa validation
					{
						return nouveauMot;
					}
				}					
			}
		}
		return xmot;
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
					sorted.Add(line.Split(' ')); //on stocke dans un tableau en se basant sur les espaces
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

	
}
