<?php

//$sourceurl = "https://www.hl.co.uk/shares/shares-search-results/3/3i-infrastructure-plc-ord-npv";
//$sourceurl = "https://www.hl.co.uk/shares/shares-search-results/m/microsoft-corporation-com-stk-us$";
//$sourceurl = "https://www.hl.co.uk/shares/shares-search-results/v/vanguard-funds-plc-s-and-p-500-etf-usdgbp";
//$sourceurl = "https://www.hl.co.uk/shares/shares-search-results/n/nvidia-corp-usd0.001";

$servername = "LAPTOP-D6H2KOAE\SQLEXPRESS01";
$database = "InvestmentBuilderTest2";
$connectionInfo = array( "Database"=>$database);
$outfile = "C:/Users/guy/AppData/Roaming/InvestmentRecordBuilder/prices.txt";


$fout = fopen($outfile,'w');

loadAllInstruments($servername, $connectionInfo, $fout);


function loadAllInstruments($servername, $connectionInfo, $fout) {

	$instruments = loadListOfInstruments($servername, $connectionInfo);

	foreach($instruments as $instrument) {
		
		$symbol = trim($instrument[0]);
		
		$result = loadInstrument($symbol, $instrument[1]);
		
		if($result != null) {
			$resultData =  sprintf("M;%s;%s;%s\r\n", $symbol,$result["price"], $result["currency"]);	
		}
		else {
			$resultData =  sprintf("M;%s;;\r\n", $symbol);	
		}
		
		printf('%s\n', $resultData);
		fwrite($fout, $resultData);		
		
		sleep(3);
	}
}

fclose($fout);

//$sourceFile = "c:/projects/data/prices/Gin.html";
// $sourceFile = "c:/projects/data/prices/sandp500.html";


// printf("Loading file %s\n", $sourceFile);

// $data = file_get_contents($sourceFile, false);
// if($data == false) {
	// printf("failed to load file %s.\n", $sourceFile);
	// return;
// }

//$result = processData($data);

//if($result != null) {
//	printf("price: %s. currency: %s\n", $result["price"], $result["currency"]);	
//}


function loadInstrument($symbol, $url) {
	
	printf("loading symbol %s url %s\n", $symbol, $url);
	
	if($url == null) {
		printf("no url specified for %s.\n", $symbol);
		return null;
	}

	$data=loadurl($url);
	if($data == false) {
		printf("failed to load url %s.\n", $url);
		return null;
	}	
	
	return processData($data);	
}

function processData($data) {
	# Create a DOM parser object
	$dom = new DOMDocument();

	# Parse the HTML.
	# The @ before the method call suppresses any warnings that
	# loadHTML might throw because of invalid HTML in the page.
	//$encoding = mb_detect_encoding($data);
	//printf("Encoding is: %s\n", $encoding);

	//$htmldata = mb_convert_encoding($data, 'HTML-ENTITIES', 'UTF8');
	//$encoding = mb_detect_encoding($htmldata);
	//printf("New Encoding is: %s\n", $encoding);
			
	if (@$dom->loadHTML($data) == false) {
		printf("failed to parse url response%s.\n", $url);
		return null;
	}

	$mainContent = $dom->getElementById('mainContent');
	
	if($mainContent == false) {
		print("unable to find content\n");
		return null;
	}
	
	//$currency = getCurrency($mainContent);
	
	//if($currency == null) {
	//	print("unable to parse currency\n");
	//	return null;
	//}
	
	//printf("currency: %s\n", $currency);
			
	$buysell = findElementByAttribute($mainContent, 'class', 'bid price-divide');
	
	if($buysell == false) {
		print("failed to find price\n");
		return null;
	}
	
	if(preg_match('/^([\d,.]+)(.*)$/', trim($buysell->nodeValue), $matches)) {
		printf("amount: %s\n", $matches[1]);
		printf("currency: %s\n", $matches[2]);
		
		return [
		"price" => $matches[1],
		"currency" => resolveCurrency($matches[2], $mainContent)
		];
	}
	 else if(preg_match('/(.)([\d.]+)$/', trim($buysell->nodeValue), $matches)) {
		 printf("amount: %s\n", $matches[2]);
		 printf("currency: %s\n", $matches[1]);
		 
		 return [
		 "price" => $matches[2],
		 "currency" => resolveCurrency($matches[1], $mainContent)
		 ];		
	}
	else {
		printf("unable to parse price %s\n", $buysell->nodeValue);
	}
		
	return null;
}

