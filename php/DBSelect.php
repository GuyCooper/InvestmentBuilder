<?php

$servername="DESKTOP-JJ9QOJA\SQLEXPRESS";
$connectionInfo = array( "Database"=>"InvestmentBuilderTest2");
//Initial Catalog=InvestmentBuilderTest2;Integrated Security=True";

$conn = sqlsrv_connect($servername, $connectionInfo);

if( $conn === false ) {
     die( print_r( sqlsrv_errors(), true));
}

echo "connection ok! <br/>";
	
$sql="select Symbol from Companies";

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

?>