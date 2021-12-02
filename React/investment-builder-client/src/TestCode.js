
const testFunc1 = function() {
    console.log("TestFunc1");    
}

const testFunc2 = function() {
    console.log("TestFunc2");    
}

const testFunc3 = function() {
    console.log("TestFunc3");    
}

let arr = [];

console.log("start");

arr.push(testFunc1);
arr.push(testFunc2);
arr.push(testFunc3);


var idx = arr.indexOf(testFunc2);
if(idx > -1) {
    arr.splice(idx, 1);
}

for(var i = 0; i < arr.length; i++) {
    arr[i]();
}

console.log("End");