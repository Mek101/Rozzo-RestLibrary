<?php
	// process client request (via URL)
	header("Content-Type_application/json");
	include("database_querier.php");

	const METHOD_INDEX = 'name';
	const CATEGORY = 'category';
	const START_DATE = 'start';
	const END_DATE = 'end';
	const CART_INDEX = 'cart';

	if(isset($_GET[METHOD_INDEX]) && !empty($_GET[METHOD_INDEX])) {
		try {
			$name = $_GET[METHOD_INDEX];

			$querier = new Querier("localhost", "rozzolibrarydb", "root", ""):
			
			switch($name) {
				case 1:				
					if(isset($_GET[CATEGORY]))
						$deliverSuccess($querier->getRepart($_GET[CATEGORY]));
					else
						deliverInvalidRequest("Category missing.");
					break;

				case 2:
					$books = $querier->orderByDiscount();
					if($books === NULL)
						deliverNotFound();
					else
						deliverSuccess($books);				
					break;

				case 3:
					if(isset($_GET[START_DATE]) && isset($_GET[END_DATE])
						deliverSuccess($querier->getBetweenDates($_GET[START_DATE], $_GET[END_DATE]));
					else
						deliverInvalidRequest();
					break;
				case 4:				
					if(isset($_GET[CART_INDEX]))
						deliverSuccess($querier->getCart($_GET[CART_INDEX]));
					else
						deliverInvalidRequest();
					break;			
				default:
					deliverInvalidRequest();
					break;
			}
		}
		catch(Exception e) {
			deliverServerError(e->getMessage());
		}
	}
	else	
		// Deliver an invalid request
		deliverInvalidRequest("$name is not a recognized method.");
	
	function deliverInvalidRequest(string $error = "") {
		$message = "Invalid request";
		
		if($error !== "")
			$message .= "\n" . $error;

		deliverResponse(400, $message, NULL);
	}

	function deliverNotFound() {
		deliverResponse(404, "Not found", NULL);
	}

	function deliverSuccess($data) {
		deliverResponse(200, "Success", $data);
	}

	function deliverServerError(string $error) {
		deliverResponse(500, "Internal error:\n$error");
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