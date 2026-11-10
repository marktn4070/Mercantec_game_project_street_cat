let openedPopup = false;
const levels = [simpleLevelPlan1, simpleLevelPlan2, simpleLevelPlan3];
const openPopup = document.getElementById('openPopup');
const startP1 = document.getElementById('startP1');
const startP2 = document.getElementById('startP2');
const popup = document.getElementById('popup');
const heading = document.getElementById('heading');
heading.innerHTML = "Wild Cat";

openPopup.addEventListener('click', () => {
	stopCurrentGame();

	popup.style.display = 'flex';
	openPopup.style.display = 'none';
	heading.innerHTML = "Game Over";
});

startP1.addEventListener('click', () => startNewGame(1));
startP2.addEventListener('click', () => startNewGame(2));

async function startNewGame(gameMode) {

    if (gameStarted) return;
    const response = await fetch(
        `${API_URL}createlobby?gameMode=${gameMode}`,
        {
            method: "POST"
        }
    );

    const pin = await response.text();
    console.log("Pin:", pin);
    await getLobby(pin);
	console.log("Players in lobby:", players);

    resetGame();
    openedPopup = false;
    gameStarted = false;

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

