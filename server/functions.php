<?php
// funzione per la prima query
function fumetti()
 {
	 $cont = 0;
	 $str = file_get_contents('libri.json');
	 $libri = json_decode($str, true);
	 
	 $str2 = file_get_contents('LibroCateg.json');
	 $libricat = json_decode($str2, true);
	 
	 foreach($libri["libro"] as $book)
	 {
		 if($book["reparto"]=="fumetti")
		 {
			 foreach($libricat["librocat"] as $libcat)
			 {
				
				 if($libcat["libro"] == $book["ID"] and $libcat["categoria"]== "Ultimi arrivi")
				 { 
					$cont++;
				 }
			 }
		 }
	 }
	 
	 return $cont;
 }

 // funzione per la seconda query
 function orderSconto()
 {
	$str = file_get_contents('Libri.json');
	$libri = json_decode($str, true);
	
	$str2 = file_get_contents('LibroCateg.json');
	$libricat = json_decode($str2, true);
	
	$str3 = file_get_contents('Categorie.json');
	$categorie = json_decode($str3, true);
	
	$queryBooks = array();
	
	foreach($categorie["categoria"] as $cat)
	{
		if($cat["sconto"] != "0")
		{
			foreach($libricat["librocat"] as $libcat)
			{
				if($libcat["categoria"] == $cat["tipo"])
				{
					foreach($libri["libro"] as $book)
					{
						if($book["ID"] == $libcat["libro"])
							array_push($queryBooks, array('sconto'=>$cat['sconto'], 'titolo'=>$book['titolo']));
					}
				}
			}
		}
		
	}
	
	
	asort($queryBooks);
	
	
	
	return $queryBooks;
 }
 
 // funzione per la terza query
 function dataarc($prima, $seconda)
 {
	$str = file_get_contents('Libri.json');
	$books = json_decode($str, true);
	 
	$queryBooks = array(); 
	$data1 = new DateTime($prima);
	$data2 = new DateTime($seconda);
	 
	
	foreach($books['libro'] as $book)
	{
		$currentDate = new DateTime($book['dataarc']);
		
		if(date_diff($data1, $currentDate)->format('%R%a') > 0)
			if ((date_diff($data1, $currentDate)->format('%R%a')) <= (date_diff($data1, $data2)->format('%R%a')))
				array_push($queryBooks, $book['titolo']);
	}
	
	
	return $queryBooks;
 }
 
 // funzione per la quarta query
 function carrello($carrello)
 {
	$str = file_get_contents("Carrelli.JSON");
	$carrelli = json_decode($str, true);
	$str = file_get_contents("Libri.JSON");
	$libri = json_decode($str, true);
	$str = file_get_contents("LibriCarrello.JSON");
	$libcar = json_decode($str, true);
	$str = file_get_contents("Utenti.JSON");
	$utenti = json_decode($str, true);
	
	$utenteAssociato;
	$ncopie;
	$query = array();
	
	foreach($carrelli['carrello'] as $car)
	{
		if($car['ID'] == $carrello)
		{
			
			foreach($utenti['utente'] as $user)
			{
				if($user['telefono'] == $car['utente']) 
				{
					$utenteAssociato = $user['nome'] . " " . $user['cognome'];

				}
			}
			foreach($libcar['libricarrello'] as $lb)
			{
				
				if($lb['carrello'] == $carrello)
				{
					
					$ncopie = $lb['ncopie'];
					
					foreach($libri['libro'] as $book)
					{
						if($book['ID'] == $lb['libro'])
						{
							array_push($query, array('libro'=>$book['titolo'], 'ncopie'=>$ncopie, 'utente' =>$utenteAssociato));
						}
					}
				}
			}
			
		}
	}
	
	return $query;
 }
?>