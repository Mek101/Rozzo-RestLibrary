<?php
// funzione per la prima query
function getRepart(string $repart) {
	$cont = 0;
	$str = file_get_contents('libri.json');
	$libri = json_decode($str, true);
	
	$str2 = file_get_contents('LibroCateg.json');
	$libricat = json_decode($str2, true);
	
	foreach($libri["libro"] as $book)
		if($book["reparto"] === $repart)
			foreach($libricat["librocat"] as $libcat)
				if($libcat["libro"] == $book["ID"] and $libcat["categoria"]== "Ultimi arrivi")$cont++;
	
	return $cont;
}

// funzione per la seconda query
function orderSconto() {
	$str = file_get_contents('Libri.json');
	$libri = json_decode($str, true);

	$str2 = file_get_contents('LibroCateg.json');
	$libricat = json_decode($str2, true);

	$str3 = file_get_contents('Categorie.json');
	$categorie = json_decode($str3, true);

	$queryBooks = array();

	foreach($categorie["categoria"] as $cat)	
		if($cat["sconto"] != "0")		
			foreach($libricat["librocat"] as $libcat)			
				if($libcat["categoria"] == $cat["tipo"])				
					foreach($libri["libro"] as $book)					
						if($book["ID"] == $libcat["libro"])
							array_push($queryBooks, array('sconto'=>$cat['sconto'], 'titolo'=>$book['titolo']));
					
	asort($queryBooks);

	return $queryBooks;
}
 
// funzione per la terza query
function dataarc($prima, $seconda) {
	$str = file_get_contents('Libri.json');
	$books = json_decode($str, true);
		
	$queryBooks = array(); 
	$data1 = new DateTime($prima);
	$data2 = new DateTime($seconda);
		

	foreach($books['libro'] as $book) {
		$currentDate = new DateTime($book['dataarc']);
		
		if(date_diff($data1, $currentDate)->format('%R%a') > 0 && (date_diff($data1, $currentDate)->format('%R%a')) <= (date_diff($data1, $data2)->format('%R%a')))
			array_push($queryBooks, $book['titolo']);
	}

	return $queryBooks;
}
 
// funzione per la quarta query
function getCart($cart) {
	$str = file_get_contents("Carrelli.JSON");
	$carrelli = json_decode($str, true);
	$str = file_get_contents("Libri.JSON");
	$libri = json_decode($str, true);
	$str = file_get_contents("LibriCarrello.JSON");
	$libcar = json_decode($str, true);
	$str = file_get_contents("Utenti.JSON");
	$utenti = json_decode($str, true);
	
	$booksAndUser = array();

	foreach($carrelli['carrello'] as $serializedCart)
		if($serializedCart['ID'] == $cart) {
			$extractedUser;
			$extractedBooks = array();

			// Extracting user	
			foreach($utenti['utente'] as $serializedUser)			
				if($serializedUser['telefono'] === $serializedCart['utente']) {
					$extractedUser = $serializedUser['nome'] . " " . $serializedUser['cognome'];
					break;
				}

			// Extracting books
			foreach($libcar['libricarrello'] as $lb)
				if($lb['carrello'] == $cart) {
					foreach($libri['libro'] as $book)					
						if($book['ID'] == $lb['libro'])						
							array_push($extractedBooks, array('libro'=>$book['titolo'], 'ncopie'=>$lb['ncopie']));
				}

			// Sets the results into the array
			$booksAndUser = array('user'=>$extractedUser, 'books'=>$extractedBooks);
			
			// Once the given cart has been found, there's nothing to do.
			break;
		}		
	
	return $booksAndUser;
}


?>