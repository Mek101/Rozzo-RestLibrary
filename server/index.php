<?php
// process client request (via URL)
	header ("Content-Type_application/json");
	include ("functions.php");
	if(!empty($_GET['name'])){
			$name=$_GET['name'];
			
			switch($name){
				case 1:
					$cont = fumetti();
					if($cont == 0)
						deliver_response(404,"not found", NULL);
					else
					{
						deliver_response(200, 'success', $cont);
					}
					
					break;
				case 2:
					$books = orderSconto();
					if($books == NULL)
						deliver_response(404,"not found", NULL);
					else
						deliver_response(200,"success", $books);
					
					break;
				case 3:
					
					$archivio = dataarc($_GET['start'], $_GET['end']);
					if($archivio == NULL)
						deliver_response(404,"not found", NULL);
					else
						deliver_response(200, "success", $archivio);
					break;
				case 4:
					$cart = carrello($_GET['cart']);
					if ($cart == NULL)
						deliver_response(404, "not found", NULL);
					else
						deliver_response(200, "success", $cart);
					break;
			}	
	}
	else
	{
		//throw invalid request
		deliver_response(400,"Invalid request", NULL);
	}
	
	function deliver_response($status, $status_message, $data)
	{
		header("HTTP/1.1 $status $status_message");
		
		$response ['status']=$status;
		$response['status_message']=$status_message;
		$response['data']=$data;
		
		$json_response=json_encode($response);
		echo $json_response;
	}

?>