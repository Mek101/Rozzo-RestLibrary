<?php
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

 function orderSconto()
 {
	$str = file_get_contents('libri.json');
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
						//var_dump($book);
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
 
 
 function archiviati($prima, $seconda)
 {
	 $str = file_get_contents('libri.json');
	 $libri = json_decode($str);
	 
	 foreach($libri["libro"] as $lib)
	 {
		
	 }
 }
?>