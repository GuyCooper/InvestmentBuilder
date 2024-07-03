<?php

$sourceFileName= "C:\Projects\Data\Prices\GuyISA.html";

printf("Parsing file %s\n", $sourceFileName);

$sourceData = file_get_contents($sourceFileName, false);
$dom = new DOMDocument();
if (@$dom->loadHTML($sourceData) == false) {
	printf("failed to parse file response%s.\n", $sourceFileName);
	return null;
}

$matching = array();
findAllElementsByAttribute($dom, 'class', 'align-right streaming-price-metric', $matching);

$tablewrapper = $dom->getElementById('holdings-table-wrapper');
$tableData = findElementByClassName($tablewrapper,'price_table dNone');
printf("class: %s\n", get_class($tableData));
$tableBody = findElementByElementName($tableData, 'tbody');
if($tableBody != NULL) {
	foreach($tableBody->childNodes as $tableRow) {
		if($tableRow->nodeName == 'tr') {
			$priceElement = findElementByClassName($tableRow,'align-right streaming-price-metric' );
			if($priceElement != NULL) {
				$symbol = findElementMatchingAttributeValue($tableRow, 'id', '/live_price_value_(.+)/' );			
				printf("symbol:%s, price:%s\n", $symbol,$priceElement->nodeValue );
			}					
		}
		// printf("sub class: %s\n", get_class($tableRow));
		// printf("sub class type %s\n", $tableRow->nodeType);
		// printf("sub class name: %s\n", $tableRow->nodeName);
	}	
}

function findElementByElementName($element, $elementName) {
	foreach($element->childNodes as $subElement) {
		if($subElement->nodeName == $elementName) {
			return $subElement;
		}
	}	
	return NULL;	
}
	

function findElementByClassName($element, $className) {

	//printf("element  node %s\n", $element->nodeName);
	//printf("element node type %d\n", $element->nodeType);
	//printf("element node value %s\n", trim($element->nodeValue));
	//printf("%s\n", get_class($element));

	return findElementByAttribute($element, 'class', $className );
}

function findElementByAttribute($element, $attribute, $attributeValue) {

	//printf("findElementByAttribute element: %s. \n", $element);
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

function findElementWithAttribute($element, $attribute) {

	//printf("findElementByAttribute element: %s. \n", $element);
	if($element->nodeType != 1) {
		return NULL;
	}
	
	$elementAttribute = $element->getAttribute($attribute);
	if($elementAttribute != '') {
		return $element;
	}
	if($element->hasChildNodes()) {	
		foreach($element->childNodes as $subchild) {
			$foundElement = findElementWithAttribute($subchild, $attribute);
			if($foundElement != NULL) {
				return $foundElement;
			}
		}	
	}
	return NULL;
}

function findElementMatchingAttributeValue($element, $attribute, $valueMatch) {
	
	if($element->nodeType != 1) {
		return NULL;
	}
	
	$attributeValue = $element->getAttribute($attribute);
	
	if(preg_match($valueMatch, trim($attributeValue), $matches)) {
		return $matches[1];	
	}
	
	if($element->hasChildNodes()) {	
		foreach($element->childNodes as $subchild) {
			$foundElement = findElementWithAttribute($subchild, $attribute);
			if($foundElement != NULL) {
				return $foundElement;
			}
		}	
	}
	return NULL;	
}

function findAllElementsByAttribute($element, $attribute, $attributeValue, $matching) {

	//printf("findElementByAttribute element: %s. \n", $element);
	if($element->nodeType != 1) {
		return;
	}
	
	$elementName = $element->getAttribute($attribute);
	if($elementName == $attributeValue) {
		array_push($matching, $element);
	}
	if($element->hasChildNodes()) {	
		foreach($element->childNodes as $subchild) {
			findAllElementsByAttribute($subchild, $attribute, $attributeValue, $matching);
		}	
	}
	return;
}