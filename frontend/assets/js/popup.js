let openedPopup = false;
const startP1 = document.getElementById('startP1');
const startP2 = document.getElementById('startP2');
const popup = document.getElementById('popup');
const heading = document.getElementById('heading');
const restartBtn = document.getElementById("restartBtn");
const subheading = document.getElementById("subheading");

heading.innerHTML = "Wild Cat";

console.log("MENU FILE LOADED");

startP1.addEventListener('click', () => startNewGame(1));
startP2.addEventListener('click', () => startNewGame(2));
let currentGameMode;
let currentPin;

restartBtn.onclick = async () =>
	{
		const success =
			await restartGame(currentPin);
	
		if (!success)
			return;
	
		popup.style.display = "none";
	
		restartBtn.style.display = "none";
	
		openedPopup = false;
	
		resetGame();
	
		gameStarted = true;
	
		startDataInterval();
		startTimer();
	
		const session = ++gameSession;
	
		runGame(
			DOMDisplay,
			session,
			document.getElementById(
				"singleplayer-container"
			)
		);
	};
console.log("startP1 =", startP1);
console.log("startP2 =", startP2);

async function startNewGame(gameMode) {
	resetGame();
	console.log("START NEW GAME", gameMode);
	currentGameMode = gameMode;

	const response = await fetch(
		`${API_URL}createlobby?gameMode=${gameMode}`,
		{
			method: "POST"
		}
	);
	
	const pin = await response.text();
	currentPin = pin;
	console.log("Pin:", pin);
	await waitForPlayers(pin);

	console.log("Players in lobby:", players);
	console.log("Players in lobby:", players);

	document.getElementById("startGameBtn").style.display = "block";
	startGameBtn.onclick = () => {

		console.log("START GAME CLICKED");

		gameStarted = true;

	console.log(lastDecisecond);



		startDataInterval();
		startTimer();

		startGameBtn.style.display = "none";
		const session = ++gameSession;
		if (currentGameMode === 1) {

			document.getElementById(
				"singleplayer-container"
			).style.display = "block";

			document.getElementById(
				"multiplayer-container"
			).style.display = "none";

			runGame(
				DOMDisplay,
				session,
				document.getElementById(
					"singleplayer-container"
				)
			);
		}
		else {

			document.getElementById(
				"singleplayer-container"
			).style.display = "none";

			document.getElementById(
				"multiplayer-container"
			).style.display = "block";

			runMultiplayerGame(
				levels,
				session
			);
		}
	};
	openedPopup = false;

	popup.style.display = 'none';

	restartBtn.style.display = "none";
	startP1.style.display ="inline-block";
	startP2.style.display ="inline-block";


}

window.addEventListener(
    "beforeunload",
    () => {

        if (!currentPin)
            return;

		//Sendbeacon is the best choice ofr browser refrhes
        navigator.sendBeacon(
            `${API_URL}closeLobby/${currentPin}`
        );
    }
);

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
