
let mapLength = 100;

function replaceAt(str, replace, amount = 1) {
 for (let i = 0; i < amount; i++) {
  let randPos = Math.floor(Math.random() * air.length);
  if (replace === "@") randPos = 0;

  if (str[randPos] !== ".") {
   i--;
   continue;
  }
  str = str.substring(0, randPos) + replace + str.substring(randPos + 1);
 }
 return str;
}
function createLevel() {
 let air = ".".repeat(mapLength);
 let lvlPart1 = air + "\n" + air + "\n";
 let lvlPart2 = replaceAt(air, "=", 5); // bird (right + left)
 let lvlPart3 = replaceAt(air, "@"); // Character
 lvlPart3 = replaceAt(lvlPart3, "|", 2); // bird (up + down)
 lvlPart3 = replaceAt(lvlPart3, "o", 1);// Coin

 let lvlPart4 = "#".repeat(mapLength);

 let lvl_1 = lvlPart1 + lvlPart2 + "\n" + lvlPart3 + "\n" + lvlPart4;

 console.log(lvl_1);
 }
 createLevel();