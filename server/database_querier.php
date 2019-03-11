<?php
	class Querier {
		private $_pdo;
		
		public function __construct(string $host, string $dbName, string $username, string passwd) {
			$pdo = new PDO("mysql:host=$host;dbname=$dbName", $username, $passwd);
			$pdo->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);
		}
		
		
		public function getRepart(string £repart) : int {
		
		}
	}
?>