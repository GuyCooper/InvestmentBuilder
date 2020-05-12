<?php

//$filename="C:\Projects\InvestmentBuilder\DOMParser\VOD.html"
if($argc < 2) {
	echo "command line: --o:output_file --a:(1:0)all_symbols --n:lookup_symbol --h:(1:0)historical_data --s:server_name --d:database_name";
	exit();
}

$allparams = parseCommandLine($argv, $argc);

$servername = getArrayValue($allparams, "s", "DESKTOP-JJ9QOJA\SQLEXPRESS");
$database = getArrayValue($allparams, "d", "InvestmentBuilderTest2");
$findSymbol = getArrayValue($allparams, "n", null);
$outfile= getArrayValue($allparams, "o", "results.txt");
$all= getArrayValue($allparams, "a", "0") === "1";
$historical= getArrayValue($allparams, "h", "0") === "1";

printf("server: %s\ndatabase: %s\noutput file: %s\n", $servername, $database, $outfile);
$connectionInfo = array( "Database"=>$database);

$fout = fopen($outfile,'w');

if($findSymbol != null) {
	printf("load symbol %s\n", $findSymbol);
	processInstrument($findSymbol, $fout);
} 

if($all){
	printf("loading all symbols\n");
	loadAllInstruments($servername, $connectionInfo, $fout);
}

fclose($fout);

if($historical) {
	echo "updating historical prices\n";
	updateHistoricalData($servername, $connectionInfo);
}

function loadAllInstruments($servername, $connectionInfo, $fout) {
	$symbols = loadListOfSymbols($servername, $connectionInfo);
	//first process company instruments
	 foreach($symbols as $symbol) {
		 processInstrument(trim($symbol), $fout);
	 }

	 //now process FX instruments
	//processSymbol("VOD.L", $fout);
	processFX("EURGBP", $fout);
	processFX("USDGBP", $fout);
	processFX("CHFGBP", $fout);	
	processFX("DKKGBP", $fout);	
	processFX("CADGBP", $fout);	
	processFX("SEKGBP", $fout);	
}	

function processInstrument($symbol, $outfile) {

	printf("loading data for symbol %s\n", $symbol);	

	$url=sprintf("https://uk.finance.yahoo.com/quote/%s?p=%s", $symbol, $symbol);	
	$result = processData($url);
	if($result != null) {
		$resultData =  sprintf("M;%s;%s;%s\r\n", $symbol,$result["price"], $result["currency"]);	
		displayResult('results', $resultData);
		fwrite($outfile, $resultData);	
	}
}

function processFX($symbol, $outfile) {

	printf("loading data for FX symbol %s\n", $symbol);	

	$url=sprintf("https://uk.finance.yahoo.com/quote/%s=X?p=%s=X", $symbol, $symbol);	
	$result = processData($url);
	if($result != null) {
		$resultData =  sprintf("F;%s;%s\r\n", $symbol, $result["price"]);	
		displayResult('results', $resultData);
		fwrite($outfile, $resultData);	
	}
}

function processData($url) {
	# Create a DOM parser object
	$dom = new DOMDocument();

	# Parse the HTML from Google.
	# The @ before the method call suppresses any warnings that
	# loadHTML might throw because of invalid HTML in the page.
	
	$data=loadUrl($url);
	if($data == false) {
		printf("failed to load url %s.\n", $url);
		return null;
	}
	
	if (@$dom->loadHTML($data) == false) {
		printf("failed to parse url response%s.\n", $url);
		return null;
	}

	//$name=NULL;
	$currency=NULL;
	$price=NULL;

	foreach($dom->getElementById('quote-header-info')->childNodes as $child) {
		//printf("child node %s\n", $child->nodeName);
		//printf("child node type %d\n", $child->nodeType);,
		//printf("child node value %s\n", trim($child->nodeValue));
		//printf("%s\n", get_class($child));
		if($child->nodeType == 1) {
			// if($name == NULL) {
				// $name=findCompanyName($child);
			//}
			if($currency == NULL) {
				$currency=findCompanyCurrency($child);
			}
			if($price == NULL) {
				$price=findCompanyPrice($child);
			}
		}
	}

	$result = [
		"price" => $price,
		"currency" => $currency
	];
	
	return $result;
}

function loadUrl($url) {
	
 $arrContextOptions=array(
  "ssl"=>array(
           "verify_peer"=>false,
           "verify_peer_name"=>false,
        ),
     );  
 $response = file_get_contents($url, false, stream_context_create($arrContextOptions));	
	// $handle = fopen($url, 'r');
	// $contents = fread($handle, filesize($filename));
	// fclose($handle);
	// return $contents;
	return $response;
}

function displayResult($context, $value) {
	if($value !=  NULL) {
		printf("%s: %s\n", $context, $value);
	}
	else {
		printf("%s. NOT FOUND\n", $context);
	}	
}

function findCompanyName($element) {
	$result = findElementByClassName($element, 'D(ib) Fz(18px)');
	if($result !=  NULL) {
		return trim($result->nodeValue); 
	}
	return NULL;
}

