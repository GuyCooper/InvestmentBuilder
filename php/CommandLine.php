<?php

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

$allparams = parseCommandLine($argv, $argc);

$server = is_null($allparams["s"]) ?  "default_server" : $allparams["s"];
$name = is_null($allparams["n"]) ?  "default_name" : $allparams["n"];

printf("server: %s. name %s\n", $server, $name);

var_dump($allparams);
		
?>