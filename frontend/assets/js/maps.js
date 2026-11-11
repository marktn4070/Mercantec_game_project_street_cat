
let mapLength = 70;
let BetweenObjLength = 30;
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
	let airBetweenObj = ".".repeat(BetweenObjLength);
	let lvlPart1 = airBetweenObj + air + "\n" + airBetweenObj + air + "\n";
	let lvlPart2 = airBetweenObj + replaceAt(air, "=", 5); // bird (right + left)
	airBetweenObj = ".".repeat(BetweenObjLength / 3);



	//  let lvlPart3 = airBetweenObj + replaceAt(air, "|", 2); // bird (up + down)
	let lvlPart3 = airBetweenObj + replaceAt(air, "=", 1); // bird (up + down)
	lvlPart3 = airBetweenObj + replaceAt(lvlPart3, "o", 1); // Coin
	//  lvlPart3 = airBetweenObj + replaceAt(lvlPart3, "=", 1); // Coin
	if (includePlayer) {
		lvlPart3 = replaceAt(lvlPart3, "@") + airBetweenObj; // Character
	}else{
		lvlPart3 = lvlPart3 + airBetweenObj; // Character
	}

	airBetweenObj = "#".repeat(BetweenObjLength);
	let lvlPart4 = airBetweenObj + "#".repeat(mapLength);

	let lvl_1 = lvlPart1 + lvlPart2 + "\n" + lvlPart3 + "\n" + lvlPart4;

	console.log(lvl_1);
	return lvl_1;
}