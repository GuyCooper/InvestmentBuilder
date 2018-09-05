<?php

//%url = 'http://markets.businessinsider.com/index/historical-prices/FTSE_100/1.1.2009_13.12.2017'

//$filename="C:\Projects\InvestmentBuilder\DOMParser\VOD.html"
if($argc < 2) {
	echo "command line: --f:data_file --i:index --n:index_name --s:server_name --d:database_name";
	exit();
}

$allparams = parseCommandLine($argv, $argc);

$servername = getArrayValue($allparams, "s", "DESKTOP-JJ9QOJA\SQLEXPRESS");
$database = getArrayValue($allparams, "d", "InvestmentBuilderTest2");
$filename = getArrayValue($allparams, "f", null);
$index = getArrayValue($allparams, "i", null);
$indexname = getArrayValue($allparams, "n", null);

printf("server: %s\ndatabase: %s\nfilename: %s\nindex: %s\n", $servername, $database, $filename, $index);

$connectionInfo = array( "Database"=>$database);

$prices = processDataFile($filename);

if($prices !== null) {

	$data = "[";
	foreach($prices as $price) {
		$data = $data . $price . ",";
		//printf("price: %s\n", $price);
	}
	
	if(strlen($data) > 1) {
		$data = rtrim($data, ",");
	}
	
	$data .= "]";
	//printf("output: %s\n", $data);
	saveHistoricalData($servername, $connectionInfo, $index, $indexname, $data);
	//var_dump($prices);
}

function processDataFile($filename) {
	# Create a DOM parser object
	$dom = new DOMDocument();

	# Parse the HTML from Google.
	# The @ before the method call suppresses any warnings that
	# loadHTML might throw because of invalid HTML in the page.

	$data=loadFile($filename);
	if($data == false) {
		printf("failed to load file %s.\n", $filename);
		return null;
	}
	
	if (@$dom->loadHTML($data) == false) {
		printf("failed to parse file data%s.\n", $data);
		return null;
	}
	
	$root = findRootNode($dom);
	if($root === null) {
		echo "unable to find root node\n";
		return null;
	}
	
	printf("root name: %s\nroot type: %s\n", $root->nodeName, $root->nodeType);
	
	$table = findElementByClassName($root, "table instruments");
	if($table === null) {
		echo "unable to find table class in document\n";
		return null;
	}
	
	$result = array();
	
	if($table->hasChildNodes()) {	
		printf("child nodes in table : %s\n", $table->childNodes->length);
		
		$previousMonth = "";
		foreach($table->childNodes as $row) {
			if($row->nodeName === "tr") {
				$className = $row->getAttribute('class');
				if($className !== "header-row") {   //ignore the header row
					$columns = getRowColumns($row);
					//first subnode contains the date in format (mm/dd/yyyy), second the closing price
					$datestr = trim($columns[0]->nodeValue);
					$price = trim($columns[1]->nodeValue);
					//printf("adding date %s. price %s. to array\n", $date, $price);
					//only add the first price of each month
					$date = date_parse($datestr);
					$month = $date["month"];
					if($month != $previousMonth) {
						$previousMonth = $month;
						$obj = "{ date: \"" . $datestr . "\", price: \"" . $price . "\"}";
						printf("%s\n", $obj);
						array_push($result, $obj);
					}
				}
			}
		}
	}
	
	return $result;
}

function getRowColumns($row) {
	$columns = array();
	foreach($row->childNodes as $column) {
		if($column->nodeName === "td") {
			array_push($columns, $column);
		}
	}		
	return $columns;
}

function findRootNode($document) {
	foreach($document->childNodes as $subroot) {
		if($subroot->nodeType === 1) {
			return $subroot;
		}
	}
	return null;
}

function loadFile($filename) {
	
	$handle = fopen($filename, 'r');
	$contents = fread($handle, filesize($filename));
	fclose($handle);
	return $contents;
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

function saveHistoricalData($servername, $connectionInfo, $index, $description,  $data) {

	$conn = sqlsrv_connect($servername, $connectionInfo);

	if( $conn === false ) {
		 die( print_r( sqlsrv_errors(), true));
	}

	echo "connection ok!\n";
	
	$sql="exec sp_AddHistoricalData ? , ? , ?";
	$params = array($description, $index, $data);
	$stmt = sqlsrv_query( $conn, $sql, $params);
	if( $stmt === false ) {
		 die( print_r( sqlsrv_errors(), true));
	}

	echo "data added to database ok\n";
	sqlsrv_close($conn);	 

}


?>