function resolveCurrency($rawCurrency, $element) {
	$currency1 = trim($rawCurrency);
	
	//printf("raw currency is %s\n", $rawCurrency);	

	if($currency1 == '$') {
		return "USD";
	}
	
	if($currency1 == '£') {
		return "GBP";
	}
	
	if($currency1 == "p") {
		return "GBp";
	}
	
	return getCurrency($element);
}

function loadUrl($url) {
	
  $arrContextOptions=array(
  "ssl"=>array(
           "verify_peer"=>false,
           "verify_peer_name"=>false,
         ),
      );  
	 
  $response = file_get_contents($url, false, stream_context_create($arrContextOptions));	
  return $response;
}

function displayElementDetails($element) {
	printf("element  node %s\n", $element->nodeName);
	printf("element node type %d\n", $element->nodeType);
	printf("element node value %s\n", trim($element->nodeValue));
	printf("%s\n", get_class($element));
}

function findElementByClassName($element, $className) {

	return findElementByAttribute($element, 'class', $className );
}

function findElementByElementName($element, $elementName) {
	foreach($element->childNodes as $subElement) {
		if($subElement->nodeName == $elementName) {
			return $subElement;
		}
	}	
	return NULL;	
}


function findElementByAttribute($element, $attribute, $attributeValue) {

	if($element->nodeType != 1) {
		return NULL;
	}
		
	$elementName = $element->getAttribute($attribute);
	if($elementName == $attributeValue) {
		return $element;
	}
	if($element->hasChildNodes()) {	
		foreach($element->childNodes as $subchild) {
			$foundElement = findElementByAttribute($subchild, $attribute, $attributeValue);
			if($foundElement != NULL) {
				return $foundElement;
			}
		}	
	}
	return NULL;
}

function getCurrency($element) {

	if($element->nodeType != 1) {
		return NULL;
	}
		
	$title = $element->getAttribute('title');
	if( $title != false ) {			
		if( preg_match('/^.+ currency: ([A-Z]{3}) .+$/', trim($title), $matches) 
		 || preg_match('/^The base currency is ([A-Z]{3}) .+$/', trim($title), $matches) ) {				
				return $matches[1];
		}			 		
	}
	
	if($element->hasChildNodes()) {	
		foreach($element->childNodes as $subchild) {
			$currency = getCurrency($subchild);
			if($currency != NULL) {
				return $currency;
			}
		}	
	}
	return NULL;
}

function loadListOfInstruments($servername, $connectionInfo) {

	$conn = sqlsrv_connect($servername, $connectionInfo);

	if( $conn === false ) {
		 die( print_r( sqlsrv_errors(), true));
	}

	echo "connection ok!\n";
	echo "retrieving list of instrument from database\n";		
	
	$sql="select Symbol, SourceUrl from Companies where IsActive = 1";

	$stmt = sqlsrv_query( $conn, $sql);
	if( $stmt === false ) {
		 die( print_r( sqlsrv_errors(), true));
	}

	$row = sqlsrv_fetch( $stmt );
	if($row === false) {
		die( print_r( sqlsrv_errors(), true));
	}
	 
	 $result = array();
	 
	while($row != null) {
		$name = sqlsrv_get_field( $stmt, 0);
		$url = sqlsrv_get_field( $stmt, 1);
		array_push($result, array($name, $url));
		$row = sqlsrv_fetch( $stmt );
		
		if($row === false) {
			die( print_r( sqlsrv_errors(), true));
		}
	}
		 
	sqlsrv_close($conn);	 

	foreach($result as $element ) {
			printf("%s:%s\n", $element[0], $element[1]);			
	}
	return $result;	
}


?>