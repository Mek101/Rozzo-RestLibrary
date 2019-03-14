<?php
	const DATE_FORMAT_MODE = '%R%a';

	// funzione per la prima query
	function getRepart(string $repart) {
		$cont = 0;
		$str = file_get_contents('libri.json');
		$libri = json_decode($str, true);
		
		$str2 = file_get_contents('LibroCateg.json');
		$libricat = json_decode($str2, true);
		
		foreach($libri["libro"] as $book)
			if($book["reparto"] === $repart)
				foreach($libricat["librocat"] as $bookCat)
					if($bookCat["libro"] == $book["ID"] && $bookCat["categoria"]== "Ultimi arrivi") 
						$cont++;
		
		return $cont;
	}

	// funzione per la seconda query
	function orderSconto() : array {
		$books = json_decode(file_get_contents('Libri.json'), true);
		$booksCat = json_decode(file_get_contents('LibroCateg.json'), true);
		$categories = json_decode(file_get_contents('Categorie.json'), true);

		$queryBooks = array();

		foreach($categories["categoria"] as $cat)	
			if($cat["sconto"] !== "0")		
				foreach($booksCat["librocat"] as $bookCat)			
					if($bookCat["categoria"] === $cat["tipo"])				
						foreach($books["libro"] as $book)					
							if($book["ID"] === $bookCat["libro"])
								array_push($queryBooks, array('sconto'=>$cat['sconto'], 'titolo'=>$book['titolo']));
						
		asort($queryBooks);

		return $queryBooks;
	}
	
	// funzione per la terza query
	function dataarc(string $start, string $end) : array {
		$str = file_get_contents('Libri.json');
		$books = json_decode($str, true);
			
		$queryBooks = array(); 
		$data1 = new DateTime($start);
		$data2 = new DateTime($end);
			

		foreach($books['libro'] as $book) {
			$currentDate = new DateTime($book['dataarc']);
			
			if(date_diff($data1, $currentDate)->format(DATE_FORMAT_MODE) > 0 && date_diff($data1, $currentDate)->format(DATE_FORMAT_MODE) <= date_diff($data1, $data2)->format(DATE_FORMAT_MODE))
				array_push($queryBooks, $book['titolo']);
		}

		return $queryBooks;
	}
	
	// funzione per la quarta query
	function getCart($cart) : array {
		$carts = json_decode(file_get_contents("Carrelli.JSON"), true);
		$books = json_decode(file_get_contents("Libri.JSON"), true);
		$bookCart = json_decode(file_get_contents("LibriCarrello.JSON"), true);
		$users = json_decode(file_get_contents("Utenti.JSON"), true);
		
		$booksAndUser = array();

		foreach($carts['carrello'] as $serializedCart)
			if($serializedCart['ID'] == $cart) {
				$extractedUser;
				$extractedBooks = array();

				// Extracting user	
				foreach($users['utente'] as $serializedUser)			
					if($serializedUser['id'] == $serializedCart['utente']) {
						$extractedUser = $serializedUser['nome'] . " " . $serializedUser['cognome'];
						break;
					}

				// Extracting books
				foreach($bookCart['libricarrello'] as $lb)
					if($lb['carrello'] == $cart) {
						foreach($books['libro'] as $book)					
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