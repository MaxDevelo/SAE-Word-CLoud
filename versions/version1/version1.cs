using System;
using System.IO;
using System.Collections.Generic;

class version1{
	
	

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
		//Affichage sur la console pour permettre à l'uilisateur de savoir quoi écrire
		Console.WriteLine("Veuillez rentrer le president (Ex: Emmanuel-Macron) :");
		//Initaliation d'un string
		string president =  "../../présidents/" + Console.ReadLine();
	
		if(verificationDossier(president) == true)
		{
			Console.WriteLine("Veuillez rentre une date (Ex: 2021, pour le 31 Décembre 2021) : ");
			int date = int.Parse(Console.ReadLine()); 
			string fichier = president + "/" + date + ".txt"; 
			StreamReader sr = File.OpenText(fichier);
			
			affiche_dictionnaire(ouvrir_fichier(sr));
		}
		else
		{
			Console.WriteLine("dossier introuvable");
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
		//Stockage de toutes les ponctuations et chiffres et caractères spéciaux dans une liste.
		List<char> ponctuation = new List<char>(){'',',', ';', '-', '_' ,'?', '!','’', '/', '.',' ','\'', ' ', ':','0','1','2','3','4','5','6','7','8','9','\n','\r'};
		//Lecture du premier caractère
		int caractere_lu = xFichier.Read() ;
		string xMot = "";
		//Lecture caractere par caractere du fichier
		while(caractere_lu != -1)
		{
				if(!ponctuation.Contains((char)caractere_lu))
				{
					mot += (char)caractere_lu;
				} 
				else if(mot != ""){
						xMot = mot.ToLower();
						if(!dict.ContainsKey(xMot))
						{
							dict.Add(xMot, 1);
						}
						else{
							dict[xMot]++;
						}
						mot = "";
					
					}
			
			caractere_lu = xFichier.Read();
		}
		
		return dict;
	}
	


	/*
	affiche_dictionnaire: proc: la procedure affiche un dictionnaire
	parametre: ictionary<string, int>: Xocc_mot: dictionnaire.
	*/
	public static void affiche_dictionnaire(Dictionary<string, int> Xocc_mot)
	{
		foreach(KeyValuePair<string, int> val in Xocc_mot)
		{
			Console.Write(val.Key +","+val.Value);
			
			Console.WriteLine();
		}
	}
	

}
