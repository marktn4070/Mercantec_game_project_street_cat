let openedPopup = false;
const levels = [lvl_1];
const openPopup = document.getElementById('openPopup');
const startP1 = document.getElementById('startP1');
const startP2 = document.getElementById('startP2');
const popup = document.getElementById('popup');
const heading = document.getElementById('heading');
heading.innerHTML = "Wild Cat";

console.log("MENU FILE LOADED");

openPopup.addEventListener('click', () => {
	stopCurrentGame();

	popup.style.display = 'flex';
	openPopup.style.display = 'none';
	heading.innerHTML = "Game Over";
});

startP1.addEventListener('click', () => startNewGame(1));
startP2.addEventListener('click', () => startNewGame(2));

console.log("startP1 =", startP1);
console.log("startP2 =", startP2);

async function startNewGame(gameMode) {
	console.log("START NEW GAME", gameMode);

    if (gameStarted) return;
    const response = await fetch(
        `${API_URL}createlobby?gameMode=${gameMode}`,
        {
            method: "POST"
        }
    );

    const pin = await response.text();
    console.log("Pin:", pin);
	await waitForPlayers(pin)
	console.log("Players in lobby:", players);
	console.log("Players in lobby:", players);

	document.getElementById("startGameBtn").style.display = "block";
	startGameBtn.addEventListener("click", () => {

		console.log("START GAME CLICKED");
	
		gameStarted = true;
	
		startDataInterval();
		startTimer();
	
		startGameBtn.style.display = "none";
	});
	resetGame();

	openedPopup = false;

	popup.style.display = 'none';
	openPopup.style.display = 'inline-block';

	const session = ++gameSession;
    runGame(levels, DOMDisplay, session);

    if (lastDecisecond !== 0){
        console.log("Highscore = " + lastDecisecond);

        const lastHighscore =
            document.getElementById('lastHighscore');

        lastHighscore.innerHTML =
            "Highscore = " +
            String(lastDecisecond).padStart(5, "0");
    }
}

async function waitForPlayers(pin) {

    while (true) {

        const response =
            await fetch(`${API_URL}lobby/${pin}`);

        const lobby =
            await response.json();

        console.log("Players:", lobby.players);

        if (lobby.players.length > 0) {

            players = lobby.players;

            console.log(
                "Player joined!",
                players
            );

            return;
        }

        await new Promise(r =>
            setTimeout(r, 1000)
        );
    }
}
