
let mapLength = 70;
let aroundObjLength = 30;
let addingLevel = false;

function replaceAt(str, replace, amount = 1) {
 for (let i = 0; i < amount; i++) {
  let randPos = Math.floor(Math.random() * str.length);
  if (replace === "@") randPos = 0;

  if (str[randPos] !== ".") {
   i--;
   continue;
  }
  str = str.substring(0, randPos) + replace + str.substring(randPos + 1);
 }
 return str;
}

function createLevel(includePlayer = false) {
	let air = ".".repeat(mapLength);
	let airAroundObj = ".".repeat(aroundObjLength);
	let lvlPart1 = airAroundObj + air + "\n" + airAroundObj + air + "\n";
	let lvlPart2 = airAroundObj + replaceAt(air, "=", 5); // Bird (top)
	airAroundObj = ".".repeat(aroundObjLength / 3);

	let lvlPart3 = airAroundObj + replaceAt(air, "=", 3); // Bird (button)
	lvlPart3 = airAroundObj + replaceAt(lvlPart3, "o", 1); // Coin
	if (includePlayer) {
		lvlPart3 = replaceAt(lvlPart3, "@") + airAroundObj; // Character
	}else{
		lvlPart3 = lvlPart3 + airAroundObj;
	}

	airAroundObj = "#".repeat(aroundObjLength);
	let lvlPart4 = airAroundObj + "#".repeat(mapLength); // Platform

	let lvl_1 = lvlPart1 + lvlPart2 + "\n" + lvlPart3 + "\n" + lvlPart4;

	console.log(lvl_1);
	return lvl_1;
}
