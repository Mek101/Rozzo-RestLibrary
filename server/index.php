<?php
// process client request (via URL)
	header("Content-Type_application/json");
	include("functions.php");

	const METHOD_CODE = 'name';
	const CATEGORY = 'category';
	const START_DATE = 'start';
	const END_DATE = 'end';

	if(isset($_GET[METHOD_CODE]) && !empty($_GET[METHOD_CODE])){
		$name = $_GET[METHOD_CODE];
		
		switch($name){
			case 1:
				$cont = NULL;
				
				if(isset($_GET[CATEGORY]))
					$cont = getRepart($_GET[CATEGORY]);

				if($cont === NULL)
					deliverNotFound();
				else					
					deliverSuccess($count);					
				break;

			case 2:
				$books = orderSconto();
				if($books === NULL)
					deliverNotFound();
				else
					deliverSuccess($books);				
				break;

			case 3:
				$archive = NULL;

				if(isset($_GET[START_DATE]) && isset($_GET[END_DATE]))
					$archive = dataarc($_GET[START_DATE], $_GET[END_DATE]);

				if($archive === NULL)
					deliverNotFound();
				else
					deliverSuccess($archive);
				break;

			case 4:
				$cart = getCart($_GET['cart']);
				if ($cart === NULL)
					deliverNotFound();
				else
					deliverSuccess($cart);
				break;
		}
	}
	else	
		// Deliver an invalid request
		deliverResponse(400, "Invalid request", NULL);
	
	function deliverNotFound() {
		deliverResponse(404, "not found", NULL);
	}

	function deliverSuccess($data) {
		deliverResponse(200, "success", $data);
	}

	function deliverResponse($status, $status_message, $data) {
		header("HTTP/1.1 $status $status_message");
		
		$response['status'] = $status;
		$response['status_message'] = $status_message;
		$response['data'] = $data;
		
		$json_response = json_encode($response);
		echo($json_response);
	}

?>