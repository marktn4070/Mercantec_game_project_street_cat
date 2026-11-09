

function startTimer() {
	if (timerInterval !== null) return;

	const timer = document.getElementById('timer');
	timer.innerHTML = "Score = " + String(decisecond).padStart(5, "0");
	
	timerInterval = setInterval(() => {
		decisecond++;
		timer.innerHTML = "Score = " + String(decisecond).padStart(5, "0");
	}, 100);
}

function stopTimer() {
	if (timerInterval !== null) {
		clearInterval(timerInterval);
		timerInterval = null;
		console.log("Timer interval stopped");
	}
}

// function startTimer() {
// 	if (gameStarted === false) return;
	
// 	timerInterval = setInterval(() => {
// 		if (gameStarted === false) {
// 			if (!timerInterval) return;
// 			clearInterval(timerInterval);
// 			timerInterval = null;
// 			console.log('timer stopped after ' + (decisecond / 100) + " seconds");
// 			return;
// 		}
// 		document.getElementById("timer").innerHTML = "Highscore = " + String(decisecond).padStart(5, '0');
// 		decisecond++;
// 	}, 100);
// }






// // Only for test (delete later)
// async function startTimer() {
// 	const gotData = await getData();

// 	if (!gotData) return; 

// 	let pollingInterval = setInterval(function() {
// 		timer();

// 		if (!gotData) {
// 			clearInterval(pollingInterval);
// 			pollingInterval = null;
// 			console.log("Interval stoppet");
// 		}
// 	}, 100);
// }


// startTimer();



// var x = setInterval(function() {
//   document.getElementById("timer").innerHTML = "Highscore = " + String(decisecond).padStart(5, '0');
//   decisecond++;
// }, 100);




// function timer() {
	// document.getElementById("timer").innerHTML = "Highscore = " + String(decisecond).padStart(5, '0');
	// decisecond++;
// }


