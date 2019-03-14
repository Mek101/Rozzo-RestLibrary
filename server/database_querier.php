<?php
	class Querier {
		private $_pdo;
		
		public function __construct(string $host, string $dbName, string $username, string passwd) {
			$pdo = new PDO("mysql:host=$host;dbname=$dbName", $username, $passwd);
			$pdo->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);
		}
		
		
		public function getRepart(string $repart) : int {
		
		}
		
		public function dates($start, $end)
		{
			$stmt = $conn->prepare("SELECT titolo from `libri` where dataarc between '$start' and '$end'"); 
				$stmt->execute();
				$stmt->setFetchMode(PDO::FETCH_ASSOC);
				$result = $stmt->fetchAll();
			return $result;
		}
		
		public function orderDiscount()
		{
			$stmt = $conn->prepare("SELECT titolo, sconto from libri join librocateg on libri.id = librocateg.libro join categorie on librocateg.categoria = categorie.tipo where sconto > 0 order by sconto");
				$stmt->execute();
				$stmt->setFetchMode(PDO::FETCH_ASSOC);
				$result = $stmt->fetchAll();
				
			return $result;
		}
		
		public function cont()
		{
			$stmt = $conn->prepare("select count(libri.id) from libri join reparti on libri.reparto = reparti.id join libricat on libri.id = librocateg.id where reparti.tipo='fumetti' and librocateg.categoria = 'Ultimi arrivi");
				$stmt->execute();
				$stmt->setFetchMode(PDO::FETCH_ASSOC);
				$result = $stmt->fetchAll();
				
			return $result;
		}
		
		public function cart($cart)
		{
			
		}
		
	}
?>