function findCompanyCurrency($element) {
	//$result = findElementByClassName($element, 'C($c-fuji-grey-j) Fz(12px)');
	$result = findElementByClassName($element, 'C($tertiaryColor) Fz(12px)');	
	if($result !=  NULL) {
		//return trim($result->nodeValue);
		if(preg_match('/\w+$/', trim($result->nodeValue), $matches)) {
			return $matches[0];
		 }
	}
	return NULL;
}

function findCompanyPrice($element) {
	$result = findElementByClassName($element, 'Trsdu(0.3s) Fw(b) Fz(36px) Mb(-4px) D(ib)');
	if($result !=  NULL) {
		return trim($result->nodeValue); 
	}
	return NULL;
}


function findElementByClassName($element, $className) {

	if($element->nodeType != 1) {
		return NULL;
	}
	$elementName = $element->getAttribute('class');
	if($elementName == $className) {
		return $element;
	}
	if($element->hasChildNodes()) {	
		foreach($element->childNodes as $subchild) {
			$foundElement = findElementByClassName($subchild, $className);
			if($foundElement != NULL) {
				return $foundElement;
			}
		}	
	}
	return NULL;
}

function loadListOfSymbols($servername, $connectionInfo) {

	$conn = sqlsrv_connect($servername, $connectionInfo);

	if( $conn === false ) {
		 die( print_r( sqlsrv_errors(), true));
	}

	echo "connection ok!\n";
	echo "retrieving list of symbols from database\n";		
	
	$sql="select Symbol from Companies where IsActive = 1";

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
		array_push($result, $name);
		$row = sqlsrv_fetch( $stmt );
		
		if($row === false) {
			die( print_r( sqlsrv_errors(), true));
		}
	}
		 
	sqlsrv_close($conn);	 

	foreach($result as $element ) {
			echo $element;
			echo "\n";
	}
	return $result;	
}

function parseCommandLine($argv, $argc) {
	$result = array();	
	for($i = 0; $i < $argc; $i++) {
		$entry = $argv[$i];
		printf("%s\n", $entry );		
		
		if(strncmp($entry, "--", 2) == 0) {
			$param = substr($entry,2);	
			$pos = strpos($param, ':');
			if($pos !== false) {
				$name = substr($param, 0, $pos);
				$value = substr($param, $pos+1);
				$result[$name] = $value;
			}
		}
	}
	return $result;
} 

function getArrayValue($arr, $key, $default) {
	return is_null($arr[$key]) ?  $default : $arr[$key];
}

function updateHistoricalData($servername, $connectionInfo) {
	$conn = sqlsrv_connect($servername, $connectionInfo);

	if( $conn === false ) {
		 die( print_r( sqlsrv_errors(), true));
	}

	// $ftse = https://uk.finance.yahoo.com/quote/^FTSE?p=^FTSE
	// $nasdaq = // $ftse = https://uk.finance.yahoo.com/quote/%5ENDX?p=%5ENDX
	// $SandP = "https://uk.finance.yahoo.com/quote/%5ESPX?p=^SPX";
	// eurostoxx50 = https://uk.finance.yahoo.com/quote/%5ESTOXX50E?p=%5ESTOXX50E
	// hang seng = https://uk.finance.yahoo.com/quote/%5EHSI?p=^HSI
	// nikkei 225 = https://uk.finance.yahoo.com/quote/%5EN225?p=^N225
	// DAX = https://uk.finance.yahoo.com/quote/%5EGDAXI?p=^GDAXI
	
	
	
	echo "connection ok!\n";
	echo "updating historical data\n";
	
	$sql = "select * from HistoricalData";

	$stmt = sqlsrv_query( $conn, $sql);
	if( $stmt === false ) {
		 die( print_r( sqlsrv_errors(), true));
	}

	$indexes = array();
	$row = sqlsrv_fetch( $stmt );
	while($row != null) {
		$symbol = sqlsrv_get_field( $stmt, 1);
		$data = sqlsrv_get_field( $stmt, 2);
		
		$indexes[$symbol] = $data;
		
		$row = sqlsrv_fetch( $stmt );
		
		if($row === false) {
			die( print_r( sqlsrv_errors(), true));
		}
	}

	$today = date("m/d/Y");
	foreach ($indexes as $key => $val) {
		$url= "https://uk.finance.yahoo.com/quote/%5E" . $key . "?p=^" .$key;	
		$result = processData($url);
		if($result != null) {
			$newPrice = sprintf("{date: \"%s\", price: \"%s\"}", $today, $result["price"]);
			$updatedData = rtrim($val, "]") . "," . $newPrice . "]";
			
			printf("updated data: %s\n", $updatedData);
			
			$sql="exec sp_AddHistoricalData ? , ? , ?";
			$params = array("", $key, $updatedData);
			$stmt = sqlsrv_query( $conn, $sql, $params);
			if( $stmt === false ) {
				 die( print_r( sqlsrv_errors(), true));
			}
		}
	}
		
	sqlsrv_close($conn);	 
		//download
}

?>