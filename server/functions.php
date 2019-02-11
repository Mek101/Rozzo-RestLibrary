<?php
function fumetti()
 {
	 $cont = 0;
	 $str = file_get_contents('http://localhost/json/libri.json');
	 $libri = json_decode($str, true);
	 
	 $str2 = file_get_contents('http://localhost/json/LibroCateg.json');
	 
	 foreach($libri["libro"] as $book)
	 {
		 if($book["reparto"]=="fumetti")
		 {
			 foreach($libricat["librocat"] as $libcat)
			 {
				 if($libcat["libro"] == $book["ID"] && $libcat["categoria"]== "Ultimi Arrivi")
				 {
					 $cont++;
				 }
			 }
		 }
	 }
	 
	 return $cont;
 }

?>