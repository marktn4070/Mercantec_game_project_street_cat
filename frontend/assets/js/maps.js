
let mapLength = 100;

function replaceAt(str, replace) {
    // for (let i = 0; i < amount; i++) {
    let randomIndex = Math.floor(Math.random() * air.length);
    if (replace === "@") randomIndex = 0;
    console.log(randomIndex);

    return str.substring(0, randomIndex) + replace + str.substring(randomIndex + 1);
}

let air = ".".repeat(mapLength);
let lvlPart1 = air + "\n" + air + "\n";
let lvlPart2 = replaceAt(air, "=");
// lvlPart2 = replaceAt(lvlPart2, "=");


let lvlPart3 = replaceAt(air, "@");

lvlPart3 = replaceAt(lvlPart3, "|");
lvlPart3 = replaceAt(lvlPart3, "o");
// lvlPart3 = replaceAt(lvlPart3, "o");
// lvlPart3 = replaceAt(lvlPart3, "o");


let lvlPart4 = "#".repeat(mapLength);

let lvl_1 = lvlPart1 + lvlPart2 + "\n" + lvlPart3 + "\n" + lvlPart4;


console.log(lvl_1);

// let lvl_2 = `
// ................................................................................................
// ................................................................................................
// ..............................................................................=.........=.......
// @.....................+.....+.....+.............................o.o......o.oo......o.o..........
// ################################################################################################`;

// let lvl_3 = `
// .......................................................
// #.....................................................#
// #..................=.........=.......=.......=........#
// #.............o.o......o.oo......o.o.....o.o.....o.o..#
// #.@..........#####....######....#####...#####...#####.#
// #########.............................................#
// ....#####+++++++++++++++++++++++++++++++++++++++++++++#
// ....###################################################`;
 