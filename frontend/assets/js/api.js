// api
const API_URL = "https://wildcat.mercantec.tech/";
console.log("API FILE LOADED");

let nextPlayerId = 1;

async function getData() {
    try {

        for (const player of players) {

            const response = await fetch(
                `${API_URL}playerstate/${player.playerId}`
            );

            if (response.status === 400) {
                console.error("400: Ugyldig forespørgsel");
                return false;
            }

            if (response.status === 404) {
                const errorText = await response.text();
                console.error(`404: ${errorText}`);
                return false;
            }

            if (!response.ok) {
                throw new Error(`HTTP-fejl: ${response.status}`);
            }

            const data = await response.json();
            console.log(
                `${player.playerId}: ${data.action}`
            );
            
            if (data.action === "Jump") {
                remoteJump = true;
            }

            if (data.action === "Crawl") {
                remoteCrawl = true;
            }
        }
        return true;
    } catch (error) {
        console.error("Kunne ikke hente data:", error);
        return false;
    }
}


// Only for test (delete later)
async function startDataInterval() {
	const gotData = await getData();

	if (!gotData || gameStarted === false) return;

	getDataInterval = setInterval(async () => {
		const gotData = await getData();

		if (!gotData || gameStarted === false) {
			stopDataInterval();
		}
	}, 50);
}

function stopDataInterval() {
	if (getDataInterval !== null) {
		clearInterval(getDataInterval);
		getDataInterval = null;
		console.log("Data interval stopped");
	}
}

let boostActive = false;

async function postBoostName(boostName) {
    if (useBoostRequestInProgress || boostActive) return false;

    boostRequestInProgress = true;

    try {
        const response = await fetch(
            `${API_URL}boost/collect/1?boostName=${boostName}`, {
                method: "POST"
            }
        );
        if (!response.ok) {
            console.error("Kunne ikke samle boost:", await response.text());
            return false;
        }

        gotBoost = true;
        console.log(`Boost samlet: ${boostName}`);
        return true;
    } catch (error) {
        console.error("Netværksfejl ved indsamling af boost:", error);
        return false;
    } finally {
        boostRequestInProgress = false;
    }
}

async function postBoostUse() {
	if (useBoostRequestInProgress || boostActive) return false;

    useBoostRequestInProgress = true;

    try {
        const response = await fetch(
            `${API_URL}boost/use/1`, {
                method: "POST"
            }
        );

        if (!response.ok) {
            console.error("Kunne ikke bruge boost:", await response.text());
            return false;
        }

		gotBoost = false;
		boostActive = true;

		console.log("Boost brugt");

		setTimeout(() => {
			boostActive = false;
			playerMode = "";
			console.log("Boost kan bruges igen");
		}, 6000);

		return true;
    } catch (error) {
        console.error("Netværksfejl ved brug af boost:", error);
        return false;
    } finally {
        useBoostRequestInProgress = false;
    }
}

let players = [];

async function getLobby(pin) {
    const response = await fetch(
        `${API_URL}lobby/${pin}`
    );

    if (!response.ok)
        return false;

    const lobby = await response.json();

    players = lobby.players;
    console.log(players);

    return true;
}