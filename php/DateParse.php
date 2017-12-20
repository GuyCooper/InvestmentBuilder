<?php

$date = date_parse("03/08/2017");
print_r($date);

if($date["month"] == "3") {
	echo "month is march\n";	
}


?>